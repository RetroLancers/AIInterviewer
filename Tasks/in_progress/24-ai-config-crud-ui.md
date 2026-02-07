# AI Config CRUD UI

## Objective
Create a Vue frontend page to manage AI Configurations.

## Requirements
1.  **Page**: New page "AI Configurations" (e.g., `/admin/ai-configs`).
2.  **Features**:
    *   List all configs.
    *   "Add New" button -> Modal/Form.
    *   Edit button -> Modal/Form (pre-filled).
    *   Delete button (with confirmation).
3.  **Form Fields**: Name, Provider (Dropdown), API Key, Model ID.
4.  **Styling**: Use "Premium" design guidelines (Tailwind if standard, else CSS).

## Implementation Steps
1.  Create `AiConfigList.vue`.
2.  Add routing in `router.ts` (or `routes.ts`).
3.  Implement calls to `AiConfigService` using the TypeScript client.
4.  Add styling matching the design system.

## Definition of Done
*   User can add, edit, delete, and view AI Configs via UI.
