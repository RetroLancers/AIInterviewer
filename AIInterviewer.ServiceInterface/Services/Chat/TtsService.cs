using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ServiceStack;
using ServiceStack.OrmLite;
using KokoroSharp;
using KokoroSharp.Core;
using KokoroSharp.Processing;
using AIInterviewer.ServiceModel.Tables.Configuration;
using AIInterviewer.ServiceModel.Types.Chat;
using AIInterviewer.ServiceInterface.Utilities;
using Microsoft.Extensions.Logging;
using AIInterviewer.ServiceModel.Tables.Interview;

namespace AIInterviewer.ServiceInterface.Services.Chat;
 

public class TtsService(SiteConfigHolder siteConfigHolder, ILogger<TtsService> logger) : Service
{
    private static KokoroTTS? _tts;
    private static readonly object _lock = new();
    private const int MaxChunkLength = 600;

    // Use compiled regex to avoid recompilation overhead
    private static readonly Regex _sentenceSplitter = new Regex("(?<=[.!?])\\s+", RegexOptions.Compiled);
 

    private static KokoroTTS GetTts()
    {
        lock (_lock)
        {
            _tts ??= KokoroTTS.LoadModel();
            return _tts;
        }
    }
 
    public async Task<HttpResult> Post(TextToSpeechRequest request)
    {
        logger.LogTrace("Processing TTS request for text: {TextSnippet}...", request.Text?.Substring(0, Math.Min(request.Text.Length, 30)));
        if (string.IsNullOrEmpty(request.Text))
            throw new HttpError(400, "ValidationError", "Text is required.");

        var sanitizedText = SanitizeForSpeech(request.Text);
        if (string.IsNullOrWhiteSpace(sanitizedText))
            return new HttpResult { StatusCode = System.Net.HttpStatusCode.NoContent };

        string? voiceName = null;

        // 1. Try to get voice from interview's AI config
        if (request.InterviewId.HasValue)
        {
             var interview = await Db.SingleByIdAsync<AIInterviewer.ServiceModel.Tables.Interview.Interview>(request.InterviewId.Value);
             if (interview?.AiConfigId != null)
             {
                 var aiConfig = await Db.SingleByIdAsync<AiServiceConfig>(interview.AiConfigId.Value);
                 voiceName = aiConfig?.Voice;
             }
        }

        // 2. Fall back to site default voice
        if (string.IsNullOrEmpty(voiceName))
        {
             voiceName = siteConfigHolder.SiteConfig?.DefaultVoice;
        }

        // 3. Final fallback
        if (string.IsNullOrEmpty(voiceName))
        {
            voiceName = "af_heart";
        }

        var tts = GetTts();
        KokoroVoice voice;
        try 
        {
             voice = KokoroVoiceManager.GetVoice(voiceName);
             if (voice == null)
             {
                 // Fallback to default if not found
                 voice = KokoroVoiceManager.GetVoice("af_heart");
             }
        }
        catch(Exception ex)
        {
            logger.LogWarning(ex, "Failed to load voice {VoiceName}, falling back to af_heart", voiceName);
            voice = KokoroVoiceManager.GetVoice("af_heart"); 
        }

        if (voice == null) throw new HttpError(500, "VoiceError", "Could not load voice.");

        try 
        {
            var audioData = new List<float>(); // Flattened

            foreach (var chunk in SplitIntoChunks(sanitizedText, MaxChunkLength))
            {
                var chunkSamples = await GenerateAudioSamples(tts, voice, chunk);
                audioData.AddRange(chunkSamples);
            }

            var allSamples = audioData.ToArray();
            var pcmBytes = KokoroPlayback.GetBytes(allSamples);
            var wavBytes = WavUtils.AddWavHeader(pcmBytes, KokoroPlayback.waveFormat.SampleRate);

            return new HttpResult(wavBytes, "audio/wav");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during TTS generation");
            throw new HttpError(500, "TtsError", ex.Message);
        }
    }

    private static string SanitizeForSpeech(string text)
    {
        var withoutCodeBlocks = Regex.Replace(text, "```[\\s\\S]*?```", " ");
        return Regex.Replace(withoutCodeBlocks, "\\s+", " ").Trim();
    }

    private static IEnumerable<string> SplitIntoChunks(string text, int maxLength)
    {
        var sentences = _sentenceSplitter.Split(text);
        var current = new StringBuilder();

        foreach (var sentence in sentences)
        {
            var trimmed = sentence.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            if (trimmed.Length > maxLength)
            {
                if (current.Length > 0)
                {
                    yield return current.ToString();
                    current.Clear();
                }

                for (var i = 0; i < trimmed.Length; i += maxLength)
                {
                    var length = Math.Min(maxLength, trimmed.Length - i);
                    yield return trimmed.Substring(i, length);
                }
                continue;
            }

            if (current.Length + trimmed.Length + 1 > maxLength && current.Length > 0)
            {
                yield return current.ToString();
                current.Clear();
            }

            if (current.Length > 0)
                current.Append(' ');
            current.Append(trimmed);
        }

        if (current.Length > 0)
            yield return current.ToString();
    }

    private async Task<List<float>> GenerateAudioSamples(KokoroTTS tts, KokoroVoice voice, string text)
    {
        var tokens = Tokenizer.Tokenize(text);
        var chunkSamples = new List<float>();

        Action<float[]> onAudioConfig = (samples) => {
            lock (chunkSamples)
            {
                chunkSamples.AddRange(samples);
            }
        };

        var job = KokoroJob.Create(tokens, voice, 1.0f, onAudioConfig);
        tts.EnqueueJob(job);

        var sw = Stopwatch.StartNew();
        while (!job.isDone && sw.Elapsed.TotalSeconds < 30)
        {
            await Task.Delay(50);
        }

        if (job.isDone) return chunkSamples;
        
        logger.LogError("TTS generation timed out after 30 seconds for text: {TextSnippet}", text.Substring(0, Math.Min(text.Length, 50)));
        job.Cancel();
        throw new HttpError(504, "Timeout", "TTS generation timed out.");
    }
}
