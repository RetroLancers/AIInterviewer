using System;
using System.Threading.Tasks;
using AIInterviewer.ServiceModel.Tables.Configuration;
using AIInterviewer.ServiceModel.Types.Chat;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace AIInterviewer.ServiceInterface.Services.Chat;

public class ChatService(SiteConfigHolder siteConfigHolder, ILogger<ChatService> logger) : Service
{
    public async Task<TranscribeAudioResponse> Post(TranscribeAudioRequest request)
    {
        if (siteConfigHolder.SiteConfig?.TranscriptionProvider == "Browser")
        {
            logger.LogInformation("Server-side transcription requested while transcription provider is set to Browser.");
        }

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
        
        if (string.IsNullOrEmpty(transcript))
        {
            logger.LogWarning("Gemini failed to return a transcript for the audio data.");
        }
        else
        {
            logger.LogInformation("Audio transcription completed. Transcript length: {Length}", transcript.Length);
        }

        return new TranscribeAudioResponse
        {
            Transcript = transcript ?? ""
        };
    }
}
