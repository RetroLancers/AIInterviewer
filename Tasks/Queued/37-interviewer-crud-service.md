# Implement Interviewer CRUD Service

## Objective
Create a ServiceStack service to handle CRUD operations for saved interviewers (Create, Read, Update, Delete, List).

## Context
After creating the Interviewer table, we need backend services to manage interviewer records. This follows the same pattern as the AI Config CRUD service.

## Dependencies
- **Task 36**: Create Interviewer Table (must be completed first)

## Files to Create/Modify

### Backend
- `AIInterviewer.ServiceModel/Types/Interview/CreateInterviewer.cs` (New DTO)
- `AIInterviewer.ServiceModel/Types/Interview/UpdateInterviewer.cs` (New DTO)
- `AIInterviewer.ServiceModel/Types/Interview/DeleteInterviewer.cs` (New DTO)
- `AIInterviewer.ServiceModel/Types/Interview/GetInterviewer.cs` (New DTO)
- `AIInterviewer.ServiceModel/Types/Interview/ListInterviewers.cs` (New DTO)
- `AIInterviewer.ServiceModel/Types/Interview/InterviewerResponse.cs` (New DTO)
- `AIInterviewer.ServiceModel/Types/Interview/ListInterviewersResponse.cs` (New DTO)
- `AIInterviewer.ServiceModel/Types/Interview/ExtensionMethods/InterviewerExtensions.cs` (New extension methods)
- `AIInterviewer.ServiceInterface/Services/Interview/InterviewerService.cs` (New service)

## Clood Groups
This task affects files tracked in the following clood groups:
- **`clood-groups/Interview.json`** - Interview services and DTOs
- **`clood-groups/service-interface.json`** - Service implementations

## Implementation Steps

1. **Create Worktree**:
    - Create a new worktree for this task:
        ```bash
        git worktree add ../AIInterviewer-interviewer-crud -b feature/interviewer-crud
        ```
    - Navigate to the worktree and create the `App_Data` folder, then run `npm run migrate`

2. **Create Response DTO**:
    - Create `AIInterviewer.ServiceModel/Types/Interview/InterviewerResponse.cs`:
        ```csharp
        namespace AIInterviewer.ServiceModel.Types.Interview;

        public class InterviewerResponse
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string SystemPrompt { get; set; }
            public int? AiConfigId { get; set; }
            public string? UserId { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }
        ```

3. **Create Extension Methods**:
    - Create `AIInterviewer.ServiceModel/Types/Interview/ExtensionMethods/InterviewerExtensions.cs`:
        ```csharp
        using AIInterviewer.ServiceModel.Tables.Interview;

        namespace AIInterviewer.ServiceModel.Types.Interview.ExtensionMethods;

        public static class InterviewerExtensions
        {
            public static InterviewerResponse ToResponse(this Interviewer interviewer)
            {
                return new InterviewerResponse
                {
                    Id = interviewer.Id,
                    Name = interviewer.Name,
                    SystemPrompt = interviewer.SystemPrompt,
                    AiConfigId = interviewer.AiConfigId,
                    UserId = interviewer.UserId,
                    CreatedAt = interviewer.CreatedAt,
                    UpdatedAt = interviewer.UpdatedAt
                };
            }
        }
        ```

4. **Create Request DTOs**:
    - Create `CreateInterviewer.cs`, `UpdateInterviewer.cs`, `DeleteInterviewer.cs`, `GetInterviewer.cs`, `ListInterviewers.cs`
    - Follow the pattern from AI Config CRUD DTOs
    - Include appropriate routes and return types

5. **Create List Response DTO**:
    - Create `ListInterviewersResponse.cs` with a list of `InterviewerResponse`

6. **Implement InterviewerService**:
    - Create `AIInterviewer.ServiceInterface/Services/Interview/InterviewerService.cs`
    - Implement all CRUD operations with database transactions
    - Set `CreatedAt` on create, update `UpdatedAt` on update
    - Include proper error handling and validation

7. **Update DTOs**:
    - From the workspace root, run:
        ```powershell
        .\Update-Dtos.ps1 -TaskNumber 37
        ```

8. **Verify**:
    - Build the project to ensure no compilation errors
    - Test the service endpoints (manually or with tests)
    - Verify all CRUD operations work correctly

9. **Update Clood Files**:
    - Update `clood-groups/Interview.json` to include all new files
    - Update `clood-groups/service-interface.json` if needed

10. **Finalize**:
    - Commit changes with descriptive message
    - Return to main repository and merge
    - Remove worktree and delete branch
    - Move this task to `Tasks/completed/`

## Notes
- Follow the same transaction pattern as other services
- Include proper validation (e.g., name required, prompt required)
- Consider adding duplicate name checking
- The service should be accessible without authentication for now (can be restricted later)
