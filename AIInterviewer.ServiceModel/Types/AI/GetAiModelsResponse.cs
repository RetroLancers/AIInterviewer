using ServiceStack;
using System.Collections.Generic;

namespace AIInterviewer.ServiceModel.Types.AI;

public class GetAiModelsResponse
{
    public List<string> Models { get; set; } = new();
    public ResponseStatus ResponseStatus { get; set; }
}