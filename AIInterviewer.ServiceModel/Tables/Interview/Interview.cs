using ServiceStack.DataAnnotations;
<<<<<<< HEAD

namespace AIInterviewer.ServiceModel.Tables.Interview;

[Alias("interviews")]
=======
using System;

namespace AIInterviewer.ServiceModel.Tables.Interview;

[Alias("interview")]
>>>>>>> feature/interview-completion-and-report
public class Interview
{
    [AutoIncrement]
    [PrimaryKey]
    public int Id { get; set; }

    [Required]
<<<<<<< HEAD
=======
    [StringLength(8000)]
>>>>>>> feature/interview-completion-and-report
    public string Prompt { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

<<<<<<< HEAD
=======
    [StringLength(255)]
>>>>>>> feature/interview-completion-and-report
    public string? UserId { get; set; }
}
