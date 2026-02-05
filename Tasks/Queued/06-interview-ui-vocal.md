# Task: Vocal Interview Chat UI & Kokoro TTS Configuration

## Objective
Implement the main interview chat interface with text and voice interaction, and configure the site settings to support selectable Kokoro TTS voices.

## Status
UI and services exist but there are backend/API correctness gaps.

## Issues Found
- `ChatService.Post(TranscribeAudioRequest)` returns `Task<object>`; it should be strongly typed `Task<TranscribeAudioResponse>`.
- `TtsService.Post(TextToSpeechRequest)` returns `Task<object>`; return `HttpResult` or `Stream` with a typed signature and keep it in the `AIInterviewer.ServiceInterface.Services.Chat` namespace.
- Frontend uses `try/catch` around `client.api` (violates API usage rules). Use `api.succeeded`/`api.error` instead.
- History role labels use `"Model"`; spec calls for `"Interviewer"`/`"User"` — align if required.

## Requirements

### 1. Vocal Interview UI
- **Dynamic route**: `AIInterviewer.Client/src/pages/interviews/[id].vue`.
- **Chat history**: Display messages between Interviewer and User.
- **Audio recording**:
    - Button to start/stop recording.
    - Visualize recording state.
    - Implement/Check `useVocal.ts` composable for MediaRecorder handling (mention of "useView" pattern).
- **Transcription**:
    - Backend-side transcription using Gemini.
    - Update history with transcribed text.
- **Manual input**: Text fallback.
- **TTS Playback**:
    - Automatic synthesis and playback for Interviewer messages.
    - Fetch from the `TextToSpeech` API.

### 2. Kokoro TTS Settings
- **SiteConfig Table**:
    - Add `string KokoroVoice` (e.g., "af_heart").
- **DTOs**:
    - Update `SiteConfigResponse` and `UpdateSiteConfigRequest`.
- **Settings UI**:
    - Update `SiteConfigEditor.vue` with voice selection.
    - Provide defaults if voice list is unavailable.

## Checklist

### Backend & Database
- [x] Modify `AIInterviewer.ServiceModel/Tables/Configuration/SiteConfig.cs`: Add `KokoroVoice`.
- [x] Update DTOs in `AIInterviewer.ServiceModel/Types/Configuration/`.
- [x] Create/Run migration for `SiteConfig` changes.
- [ ] Make transcription endpoint strongly typed (`TranscribeAudioResponse`, no `object`).
- [ ] Make TTS endpoint strongly typed and in correct namespace (return `HttpResult`/`Stream`).

### Frontend
- [x] Create `AIInterviewer.Client/src/pages/interviews/[id].vue`.
- [x] Implement `AIInterviewer.Client/src/composables/useVocal.ts`.
- [x] Update `AIInterviewer.Client/src/components/SiteConfigEditor.vue` for voice selection.
- [x] Integrate TTS playback logic for AI messages.
- [x] Build responsive chat UI with scroll-to-bottom behavior.
- [ ] Remove `try/catch` around `client.api` calls and handle `ApiResult`.
- [ ] Align history role labels if required (`Interviewer` vs `Model`).

### Verification
- [ ] Verify voice-to-text and text-to-voice flow.
- [ ] Verify settings are saved correctly in the DB.
- [x] Update clood files for Frontend and Service Interface domains.
