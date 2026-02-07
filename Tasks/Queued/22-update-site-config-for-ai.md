# Update Site Config for AI

## Objective
Update the `SiteConfig` table to reference the active `AiServiceConfig` instead of hardcoding keys/models.

## Context
We are moving away from storing single keys in `SiteConfig` to selecting one from `AiServiceConfig`.

## Requirements
1.  **Modify SiteConfig**:
    *   Remove `GeminiApiKey`.
    *   Remove `InterviewModel`.
    *   Add `ActiveAiConfigId` (Int, ForeignKey to `AiServiceConfig.Id`).
2.  **Compliance**: Update both ServiceModel POCO and create a Migration.

## Implementation Steps
1.  Update `ServiceModel/Tables/Configuration/SiteConfig.cs`.
2.  Create Migration (e.g., `MigrationXXXX_UpdateSiteConfigForAi.cs`).
    *   Use `Db.AlterTable` or Drop/Create strategies as appropriate.
3.  Remove old fields.
4.  Add new Foreign Key field.

## Definition of Done
*   `SiteConfig` table schema updated.
*   Old fields removed (or deprecated).
*   New relation established.
