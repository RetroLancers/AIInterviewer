# Move Voice Configuration to AI Config & Auto-Fetch Models

## Objective
Refactor the voice configuration from `SiteConfig` to `AiServiceConfig` so each AI configuration can have its own voice selection. Additionally, remove the `BaseUrl` field from `AiServiceConfig` and implement automatic model fetching when an API key is entered.

## Context
Currently, voice selection is stored in `SiteConfig` as a global setting, but it makes more sense for each AI configuration to have its own voice preference. The `BaseUrl` field in `AiServiceConfig` is no longer needed. Additionally, the user experience should be improved by automatically fetching available models when an API key is pasted into the input field, rather than requiring a manual button click.

## Files to Modify

### Backend
- `AIInterviewer.ServiceModel/Tables/AI/AiServiceConfig.cs` (Add voice field, remove BaseUrl)
- `AIInterviewer.ServiceModel/Tables/Configuration/SiteConfig.cs` (Remove voice field)
- `AIInterviewer/Migrations/Migration****_UpdateAiConfigAddVoice.cs` (New migration to add voice column)
- `AIInterviewer/Migrations/Migration****_RemoveVoiceFromSiteConfig.cs` (New migration to remove voice from SiteConfig)
- `AIInterviewer/Migrations/Migration****_RemoveBaseUrlFromAiConfig.cs` (New migration to remove BaseUrl)
- `AIInterviewer.ServiceModel/Types/AI/AiServiceConfigDto.cs` (Update DTOs to reflect schema changes)
- `AIInterviewer.ServiceModel/Types/Configuration/SiteConfigDto.cs` (Update DTOs to reflect schema changes)

### Frontend
- `AIInterviewer/src/components/AiConfigEditor.vue` (Add voice selection dropdown)
- `AIInterviewer/src/components/SiteConfigEditor.vue` (Remove voice selection, implement auto-fetch on key paste)
- `AIInterviewer/src/dtos.ts` (Update after DTO regeneration)

## Clood Groups
This task affects files tracked in the following clood groups:
- **`clood-groups/ai-integration.json`** - AI provider integration and configuration
- **`clood-groups/Configuration.json`** - Site and AI service configuration tables, migrations, and services
- **`clood-groups/Frontend.json`** - Frontend components and pages

## Implementation Steps

1. **Create Worktree**:
    - Create a new worktree for this task:
        ```bash
        git worktree add ../AIInterviewer-voice-config -b feature/voice-config
        ```
    - Navigate to the worktree and create the `App_Data` folder, then run `npm run migrate`

2. **Remove BaseUrl from AiServiceConfig**:
    - Open `AIInterviewer.ServiceModel/Tables/AI/AiServiceConfig.cs`
    - Remove the `BaseUrl` property
    - Create migration `Migration****_RemoveBaseUrlFromAiConfig.cs` to drop the column
        - Use appropriate migration number (check existing migrations)
        - Define the `AiServiceConfig` class inside the migration with the `BaseUrl` field
        - In `Up()`: `Db.DropColumn<AiServiceConfig>(nameof(AiServiceConfig.BaseUrl));`
        - In `Down()`: `Db.AddColumn<AiServiceConfig>(new { BaseUrl = "" });`

3. **Add Voice to AiServiceConfig**:
    - Open `AIInterviewer.ServiceModel/Tables/AI/AiServiceConfig.cs`
    - Add a new property:
        ```csharp
        [StringLength(100)]
        public string? Voice { get; set; }
        ```
    - Create migration `Migration****_UpdateAiConfigAddVoice.cs` to add the column
        - Define the `AiServiceConfig` class inside the migration with the new `Voice` field
        - In `Up()`: `Db.AddColumn<AiServiceConfig>(new { Voice = (string?)null });`
        - In `Down()`: `Db.DropColumn<AiServiceConfig>(nameof(AiServiceConfig.Voice));`

4. **Remove Voice from SiteConfig**:
    - Open `AIInterviewer.ServiceModel/Tables/Configuration/SiteConfig.cs`
    - Remove the `Voice` property (if it exists)
    - Create migration `Migration****_RemoveVoiceFromSiteConfig.cs` to drop the column
        - Define the `SiteConfig` class inside the migration with the `Voice` field
        - In `Up()`: `Db.DropColumn<SiteConfig>(nameof(SiteConfig.Voice));`
        - In `Down()`: `Db.AddColumn<SiteConfig>(new { Voice = (string?)null });`

5. **Run Migrations**:
    - From the worktree directory (`AIInterviewer-voice-config/AIInterviewer`), run:
        ```bash
        npm run migrate
        ```
    - Verify migrations complete successfully

6. **Update DTOs**:
    - From the workspace root, run:
        ```powershell
        .\Update-Dtos.ps1 -TaskNumber 34
        ```
    - This will regenerate the TypeScript DTOs with the updated schema

7. **Update Frontend - AiConfigEditor.vue**:
    - Open `AIInterviewer/src/components/AiConfigEditor.vue`
    - Add a voice selection dropdown to the form (similar to how it appears in SiteConfigEditor)
    - Bind it to `config.voice` (or appropriate property name from DTO)
    - Include common voice options (e.g., "alloy", "echo", "fable", "onyx", "nova", "shimmer" for OpenAI)

8. **Update Frontend - SiteConfigEditor.vue**:
    - Open `AIInterviewer/src/components/SiteConfigEditor.vue`
    - Remove the voice selection dropdown if it exists
    - Locate the API key input field
    - Add an `@paste` event handler to the API key input
    - Implement auto-fetch logic:
        ```javascript
        async function onApiKeyPaste(event: ClipboardEvent) {
          // Wait for the paste to complete
          await nextTick();
          // Trigger the fetch models action automatically
          await fetchModels();
        }
        ```
    - Keep the existing fetch button but ensure it calls the same `fetchModels()` function

9. **Verify**:
    - Build the project to ensure no compilation errors
    - Test the UI:
        - Verify voice selection appears in AI Config editor
        - Verify voice selection is removed from Site Config editor
        - Test pasting an API key triggers model fetch automatically
        - Verify the manual fetch button still works
    - Run any relevant tests

10. **Update Clood Files**:
    - Update `clood-groups/ai-configuration.json` (or similar) to include modified files
    - Update `clood-groups/site-configuration.json` to reflect changes

11. **Finalize**:
    - Commit changes with descriptive message
    - Return to main repository and merge:
        ```bash
        cd c:\DevCurrent\AIInterviewer\AIInterviewer
        git merge feature/voice-config
        ```
    - Remove worktree:
        ```bash
        git worktree remove ../AIInterviewer-voice-config
        git branch -d feature/voice-config
        ```
    - Move this task to `Tasks/completed/`

## Notes
- Ensure migration numbers are sequential and don't conflict with existing migrations
- The voice field is nullable to support AI configs that don't use TTS
- Auto-fetch on paste improves UX by reducing manual steps
- Keep the manual fetch button as a fallback option
