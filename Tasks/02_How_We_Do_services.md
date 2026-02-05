# Service Stack DTO Procedures

## DTO Placement
*   **Location**: `AIInterviewer.ServiceModel\Types`
*   **Strong Typing**: We want to create strongly typed DTOs for any service and never return `object`.
*   **Organization**:
    *   Each Dto response/request pair gets its own table (if applicable/implied context, though "Each Dto response/request pair gets its own table" usually refers to database or just separation of concerns. Wait, user said "Each Dto response/request pair gets its own table." strictly. I will copy that).
    *   Above that, they are grouped in folders with similar domains.
    *   Example: `AIInterviewer.ServiceModel\Types\Chat\ChatResponse.cs`
    *   Example: `AIInterviewer.ServiceModel\Types\Chat\ChatRequest.cs`
    *   These should be updated in clood as well.

## Service Placement
*   **Location**: `AIInterviewer.ServiceInterface\Services`
*   **Organization**:
    *   Each service gets its own file.
    *   Grouped in folders with similar domains (matching the DTOs).

## Configuration
*   **Routes**: Should be applied to the request object.
*   **Authentication**: Should be applied to the function on the Service object.
