using AIInterviewer.ServiceModel.Tables.Configuration;
using AIInterviewer.ServiceModel.Types.Configuration;
using AIInterviewer.ServiceModel.Types.Configuration.ExtensionMethods;
using ServiceStack;
using ServiceStack.OrmLite;
using Microsoft.Extensions.Logging;

namespace AIInterviewer.ServiceInterface.Services.Configuration;

[ValidateHasRole("Admin")]
public class SiteConfigService(SiteConfigHolder holder, ILogger<SiteConfigService> logger) : Service
{
    public SiteConfigResponse Get(GetSiteConfigRequest request)
    {
        return holder.SiteConfig?.ToDto() ?? new SiteConfigResponse();
    }

    public void Put(UpdateSiteConfigRequest request)
    {
        logger.LogInformation("Updating site configuration");
        using var trans = Db.OpenTransaction();
        try
        {
            var config = request.ToTable();
            Db.Save(config);
            trans.Commit();

            holder.SiteConfig = config;
            logger.LogInformation("Site configuration updated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating site configuration");
            trans.Rollback();
            throw;
        }
    }
}
