# AI Config CRUD Service

## Objective
Create the backend service to allow users (admins) to Create, Read, Update, and Delete AI configurations.

## Requirements
1.  **Service**: `AiConfigService` in `ServiceInterface/Services/Configuration`.
2.  **DTOs**:
    *   `CreateAiConfig`, `UpdateAiConfig`, `DeleteAiConfig`, `GetAiConfig`.
    *   Follow `Tasks/02_How_We_Do_services.md` standards.
    *   Use Extension methods for Table<->DTO conversion.
3.  **Validation**: Ensure API Key is present, Provider is valid.

## Implementation Steps
1.  Create Request/Response DTOs in `ServiceModel/Types/Configuration`.
2.  Run `Update-Dtos.ps1`.
3.  Implement `AiConfigService.cs`.
4.  Implement CRUD logic using AutoQuery or custom logic.
5.  Implement Extension methods (`ToTable`, `ToDto`).

## Definition of Done
*   Full CRUD API available for `AiServiceConfig`.
*   Swagger UI shows new endpoints.
