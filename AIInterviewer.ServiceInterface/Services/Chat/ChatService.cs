using ServiceStack;
using AIInterviewer.ServiceModel.Tables.Configuration;
using AIInterviewer.ServiceModel.Types.Chat;
using System.Threading.Tasks;
using System;

namespace AIInterviewer.ServiceInterface.Services.Chat;

public class ChatService(SiteConfigHolder siteConfigHolder) : Service
{
    public async Task<TranscribeAudioResponse> Post(TranscribeAudioRequest request)
    {
        var client = siteConfigHolder.GetGeminiClient();

        // The DTO expects AudioData as a Base64 string
        if (string.IsNullOrEmpty(request.AudioData))
            throw new HttpError(400, "ValidationError", "AudioData is required.");

        byte[] audioBytes;
        try 
        {
            audioBytes = Convert.FromBase64String(request.AudioData);
        }
        catch (FormatException)
        {
            throw new HttpError(400, "ValidationError", "Invalid Base64 audio data.");
        }

        var prompt = "Transcribe the following audio exactly. Do not add any commentary.";
        var transcript = await client.GenerateTextFromAudioAsync(prompt, audioBytes, request.MimeType ?? "audio/webm");

        return new TranscribeAudioResponse
        {
            Transcript = transcript ?? ""
        };
    }
}
