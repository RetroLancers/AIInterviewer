# Task: Vocal Interview Chat UI

## Objective
Implement the main interview chat interface with support for text and voice interaction.

## Requirements
- Dynamic route: `AIInterviewer.Client/src/pages/interviews/[id].vue`.
- Chat history display (Interviewer vs. User).
- Audio recording integration:
    - Button to start/stop recording.
    - Visualize recording state.
    - Mentioned "useView" pattern for recording (check for or create `useVocal.ts`).
- Transcription:
    - Send recorded audio to backend.
    - Use Gemini to transcribe the audio (backend-side).
    - Update chat history with transcribed text.
- Manual text input fallback.

## Checklist
- [ ] Create `interviews/[id].vue`
- [ ] Implement `useVocal.ts` composable for MediaRecorder handling
- [ ] Implement backend endpoint for audio transcription using Gemini
- [ ] Build responsive chat UI with scroll-to-bottom behavior
- [ ] Verify voice-to-text flow
- [ ] Update clood files for Frontend and Service Interface domains
