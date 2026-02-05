using System;

namespace AIInterviewer.ServiceModel.Types.Interview;

public class InterviewDto
{
    public int Id { get; set; }
    public string Prompt { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class InterviewChatHistoryDto
{
    public int Id { get; set; }
    public string Role { get; set; }
    public string Content { get; set; }
    public DateTime EntryDate { get; set; }
}
