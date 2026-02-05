using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;
using AIInterviewer.ServiceInterface;
using AIInterviewer.ServiceModel;

namespace AIInterviewer.Tests;

public class UnitTest
{
    private readonly ServiceStackHost appHost;

    public UnitTest()
    {
        appHost = new BasicAppHost().Init();
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