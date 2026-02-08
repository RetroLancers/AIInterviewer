# Integrate Saved Interviewers with New Interview Page

## Objective
Update the "New Interview" page to support selecting a saved interviewer, with options to save the current configuration before starting, or use a temporary interviewer.

## Context
Users should be able to:
1. Select a saved interviewer from a dropdown
2. Create a temporary interviewer (current behavior)
3. Save a temporary interviewer before starting the interview
4. Start an interview with either a saved or temporary interviewer

## Dependencies
- **Task 36**: Create Interviewer Table
- **Task 37**: Interviewer CRUD Service
- **Task 38**: Create Interviewer Management Page
- **Task 39**: Interviewer List Page

## Files to Modify

### Backend
- `AIInterviewer.ServiceModel/Types/Interview/CreateInterview.cs` (Add `InterviewerId` property)
- `AIInterviewer.ServiceModel/Tables/Interview/Interview.cs` (Add `InterviewerId` column)
- `AIInterviewer/Migrations/Migration****_AddInterviewerIdToInterview.cs` (New migration)
- `AIInterviewer.ServiceInterface/Services/Interview/InterviewService.cs` (Handle interviewer selection)

### Frontend
- `AIInterviewer.Client/src/pages/interviews/new.vue` (Add interviewer selection and save options)
- `AIInterviewer.Client/src/dtos.ts` (Update after DTO regeneration)

## Clood Groups
This task affects files tracked in the following clood groups:
- **`clood-groups/Interview.json`** - Interview tables, services, and pages
- **`clood-groups/Frontend.json`** - Frontend pages

## Implementation Steps

1. **Create Worktree**:
    - Create a new worktree for this task:
        ```bash
        git worktree add ../AIInterviewer-integrate-interviewer -b feature/integrate-interviewer
        ```
    - Navigate to the worktree and create the `App_Data` folder, then run `npm run migrate`

2. **Add InterviewerId to Interview Table**:
    - Open `AIInterviewer.ServiceModel/Tables/Interview/Interview.cs`
    - Add nullable property:
        ```csharp
        /// <summary>
        /// Optional reference to saved interviewer. Null for temporary interviewers.
        /// </summary>
        public int? InterviewerId { get; set; }
        ```
    - Create migration `Migration****_AddInterviewerIdToInterview.cs`:
        - In `Up()`: `Db.AddColumn<Interview>(new { InterviewerId = (int?)null });`
        - In `Down()`: `Db.DropColumn<Interview>(nameof(Interview.InterviewerId));`

3. **Update CreateInterview DTO**:
    - Open `AIInterviewer.ServiceModel/Types/Interview/CreateInterview.cs`
    - Add optional properties:
        ```csharp
        /// <summary>
        /// Optional saved interviewer ID. If provided, loads configuration from saved interviewer.
        /// </summary>
        public int? InterviewerId { get; set; }

        /// <summary>
        /// Optional voice selection for temporary interviewers. Used when InterviewerId is null.
        /// If not provided, will use the voice from the selected AI config.
        /// </summary>
        [StringLength(100)]
        public string? Voice { get; set; }
        ```

4. **Run Migrations**:
    - From the worktree directory, run:
        ```bash
        npm run migrate
        ```

5. **Update InterviewService**:
    - Open `AIInterviewer.ServiceInterface/Services/Interview/InterviewService.cs`
    - Update `Post(CreateInterview request)` method:
        - If `InterviewerId` is provided:
            - Fetch the `Interviewer` record
            - Use its `SystemPrompt` and `AiConfigId`
            - Store the `InterviewerId` in the `Interview` record
            - Voice will come from the interviewer's AI config
        - If `InterviewerId` is not provided (temporary interviewer):
            - Use the provided `SystemPrompt` and `AiConfigId` from the request (current behavior)
            - Use the provided `Voice` parameter if specified
            - `InterviewerId` remains null (temporary interviewer)

6. **Update DTOs**:
    - From the workspace root, run:
        ```powershell
        .\Update-Dtos.ps1 -TaskNumber 40
        ```

7. **Update Frontend - new.vue**:
    - Open `AIInterviewer.Client/src/pages/interviews/new.vue`
    - Add interviewer selection section at the top:
        - **Dropdown**: "Select Saved Interviewer (optional)"
            - Populated with saved interviewers from `ListInterviewers`
            - Default: "Create temporary interviewer"
        - When an interviewer is selected:
            - Auto-fill the prompt fields with the interviewer's system prompt
            - Auto-select the interviewer's AI config (if set)
            - Show "Using saved interviewer: {name}" indicator
            - Voice will come from the selected AI config
    - **Add Voice Selection for Temporary Interviewers**:
        - When creating a temporary interviewer (no saved interviewer selected):
            - Add a voice selection dropdown (similar to what was in SiteConfig)
            - Options: "alloy", "echo", "fable", "onyx", "nova", "shimmer" (or fetch from AI config)
            - This is required since voice was removed from SiteConfig (Task 34)
            - The selected voice should be used for the interview session
        - When using a saved interviewer:
            - Voice comes from the interviewer's AI config, so no manual selection needed
    - Update the action buttons:
        - If using a saved interviewer:
            - "Start Interview" button
        - If creating a temporary interviewer:
            - "Start Interview" button (current behavior)
            - "Save and Start Interview" button
                - Opens a dialog to enter interviewer name
                - Calls `CreateInterviewer` with current configuration
                - Then starts the interview with the newly saved interviewer
    - Update form submission:
        - If saved interviewer selected: Include `interviewerId` in request
        - If temporary: Include `systemPrompt`, optional `aiConfigId`, and selected voice (current behavior)

8. **Handle Query Parameters**:
    - Check for `?interviewerId={id}` query parameter on page load
    - If present, auto-select that interviewer and populate fields

9. **Verify**:
    - Test selecting a saved interviewer and starting an interview
    - Test creating a temporary interviewer (current flow)
    - Test "Save and Start" functionality
    - Test navigation from interviewer list "Use" button
    - Verify the correct interviewer configuration is used
    - Verify `InterviewerId` is stored correctly in the database

10. **Update Clood Files**:
    - Update `clood-groups/Interview.json` to include modified files

11. **Finalize**:
    - Commit changes with descriptive message
    - Return to main repository and merge
    - Remove worktree and delete branch
    - Move this task to `Tasks/completed/`

## Notes
- Saved interviewers provide consistency and reusability
- Temporary interviewers (current behavior) remain fully supported
- The "Save and Start" option bridges the gap between temporary and saved
- Consider showing which interviewer was used in the interview history
- **Voice Selection**: Since voice was moved from SiteConfig to AiConfig (Task 34), temporary interviewers must select a voice on the new interview page. Saved interviewers get their voice from their associated AI config.
