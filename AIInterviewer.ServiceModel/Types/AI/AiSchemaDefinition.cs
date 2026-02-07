namespace AIInterviewer.ServiceModel.Types.Ai;

public class AiSchemaDefinition
{
    public string Type { get; set; } = "object";
    public string? Description { get; set; }
    public Dictionary<string, AiSchemaDefinition>? Properties { get; set; }
    public List<string>? Required { get; set; }
    public AiSchemaDefinition? Items { get; set; }
    public bool? AdditionalProperties { get; set; }
    public List<string>? Enum { get; set; }
}
