using ServiceStack;
using System;
using System.Collections.Generic;

namespace AIInterviewer.ServiceModel.Types.Interview;

[Route("/interview/{Id}", "GET")]
public class GetInterview : IReturn<GetInterviewResponse>
{
    public int Id { get; set; }
}

public class GetInterviewResponse
{
    public InterviewDto Interview { get; set; }
    public List<InterviewChatHistoryDto> History { get; set; }
}
