using ServiceStack;
using System.Collections.Generic;

namespace AIInterviewer.ServiceModel.Types.Interview;

[Route("/interview/{InterviewId}/chat", "POST")]
public class AddChatMessage : IReturn<AddChatMessageResponse>
{
    public int InterviewId { get; set; }
    public string Message { get; set; }
}

public class AddChatMessageResponse
{
    public List<InterviewChatHistoryDto> History { get; set; }
}
