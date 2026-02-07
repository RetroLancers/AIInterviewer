# Register AI Services & DI Logic

## Objective
Implement the logic to register the correctly configured `IAiProvider` into the ServiceStack container based on the `SiteConfig`.

## Context
The application needs to instantiate either `GeminiAiProvider` or `OpenAiAiProvider` based on what is selected in the database, and inject it where needed.

## Requirements
1.  **Resolution Logic**:
    *   When an `IAiProvider` is requested (or at startup/request scope), look up `SiteConfig`.
    *   Get `ActiveAiConfigId`.
    *   Load `AiServiceConfig` details from the table.
    *   Factory/Container resolves the appropriate implementation (`Gemini` vs `OpenAI`).
    *   **Crucial**: Pass the config (Key, Model) to the provider's constructor.
2.  **Lifecycle**: Consider Request Scope or Scoped.

## Implementation Steps
1.  Update `Configure.AppHost.cs` or `Configure.Services.cs`.
2.  Register a factory for `IAiProvider`:
    *   Logic: Retrieve `SiteConfig -> AiServiceConfig`.
    *   Instantiate Provider with `new Provider(key, model)`.
3.  Ensure `InterviewService` and others inject `IAiProvider` instead of concrete classes.

## Definition of Done
*   Application uses the configured provider.
*   Switching the provider in Site Config changes the backend behavior.
