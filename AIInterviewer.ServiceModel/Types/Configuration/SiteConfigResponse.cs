namespace AIInterviewer.ServiceModel.Types.Configuration;

public class SiteConfigResponse
{
    public int Id { get; set; }
    public int? ActiveAiConfigId { get; set; }
    public string? GeminiApiKey { get; set; }
    public string? InterviewModel { get; set; }
    public string? GlobalFallbackModel { get; set; }
    public string? KokoroVoice { get; set; }
    public string TranscriptionProvider { get; set; } = "Gemini";
}
