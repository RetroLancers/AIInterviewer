using ServiceStack;

namespace AIInterviewer.ServiceModel.Types;

[Route("/interview/tts", "GET")]
[Route("/interview/tts/{Text*}", "GET")]
public class TextToSpeech : IGet, IReturn<byte[]>
{
    public string Text { get; set; }
}
