using System;

namespace AIInterviewer.ServiceModel.Types.Interview;

public class InterviewerResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SystemPrompt { get; set; }
    public int? AiConfigId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
