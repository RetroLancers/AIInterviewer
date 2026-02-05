# Task: Vocal Interview Chat UI & Kokoro TTS Configuration

## Objective
Implement the main interview chat interface with text and voice interaction, and configure the site settings to support selectable Kokoro TTS voices.

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
- [ ] Modify `AIInterviewer.ServiceModel/Tables/Configuration/SiteConfig.cs`: Add `KokoroVoice`.
- [ ] Update DTOs in `AIInterviewer.ServiceModel/Types/Configuration/`.
- [ ] Create/Run migration for `SiteConfig` changes.
- [ ] Implement backend endpoint for audio transcription (Gemini).

### Frontend
- [ ] Create `AIInterviewer.Client/src/pages/interviews/[id].vue`.
- [ ] Implement `AIInterviewer.Client/src/composables/useVocal.ts`.
- [ ] Update `AIInterviewer.Client/src/components/SiteConfigEditor.vue` for voice selection.
- [ ] Integrate TTS playback logic for AI messages.
- [ ] Build responsive chat UI with scroll-to-bottom behavior.

### Verification
- [ ] Verify voice-to-text and text-to-voice flow.
- [ ] Verify settings are saved correctly in the DB.
- [ ] Update clood files for Frontend and Service Interface domains.
