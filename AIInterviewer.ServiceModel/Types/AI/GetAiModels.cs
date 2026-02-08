using ServiceStack;
using System.Collections.Generic;

namespace AIInterviewer.ServiceModel.Types.AI;

[Route("/ai/models", "GET")]
public class GetAiModels : IReturn<GetAiModelsResponse>
{
    public string? ProviderType { get; set; }
    public string? ApiKey { get; set; }
}