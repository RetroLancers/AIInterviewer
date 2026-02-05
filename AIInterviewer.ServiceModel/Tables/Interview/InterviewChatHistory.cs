using ServiceStack.DataAnnotations;
<<<<<<< HEAD

namespace AIInterviewer.ServiceModel.Tables.Interview;

[Alias("interviewchathistories")]
=======
using System;

namespace AIInterviewer.ServiceModel.Tables.Interview;

[Alias("interview_chat_history")]
>>>>>>> feature/interview-completion-and-report
public class InterviewChatHistory
{
    [AutoIncrement]
    [PrimaryKey]
    public int Id { get; set; }

    [ForeignKey(typeof(Interview), OnDelete = "Cascade")]
    public int InterviewId { get; set; }

<<<<<<< HEAD
    [References(typeof(Interview))]
=======
    [References(typeof(Interview))] 
>>>>>>> feature/interview-completion-and-report
    public Interview Interview { get; set; }

    [Required]
    public DateTime EntryDate { get; set; }

    [Required]
<<<<<<< HEAD
=======
    [StringLength(50)]
>>>>>>> feature/interview-completion-and-report
    public string Role { get; set; }

    [Required]
    public string Content { get; set; }
}
