---
description: Configure LLM Site Config and perform spot checks
---

# Configure LLM Site Config

We have migrated the LLM configuration (SiteConfig) from another project. We need to perform a spot check and finalize the integration into `AIInterviewer`.

## Relevant Files

*   **Table**: `AIInterviewer.ServiceModel/Tables/Configuration/SiteConfig.cs`
*   **Holder**: `AIInterviewer.ServiceModel/Tables/Configuration/SiteConfigHolder.cs`
*   **Migration**: `AIInterviewer/Migrations/Migration1000_CreateSiteConfig.cs`
*   **Extensions**: `AIInterviewer.ServiceModel/Types/Configuration/ExtensionMethods/SiteConfigExtensions.cs`
*   **DTOs**:
    *   `AIInterviewer.ServiceModel/Types/Configuration/GetSiteConfigRequest.cs`
    *   `AIInterviewer.ServiceModel/Types/Configuration/UpdateSiteConfigRequest.cs`
    *   `AIInterviewer.ServiceModel/Types/Configuration/SiteConfigResponse.cs`

## Objectives

1.  **Namespace Correction**:
    *   Check `AIInterviewer/Migrations/Migration1000_CreateSiteConfig.cs`. The namespace is currently `TyphoonSharp.Migrations` (from the old project). Change it to `AIInterviewer.Migrations`.

2.  **Service Verification**:
    *   Verify if the ServiceStack *Service* implementation exists for `GetSiteConfigRequest` and `UpdateSiteConfigRequest`.
    *   If missing, create a new Service class (e.g., `SiteConfigService` in `AIInterviewer.ServiceInterface`) to handle these requests.
    *   The `GET` should enforce fetching the singleton configuration (or the specific ID if multiple are allowed).
    *   The `PUT` should update the configuration.

3.  **Validation**:
    *   Add `[Required]` attributes to the properties in `UpdateSiteConfigRequest` to match the table constraints (specifically `GeminiApiKey` and `InterviewModel`).

4.  **Security & Logic Check**:
    *   Review if exposing `GeminiApiKey` in `SiteConfigResponse` is intended. If this is an Admin-only endpoint, ensure proper authentication/authorization attributes are added to the Service or Request DTOs (e.g., `[ValidateHasRole("Admin")]`).
    *   Confirm the logic for `SiteConfigHolder`. Ensure the application knows how to load/cache the active configuration on startup or upon request.

5.  **Build & Test**:
    *   Ensure the project builds.
    *   Run the migration to create the table.
    *   Verify the API endpoints work as expected.

## Checklist

- [ ] Fix Namespace in `Migration1000_CreateSiteConfig.cs`
- [ ] Add Validation attributes to `UpdateSiteConfigRequest`
- [ ] Create/Verify `SiteConfigService` implementation
- [ ] Ensure `GeminiApiKey` security/visibility is considered
- [ ] Verify `SiteConfigHolder` usage/initialization
- [ ] Run Migration
- [ ] Test Endpoints
