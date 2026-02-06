using ServiceStack;
using System.Collections.Generic;

namespace AIInterviewer.ServiceModel.Types.Interview;

[Route("/interviews/history", "GET")]
public class GetInterviewHistory : IReturn<GetInterviewHistoryResponse>
{
    public int? Limit { get; set; }
    public int? Offset { get; set; }
}

public class GetInterviewHistoryResponse
{
    public List<InterviewDto> Interviews { get; set; } = [];
}
