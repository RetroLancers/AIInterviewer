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

namespace AIInterviewer.Tests;

public class GeminiModelsServiceTests
{
    private ServiceStackHost appHost;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        appHost = new BasicAppHost().Init();
        appHost.Container.AddSingleton<IDbConnectionFactory>(new OrmLiteConnectionFactory("file::memory:?cache=shared", SqliteDialect.Provider));
        appHost.Container.AddSingleton<ILogger<GeminiModelsService>>(NullLogger<GeminiModelsService>.Instance);

        // Mock IAiProviderFactory
        appHost.Container.AddSingleton<IAiProviderFactory>(new MockAiProviderFactory());
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() => appHost.Dispose();

    [Test]
    public async Task Get_ShouldCallDbOnlyOnce_WhenConfigIsNotGemini()
    {
        var dbFactory = appHost.Container.Resolve<IDbConnectionFactory>();
        using var db = dbFactory.OpenDbConnection();
        db.CreateTable<AiServiceConfig>();

        var aiConfigId = (int)db.Insert(new AiServiceConfig
        {
            ProviderType = "OpenAI", // Not Gemini
            Name = "OpenAI Config",
            ApiKey = "test-key",
            ModelId = "gpt-4"
        }, selectIdentity: true);

        var siteConfigHolder = new SiteConfigHolder
        {
            SiteConfig = new SiteConfig
            {
                ActiveAiConfigId = aiConfigId
            }
        };

        var service = new GeminiModelsService(
            appHost.Container.Resolve<IAiProviderFactory>(),
            siteConfigHolder,
            appHost.Container.Resolve<ILogger<GeminiModelsService>>()
        );
        // Inject the Request context which is needed for resolving dependencies via ServiceStack if needed
        service.Request = new MockHttpRequest();

        int queryCount = 0;
        // Reset and set the filter
        OrmLiteConfig.BeforeExecFilter = cmd => {
            // We are interested in queries selecting from AiServiceConfig
            if (cmd.CommandText.Contains("ai_service_config") || cmd.CommandText.Contains("AiServiceConfig"))
            {
                queryCount++;
                TestContext.WriteLine($"Query executed: {cmd.CommandText}");
            }
        };

        try
        {
            await service.Get(new GetGeminiModels());
        }
        catch (HttpError)
        {
            // Expected 400 error because config is not Gemini
        }
        finally
        {
             OrmLiteConfig.BeforeExecFilter = null;
        }

        // With the bug, it fetches once, sees it's not Gemini (sets config=null), then fetches again.
        // So expected count is 2.
        Assert.That(queryCount, Is.EqualTo(1), "Expected 1 DB query");
    }

    [Test]
    public async Task Get_ShouldCallDbOnce_WhenConfigIsGemini()
    {
        var dbFactory = appHost.Container.Resolve<IDbConnectionFactory>();
        using var db = dbFactory.OpenDbConnection();
        // Ensure table exists
        db.CreateTable<AiServiceConfig>(overwrite: false);

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

        var service = new GeminiModelsService(
            appHost.Container.Resolve<IAiProviderFactory>(),
            siteConfigHolder,
            appHost.Container.Resolve<ILogger<GeminiModelsService>>()
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
            await service.Get(new GetGeminiModels());
        }
        catch (NullReferenceException)
        {
            // MockAiProviderFactory.GetProvider returns null, so provider.ListModelsAsync throws NullReferenceException
            // This is expected as we are only testing the config fetching logic here
        }
        finally
        {
             OrmLiteConfig.BeforeExecFilter = null;
        }

        Assert.That(queryCount, Is.EqualTo(1), "Expected 1 DB query when config is found");
    }

    [Test]
    public async Task Get_ShouldNotCallDb_WhenSiteConfigIsNull()
    {
        var siteConfigHolder = new SiteConfigHolder
        {
            SiteConfig = null
        };

        var service = new GeminiModelsService(
            appHost.Container.Resolve<IAiProviderFactory>(),
            siteConfigHolder,
            appHost.Container.Resolve<ILogger<GeminiModelsService>>()
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
            await service.Get(new GetGeminiModels());
        }
        catch (HttpError)
        {
            // Expected 400 error
        }
        finally
        {
             OrmLiteConfig.BeforeExecFilter = null;
        }

        Assert.That(queryCount, Is.EqualTo(0), "Expected 0 DB queries when SiteConfig is null");
    }

    [Test]
    public async Task Get_ShouldNotCallDb_WhenActiveAiConfigIdIsZero()
    {
        var siteConfigHolder = new SiteConfigHolder
        {
            SiteConfig = new SiteConfig
            {
                ActiveAiConfigId = 0
            }
        };

        var service = new GeminiModelsService(
            appHost.Container.Resolve<IAiProviderFactory>(),
            siteConfigHolder,
            appHost.Container.Resolve<ILogger<GeminiModelsService>>()
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
            await service.Get(new GetGeminiModels());
        }
        catch (HttpError)
        {
            // Expected 400 error
        }
        finally
        {
             OrmLiteConfig.BeforeExecFilter = null;
        }

        Assert.That(queryCount, Is.EqualTo(0), "Expected 0 DB queries when ActiveAiConfigId is 0");
    }

    class MockAiProviderFactory : IAiProviderFactory
    {
        public IAiProvider GetProvider(AiServiceConfig config) => null;
    }
}
