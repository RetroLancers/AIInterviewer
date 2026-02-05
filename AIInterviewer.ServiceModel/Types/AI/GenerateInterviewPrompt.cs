using ServiceStack;

namespace AIInterviewer.ServiceModel.Types.AI;

[Route("/ai/generate-prompt", "POST")]
public class GenerateInterviewPrompt : IPost, IReturn<GenerateInterviewPromptResponse>
{
    public string Context { get; set; }
}
