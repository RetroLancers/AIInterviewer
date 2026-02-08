using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Sqlite;
using AIInterviewer.ServiceInterface.Services.Interview;
using AIInterviewer.ServiceModel.Types.Interview;
using AIInterviewer.ServiceModel.Tables.Interview;
using AIInterviewer.ServiceModel.Tables.Configuration;
using ServiceStack.Validation;
using AIInterviewer.ServiceInterface.Validators.Interview;
using System.Data;
using System;

namespace AIInterviewer.Tests;

[TestFixture]
public class InterviewerServiceTests
{
    private ServiceStackHost appHost;
    private IDbConnection _db;

    public class TestInterviewerService : InterviewerService
    {
        private readonly IDbConnection _testDb;
        public TestInterviewerService(IDbConnection testDb)
        {
            _testDb = testDb;
        }

        public override IDbConnection Db => _testDb;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        AIInterviewer.AppHost.RegisterKey();
        appHost = new BasicAppHost().Init();

        var dbFactory = new OrmLiteConnectionFactory(":memory:", SqliteDialect.Provider);
        _db = dbFactory.Open();

        _db.CreateTable<AiServiceConfig>();
        _db.CreateTable<Interviewer>();

        appHost.Container.Register<InterviewerService>(c => new TestInterviewerService(_db));

        // Register Validation
        appHost.Plugins.Add(new ValidationFeature());
        appHost.Container.RegisterValidator(typeof(CreateInterviewerValidator));
        appHost.Container.RegisterValidator(typeof(UpdateInterviewerValidator));
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _db.Dispose();
        appHost.Dispose();
    }

    [Test]
    public void Can_CRUD_Interviewer()
    {
        var service = appHost.Container.Resolve<InterviewerService>();
        service.Request = new MockHttpRequest();

        // Create
        var createRequest = new CreateInterviewer
        {
            Name = "Test Interviewer",
            SystemPrompt = "You are a helpful interviewer.",
            AiConfigId = null
        };

        var created = service.Any(createRequest);
        Assert.That(created.Id, Is.GreaterThan(0));
        Assert.That(created.Name, Is.EqualTo("Test Interviewer"));
        Assert.That(created.SystemPrompt, Is.EqualTo("You are a helpful interviewer."));
        Assert.That(created.CreatedAt, Is.GreaterThan(DateTime.MinValue));

        // Get
        var fetched = service.Any(new GetInterviewer { Id = created.Id });
        Assert.That(fetched.Name, Is.EqualTo("Test Interviewer"));

        // List
        var list = service.Any(new ListInterviewers());
        Assert.That(list.Interviewers.Count, Is.GreaterThan(0));

        // Update
        var updateRequest = new UpdateInterviewer
        {
            Id = created.Id,
            Name = "Updated Name",
            SystemPrompt = "Updated Prompt",
            AiConfigId = null
        };
        var updated = service.Any(updateRequest);
        Assert.That(updated.Name, Is.EqualTo("Updated Name"));
        Assert.That(updated.UpdatedAt, Is.GreaterThan(created.CreatedAt));

        // Verify Update persistence
        var fetchedUpdated = service.Any(new GetInterviewer { Id = created.Id });
        Assert.That(fetchedUpdated.Name, Is.EqualTo("Updated Name"));

        // Delete
        service.Any(new DeleteInterviewer { Id = created.Id });

        Assert.Throws<HttpError>(() => service.Any(new GetInterviewer { Id = created.Id }));
    }
}
