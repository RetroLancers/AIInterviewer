using ServiceStack.DataAnnotations;
 
namespace AIInterviewer.ServiceModel.Tables.Configuration;

[Alias("siteconfig")]
public class SiteConfig
{
    [AutoIncrement]
    [PrimaryKey]
    public int Id { get; set; }

    [Required]
    [References(typeof(AiServiceConfig))]
    public int ActiveAiConfigId { get; set; }

    [StringLength(255)]
    public string? GlobalFallbackModel { get; set; }

    [StringLength(100)]
    public string? DefaultVoice { get; set; }

    [Required]
    [StringLength(64)]
    public string TranscriptionProvider { get; set; } = "Gemini";
}
