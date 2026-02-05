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


## Data Conversion (Table <-> DTO)
*   **Rule**: NEVER return a Table object (from `ServiceModel\Tables`) directly from a Service. Always return a DTO.
*   **Mechanism**: Use **Extension Methods** to handle the conversion logic.
*   **Location**: `AIInterviewer.ServiceModel\Types\[Domain]\ExtensionMethods\[Entity]Extensions.cs`
*   **Naming**: Class should be named `[Entity]Extensions`.
*   **Standard Methods**:
    *   `ToTable(this Create[Entity]Request request)` -> Returns Table
    *   `ToDto(this [Entity] table)` -> Returns Response DTO
    *   `ToDto(this IEnumerable<[Entity]> tables)` -> Returns List of Response DTOs
*   **Example**:
    ```csharp
    // AIInterviewer.ServiceModel\Types\Configuration\ExtensionMethods\SiteConfigExtensions.cs
    public static class SiteConfigExtensions
    {
        public static SiteConfig ToTable(this UpdateSiteConfigRequest request) { ... }
        public static SiteConfigResponse ToDto(this SiteConfig table) { ... }
    }
    ```

## Configuration
*   **Routes**: Should be applied to the request object.
*   **Authentication**: Should be applied to the function on the Service object.
