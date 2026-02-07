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

## Completion Notes
- Created `/admin/ai-configs.vue` page with full CRUD functionality
- Integrated with existing `AiConfigService` backend
- Added to admin sidebar navigation under "Configuration" group
- Uses Tailwind CSS with dark mode support
- Modal-based add/edit forms with proper validation
- Delete confirmation dialog
- Updated clood-groups/ai-integration.json with new files
