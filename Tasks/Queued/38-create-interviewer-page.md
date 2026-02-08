# Create Interviewer Management Page

## Objective
Create a Vue page for creating and editing saved interviewers. This page should allow users to generate or paste a system prompt, optionally select an AI config, and give the interviewer a name.

## Context
This page mirrors the "new interview" page but is focused on creating/editing reusable interviewer configurations rather than starting an interview immediately.

## Dependencies
- **Task 36**: Create Interviewer Table
- **Task 37**: Interviewer CRUD Service

## Files to Create/Modify

### Frontend
- `AIInterviewer.Client/src/pages/interviewers/new.vue` (New page - create interviewer)
- `AIInterviewer.Client/src/pages/interviewers/edit/[id].vue` (New page - edit interviewer)
- `AIInterviewer.Client/src/components/NavHeader.vue` (Add navigation link)
- `AIInterviewer.Client/src/dtos.ts` (Already updated in task 37)

## Clood Groups
This task affects files tracked in the following clood groups:
- **`clood-groups/Frontend.json`** - Frontend pages and components

## Implementation Steps

1. **Create Worktree**:
    - Create a new worktree for this task:
        ```bash
        git worktree add ../AIInterviewer-create-interviewer-page -b feature/create-interviewer-page
        ```

2. **Create New Interviewer Page**:
    - Create `AIInterviewer.Client/src/pages/interviewers/new.vue`
    - Include the following sections:
        - **Name Input**: Text field for interviewer name (required)
        - **AI Config Selector**: Dropdown to select AI config (optional, defaults to site default)
        - **Prompt Generation**: 
            - Job title input
            - Job description textarea
            - "Generate Prompt" button
        - **Manual Prompt**: Textarea to paste or edit system prompt
        - **Actions**:
            - "Save Interviewer" button
            - "Cancel" button (navigate back)
    - Reuse logic from `interviews/new.vue` for prompt generation
    - On save, call `CreateInterviewer` service and navigate to interviewer list

3. **Create Edit Interviewer Page**:
    - Create `AIInterviewer.Client/src/pages/interviewers/edit/[id].vue`
    - Similar to new page but:
        - Load existing interviewer data on mount
        - Pre-fill all fields with existing values
        - "Update Interviewer" button instead of "Save"
        - Include "Delete" button with confirmation

4. **Update Navigation**:
    - Open `AIInterviewer.Client/src/components/NavHeader.vue`
    - Add navigation link to "Interviewers" (route to `/interviewers`)

5. **Styling and UX**:
    - Ensure consistent styling with existing pages
    - Add loading states while fetching data
    - Add validation feedback (required fields, etc.)
    - Show success/error messages after save/update/delete

6. **Verify**:
    - Test creating a new interviewer
    - Test editing an existing interviewer
    - Test deleting an interviewer
    - Verify navigation works correctly
    - Test prompt generation functionality
    - Verify AI config selection works

7. **Update Clood Files**:
    - Update `clood-groups/Frontend.json` to include new pages

8. **Finalize**:
    - Commit changes with descriptive message
    - Return to main repository and merge
    - Remove worktree and delete branch
    - Move this task to `Tasks/completed/`

## Notes
- The page should feel similar to creating a new interview but focused on saving the configuration
- Consider adding a "Test" button that creates a temporary interview with this configuration
- Validation should ensure name and prompt are provided
- The AI config selector should show "Use site default" as the default option
