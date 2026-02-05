# How We Do Tables

We follow a strict "Dual Definition" pattern for database tables to ensure migrations are immutable while keeping the application code clean.

## 1. Locations & Organization

Tables are defined in **two locations** for every table.

### A. The ServiceModel (Application usage)
*   **Location**: `AIInterviewer.ServiceModel\Tables\<Domain>\<TableName>.cs`
    *   *Example*: `AIInterviewer.ServiceModel\Tables\Configuration\SiteConfig.cs`
*   **Purpose**: This is the POCO used by the running application services to access data.
*   **Grouping**: Grouped in folders by domain (e.g., `Configuration`, `AI`, `Chat`).

### B. The Migration (Database Creation)
*   **Location**: `AIInterviewer\Migrations\Migration<Number>_<Description>.cs`
    *   *Example*: `AIInterviewer\Migrations\Migration1000_CreateSiteConfig.cs`
*   **Purpose**: Defines the schema snapshot *at the time of creation*. This ensures that future changes to the ServiceModel POCO do not break old migrations.
*   **Structure**: The table POCO is defined **inside** the migration class.

## 2. Implementation Rules

1.  **Dual Maintenance**: When you add or modify a table, you must update both the ServiceModel POCO and the relevant Migration (or create a new migration for changes).
2.  **Table Aliasing**: YOU MUST use `[Alias("table_name")]` on both classes to ensure they map to the same physical database table.
3.  **Data Annotations**: Use ServiceStack attributes (`[AutoIncrement]`, `[PrimaryKey]`, `[Required]`, `[StringLength]`) in both definitions to define constraints.

## 3. Example

### ServiceModel Definition
**File**: `AIInterviewer.ServiceModel\Tables\Configuration\SiteConfig.cs`
```csharp
using ServiceStack.DataAnnotations;

namespace AIInterviewer.ServiceModel.Tables.Configuration;

[Alias("siteconfig")]
public class SiteConfig
{
    [AutoIncrement]
    [PrimaryKey]
    public int Id { get; set; }

    [Required]
    [StringLength(2096)]
    public string GeminiApiKey { get; set; }

    [Required]
    [StringLength(255)]
    public string InterviewModel { get; set; }

    [StringLength(255)]
    public string? GlobalFallbackModel { get; set; }
}
```

### Migration Definition
**File**: `AIInterviewer\Migrations\Migration1000_CreateSiteConfig.cs`
```csharp
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;

public class Migration1000_CreateSiteConfig : MigrationBase
{
    // Define the table structure locally inside the migration
    [Alias("siteconfig")]
    public class SiteConfig
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [Required]
        [StringLength(2096)]
        public string GeminiApiKey { get; set; }

        [Required]
        [StringLength(255)]
        public string InterviewModel { get; set; }

        [StringLength(255)]
        public string? GlobalFallbackModel { get; set; }
    }

    public override void Up()
    {
        Db.CreateTable<SiteConfig>();
    }

    public override void Down()
    {
        Db.DropTable<SiteConfig>();
    }
}
```
