using ServiceStack;

namespace AIInterviewer.ServiceModel.Types.Interview;

[Route("/api/interviewers", "GET")]
public class ListInterviewers : IReturn<ListInterviewersResponse>
{
}
