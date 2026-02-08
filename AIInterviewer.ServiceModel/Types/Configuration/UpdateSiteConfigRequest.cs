using ServiceStack; 

namespace AIInterviewer.ServiceModel.Types.Configuration;

[Route("/configuration/site-config/{Id}", "PUT")]
public class UpdateSiteConfigRequest : IReturn<IdResponse>
{
    public int Id { get; set; }

    [ValidateGreaterThan(0)]
    public int ActiveAiConfigId { get; set; }

    public string? GlobalFallbackModel { get; set; }

    public string? DefaultVoice { get; set; }

    [ValidateNotEmpty]
    public string TranscriptionProvider { get; set; }
}
