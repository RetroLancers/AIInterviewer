# KokoroSharp TTS Integration

KokoroSharp is a fully-featured inference engine for Kokoro TTS, built entirely in C# with ONNX runtime. It enables developers to perform flexible and fast text-to-speech synthesis utilizing multiple speakers and languages.

## Key Features
*   **Plug & Play**: Nuget packages (`KokoroSharp.CPU`, `KokoroSharp.GPU`, etc.) handle dependencies.
*   **Voices**: Includes all voices from hexgrad's Kokoro 82M v1.0 release.
*   **Performance**: Text-segment streaming for near-instant responses.
*   **Voice Mixing**: Unlimited voice mixing capabilities with save/load support.
*   **Job Scheduling**: Linear job scheduling with a background dispatcher.
*   **Multi-platform**: Supports Windows, Linux, and MacOS. Integrated phonemization via eSpeak NG.

## Supported Languages/Accents
American English, British English, Mandarin Chinese, Japanese, Hindi, Spanish, French, Italian, Brazilian/Portuguese.

## Usage Patterns

### Basic Setup (CPU)
```csharp
KokoroTTS tts = KokoroTTS.LoadModel(); // Downloads model if not found (~320MB)
KokoroVoice voice = KokoroVoiceManager.GetVoice("af_heart");
tts.SpeakFast("Hello world!", voice);
```

### GPU Support (CUDA)
For NVIDIA GPUs, requires CUDA Toolkit (v12.x) and cuDNN in SYSTEM PATH.
```csharp
var options = new SessionOptions();
options.AppendExecutionProvider_CUDA();
KokoroTTS tts = KokoroTTS.LoadModel(sessionOptions: options);
```

## AI Interviewer Implementation Plan
*   **SiteConfig**: Store `KokoroEngine` (CPU, GPU, etc.) and `KokoroVoice` (e.g., `af_heart`) in the database.
*   **TTS Service**: Create a backend service that takes text, processes it through KokoroSharp, and returns/streams the audio.
*   **Frontend Integration**: The chat UI will fetch audio from this service to "speak" Gemini's responses.
