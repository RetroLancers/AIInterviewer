# Complete Voice Configuration Migration

## Objective
Complete the remaining work from task 34 (move-voice-to-ai-config) by:
1. Adding a default voice field to `SiteConfig` for site-wide voice selection when no AI config voice is specified
2. Updating the `SiteConfigService` and DTOs to handle the new default voice field
3. Adding voice selection dropdown to the AI Configs page (`ai-configs.vue`)
4. Removing the `BaseUrl` field from the AI Configs page (it's still showing in the UI but was removed from the backend)
5. Updating the `TtsService` to resolve voice from interview's AI config with fallback to site default

## Context
Task 34 successfully moved voice configuration from `SiteConfig` to `AiServiceConfig`, but there are a few remaining issues:
- `SiteConfig` needs a default/fallback voice field for when an AI config doesn't have a voice specified
- The AI Configs page (`ai-configs.vue`) still has a `BaseUrl` field in the form (lines 115-120) that should be removed
- The AI Configs page is missing the voice selection dropdown that should have been added
- The `SiteConfigService` and DTOs need to be updated to support the default voice field
- The `TtsService` currently only reads from `SiteConfig.KokoroVoice` and needs to be updated to:
  1. Accept an optional `interviewId` parameter
  2. Look up the interview's AI config
  3. Use the AI config's voice if specified
  4. Fall back to the site's default voice
  5. Finally fall back to "af_heart" as a hard-coded default

## Dependencies
- **Task 35**: AI Config Override on Interview (should be completed first, as TTS needs to look up the interview's AI config)

## Files to Modify

### Backend
- `AIInterviewer.ServiceModel/Tables/Configuration/SiteConfig.cs` (Add default voice field)
- `AIInterviewer/Migrations/Migration****_AddDefaultVoiceToSiteConfig.cs` (New migration)
- `AIInterviewer.ServiceModel/Types/Configuration/SiteConfigResponse.cs` (Add default voice to DTO)
- `AIInterviewer.ServiceModel/Types/Configuration/ExtensionMethods/SiteConfigExtensions.cs` (Update mapping)
- `AIInterviewer.ServiceModel/Types/Chat/TextToSpeechRequest.cs` (Add optional interviewId parameter)
- `AIInterviewer.ServiceInterface/Services/Chat/TtsService.cs` (Update voice resolution logic)

### Frontend
- `AIInterviewer.Client/src/pages/ai-configs.vue` (Remove BaseUrl field, add voice dropdown)
- `AIInterviewer.Client/src/components/SiteConfigEditor.vue` (Update to use default voice field)
- `AIInterviewer.Client/src/composables/useInterview.ts` (Pass interviewId to TTS requests)
- `AIInterviewer.Client/src/lib/dtos.ts` (Will be regenerated)

## Clood Groups
This task affects files tracked in the following clood groups:
- **`clood-groups/Configuration.json`** - Site configuration tables, migrations, and services
- **`clood-groups/ai-integration.json`** - TTS service and AI provider integration
- **`clood-groups/Frontend.json`** - Frontend components and pages

## Implementation Steps

1. **Create Worktree**:
    - Create a new worktree for this task:
        ```bash
        git worktree add ../AIInterviewer-complete-voice-config -b feature/complete-voice-config
        ```
    - Navigate to the worktree and create the `App_Data` folder, then run `npm run migrate`

2. **Add Default Voice to SiteConfig**:
    - Open `AIInterviewer.ServiceModel/Tables/Configuration/SiteConfig.cs`
    - Add a new property for the default voice:
        ```csharp
        [StringLength(100)]
        public string? DefaultVoice { get; set; }
        ```
    - Create migration `Migration****_AddDefaultVoiceToSiteConfig.cs` to add the column
        - Define the `SiteConfig` class inside the migration with the new `DefaultVoice` field
        - In `Up()`: `Db.AddColumn<SiteConfig>(new { DefaultVoice = (string?)null });`
        - In `Down()`: `Db.DropColumn<SiteConfig>(nameof(SiteConfig.DefaultVoice));`

3. **Run Migration**:
    - From the worktree directory, run:
        ```bash
        npm run migrate
        ```
    - Verify migration completes successfully

4. **Update DTOs**:
    - Update `AIInterviewer.ServiceModel/Types/Configuration/SiteConfigResponse.cs`:
        - Add `public string? DefaultVoice { get; set; }`
    - Update `AIInterviewer.ServiceModel/Types/Configuration/ExtensionMethods/SiteConfigExtensions.cs`:
        - Add mapping for `DefaultVoice` in both `ToDto()` and `ToTable()` methods
    - From the workspace root, run:
        ```powershell
        .\Update-Dtos.ps1 -TaskNumber 44
        ```
    - This will regenerate the TypeScript DTOs with the updated schema

5. **Update AI Configs Page**:
    - Open `AIInterviewer.Client/src/pages/ai-configs.vue`
    - **Remove BaseUrl field** (lines 115-120):
        - Delete the entire div containing the BaseUrl input
        - Remove `baseUrl: ''` from the form initialization (line 165)
        - Remove `baseUrl` from the form reset in `openAddModal()` (line 226)
        - Remove `baseUrl` from the `editConfig()` mapping (line 237)
        - Remove `baseUrl` from the save payload (line 258)
    - **Add Voice selection dropdown** after the Fallback Model field (after line 113):
        ```vue
        <div>
            <label for="voice" class="block text-sm font-medium leading-6 text-gray-900 dark:text-gray-100">Voice (Optional)</label>
            <div class="mt-1">
                <select v-model="form.voice" id="voice" class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6 dark:bg-gray-700 dark:text-white dark:ring-gray-600">
                    <option :value="undefined">Use site default</option>
                    <option value="alloy">alloy</option>
                    <option value="echo">echo</option>
                    <option value="fable">fable</option>
                    <option value="onyx">onyx</option>
                    <option value="nova">nova</option>
                    <option value="shimmer">shimmer</option>
                </select>
            </div>
            <p class="mt-1 text-xs text-gray-500">Voice for TTS. Leave as default to use the site-wide default voice.</p>
        </div>
        ```
    - Add `voice: undefined as string | undefined` to the form ref (line 158)
    - Add `voice: undefined` to the form reset in `openAddModal()` (around line 225)
    - Update `editConfig()` to include: `voice: config.voice || undefined` (around line 236)
    - Update the save payload to include: `voice: form.value.voice` (around line 257)

6. **Update Site Config Editor**:
    - Open `AIInterviewer.Client/src/components/SiteConfigEditor.vue`
    - Update the Voice Configuration section (lines 98-127):
        - Change the label from "Default Interviewer Voice" to "Site Default Voice"
        - Update the help text to: "Default voice used when an AI config doesn't specify a voice."
    - Update the form data (line 157):
        - Add `defaultVoice: ''` to the formData ref
    - Update the watch function (line 176):
        - Add `formData.value.defaultVoice = newConfig.defaultVoice || ''`
    - Update the `handleSubmit` function (line 185):
        - Pass `formData.value.defaultVoice` as an additional parameter to `saveSiteConfig`
    - Update the composable call if needed to handle the new field

7. **Update Site Config Composable** (if needed):
    - Check `AIInterviewer.Client/src/composables/useSiteConfig.ts`
    - Ensure the `saveSiteConfig` function accepts and sends the `defaultVoice` parameter
    - Update the `UpdateSiteConfigRequest` to include `defaultVoice`

8. **Update TTS Service for Voice Resolution**:
    - **Update TextToSpeechRequest DTO**:
        - Open `AIInterviewer.ServiceModel/Types/Chat/TextToSpeechRequest.cs`
        - Add optional `InterviewId` property:
            ```csharp
            public int? InterviewId { get; set; }
            ```
    - **Update TtsService**:
        - Open `AIInterviewer.ServiceInterface/Services/Chat/TtsService.cs`
        - Inject `IDbConnectionFactory` into the constructor (if not already present)
        - Update the `Post(TextToSpeechRequest request)` method to resolve voice:
            ```csharp
            string? voiceName = null;
            
            // 1. Try to get voice from interview's AI config
            if (request.InterviewId.HasValue)
            {
                using var db = DbFactory.Open();
                var interview = db.SingleById<Interview>(request.InterviewId.Value);
                if (interview?.AiConfigId != null)
                {
                    var aiConfig = db.SingleById<AiServiceConfig>(interview.AiConfigId.Value);
                    voiceName = aiConfig?.Voice;
                }
            }
            
            // 2. Fall back to site default voice
            voiceName ??= siteConfigHolder.SiteConfig?.DefaultVoice;
            
            // 3. Final fallback to hard-coded default
            voiceName ??= "af_heart";
            ```
        - Keep the existing voice loading and error handling logic
    - **Update Frontend to pass InterviewId**:
        - Open `AIInterviewer.Client/src/composables/useInterview.ts`
        - Update the `playAiResponse` function (line 29) to pass the interview ID:
            ```typescript
            const api = await client.api(new TextToSpeechRequest({ 
                text: sanitizedText,
                interviewId: id.value 
            }))
            ```
    - **Regenerate DTOs**:
        - From the workspace root, run:
            ```powershell
            .\Update-Dtos.ps1 -TaskNumber 44
            ```

9. **Verify**:
    - Build the project to ensure no compilation errors
    - Test the UI:
        - Verify the AI Configs page no longer shows BaseUrl field
        - Verify voice selection dropdown appears in AI Config editor
        - Verify the Site Config editor has the updated default voice field
        - Test creating/editing AI configs with and without voice selection
        - Test saving site config with default voice
        - **Test TTS voice resolution**:
            - Create an interview with an AI config that has a specific voice
            - Verify TTS uses the AI config's voice
            - Create an interview with an AI config that has no voice
            - Verify TTS falls back to the site default voice
            - Test with no site default voice set
            - Verify TTS falls back to "af_heart"
    - Run any relevant tests

10. **Update Clood Files**:
    - Update `clood-groups/Configuration.json` to include modified files
    - Update `clood-groups/ai-integration.json` to include TTS service changes
    - Update `clood-groups/Frontend.json` to reflect changes

11. **Finalize**:
    - Commit changes with descriptive message
    - Return to main repository and merge:
        ```bash
        cd c:\DevCurrent\AIInterviewer\AIInterviewer
        git merge feature/complete-voice-config
        ```
    - Remove worktree:
        ```bash
        git worktree remove ../AIInterviewer-complete-voice-config
        git branch -d feature/complete-voice-config
        ```
    - Move this task to `Tasks/Completed/`

## Notes
- The `DefaultVoice` field in `SiteConfig` serves as a fallback when an AI config doesn't specify a voice
- Voice options shown are for OpenAI TTS, but can be used with other providers that support similar voices
- The voice field is nullable everywhere to support AI configs that don't use TTS
- Removing `BaseUrl` from the frontend completes the backend removal done in task 34
- **TTS Voice Resolution Hierarchy**:
  1. Interview's AI Config voice (if interview has an AI config and the config has a voice)
  2. Site's default voice (from `SiteConfig.DefaultVoice`)
  3. Hard-coded fallback: "af_heart"
- The `InterviewId` parameter in `TextToSpeechRequest` is optional for backward compatibility
- This change requires Task 35 to be completed first so that interviews have an `AiConfigId` field
