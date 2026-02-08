using ServiceStack;

namespace AIInterviewer.ServiceModel.Types.Interview;

[Route("/api/interviewers/{Id}", "PUT")]
public class UpdateInterviewer : IReturn<InterviewerResponse>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SystemPrompt { get; set; }
    public int? AiConfigId { get; set; }
}
