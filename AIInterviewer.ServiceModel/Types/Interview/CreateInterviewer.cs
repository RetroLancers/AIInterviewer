using ServiceStack;

namespace AIInterviewer.ServiceModel.Types.Interview;

[Route("/api/interviewers", "POST")]
public class CreateInterviewer : IReturn<InterviewerResponse>
{
    public string Name { get; set; }
    public string SystemPrompt { get; set; }
    public int? AiConfigId { get; set; }
}
