# Update Site Config UI

## Objective
Update the Site Configuration page to support selecting the active AI Service.

## Requirements
1.  **UI Modification**:
    *   Remove inputs for "Gemini API Key" and "Interview Model".
    *   Add a Dropdown (Select) for "Active AI Service".
    *   Populate dropdown from `AiServiceConfig` table/api.
2.  **Logic**:
    *   On save, update `ActiveAiConfigId` in `SiteConfig`.

## Implementation Steps
1.  Modify `SiteConfig.vue`.
2.  Fetch list of `AiServiceConfigs` on mount.
3.  Bind selection to `request.ActiveAiConfigId`.
4.  Verify saving updates the backend correctly.

## Definition of Done
*   Site Config page allows selecting a provider.
*   Legacy fields are gone from UI.
