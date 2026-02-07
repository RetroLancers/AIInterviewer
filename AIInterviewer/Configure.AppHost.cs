using AIInterviewer.ServiceInterface;
using AIInterviewer.ServiceModel.Tables.Configuration;
using ServiceStack.NativeTypes.TypeScript;

[assembly: HostingStartup(typeof(AIInterviewer.AppHost))]

namespace AIInterviewer;

public class AppHost() : AppHostBase("AIInterviewer"), IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((Action<WebHostBuilderContext, IServiceCollection>)((context, services) => {
            var siteConfigHolder = new SiteConfigHolder();
            services.AddSingleton(siteConfigHolder);
            services.AddScoped<AIInterviewer.ServiceInterface.Interfaces.IAiProviderFactory, AIInterviewer.ServiceInterface.Factories.AiProviderFactory>();

            services.AddSingleton(context.Configuration.GetSection(nameof(AppConfig))?.Get<AppConfig>()
                ?? new AppConfig {
                        BaseUrl = context.HostingEnvironment.IsDevelopment()
                            ? "https://localhost:5001"  
                            : Environment.GetEnvironmentVariable("KAMAL_DEPLOY_HOST"),
                    });
            }));

    // Configure your AppHost with the necessary configuration and dependencies your App needs
    public override void Configure()
    {
        TypeScriptGenerator.InsertTsNoCheck = true;
        
        SetConfig(new HostConfig {
        });
    }
    
    // TODO: Replace with your own License Key. FREE Individual or OSS License available from: https://servicestack.net/free
    public static void RegisterKey() =>
        ServiceStack.Licensing.RegisterLicense("Individual (c) 2026 Sterling Kooshesh sTWLaPYQlgdtZoHvx+gczZdmepDtoKPlR5bOIbV1XZZNHfe6U4hdO1F4C7T/N/5dRuox3Ga57oaVYfWTSC6MN8X6WfzMw1IEPRZoCorUsLWIw9NyxgcQ7Uds4ygM4Dw+wrw5Zp3gFg+r/KUd/IRIqJHmBOS7n6TRm+XmQOQuU6M=");
}
