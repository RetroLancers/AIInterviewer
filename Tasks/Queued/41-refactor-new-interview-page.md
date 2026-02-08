# Refactor New Interview Page into Composables and Components

## Objective
Refactor the `interviews/new.vue` page to extract reusable composables and components, improving code organization, testability, and reusability.

## Context
The new interview page currently has ~250 lines mixing UI, state management, and business logic. This task breaks it down into smaller, focused pieces:
- Composables for prompt generation logic
- Components for UI sections
- Better separation of concerns

## Files to Create/Modify

### New Composables
- `AIInterviewer.Client/src/composables/usePromptGeneration.ts` (Extract prompt generation logic)
- `AIInterviewer.Client/src/composables/useInterviewCreation.ts` (Extract interview creation logic)

### New Components
- `AIInterviewer.Client/src/components/interview/PromptSourceSelector.vue` (Radio buttons for generate vs custom)
- `AIInterviewer.Client/src/components/interview/PromptGenerationForm.vue` (Target role + context inputs)
- `AIInterviewer.Client/src/components/interview/PromptEditor.vue` (System prompt textarea with loading state)
- `AIInterviewer.Client/src/components/interview/InterviewTips.vue` (The 3 tip cards at the bottom)

### Modified Files
- `AIInterviewer.Client/src/pages/interviews/new.vue` (Simplified to use new composables/components)

## Clood Groups
This task affects files tracked in the following clood groups:
- **`clood-groups/Frontend.json`** - Frontend pages and components

## Implementation Steps

1. **Create Worktree**:
    - Create a new worktree for this task:
        ```bash
        git worktree add ../AIInterviewer-refactor-new-interview -b feature/refactor-new-interview
        ```

2. **Create usePromptGeneration Composable**:
    - Create `AIInterviewer.Client/src/composables/usePromptGeneration.ts`
    - Extract:
        - `targetRole`, `context`, `promptSource` state
        - `systemPrompt` state
        - `loading` state
        - `generatePrompt()` function
        - `hasPrefilledPrompt`, `prefilledPrompt` computed properties
        - `applyPrefilledPrompt()` function
    - Return all state and functions as a composable
    - Accept route as a parameter

3. **Create useInterviewCreation Composable**:
    - Create `AIInterviewer.Client/src/composables/useInterviewCreation.ts`
    - Extract:
        - `starting` state
        - `error` state
        - `startInterview(systemPrompt: string)` function
    - Return state and functions
    - Accept router as a parameter

4. **Create PromptSourceSelector Component**:
    - Create `AIInterviewer.Client/src/components/interview/PromptSourceSelector.vue`
    - Props: `modelValue: 'generate' | 'custom'`
    - Emits: `update:modelValue`
    - Extract the radio button UI (lines 36-63 from original)

5. **Create PromptGenerationForm Component**:
    - Create `AIInterviewer.Client/src/components/interview/PromptGenerationForm.vue`
    - Props: `targetRole`, `context`, `loading`
    - Emits: `update:targetRole`, `update:context`, `generate`
    - Extract the input fields and generate button (lines 66-114 from original)

6. **Create PromptEditor Component**:
    - Create `AIInterviewer.Client/src/components/interview/PromptEditor.vue`
    - Props: `modelValue: string`, `loading: boolean`, `promptSource: string`
    - Emits: `update:modelValue`
    - Extract the textarea with loading overlay (lines 117-137 from original)

7. **Create InterviewTips Component**:
    - Create `AIInterviewer.Client/src/components/interview/InterviewTips.vue`
    - No props needed (static content)
    - Extract the tips grid (lines 156-170 from original)

8. **Refactor new.vue Page**:
    - Import and use the new composables
    - Replace inline sections with the new components
    - The page should be ~80-100 lines instead of 250
    - Maintain all existing functionality

9. **Verify**:
    - Test all functionality still works:
        - Generate prompt flow
        - Custom prompt flow
        - Prefilled prompt from query params
        - Starting an interview
        - Error handling
    - Ensure styling is identical
    - Test responsive behavior

10. **Update Clood Files**:
    - Update `clood-groups/Frontend.json` to include all new files

11. **Finalize**:
    - Commit changes with descriptive message
    - Return to main repository and merge
    - Remove worktree and delete branch
    - Move this task to `Tasks/completed/`

## Notes
- This refactoring makes the code more maintainable and testable
- Composables can be reused in the interviewer creation page (Task 38)
- Components can be unit tested independently
- The page becomes easier to understand at a glance
