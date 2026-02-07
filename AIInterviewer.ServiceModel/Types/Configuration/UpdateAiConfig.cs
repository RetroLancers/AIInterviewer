using ServiceStack;

namespace AIInterviewer.ServiceModel.Types.Configuration;

[Route("/api/config/ai/{Id}", "PUT")]
public class UpdateAiConfig : IReturn<AiConfigResponse>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ProviderType { get; set; }
    public string ApiKey { get; set; }
    public string ModelId { get; set; }
    public string? BaseUrl { get; set; }
}
