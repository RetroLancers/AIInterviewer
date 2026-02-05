using System.Linq;
using System.Collections.Generic;
using TableNamespace = AIInterviewer.ServiceModel.Tables.Interview;

namespace AIInterviewer.ServiceModel.Types.Interview.ExtensionMethods;

public static class InterviewExtensions
{
    public static InterviewDto ToDto(this TableNamespace.Interview table)
    {
        if (table == null) return null;
        return new InterviewDto
        {
            Id = table.Id,
            Prompt = table.Prompt,
            CreatedDate = table.CreatedDate
        };
    }

    public static InterviewChatHistoryDto ToDto(this TableNamespace.InterviewChatHistory table)
    {
        if (table == null) return null;
        return new InterviewChatHistoryDto
        {
            Id = table.Id,
            Role = table.Role,
            Content = table.Content,
            EntryDate = table.EntryDate
        };
    }

    public static List<InterviewChatHistoryDto> ToDto(this IEnumerable<TableNamespace.InterviewChatHistory> tables)
    {
        if (tables == null) return new List<InterviewChatHistoryDto>();
        return tables.Select(t => t.ToDto()).ToList();
    }

    public static InterviewResultDto ToDto(this TableNamespace.InterviewResult table)
    {
        if (table == null) return null;
        return new InterviewResultDto
        {
            Id = table.Id,
            InterviewId = table.InterviewId,
            ReportText = table.ReportText,
            Score = table.Score,
            CreatedDate = table.CreatedDate
        };
    }
}
