using ServiceStack;

namespace TyphoonSharp.ServiceModel.Types.Configuration;

[Route("/configuration/site-config/{Id}", "GET")]
public class GetSiteConfigRequest : IReturn<SiteConfigResponse>
{
    public int Id { get; set; }
}
