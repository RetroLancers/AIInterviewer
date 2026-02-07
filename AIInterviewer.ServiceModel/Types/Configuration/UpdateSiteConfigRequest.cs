using ServiceStack; 

namespace AIInterviewer.ServiceModel.Types.Configuration;

[Route("/configuration/site-config/{Id}", "PUT")]
public class UpdateSiteConfigRequest : IReturn<IdResponse>
{
    public int Id { get; set; }

    public int? ActiveAiConfigId { get; set; }

    public string? GeminiApiKey { get; set; }

    public string? InterviewModel { get; set; }

    public string? GlobalFallbackModel { get; set; }

    public string? KokoroVoice { get; set; }

    [ValidateNotEmpty]
    public string TranscriptionProvider { get; set; }
}
