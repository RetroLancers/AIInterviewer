using ServiceStack;

namespace AIInterviewer.ServiceModel.Types.Interview;

[Route("/api/interviewers/{Id}", "DELETE")]
public class DeleteInterviewer : IReturnVoid
{
    public int Id { get; set; }
}
