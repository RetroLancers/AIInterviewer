using ServiceStack;
using AIInterviewer.ServiceModel.Tables.Configuration;
using AIInterviewer.ServiceModel.Types.Chat;

namespace AIInterviewer.ServiceInterface.Services.Chat;

public class ChatService(SiteConfigHolder siteConfigHolder) : Service
{
    public async Task<TranscribeAudioResponse> Post(TranscribeAudioRequest request)
    {
        var config = siteConfigHolder.SiteConfig;
        if (config == null || string.IsNullOrEmpty(config.GeminiApiKey))
            throw new HttpError(400, "ConfigurationError", "Site configuration is missing or API Key is not set.");

        var client = new GeminiClient(config.GeminiApiKey, config.InterviewModel ?? "gemini-2.5-flash");

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
