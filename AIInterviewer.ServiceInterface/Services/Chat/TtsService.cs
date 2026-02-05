using System.Diagnostics;
using ServiceStack;
using System.IO;
using KokoroSharp;
using KokoroSharp.Core;
using KokoroSharp.Processing;
using AIInterviewer.ServiceModel.Tables.Configuration;
using AIInterviewer.ServiceModel.Types.Chat;

public class TtsService(SiteConfigHolder siteConfigHolder) : Service
{
    private static KokoroTTS _tts;
    private static readonly object _lock = new();

    private KokoroTTS GetTts()
    {
        lock (_lock)
        {
            if (_tts == null)
            {
                // Load model. This might download the model if not present (~300MB) or use bundled if configured.
                // The nuget package wraps it.
                _tts = KokoroTTS.LoadModel(); 
                
                // Ensure voices are available? 
                // If GetVoice returns null or throws, we might need to handle it.
                // Assuming defaults work.
            }
            return _tts;
        }
    }

    public async Task<object> Post(TextToSpeechRequest request)
    {
        if (string.IsNullOrEmpty(request.Text))
            throw new HttpError(400, "ValidationError", "Text is required.");

        var voiceName = siteConfigHolder.SiteConfig?.KokoroVoice ?? "af_heart";

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
        catch(Exception)
        {
            voice = KokoroVoiceManager.GetVoice("af_heart"); 
        }

        if (voice == null) throw new HttpError(500, "VoiceError", "Could not load voice.");

        try 
        {
            var tokens = Tokenizer.Tokenize(request.Text);
            
            var audioData = new List<float>(); // Flattened
            
            // Callback for each segment
            Action<float[]> onAudioConfig = (samples) => {
                lock(audioData) {
                    audioData.AddRange(samples);
                }
            };

            var job = KokoroJob.Create(tokens, voice, 1.0f, onAudioConfig);
            
            tts.EnqueueJob(job);

            // Wait for completion
            // Job is run on background thread. We poll for completion.
            // Timeout after 30 seconds to be safe.
            var sw = Stopwatch.StartNew();
            while (!job.isDone && sw.Elapsed.TotalSeconds < 30)
            {
                await Task.Delay(50);
            }

            if (!job.isDone)
            {
                // Cleanup if possible? job.Cancel()?
                // job.Cancel(); // Private/Internal? 
                // Inspector said Cancel is public method on KokoroJob!
                job.Cancel();
                throw new HttpError(504, "Timeout", "TTS generation timed out.");
            }

            var allSamples = audioData.ToArray();
            var pcmBytes = KokoroPlayback.GetBytes(allSamples);
            var wavBytes = AddWavHeader(pcmBytes, KokoroPlayback.waveFormat.SampleRate);

            return new HttpResult(wavBytes, "audio/wav");
        }
        catch (Exception ex)
        {
            throw new HttpError(500, "TtsError", ex.Message);
        }
    }

    private byte[] AddWavHeader(byte[] pcmData, int sampleRate)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        
        writer.Write("RIFF".ToCharArray());
        writer.Write(36 + pcmData.Length);
        writer.Write("WAVE".ToCharArray());
        writer.Write("fmt ".ToCharArray());
        writer.Write(16);
        writer.Write((short)1); // AudioFormat 1 = PCM
        writer.Write((short)1); // NumChannels 1
        writer.Write(sampleRate);
        writer.Write(sampleRate * 1 * 16 / 8); // ByteRate
        writer.Write((short)(1 * 16 / 8)); // BlockAlign
        writer.Write((short)16); // BitsPerSample
        writer.Write("data".ToCharArray());
        writer.Write(pcmData.Length);
        writer.Write(pcmData);
        
        return stream.ToArray();
    }
}
