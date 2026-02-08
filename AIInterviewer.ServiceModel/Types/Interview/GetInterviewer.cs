using ServiceStack;

namespace AIInterviewer.ServiceModel.Types.Interview;

[Route("/api/interviewers/{Id}", "GET")]
public class GetInterviewer : IReturn<InterviewerResponse>
{
    public int Id { get; set; }
}
