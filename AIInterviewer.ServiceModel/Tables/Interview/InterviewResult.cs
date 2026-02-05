using ServiceStack.DataAnnotations;
<<<<<<< HEAD

namespace AIInterviewer.ServiceModel.Tables.Interview;

[Alias("interviewresults")]
=======
using System;

namespace AIInterviewer.ServiceModel.Tables.Interview;

[Alias("interview_result")]
>>>>>>> feature/interview-completion-and-report
public class InterviewResult
{
    [AutoIncrement]
    [PrimaryKey]
    public int Id { get; set; }

    [ForeignKey(typeof(Interview), OnDelete = "Cascade")]
    public int InterviewId { get; set; }

    [References(typeof(Interview))]
    public Interview Interview { get; set; }

    [Required]
    public string ReportText { get; set; }

    public int Score { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }
}
