using ServiceStack.DataAnnotations;
using System;

namespace AIInterviewer.ServiceModel.Tables.Interview;

[Alias("interviewer")]
public class Interviewer
{
    [AutoIncrement]
    [PrimaryKey]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [Required]
    [StringLength(8000)]
    public string SystemPrompt { get; set; }

    /// <summary>
    /// Optional AI config override. If null, uses site default.
    /// </summary>
    public int? AiConfigId { get; set; }

    /// <summary>
    /// Optional user ID if we want to support per-user interviewers in the future
    /// </summary>
    [StringLength(255)]
    public string? UserId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
