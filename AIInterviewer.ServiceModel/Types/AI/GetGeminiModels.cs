using ServiceStack;
using System.Collections.Generic;

namespace AIInterviewer.ServiceModel.Types.AI;

[Route("/ai/models", "GET")]
public class GetGeminiModels : IReturn<GetGeminiModelsResponse>
{
    public string? ApiKey { get; set; }
}
