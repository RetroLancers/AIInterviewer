# Task: Browser Speech Recognition Integration

## Objective
Update the application to support both Gemini (server-side) and Browser (client-side) speech recognition. This includes adding a configuration option and integrating the `useSpeechRecognition` hook from VueUse.

## Requirements

### 1. Database & Models
- Update `SiteConfig` in `AIInterviewer.ServiceModel/Tables/Configuration/SiteConfig.cs`:
    - Add `string TranscriptionProvider` (e.g., "Gemini" or "Browser").
- Create a new migration `Migration1002_AddTranscriptionProviderToSiteConfig.cs` to add the column with "Gemini" as default.
- Update `ConfigureSiteConfig.cs` to include the default value in the seeding logic.

### 2. DTOs & Extensions
- Update `SiteConfigResponse` and `UpdateSiteConfigRequest` in `AIInterviewer.ServiceModel/Types/Configuration/` to include `TranscriptionProvider`.
- Update `SiteConfigExtensions.cs` to map the new field between table and DTO.

### 3. Settings UI
- Update `SiteConfigEditor.vue`:
    - Add a selection (Radio or Select) for "Transcription Provider".
    - Options: "Gemini (Server-side)" and "Browser (Client-side)".
- Update `useSiteConfig.ts` to handle the new field in state and save operations.

### 4. Interview Chat UI
- Update `AIInterviewer.Client/src/pages/interviews/[id]/index.vue`:
    - Integrate `useSpeechRecognition` from `@vueuse/core`.
    - Modify `toggleRecording` and `processAudioResponse` to check the `TranscriptionProvider` setting.
    - If "Browser": Use the hook to get text results directly and send them to the chat service.
    - If "Gemini": Fall back to the current audio recording and server-side transcription flow.
- Ensure visual indicators (recording pulse, etc.) stay synchronized for both modes.

### 5. Backend Service
- Update `ChatService.cs` to optionally validate or log the transcription provider usage if necessary.

## Checklist

### Backend & Database
- [ ] Add `TranscriptionProvider` to `SiteConfig` table
- [ ] Create and run migration `1002`
- [ ] Update SiteConfig DTOs and Extension methods
- [ ] Update seeding logic in `ConfigureSiteConfig.cs`

### Frontend Configuration
- [ ] Update `useSiteConfig.ts` composable
- [ ] Add Transcription Provider selection to `SiteConfigEditor.vue`

### Frontend Transcription Implementation
- [ ] Install/Verify `@vueuse/core` dependency (already in package.json)
- [ ] Integrate `useSpeechRecognition` in `interviews/[id]/index.vue`
- [ ] Implement conditional transcription logic (Browser vs Gemini)
- [ ] Test Browser-based transcription (Web Speech API)
- [ ] Test Gemini-based transcription (Server-side)

### Verification
- [ ] Verify settings are persisted in the database
- [ ] Ensure the UI correctly reflects the chosen transcription method
- [ ] Update clood files for Database, Service Interface, and Frontend domains
