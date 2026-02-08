using ServiceStack;
using System.IO;

namespace AIInterviewer.ServiceModel.Types.Chat;

[Route("/chat/tts", "POST")]
public class TextToSpeechRequest : IReturn<Stream>
{
    public string Text { get; set; }
    public int? InterviewId { get; set; }
}
