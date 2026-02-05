using ServiceStack;

namespace AIInterviewer.ServiceModel.Types.Chat;

[Route("/chat/transcribe", "POST")]
public class TranscribeAudioRequest : IReturn<TranscribeAudioResponse>
{
    public string AudioData { get; set; } // Base64 encoded audio
    public string MimeType { get; set; } // e.g. "audio/webm" or "audio/mp3"
}
