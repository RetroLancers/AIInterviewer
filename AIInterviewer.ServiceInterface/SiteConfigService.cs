using AIInterviewer.ServiceModel.Tables.Configuration;
using AIInterviewer.ServiceModel.Types.Configuration;
using AIInterviewer.ServiceModel.Types.Configuration.ExtensionMethods;
using ServiceStack;
using ServiceStack.OrmLite;

namespace AIInterviewer.ServiceInterface;

public class SiteConfigService(SiteConfigHolder holder) : Service
{
    public SiteConfigResponse Get(GetSiteConfigRequest request)
    {
        return holder.SiteConfig?.ToDto() ?? new SiteConfigResponse();
    }

    public void Put(UpdateSiteConfigRequest request)
    {
        var config = request.ToTable();
        Db.Save(config);
        holder.SiteConfig = config;
    }
}