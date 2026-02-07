using ServiceStack;

namespace AIInterviewer.ServiceModel.Types.Configuration;

[Route("/api/config/ai", "GET")]
public class ListAiConfigs : IReturn<ListAiConfigsResponse>
{
}
