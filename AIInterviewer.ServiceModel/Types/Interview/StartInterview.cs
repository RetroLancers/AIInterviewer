using ServiceStack;
using System.Collections.Generic;

namespace AIInterviewer.ServiceModel.Types.Interview;

[Route("/interview/{InterviewId}/start", "POST")]
public class StartInterview : IReturn<StartInterviewResponse>
{
    public int InterviewId { get; set; }
}

public class StartInterviewResponse
{
    public List<InterviewChatHistoryDto> History { get; set; } = [];
}
