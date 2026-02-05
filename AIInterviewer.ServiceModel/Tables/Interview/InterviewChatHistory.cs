using ServiceStack.DataAnnotations;

namespace AIInterviewer.ServiceModel.Tables.Interview;

[Alias("interviewchathistories")]
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
    public string Role { get; set; }

    [Required]
    public string Content { get; set; }
}
