namespace AIInterviewer.ServiceModel.Types.Configuration;

public class AiConfigResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ProviderType { get; set; }
    public string ApiKey { get; set; }
    public string ModelId { get; set; }
    public string? BaseUrl { get; set; }
}
