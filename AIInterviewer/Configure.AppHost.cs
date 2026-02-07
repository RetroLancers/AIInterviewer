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
        ServiceStack.Licensing.RegisterLicense("OSS BSD-3-Clause 2025 https://github.com/NetCoreTemplates/nextjs fikwwuuSWXcMlSp9HNi2aWOqg+XskqXNG/2epBNhPeQ4zEddXhxSFlFwv3Rfvr74nB5kaGPVvA1sY6MgfMkHYngn8FSLnDZPXUtCt54N5hEen1mKPxTIAPJjBGjWNSP+vccFGl4mpbpxjmpZ8+eKcAuzvMuWa3UqZFpV/49Buis=");
}
