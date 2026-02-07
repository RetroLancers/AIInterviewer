# Implement AI Provider Factory Pattern

## Objective
Implement a factory pattern that accepts `AiServiceConfig` and returns the appropriate AI provider implementation based on the `ProviderType`. Only the factory should be registered in Dependency Injection, allowing for dynamic provider selection based on database configuration.

## Context
This task complements Task 18 (Extract AI Interface) by introducing a factory pattern. Instead of registering concrete AI providers directly in DI, we register a factory that creates the appropriate provider instance based on the `AiServiceConfig` stored in the database. This enables runtime provider selection without code changes.

## Requirements

### 1. Factory Interface
Create `IAiProviderFactory` with:
- Method: `IAiProvider GetProvider(AiServiceConfig config)`
- Purpose: Return the appropriate provider implementation based on config

### 2. Factory Implementation
Create `AiProviderFactory` that:
- Takes `IServiceProvider` in constructor (for resolving dependencies needed by providers)
- Implements `GetProvider(AiServiceConfig config)` using switch on `config.ProviderType`
- Returns:
  - `GeminiAiProvider` when `ProviderType == "Gemini"`
  - `OpenAiProvider` when `ProviderType == "OpenAI"`
  - Throws `ArgumentException` for unknown provider types
- Uses `ActivatorUtilities.CreateInstance<T>(serviceProvider, config)` to instantiate providers with dependencies

### 3. DI Registration
In `Configure.AppHost.cs`:
- **Register ONLY the factory**: `services.AddScoped<IAiProviderFactory, AiProviderFactory>()`
- **DO NOT register** individual providers (`GeminiAiProvider`, `OpenAiProvider`) directly
- Individual providers will be instantiated by the factory as needed

### 4. Provider Constructors
Each provider implementation must:
- Accept `AiServiceConfig` as a constructor parameter
- Extract configuration values from `AiServiceConfig`:
  - `config.ApiKey` → API authentication
  - `config.ModelId` → Model to use
  - `config.BaseUrl` → Optional custom endpoint (for self-hosted or proxy scenarios)
- Accept any other dependencies via DI (e.g., `ILogger`, `HttpClient`)

## Implementation Steps

1. **Create Factory Interface** (`IAiProviderFactory.cs` in `ServiceInterface/Interfaces/`)
   ```csharp
   public interface IAiProviderFactory
   {
       IAiProvider GetProvider(AiServiceConfig config);
   }
   ```

2. **Create Factory Implementation** (`AiProviderFactory.cs` in `ServiceInterface/Factories/`)
   ```csharp
   public class AiProviderFactory : IAiProviderFactory
   {
       private readonly IServiceProvider _serviceProvider;

       public AiProviderFactory(IServiceProvider serviceProvider)
       {
           _serviceProvider = serviceProvider;
       }

       public IAiProvider GetProvider(AiServiceConfig config)
       {
           return config.ProviderType switch
           {
               "Gemini" => ActivatorUtilities.CreateInstance<GeminiAiProvider>(_serviceProvider, config),
               "OpenAI" => ActivatorUtilities.CreateInstance<OpenAiProvider>(_serviceProvider, config),
               _ => throw new ArgumentException($"Unknown AI Provider Type: {config.ProviderType}", nameof(config))
           };
       }
   }
   ```

3. **Update DI Registration** in `Configure.AppHost.cs`
   - Remove any existing `services.AddScoped<GeminiAiProvider>()` or similar
   - Add: `services.AddScoped<IAiProviderFactory, AiProviderFactory>()`

4. **Update Provider Constructors**
   - Ensure `GeminiAiProvider` accepts `AiServiceConfig` and extracts needed values
   - Ensure `OpenAiProvider` (Task 20) accepts `AiServiceConfig` and extracts needed values

5. **Update Services Using AI Providers** (e.g., `InterviewService`, `ChatService`)
   - Inject `IAiProviderFactory` instead of `IAiProvider` directly
   - Fetch `AiServiceConfig` from database (via `IDbConnection` or repository)
   - Call `factory.GetProvider(config)` to get the appropriate provider instance
   - Use the returned provider

## Example Service Usage
```csharp
public class InterviewService : Service
{
    private readonly IAiProviderFactory _aiProviderFactory;
    private readonly IDbConnection _db;

    public InterviewService(IAiProviderFactory aiProviderFactory, IDbConnection db)
    {
        _aiProviderFactory = aiProviderFactory;
        _db = db;
    }

    public async Task<object> Post(StartInterview request)
    {
        // Fetch AI config (could use a specific name or get default)
        var aiConfig = _db.Single<AiServiceConfig>(x => x.Name == "default");
        
        // Get the appropriate provider
        var aiProvider = _aiProviderFactory.GetProvider(aiConfig);
        
        // Use the provider
        var response = await aiProvider.ChatAsync(...);
        
        // ... rest of service logic
    }
}
```

## Definition of Done
*   [x] `IAiProviderFactory` interface exists in `ServiceInterface/Interfaces/`
*   [x] `AiProviderFactory` concrete implementation exists in `ServiceInterface/Factories/`
*   [x] Factory properly instantiates providers based on `ProviderType`
*   [x] Factory is registered in DI (`Configure.AppHost.cs`)
*   [x] Individual providers are NOT registered directly in DI
*   [x] Provider constructors accept `AiServiceConfig` parameter
*   [x] Services have been updated to use the factory pattern
*   [x] Code compiles without errors
*   [x] Pattern supports adding new providers without changing DI registration

## Clood Groups
Reference these files when implementing or extending this pattern:
- **Factory Pattern Files**:
  - `AIInterviewer.ServiceInterface/Interfaces/IAiProviderFactory.cs`
  - `AIInterviewer.ServiceInterface/Factories/AiProviderFactory.cs`
  - `AIInterviewer/Configure.AppHost.cs` (DI registration)
- **Configuration**:
  - `AIInterviewer.ServiceModel/Tables/Configuration/AiServiceConfig.cs`
  - `AIInterviewer.ServiceModel/Tables/Configuration/SiteConfig.cs`
- **Provider Implementations**:
  - `AIInterviewer.ServiceInterface/Providers/GeminiAiProvider.cs` (see Task 19)
  - `AIInterviewer.ServiceInterface/Providers/OpenAiProvider.cs` (see Task 20)
- **Related Tasks**:
  - Task 18: Extract AI Interface
  - Task 19: Implement Gemini Adapter
  - Task 20: Implement OpenAI Adapter
  - Task 21: Create AI Config Table
  - Task 26: Register AI Services & DI Logic

## Notes
- This pattern allows adding new AI providers simply by creating a new provider class and adding a case to the factory's switch statement
- The factory pattern enables A/B testing different providers without code changes
- Configuration can be changed at runtime by updating the database
