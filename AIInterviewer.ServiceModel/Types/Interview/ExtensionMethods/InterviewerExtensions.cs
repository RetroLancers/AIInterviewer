using System.Collections.Generic;
using System.Linq;
using AIInterviewer.ServiceModel.Tables.Interview;
using AIInterviewer.ServiceModel.Types.Interview;

namespace AIInterviewer.ServiceModel.Types.Interview.ExtensionMethods;

public static class InterviewerExtensions
{
    public static InterviewerResponse ToDto(this Interviewer interviewer)
    {
        return new InterviewerResponse
        {
            Id = interviewer.Id,
            Name = interviewer.Name,
            SystemPrompt = interviewer.SystemPrompt,
            AiConfigId = interviewer.AiConfigId,
            CreatedAt = interviewer.CreatedAt,
            UpdatedAt = interviewer.UpdatedAt
        };
    }

    public static List<InterviewerResponse> ToDto(this IEnumerable<Interviewer> interviewers)
    {
        return interviewers.Select(x => x.ToDto()).ToList();
    }

    public static Interviewer ToTable(this CreateInterviewer request)
    {
        return new Interviewer
        {
            Name = request.Name,
            SystemPrompt = request.SystemPrompt,
            AiConfigId = request.AiConfigId
        };
    }

    public static void UpdateTable(this Interviewer interviewer, UpdateInterviewer request)
    {
        interviewer.Name = request.Name;
        interviewer.SystemPrompt = request.SystemPrompt;
        interviewer.AiConfigId = request.AiConfigId;
    }
}
