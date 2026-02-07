using ServiceStack.DataAnnotations;
 
namespace AIInterviewer.ServiceModel.Tables.Configuration;

[Alias("siteconfig")]
public class SiteConfig
{
    [AutoIncrement]
    [PrimaryKey]
    public int Id { get; set; }

    public int? ActiveAiConfigId { get; set; }

    [StringLength(2096)]
    public string? GeminiApiKey { get; set; }

    [StringLength(255)]
    public string? InterviewModel { get; set; }

    [StringLength(255)]
    public string? GlobalFallbackModel { get; set; }

    [StringLength(255)]
    public string? KokoroVoice { get; set; }

    [Required]
    [StringLength(64)]
    public string TranscriptionProvider { get; set; } = "Gemini";
}
