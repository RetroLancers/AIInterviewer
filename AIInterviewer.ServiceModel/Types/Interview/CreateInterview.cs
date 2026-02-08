using ServiceStack;

namespace AIInterviewer.ServiceModel.Types.Interview;

[Route("/interview", "POST")]
public class CreateInterview : IReturn<CreateInterviewResponse>
{
    public string SystemPrompt { get; set; }
    public string? UserId { get; set; }

    /// <summary>
    /// Optional AI config to use for this interview. If null, uses site default.
    /// </summary>
    public int? AiConfigId { get; set; }
}

public class CreateInterviewResponse
{
    public int Id { get; set; }
}
