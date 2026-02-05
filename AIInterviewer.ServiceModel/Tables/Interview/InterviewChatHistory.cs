using ServiceStack.DataAnnotations;
using System;

namespace AIInterviewer.ServiceModel.Tables.Interview;

[Alias("interview_chat_history")]
public class InterviewChatHistory
{
    [AutoIncrement]
    [PrimaryKey]
    public int Id { get; set; }

    [ForeignKey(typeof(Interview), OnDelete = "Cascade")]
    public int InterviewId { get; set; }

    [References(typeof(Interview))] 
    public Interview Interview { get; set; }

    [Required]
    public DateTime EntryDate { get; set; }

    [Required]
    [StringLength(50)]
    public string Role { get; set; }

    [Required]
    public string Content { get; set; }
}