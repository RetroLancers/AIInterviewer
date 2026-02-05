using ServiceStack;

namespace TyphoonSharp.ServiceModel.Types.Configuration;

public class SiteConfigResponse
{
    public int Id { get; set; }
    public string GeminiApiKey { get; set; }
    public string InterviewModel { get; set; }
    public string? GlobalFallbackModel { get; set; }
}
