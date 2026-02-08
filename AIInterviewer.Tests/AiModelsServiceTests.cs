using NUnit.Framework;
using ServiceStack;
using ServiceStack.OrmLite;
using ServiceStack.Testing;
using AIInterviewer.ServiceInterface.Services.AI;
using AIInterviewer.ServiceInterface.Interfaces;
using AIInterviewer.ServiceModel.Tables.Configuration;
using AIInterviewer.ServiceModel.Types.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ServiceStack.Data;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace AIInterviewer.Tests;

public class AiModelsServiceTests
{
    private ServiceStackHost appHost;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        appHost = new BasicAppHost().Init();
        appHost.Container.AddSingleton<IDbConnectionFactory>(new OrmLiteConnectionFactory("file::memory:?cache=shared", SqliteDialect.Provider));
        appHost.Container.AddSingleton<ILogger<AiModelsService>>(NullLogger<AiModelsService>.Instance);

        // Mock IAiProviderFactory
        appHost.Container.AddSingleton<IAiProviderFactory>(new MockAiProviderFactory());
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() => appHost.Dispose();

    [Test]
    public async Task Get_ShouldReturnModels_WhenApiKeyProvided()
    {
        var siteConfigHolder = new SiteConfigHolder { SiteConfig = new SiteConfig() };
        var service = new AiModelsService(
            appHost.Container.Resolve<IAiProviderFactory>(),
            siteConfigHolder,
            appHost.Container.Resolve<ILogger<AiModelsService>>()
        );
        service.Request = new MockHttpRequest();

        var response = await service.Get(new GetAiModels { ProviderType = "Gemini", ApiKey = "test-key" });
        
        Assert.That(response.Models, Contains.Item("mock-model"));
    }

    [Test]
    public async Task Get_ShouldCallDbOnce_WhenConfigIsActive()
    {
        var dbFactory = appHost.Container.Resolve<IDbConnectionFactory>();
        using var db = dbFactory.OpenDbConnection();
        db.CreateTable<AiServiceConfig>();

        var aiConfigId = (int)db.Insert(new AiServiceConfig
        {
            ProviderType = "Gemini",
            Name = "Gemini Config",
            ApiKey = "test-key",
            ModelId = "gemini-pro"
        }, selectIdentity: true);

        var siteConfigHolder = new SiteConfigHolder
        {
            SiteConfig = new SiteConfig
            {
                ActiveAiConfigId = aiConfigId
            }
        };

        var service = new AiModelsService(
            appHost.Container.Resolve<IAiProviderFactory>(),
            siteConfigHolder,
            appHost.Container.Resolve<ILogger<AiModelsService>>()
        );
        service.Request = new MockHttpRequest();

        int queryCount = 0;
        OrmLiteConfig.BeforeExecFilter = cmd => {
            if (cmd.CommandText.Contains("ai_service_config") || cmd.CommandText.Contains("AiServiceConfig"))
            {
                queryCount++;
            }
        };

        try
        {
            await service.Get(new GetAiModels());
        }
        finally
        {
             OrmLiteConfig.BeforeExecFilter = null;
        }

        Assert.That(queryCount, Is.EqualTo(1), "Expected 1 DB query when config is found");
    }

    class MockAiProviderFactory : IAiProviderFactory
    {
        public IAiProvider GetProvider(AiServiceConfig config) => new MockAiProvider();
    }

    class MockAiProvider : IAiProvider
    {
        public string ProviderName => "Mock";
        public Task<string?> GenerateTextAsync(string prompt, string? systemPrompt = null, double? temperature = null, int? maxOutputTokens = null) => Task.FromResult<string?>("test");
        public Task<string?> GenerateTextAsync(IEnumerable<AIInterviewer.ServiceModel.Types.Ai.AiMessage> messages, string? systemPrompt = null, double? temperature = null, int? maxOutputTokens = null) => Task.FromResult<string?>("test");
        public Task<string?> GenerateTextFromAudioAsync(string prompt, byte[] audioData, string mimeType, string? systemPrompt = null) => Task.FromResult<string?>("test");
        public Task<T?> GenerateJsonAsync<T>(string prompt, AIInterviewer.ServiceModel.Types.Ai.AiSchemaDefinition schema, string schemaName, string? systemPrompt = null) where T : class => Task.FromResult<T?>(null);
        public Task<IEnumerable<string>> ListModelsAsync() => Task.FromResult<IEnumerable<string>>(new[] { "mock-model" });
    }
}