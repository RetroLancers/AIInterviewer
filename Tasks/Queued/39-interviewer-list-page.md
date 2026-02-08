# Create Interviewer List Page

## Objective
Create a Vue page to display all saved interviewers with options to view, edit, delete, and create new interviewers.

## Context
Users need a central place to manage their saved interviewer configurations. This page provides a list view with quick actions.

## Dependencies
- **Task 36**: Create Interviewer Table
- **Task 37**: Interviewer CRUD Service
- **Task 38**: Create Interviewer Management Page

## Files to Create/Modify

### Frontend
- `AIInterviewer.Client/src/pages/interviewers/index.vue` (New page - list interviewers)
- `AIInterviewer.Client/src/dtos.ts` (Already updated in task 37)

## Clood Groups
This task affects files tracked in the following clood groups:
- **`clood-groups/Frontend.json`** - Frontend pages and components

## Implementation Steps

1. **Create Worktree**:
    - Create a new worktree for this task:
        ```bash
        git worktree add ../AIInterviewer-interviewer-list -b feature/interviewer-list
        ```

2. **Create List Page**:
    - Create `AIInterviewer.Client/src/pages/interviewers/index.vue`
    - Include the following:
        - **Header**: "Saved Interviewers"
        - **Create Button**: "Create New Interviewer" (navigate to `/interviewers/new`)
        - **Interviewer List/Grid**: Display all interviewers with:
            - Interviewer name
            - AI config name (if set) or "Site Default"
            - Created date
            - Preview of system prompt (first 100 chars)
            - Action buttons:
                - "Use" - Navigate to new interview page with this interviewer pre-selected
                - "Edit" - Navigate to edit page
                - "Delete" - Delete with confirmation
        - **Empty State**: Message when no interviewers exist with "Create your first interviewer" button

3. **Fetch Interviewers**:
    - On component mount, call `ListInterviewers` service
    - Store results in reactive state
    - Handle loading and error states

4. **Implement Actions**:
    - **Use**: Navigate to `/interviews/new?interviewerId={id}`
    - **Edit**: Navigate to `/interviewers/edit/{id}`
    - **Delete**: 
        - Show confirmation dialog
        - Call `DeleteInterviewer` service
        - Refresh list on success

5. **Styling**:
    - Use card or table layout for interviewer list
    - Ensure responsive design
    - Add hover effects and clear action buttons
    - Show loading spinner while fetching data

6. **Verify**:
    - Test list displays correctly with multiple interviewers
    - Test empty state when no interviewers exist
    - Test all action buttons (Use, Edit, Delete)
    - Test delete confirmation dialog
    - Verify list refreshes after delete

7. **Update Clood Files**:
    - Update `clood-groups/Frontend.json` to include the new page

8. **Finalize**:
    - Commit changes with descriptive message
    - Return to main repository and merge
    - Remove worktree and delete branch
    - Move this task to `Tasks/completed/`

## Notes
- Consider adding search/filter functionality if the list grows large
- Consider adding sorting options (by name, date created, etc.)
- The "Use" button provides quick access to start an interview with a saved interviewer
- Consider showing the AI config name by joining with the AiServiceConfig table
