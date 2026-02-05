using ServiceStack;

namespace AIInterviewer.ServiceModel.Types.AI;

public class GenerateInterviewPromptResponse
{
    public string Prompt { get; set; }
    public ResponseStatus ResponseStatus { get; set; }
}
