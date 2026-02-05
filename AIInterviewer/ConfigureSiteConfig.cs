using AIInterviewer.ServiceModel.Tables.Configuration;
using ServiceStack.Data;
using ServiceStack.OrmLite;

[assembly: HostingStartup(typeof(ConfigureSiteConfig))]

namespace AIInterviewer;

public class ConfigureSiteConfig : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureAppHost(UpdateSiteConfigHolder);


    private void UpdateSiteConfigHolder(ServiceStackHost appHost)
    {
        using var db = appHost.Resolve<IDbConnectionFactory>().Open();
        if (db.TableExists<SiteConfig>())
        {
            var siteConfigs = db.Select<SiteConfig>();
            var siteConfigHolder = appHost.Resolve<SiteConfigHolder>();
            if (siteConfigs is { Count: > 0 })
            {
                siteConfigHolder.SiteConfig = siteConfigs[0];
            }
            else
            {
                var siteConfig = new SiteConfig()
                {
                    GeminiApiKey = "",
                    InterviewModel = "",
                    GlobalFallbackModel = ""
                };
                siteConfigHolder.SiteConfig = siteConfig;
                siteConfig.Id = (int)db.Insert(siteConfig, true);
            }
        }
    }
}