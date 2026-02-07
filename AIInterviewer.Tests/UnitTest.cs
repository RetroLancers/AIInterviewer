using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;
using AIInterviewer.ServiceInterface;
using AIInterviewer.ServiceInterface.Services.Configuration;
using AIInterviewer.ServiceModel;
using AIInterviewer.ServiceModel.Tables.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AIInterviewer.Tests;

public class UnitTest
{
    private readonly ServiceStackHost appHost;

    public UnitTest()
    {
        AIInterviewer.AppHost.RegisterKey();
        appHost = new BasicAppHost().Init();
        appHost.Container.AddSingleton<SiteConfigHolder>();
        appHost.Container.AddSingleton<ILogger<SiteConfigService>>(new NullLogger<SiteConfigService>());
        appHost.Container.AddTransient<SiteConfigService>();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() => appHost.Dispose();

    [Test]
    public void Can_call_MyServices()
    {
        var service = appHost.Container.Resolve<SiteConfigService>();

 
    }
}