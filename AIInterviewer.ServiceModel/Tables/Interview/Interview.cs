using ServiceStack.DataAnnotations;

namespace AIInterviewer.ServiceModel.Tables.Interview;

[Alias("interviews")]
public class Interview
{
    [AutoIncrement]
    [PrimaryKey]
    public int Id { get; set; }

    [Required]
    public string Prompt { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    public string? UserId { get; set; }
}
