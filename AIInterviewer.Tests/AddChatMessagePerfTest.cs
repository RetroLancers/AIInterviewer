using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;
using ServiceStack.OrmLite;
using AIInterviewer.ServiceInterface.Services.Interview;
using AIInterviewer.ServiceModel.Tables.Configuration;
using AIInterviewer.ServiceModel.Types.Interview;
using AIInterviewer.ServiceInterface.Interfaces;
using AIInterviewer.ServiceModel.Types.Ai;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Data;
using ServiceStack.Data;

namespace AIInterviewer.Tests;

public class MockAiProvider : IAiProvider
{
    public string ProviderName => "MockProvider";

    public Task<string?> GenerateTextAsync(string prompt, string? systemPrompt = null, double? temperature = null, int? maxOutputTokens = null)
    {
        return Task.FromResult<string?>("Mock AI Response");
    }

    public Task<string?> GenerateTextAsync(IEnumerable<AiMessage> messages, string? systemPrompt = null, double? temperature = null, int? maxOutputTokens = null)
    {
        return Task.FromResult<string?>("Mock AI Response");
    }

    public Task<string?> GenerateTextFromAudioAsync(string prompt, byte[] audioData, string mimeType, string? systemPrompt = null)
    {
        return Task.FromResult<string?>("Mock AI Response from Audio");
    }

    public Task<T?> GenerateJsonAsync<T>(string prompt, string? systemPrompt = null) where T : class
    {
        return Task.FromResult<T?>(default);
    }

    public Task<IEnumerable<string>> ListModelsAsync()
    {
        return Task.FromResult<IEnumerable<string>>(new List<string> { "model-1" });
    }
}

public class MockAiProviderFactory : IAiProviderFactory
{
    public IAiProvider GetProvider(AiServiceConfig config)
    {
        return new MockAiProvider();
    }
}

[TestFixture]
public class AddChatMessagePerfTest
{
    private ServiceStackHost appHost;
    private IDbConnection _keepAliveConnection;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        appHost = new BasicAppHost().Init();
        appHost.Container.AddTransient<IAiProviderFactory, MockAiProviderFactory>();
        appHost.Container.AddTransient<SiteConfigHolder>();
        appHost.Container.AddTransient<InterviewService>();
        appHost.Container.AddSingleton<ILogger<InterviewService>>(NullLogger<InterviewService>.Instance);

        // Setup SQLite In-Memory DB
        var dbFactory = new OrmLiteConnectionFactory("Data Source=file:memdb1?mode=memory&cache=shared", SqliteDialect.Provider);
        appHost.Container.Register<IDbConnectionFactory>(dbFactory);

        _keepAliveConnection = dbFactory.Open();
        _keepAliveConnection.CreateTable<AiServiceConfig>();
        _keepAliveConnection.CreateTable<AIInterviewer.ServiceModel.Tables.Interview.Interview>();
        _keepAliveConnection.CreateTable<AIInterviewer.ServiceModel.Tables.Interview.InterviewChatHistory>();

        // Add minimal data
        _keepAliveConnection.Insert(new AiServiceConfig {
            Name = "Test AI",
            ProviderType = "Gemini",
            ApiKey = "dummy",
            ModelId = "gemini-pro"
        });
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() {
        _keepAliveConnection?.Dispose();
        appHost.Dispose();
    }

    [Test]
    public async Task Verify_AddChatMessage_Logic()
    {
        var service = appHost.Resolve<InterviewService>();
        var dbFactory = appHost.Resolve<IDbConnectionFactory>();

        int interviewId;

        // We use a separate connection for setup, verifying that shared cache works
        using (var db = dbFactory.Open())
        {
             var interview = new AIInterviewer.ServiceModel.Tables.Interview.Interview
            {
                Prompt = "Test Prompt",
                CreatedDate = DateTime.UtcNow,
                UserId = "user1"
            };
            db.Save(interview);
            interviewId = interview.Id;
        }

        var request = new AddChatMessage
        {
            InterviewId = interviewId,
            Message = "Hello AI"
        };

        // Act
        // Without Request context, Service will resolve a NEW connection from IDbConnectionFactory
        var response = await service.Post(request);

        // Assert
        Assert.That(response.History, Is.Not.Null);
        // Should have at least User message + AI message
        Assert.That(response.History.Count, Is.GreaterThanOrEqualTo(2), "History should contain user message and AI response");

        var lastMessage = response.History.Last();
        Assert.That(lastMessage.Role, Is.EqualTo("Interviewer"));
        Assert.That(lastMessage.Content, Is.EqualTo("Mock AI Response"));

        // Verify User message is also there
        var userMessage = response.History.FirstOrDefault(x => x.Role == "User");
        Assert.That(userMessage, Is.Not.Null);
        Assert.That(userMessage!.Content, Is.EqualTo("Hello AI"));
    }
}
