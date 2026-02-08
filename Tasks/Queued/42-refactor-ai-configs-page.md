# Refactor AI Configs Page into Composables and Components

## Objective
Refactor the `ai-configs.vue` page to extract reusable composables and components, improving code organization and reducing the 290-line monolithic component.

## Context
The AI configs page handles CRUD operations, model fetching, and modal management all in one file. This task breaks it down into focused, reusable pieces.

## Files to Create/Modify

### New Composables
- `AIInterviewer.Client/src/composables/useAiConfigs.ts` (CRUD operations and state management)
- `AIInterviewer.Client/src/composables/useModelFetching.ts` (Model fetching logic)

### New Components
- `AIInterviewer.Client/src/components/ai-config/AiConfigList.vue` (Table/list display)
- `AIInterviewer.Client/src/components/ai-config/AiConfigModal.vue` (Add/Edit modal form)
- `AIInterviewer.Client/src/components/ai-config/AiConfigForm.vue` (Form fields within modal)

### Modified Files
- `AIInterviewer.Client/src/pages/ai-configs.vue` (Simplified orchestration)

## Clood Groups
This task affects files tracked in the following clood groups:
- **`clood-groups/Frontend.json`** - Frontend pages and components
- **`clood-groups/Configuration.json`** - AI configuration UI

## Implementation Steps

1. **Create Worktree**:
    - Create a new worktree for this task:
        ```bash
        git worktree add ../AIInterviewer-refactor-ai-configs -b feature/refactor-ai-configs
        ```

2. **Create useAiConfigs Composable**:
    - Create `AIInterviewer.Client/src/composables/useAiConfigs.ts`
    - Extract:
        - `configs` state
        - `loading` state
        - `loadConfigs()` function
        - `saveConfig()` function
        - `deleteConfig()` function
    - Return all state and functions

3. **Create useModelFetching Composable**:
    - Create `AIInterviewer.Client/src/composables/useModelFetching.ts`
    - Extract:
        - `availableModels` state
        - `loadingModels` state
        - `fetchModels(providerType, apiKey)` function
    - Return state and functions

4. **Create AiConfigList Component**:
    - Create `AIInterviewer.Client/src/components/ai-config/AiConfigList.vue`
    - Props: `configs: AiConfigResponse[]`, `loading: boolean`
    - Emits: `edit`, `delete`
    - Extract the table display (lines 10-48 from original)
    - Include empty state handling

5. **Create AiConfigForm Component**:
    - Create `AIInterviewer.Client/src/components/ai-config/AiConfigForm.vue`
    - Props: `modelValue`, `availableModels`, `loadingModels`
    - Emits: `update:modelValue`, `fetchModels`, `providerChange`
    - Extract all form fields (lines 64-120 from original)
    - Handle form validation

6. **Create AiConfigModal Component**:
    - Create `AIInterviewer.Client/src/components/ai-config/AiConfigModal.vue`
    - Props: `show: boolean`, `isEditing: boolean`, `config`, `availableModels`, `loadingModels`
    - Emits: `close`, `save`, `fetchModels`, `providerChange`
    - Extract modal wrapper and form (lines 50-133 from original)
    - Use AiConfigForm component inside

7. **Refactor ai-configs.vue Page**:
    - Import and use the new composables
    - Replace sections with new components
    - Handle orchestration between components
    - The page should be ~80-100 lines instead of 290

8. **Verify**:
    - Test all CRUD operations
    - Test model fetching
    - Test provider switching
    - Test validation
    - Ensure modal behavior is correct
    - Test empty states

9. **Update Clood Files**:
    - Update `clood-groups/Frontend.json` to include all new files
    - Update `clood-groups/Configuration.json` if needed

10. **Finalize**:
    - Commit changes with descriptive message
    - Return to main repository and merge
    - Remove worktree and delete branch
    - Move this task to `Tasks/completed/`

## Notes
- The modal component can be reused for other CRUD pages
- The form component makes it easy to add new fields
- Composables can be unit tested independently
- This pattern can be applied to the interviewer CRUD pages (Tasks 38-39)
