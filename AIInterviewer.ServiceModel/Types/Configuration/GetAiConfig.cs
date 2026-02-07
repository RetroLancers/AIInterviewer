using ServiceStack;

namespace AIInterviewer.ServiceModel.Types.Configuration;

[Route("/api/config/ai/{Id}", "GET")]
public class GetAiConfig : IReturn<AiConfigResponse>
{
    public int Id { get; set; }
}
