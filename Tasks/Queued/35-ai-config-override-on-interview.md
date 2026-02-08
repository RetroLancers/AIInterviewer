# Add AI Config Override to Interview Creation

## Objective
Allow users to optionally override the default AI configuration when creating a new interview. This will enable users to select a specific AI config from the available options on the "New Interview" page, rather than always using the site's default active AI config.

## Context
Currently, the interview service uses the site's default active AI configuration (`ActiveAiConfigId` from `SiteConfig`). This task adds the ability to override that default on a per-interview basis. The user should be able to select from available AI configs on the new interview page, and the selected config should be used for that specific interview session.

## Dependencies
- **Task 34**: Move voice config to AI config (must be completed first, as this task builds on the AI config structure)

## Files to Modify

### Backend
- `AIInterviewer.ServiceModel/Types/Interview/CreateInterview.cs` (Add optional `AiConfigId` property)
- `AIInterviewer.ServiceModel/Tables/Interview/Interview.cs` (Add `AiConfigId` column to store the config used)
- `AIInterviewer/Migrations/Migration****_AddAiConfigIdToInterview.cs` (New migration to add column)
- `AIInterviewer.ServiceInterface/Services/Interview/InterviewService.cs` (Update to use override config if provided)

### Frontend
- `AIInterviewer.Client/src/pages/interviews/new.vue` (Add AI config dropdown selector)
- `AIInterviewer.Client/src/dtos.ts` (Update after DTO regeneration)

## Clood Groups
This task affects files tracked in the following clood groups:
- **`clood-groups/Interview.json`** - Interview tables, services, and pages
- **`clood-groups/Configuration.json`** - AI service configuration (for dropdown population)
- **`clood-groups/Frontend.json`** - Frontend pages and components

## Implementation Steps

1. **Create Worktree**:
    - Create a new worktree for this task:
        ```bash
        git worktree add ../AIInterviewer-ai-config-override -b feature/ai-config-override
        ```
    - Navigate to the worktree and create the `App_Data` folder, then run `npm run migrate`

2. **Add AiConfigId to Interview Table**:
    - Open `AIInterviewer.ServiceModel/Tables/Interview/Interview.cs`
    - Add a new nullable property:
        ```csharp
        /// <summary>
        /// Optional override for AI config. If null, uses site default.
        /// </summary>
        public int? AiConfigId { get; set; }
        ```
    - Create migration `Migration****_AddAiConfigIdToInterview.cs`:
        - Define the `Interview` class inside the migration with the new `AiConfigId` field
        - In `Up()`: `Db.AddColumn<Interview>(new { AiConfigId = (int?)null });`
        - In `Down()`: `Db.DropColumn<Interview>(nameof(Interview.AiConfigId));`

3. **Update CreateInterview Request DTO**:
    - Open `AIInterviewer.ServiceModel/Types/Interview/CreateInterview.cs`
    - Add optional property:
        ```csharp
        /// <summary>
        /// Optional AI config to use for this interview. If null, uses site default.
        /// </summary>
        public int? AiConfigId { get; set; }
        ```

4. **Run Migrations**:
    - From the worktree directory, run:
        ```bash
        npm run migrate
        ```

5. **Update InterviewService**:
    - Open `AIInterviewer.ServiceInterface/Services/Interview/InterviewService.cs`
    - Locate the `Post(CreateInterview request)` method
    - Update the logic to:
        1. Check if `request.AiConfigId` is provided
        2. If provided, use that config ID
        3. If not provided, fall back to `siteConfig.ActiveAiConfigId`
        4. Store the selected `AiConfigId` in the `Interview` record
    - Example logic:
        ```csharp
        var aiConfigId = request.AiConfigId ?? siteConfig.ActiveAiConfigId;
        
        var interview = new Interview
        {
            SystemPrompt = request.SystemPrompt,
            UserId = request.UserId,
            AiConfigId = aiConfigId,  // Store which config was used
            // ... other fields
        };
        ```
    - When retrieving the AI provider, use the stored `aiConfigId` instead of always using the site default

6. **Update DTOs**:
    - From the workspace root, run:
        ```powershell
        .\Update-Dtos.ps1 -TaskNumber 35
        ```

7. **Update Frontend - new.vue**:
    - Open `AIInterviewer.Client/src/pages/interviews/new.vue`
    - Add a new dropdown/select component for AI config selection
    - Fetch available AI configs using the existing `ListAiConfigs` service
    - Add UI elements:
        - Label: "AI Configuration (optional)"
        - Dropdown populated with AI configs (showing name and provider type)
        - Default option: "Use site default"
        - Bind to a reactive variable (e.g., `selectedAiConfigId`)
    - Update the form submission to include `aiConfigId` in the `CreateInterview` request:
        ```typescript
        const createRequest: CreateInterview = {
          systemPrompt: prompt.value,
          aiConfigId: selectedAiConfigId.value || undefined
        };
        ```

8. **Verify**:
    - Build the project to ensure no compilation errors
    - Test the UI:
        - Verify AI config dropdown appears on new interview page
        - Verify dropdown is populated with available configs
        - Test creating interview with default config (no selection)
        - Test creating interview with specific config override
        - Verify the correct AI config is used during the interview
    - Check that the `AiConfigId` is properly stored in the `Interview` table

9. **Update Clood Files**:
    - Update `clood-groups/Interview.json` to include:
        - Migration file
        - Updated `CreateInterview.cs`
        - Updated `Interview.cs`
    - Update `clood-groups/Frontend.json` to reflect changes to `new.vue`

10. **Finalize**:
    - Commit changes with descriptive message
    - Return to main repository and merge:
        ```bash
        cd c:\DevCurrent\AIInterviewer\AIInterviewer
        git merge feature/ai-config-override
        ```
    - Remove worktree:
        ```bash
        git worktree remove ../AIInterviewer-ai-config-override
        git branch -d feature/ai-config-override
        ```
    - Move this task to `Tasks/completed/`

## Notes
- The AI config override is optional - if not specified, the site default is used
- Store the `AiConfigId` in the `Interview` table so we know which config was used for historical purposes
- This allows for flexibility in testing different AI configs or using specialized configs for specific interview types
- Consider adding the AI config name to the interview history view for clarity
