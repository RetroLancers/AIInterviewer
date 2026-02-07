namespace AIInterviewer.ServiceModel.Types.Ai;

public class AiMessage
{
    public AiRole Role { get; set; }
    public string Content { get; set; } = string.Empty;
}
