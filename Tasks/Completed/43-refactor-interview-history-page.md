# Refactor Interview History Page into Composables and Components

## Objective
Refactor the `interviews/history/index.vue` page to extract reusable composables and components for better code organization and reusability.

## Context
The interview history page is relatively clean at 132 lines, but can still benefit from extracting the interview list display and history fetching logic into reusable pieces.

## Files to Create/Modify

### New Composables
- `AIInterviewer.Client/src/composables/useInterviewHistory.ts` (Fetch and manage interview history)

### New Components
- `AIInterviewer.Client/src/components/interview/InterviewHistoryCard.vue` (Single interview card display)
- `AIInterviewer.Client/src/components/interview/InterviewHistoryList.vue` (List of interview cards)

### Modified Files
- `AIInterviewer.Client/src/pages/interviews/history/index.vue` (Simplified)

## Clood Groups
This task affects files tracked in the following clood groups:
- **`clood-groups/Frontend.json`** - Frontend pages and components
- **`clood-groups/Interview.json`** - Interview-related UI

## Implementation Steps

1. **Create Worktree**:
    - Create a new worktree for this task:
        ```bash
        git worktree add ../AIInterviewer-refactor-history -b feature/refactor-history
        ```

2. **Create useInterviewHistory Composable**:
    - Create `AIInterviewer.Client/src/composables/useInterviewHistory.ts`
    - Extract:
        - `interviews` state
        - `loading` state
        - `error` state
        - `fetchHistory()` function
        - `formatDate()` utility function
        - `promptPreview()` utility function
    - Return all state and functions

3. **Create InterviewHistoryCard Component**:
    - Create `AIInterviewer.Client/src/components/interview/InterviewHistoryCard.vue`
    - Props: `interview: InterviewDto`
    - Emits: `startFromPrompt`
    - Extract the single interview card UI (lines 46-86 from original)
    - Include all action buttons (View Transcript, Resume, Start from prompt)

4. **Create InterviewHistoryList Component**:
    - Create `AIInterviewer.Client/src/components/interview/InterviewHistoryList.vue`
    - Props: `interviews: InterviewDto[]`, `loading: boolean`, `error: string`
    - Emits: `retry`, `startFromPrompt`
    - Extract the list container with loading/error/empty states (lines 18-87 from original)
    - Use InterviewHistoryCard component for each interview

5. **Refactor history/index.vue Page**:
    - Import and use the new composable
    - Replace list section with InterviewHistoryList component
    - The page should be ~40-50 lines instead of 132
    - Keep the header section in the page

6. **Verify**:
    - Test loading state
    - Test error state with retry
    - Test empty state
    - Test all action buttons
    - Test navigation to different pages
    - Ensure styling is identical

7. **Update Clood Files**:
    - Update `clood-groups/Frontend.json` to include all new files
    - Update `clood-groups/Interview.json` to include new components

8. **Finalize**:
    - Commit changes with descriptive message
    - Return to main repository and merge
    - Remove worktree and delete branch
    - Move this task to `Tasks/completed/`

## Notes
- The InterviewHistoryCard component can be reused in other places that display interview summaries
- The useInterviewHistory composable can be used in dashboard views
- This pattern makes it easy to add filtering/sorting in the future
