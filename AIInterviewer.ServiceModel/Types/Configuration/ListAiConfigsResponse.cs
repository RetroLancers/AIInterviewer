using System.Collections.Generic;

namespace AIInterviewer.ServiceModel.Types.Configuration;

public class ListAiConfigsResponse
{
    public List<AiConfigResponse> Configs { get; set; } = new();
}
