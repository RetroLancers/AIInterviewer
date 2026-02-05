using ServiceStack.DataAnnotations;
using System;

namespace AIInterviewer.ServiceModel.Tables.Interview;

[Alias("interview")]
public class Interview
{
    [AutoIncrement]
    [PrimaryKey]
    public int Id { get; set; }

    [Required]
    [StringLength(8000)]
    public string Prompt { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    [StringLength(255)]
    public string? UserId { get; set; }
}
