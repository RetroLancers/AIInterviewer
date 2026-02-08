using ServiceStack.DataAnnotations;
using System;
using AIInterviewer.ServiceModel.Tables.Configuration;

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
    [ForeignKey(typeof(AiServiceConfig))]
    public int? AiConfigId { get; set; }

    [References(typeof(AiServiceConfig))]
    public AiServiceConfig AiConfig { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
