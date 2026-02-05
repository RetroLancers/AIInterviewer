using ServiceStack; 

namespace AIInterviewer.ServiceModel.Types.Configuration;

[Route("/configuration/site-config/{Id}", "PUT")]
public class UpdateSiteConfigRequest : IReturn<IdResponse>
{
    public int Id { get; set; }

    [ValidateNotEmpty]
    public string GeminiApiKey { get; set; }

    [ValidateNotEmpty]
    public string InterviewModel { get; set; }

    public string? GlobalFallbackModel { get; set; }
}
