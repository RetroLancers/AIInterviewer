using ServiceStack.DataAnnotations;

namespace AIInterviewer.ServiceModel.Tables.Configuration;

[Alias("ai_service_config")]
public class AiServiceConfig
{
    [AutoIncrement]
    [PrimaryKey]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [Required]
    [StringLength(50)]
    public string ProviderType { get; set; } // "Gemini", "OpenAI"

    [Required]
    [StringLength(2096)]
    public string ApiKey { get; set; }

    [Required]
    [StringLength(255)]
    public string ModelId { get; set; }

    [StringLength(255)]
    public string? FallbackModelId { get; set; }

    [StringLength(2096)]
    public string? BaseUrl { get; set; }
}
