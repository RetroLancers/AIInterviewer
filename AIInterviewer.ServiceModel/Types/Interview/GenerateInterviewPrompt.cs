using ServiceStack;

namespace AIInterviewer.ServiceModel.Types.Interview;

[Route("/interview/generate-prompt", "POST")]
public class GenerateInterviewPrompt : IReturn<GenerateInterviewPromptResponse>
{
    public string TargetRole { get; set; }
    public string? Context { get; set; }
}

public class GenerateInterviewPromptResponse
{
    public string SystemPrompt { get; set; }
}
