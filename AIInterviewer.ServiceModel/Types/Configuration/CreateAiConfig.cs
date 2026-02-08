using ServiceStack;

namespace AIInterviewer.ServiceModel.Types.Configuration;

[Route("/api/config/ai", "POST")]
public class CreateAiConfig : IReturn<AiConfigResponse>
{
    public string Name { get; set; }
    public string ProviderType { get; set; }
    public string ApiKey { get; set; }
    public string ModelId { get; set; }
    public string? FallbackModelId { get; set; }
    public string? BaseUrl { get; set; }
}
