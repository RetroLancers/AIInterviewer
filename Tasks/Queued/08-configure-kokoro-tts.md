# Task: Configure Kokoro TTS Settings

## Objective
Update the site configuration to allow users to select their preferred Kokoro voice.

## Requirements
- Update `SiteConfig.cs` table:
    - Add `string KokoroVoice` (Required, e.g., "af_heart").
    - Note: Engine selection (CPU/GPU) is moved to optional tasks.
- Update `SiteConfigResponse` and `UpdateSiteConfigRequest` DTOs.
- Update `SiteConfigEditor.vue` component:
    - Add dropdown/input for Kokoro Voice.
    - Fetch list of available voices if possible, or provide common defaults.

## Checklist
- [ ] Modify `AIInterviewer.ServiceModel/Tables/Configuration/SiteConfig.cs`
- [ ] Modify `AIInterviewer.ServiceModel/Types/Configuration/SiteConfigResponse.cs`
- [ ] Modify `AIInterviewer.ServiceModel/Types/Configuration/UpdateSiteConfigRequest.cs`
- [ ] Create migration for `SiteConfig` changes
- [ ] Update `AIInterviewer.Client/src/components/SiteConfigEditor.vue`
- [ ] Verify settings are saved correctly in the DB
