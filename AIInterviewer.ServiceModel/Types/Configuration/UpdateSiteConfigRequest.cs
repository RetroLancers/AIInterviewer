using ServiceStack;

namespace TyphoonSharp.ServiceModel.Types.Configuration;

[Route("/configuration/site-config/{Id}", "PUT")]
public class UpdateSiteConfigRequest : IReturn<IdResponse>
{
    public int Id { get; set; }
    public string GeminiApiKey { get; set; }


    public string InterviewModel { get; set; }
    public string? GlobalFallbackModel { get; set; }
}