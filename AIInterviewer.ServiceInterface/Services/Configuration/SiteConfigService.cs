using AIInterviewer.ServiceModel.Tables.Configuration;
using AIInterviewer.ServiceModel.Types.Configuration;
using AIInterviewer.ServiceModel.Types.Configuration.ExtensionMethods;
using ServiceStack;
using ServiceStack.OrmLite;

namespace AIInterviewer.ServiceInterface.Services.Configuration;

[ValidateHasRole("Admin")]
public class SiteConfigService(SiteConfigHolder holder) : Service
{
    public SiteConfigResponse Get(GetSiteConfigRequest request)
    {
        return holder.SiteConfig?.ToDto() ?? new SiteConfigResponse();
    }

    public void Put(UpdateSiteConfigRequest request)
    {
        using var trans = Db.OpenTransaction();
        var config = request.ToTable();
        Db.Save(config);
        trans.Commit();

        holder.SiteConfig = config;
    }
}
