using System.Collections.Generic;

namespace AIInterviewer.ServiceModel.Types.Interview;

public class ListInterviewersResponse
{
    public List<InterviewerResponse> Interviewers { get; set; }
}
