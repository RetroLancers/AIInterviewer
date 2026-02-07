using ServiceStack;

namespace AIInterviewer.ServiceModel.Types.Configuration;

[Route("/api/config/ai/{Id}", "DELETE")]
public class DeleteAiConfig : IReturnVoid
{
    public int Id { get; set; }
}
