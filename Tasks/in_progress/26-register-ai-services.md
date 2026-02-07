# Register AI Services & DI Logic (Update for Site Config Integration)

## Status
**MOSTLY COMPLETED** by Task 18a. This task now focuses on the **SiteConfig integration** aspect.

## What's Already Done (Task 18a)
- ✅ `IAiProviderFactory` interface created
- ✅ `AiProviderFactory` implementation created
- ✅ Factory registered in DI (`Configure.AppHost.cs`)
- ✅ Factory can instantiate providers based on `AiServiceConfig.ProviderType`

## What's Remaining

### 1. Add `ActiveAiConfigId` to SiteConfig
Add a field to the `SiteConfig` table to reference the active AI configuration:

```sql
ALTER TABLE site_config ADD COLUMN active_ai_config_id INT NULL;
```

Update the `SiteConfig` POCO:
```csharp
public int? ActiveAiConfigId { get; set; }
```

### 2. Update Services to Use Factory Pattern
Services like `InterviewService` and `ChatService` need to:
- Inject `IAiProviderFactory` (not `IAiProvider` directly)
- Inject `IDbConnection` or repository to fetch configs
- Fetch `SiteConfig` to get `ActiveAiConfigId`
- Load the corresponding `AiServiceConfig` from the database
- Call `factory.GetProvider(aiConfig)` to get the provider instance

**Example Implementation:**
```csharp
public class InterviewService : Service
{
    private readonly IAiProviderFactory _aiProviderFactory;
    private readonly SiteConfigHolder _siteConfigHolder;

    public InterviewService(
        IAiProviderFactory aiProviderFactory,
        SiteConfigHolder siteConfigHolder)
    {
        _aiProviderFactory = aiProviderFactory;
        _siteConfigHolder = siteConfigHolder;
    }

    public async Task<object> Post(StartInterview request)
    {
        // Get active AI config ID from site config
        var activeConfigId = _siteConfigHolder.Config.ActiveAiConfigId;
        
        if (activeConfigId == null)
            throw new InvalidOperationException("No active AI configuration set");
        
        // Load AI config from database
        var aiConfig = Db.SingleById<AiServiceConfig>(activeConfigId.Value);
        
        // Get the appropriate provider
        var aiProvider = _aiProviderFactory.GetProvider(aiConfig);
        
        // Use the provider
        var response = await aiProvider.ChatAsync(...);
        
        // ... rest of logic
    }
}
```

### 3. (Optional) Create Helper Method
Consider creating a helper extension or service method to reduce boilerplate:

```csharp
public static class AiProviderExtensions
{
    public static IAiProvider GetActiveProvider(
        this IAiProviderFactory factory,
        SiteConfigHolder configHolder,
        IDbConnection db)
    {
        var activeConfigId = configHolder.Config.ActiveAiConfigId
            ?? throw new InvalidOperationException("No active AI configuration set");
        
        var aiConfig = db.SingleById<AiServiceConfig>(activeConfigId);
        return factory.GetProvider(aiConfig);
    }
}
```

Then usage becomes:
```csharp
var aiProvider = _aiProviderFactory.GetActiveProvider(_siteConfigHolder, Db);
```

## Implementation Steps

1. **Add Migration** for `ActiveAiConfigId` column to `SiteConfig` table
2. **Update `SiteConfig` POCO** to include `ActiveAiConfigId` property
3. **Update `SiteConfig.vue`** to allow selection of active AI configuration
4. **Update `InterviewService`** to use factory pattern with active config
5. **Update `ChatService`** to use factory pattern with active config
6. **Update any other services** that use AI providers
7. **Test** switching between providers via Site Config UI

## Files to Update
- `Migrations/Migration10XX_AddActiveAiConfigToSiteConfig.cs` (new)
- `ServiceModel/Tables/Configuration/SiteConfig.cs`
- `ServiceInterface/Services/InterviewService.cs`
- `ServiceInterface/Services/ChatService.cs`
- `AIInterviewer/src/pages/SiteConfig.vue`

## Clood Groups
Reference these clood groups when making changes:
- **AI Provider Architecture**: See `18a-implement-ai-provider-factory.md`, `19-implement-gemini-adapter.md`, `20-implement-openai-adapter.md`
- **Site Config Pattern**: See `01-configure-llm-site-config.md`, `16-site-config-service-transactions.md`
- **Service Patterns**: See `02_How_We_Do_services.md`

## Definition of Done
*   [ ] `ActiveAiConfigId` field exists in `SiteConfig` table and POCO
*   [ ] Services inject `IAiProviderFactory` instead of concrete providers
*   [ ] Services correctly fetch and use the active AI configuration
*   [ ] Site Config UI allows selecting the active AI configuration
*   [ ] Switching the provider in Site Config changes the backend behavior
*   [ ] Code compiles without errors
*   [ ] End-to-end test confirms provider switching works

## Notes
- This task is now **much smaller** than originally scoped
- Most of the heavy lifting was done in Task 18a
- Focus on the integration points between SiteConfig and the factory pattern
