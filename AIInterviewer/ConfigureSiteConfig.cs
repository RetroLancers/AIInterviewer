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
        try
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
                    // Check if there's at least one AI config to reference
                    var firstAiConfig = db.Select<AiServiceConfig>().FirstOrDefault();
                    if (firstAiConfig == null)
                    {
                        // Create a default Gemini config if none exists
                        firstAiConfig = new AiServiceConfig
                        {
                            Name = "Default Gemini Config",
                            ProviderType = "Gemini",
                            ApiKey = "",
                            ModelId = "gemini-2.0-flash-exp"
                        };
                        firstAiConfig.Id = (int)db.Insert(firstAiConfig, true);
                    }

                    var siteConfig = new SiteConfig()
                    {
                        ActiveAiConfigId = firstAiConfig.Id,
                        GlobalFallbackModel = "",
                        KokoroVoice = "af_heart",
                        TranscriptionProvider = "Gemini"
                    };
                    siteConfigHolder.SiteConfig = siteConfig;
                    siteConfig.Id = (int)db.Insert(siteConfig, true);
                }
            }
        }
        catch (Exception)
        {
            // Ignore errors during startup - this can happen during migrations
            // The SiteConfigHolder will remain with a null SiteConfig, which services should handle
        }
    }
}
