# Fix SiteConfigEditor Compilation Error

## Objective
Fix the frontend compilation error causing the application to fail to build.

## Error Log
```
main.ts:68 [Vue Router warn]: uncaught error during route navigation:
main.ts:68 ReferenceError: loadAiConfigs is not defined
    at SiteConfigEditor.vue:184:5
```

## Research Findings
- The error occurs in `AIInterviewer.Client/src/components/SiteConfigEditor.vue`.
- Line 184 calls `loadAiConfigs()` inside `onMounted`.
- `loadAiConfigs` is NOT destructured from the `useAiConfigs()` composable on line 154.
- The `useAiConfigs.ts` composable *already* calls `loadAiConfigs()` internally within its own `onMounted` hook.

## Plan
1.  **Modify `SiteConfigEditor.vue`**:
    -   Remove the `onMounted` hook from `SiteConfigEditor.vue` (lines 183-185) as it is redundant and causing the reference error.
    -   Alternatively, if manual reloading is required later, destructure `loadAiConfigs` from `useAiConfigs()`:
        ```typescript
        const { aiConfigs, isLoading: aiConfigsLoading, error: aiConfigsError, loadAiConfigs } = useAiConfigs()
        ```
    -   *Recommendation*: Remove the redundant call first.

2.  **Verify**:
    -   Compile the frontend.
    -   Ensure the Site Configuration page loads without errors.
    -   Verify AI configs are loaded correctly (handled by `useAiConfigs`).
