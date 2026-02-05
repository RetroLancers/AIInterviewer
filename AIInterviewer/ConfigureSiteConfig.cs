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
                var siteConfig = siteConfigs[0];
                if (string.IsNullOrWhiteSpace(siteConfig.TranscriptionProvider))
                {
                    siteConfig.TranscriptionProvider = "Gemini";
                    db.Update(siteConfig);
                }

                siteConfigHolder.SiteConfig = siteConfig;
            }
            else
            {
                var siteConfig = new SiteConfig()
                {
                    GeminiApiKey = "",
                    InterviewModel = "",
                    GlobalFallbackModel = "",
                    KokoroVoice = "af_heart",
                    TranscriptionProvider = "Gemini"
                };
                siteConfigHolder.SiteConfig = siteConfig;
                siteConfig.Id = (int)db.Insert(siteConfig, true);
            }
        }
    }
}
