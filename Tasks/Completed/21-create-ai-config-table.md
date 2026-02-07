# Create AI Config Table

## Objective
Create a database table to store configurations for different AI services.

## Context
We need to store API keys, models, and endpoints for multiple providers (Gemini, OpenAI, etc.) dynamically.

## Requirements
1.  **Table Name**: `AiServiceConfig`.
2.  **Fields**:
    *   `Id` (AutoIncrement)
    *   `Name` (Friendly name, e.g. "My Gemini Pro")
    *   `ProviderType` (Enum/String: "Gemini", "OpenAI")
    *   `ApiKey` (String, required)
    *   `ModelId` (String, e.g. "gemini-1.5-pro", "gpt-4")
    *   `BaseUrl` (String, optional, for custom endpoints/proxies)
3.  **Compliance**: Follow `Tasks/01_How_We_Do_Tables.md`.
    *   POCO in `ServiceModel/Tables/Configuration`.
    *   Migration class in `Migrations`.

## Implementation Steps
1.  [x] Define `AiServiceConfig` in ServiceModel.
2.  [x] Create Migration script (e.g., `MigrationXXXX_CreateAiServiceConfig.cs`).
3.  [x] Run `npm run migrate`.

## Definition of Done
*   [x] Table created in DB.
*   [x] POCO exists in ServiceModel.
