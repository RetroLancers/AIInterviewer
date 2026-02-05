# Task: Implement TTS Service with KokoroSharp

## Objective
Create a backend service that utilizes KokoroSharp to convert text responses from Gemini into audio.

## Requirements
- Create `TextToSpeech` Request DTO in `AIInterviewer.ServiceModel/Types/Interview.cs`:
    - Takes `string Text`.
    - Returns audio stream or byte array.
- Implement `TtsService` in `AIInterviewer.ServiceInterface/TtsService.cs`.
- Logic:
    - Load KokoroSharp model using CPU by default.
    - Use `SiteConfig.KokoroVoice` for synthesis.
    - Synthesize the provided text.
    - Return the audio data (e.g., as a WAV stream).
- Optimization: Consider caching the TTS engines/models in memory (singleton/static) to avoid reloading for every request.
- Note: GPU/CUDA support is currently an optional enhancement.

## Checklist
- [ ] Define `TextToSpeech` DTO
- [ ] Implement `TtsService.cs`
- [ ] Integrate with `SiteConfigHolder` to get current settings
- [ ] Test with a sample text to ensure audio is generated
- [ ] Update clood files for Service Interface domain
