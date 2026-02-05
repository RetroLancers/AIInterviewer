using ServiceStack;
using System.Collections.Generic;

namespace AIInterviewer.ServiceModel.Types.Interview;

[Route("/interviews/{Id}/finish", "POST")]
public class FinishInterview : IReturn<FinishInterviewResponse>
{
    public int Id { get; set; }
}

public class FinishInterviewResponse
{
    public InterviewResultDto? Result { get; set; }
}
