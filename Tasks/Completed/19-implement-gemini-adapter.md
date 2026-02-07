# Implement Gemini Adapter

## Objective
Implement the newly created `IAiProvider` using the existing Gemini logic. This provider will work with the factory pattern (Task 18a) and accept `AiServiceConfig` for configuration.

## Context
We have extracted the interface (Task 18) and implemented the factory pattern (Task 18a). Now we need to move the current hardcoded Gemini logic into a concrete implementation of `IAiProvider` that can be instantiated by the `AiProviderFactory`.

## Requirements

### 1. Create Provider Class
Create `GeminiAiProvider` in `ServiceInterface/Providers/` implementing `IAiProvider`.

### 2. Constructor Requirements
The provider constructor must:
- Accept `AiServiceConfig` as the **first parameter** (required for `ActivatorUtilities.CreateInstance`)
- Accept any other dependencies via DI (e.g., `ILogger<GeminiAiProvider>`, `HttpClient`)
- Extract configuration values from `AiServiceConfig`:
  - `config.ApiKey` → Gemini API authentication
  - `config.ModelId` → Gemini model to use (e.g., "gemini-1.5-pro")
  - `config.BaseUrl` → Optional custom endpoint (if supporting self-hosted or proxy)

Example constructor:
```csharp
public GeminiAiProvider(
    AiServiceConfig config,
    ILogger<GeminiAiProvider> logger)
{
    _apiKey = config.ApiKey;
    _modelId = config.ModelId;
    _baseUrl = config.BaseUrl ?? "https://generativelanguage.googleapis.com";
    _logger = logger;
    
    // Initialize Gemini SDK client
    _geminiClient = new GeminiClient(_apiKey, _baseUrl);
}
```

### 3. Migration from Existing Logic
- Move the logic from current services (e.g., `InterviewService` methods that call Gemini directly) into this provider
- Consolidate all Gemini SDK interactions into this single class
- Ensure no static/global config reading within the provider (all config comes from `AiServiceConfig`)

### 4. Message Mapping
Implement bidirectional mapping:

**Input Mapping** (`AiMessage` → Gemini SDK):
- `AiMessage.Role` → Gemini `MessageRole` (e.g., "user", "model")
- `AiMessage.Content` → Gemini message content
- Handle system messages (Gemini may require special handling)

**Output Mapping** (Gemini SDK → `AiChatResponse`):
- Gemini response text → `AiChatResponse.Content`
- Gemini finish reason → `AiChatResponse.FinishReason`
- Gemini token usage → `AiChatResponse.TokenUsage` (if tracking usage)

## Implementation Steps

1. **Create `GeminiAiProvider.cs`** in `ServiceInterface/Providers/`
   ```csharp
   using AIInterviewer.ServiceInterface.Interfaces;
   using AIInterviewer.ServiceInterface.Models;
   using AIInterviewer.ServiceModel.Tables.Configuration;
   
   namespace AIInterviewer.ServiceInterface.Providers;
   
   public class GeminiAiProvider : IAiProvider
   {
       private readonly string _apiKey;
       private readonly string _modelId;
       private readonly string? _baseUrl;
       private readonly ILogger<GeminiAiProvider> _logger;
       
       public GeminiAiProvider(
           AiServiceConfig config,
           ILogger<GeminiAiProvider> logger)
       {
           _apiKey = config.ApiKey;
           _modelId = config.ModelId;
           _baseUrl = config.BaseUrl;
           _logger = logger;
       }
       
       public async Task<AiChatResponse> ChatAsync(AiChatRequest request)
       {
           // Implementation
       }
       
       public async Task<string> GenerateTextAsync(string prompt)
       {
           // Implementation
       }
   }
   ```

2. **Implement `ChatAsync` Method**
   - Convert `AiChatRequest` messages to Gemini format
   - Call Gemini SDK
   - Convert response back to `AiChatResponse`
   - Handle errors gracefully with logging

3. **Implement `GenerateTextAsync` Method**
   - Simple text generation endpoint
   - Convert prompt to Gemini format
   - Return generated text

4. **Implement Message Conversion Helpers**
   ```csharp
   private GeminiMessage ToGeminiMessage(AiMessage message)
   {
       return new GeminiMessage
       {
           Role = message.Role == AiRole.User ? "user" : "model",
           Content = message.Content
       };
   }
   
   private AiChatResponse FromGeminiResponse(GeminiChatResponse geminiResponse)
   {
       return new AiChatResponse
       {
           Content = geminiResponse.Candidates[0].Content.Parts[0].Text,
           FinishReason = geminiResponse.Candidates[0].FinishReason,
           // ... other mappings
       };
   }
   ```

5. **Ensure Feature Parity**
   - Verify all existing Gemini functionality still works
   - Test with the same models used in current implementation
   - Validate error handling

6. **Update Services** (if not done in Task 18)
   - Remove direct Gemini SDK usage from `InterviewService`, `ChatService`, etc.
   - These services should now use `IAiProviderFactory` to get the provider

## Gemini SDK Reference
Using the official Google Gemini SDK for .NET:
```bash
dotnet add package Google.Generative.AI
```

Example Gemini SDK usage:
```csharp
var geminiClient = new GeminiClient(apiKey);
var model = geminiClient.GetGenerativeModel(modelId);

var response = await model.GenerateContentAsync(prompt);
var text = response.Text;
```

## Definition of Done
*   [x] `GeminiAiProvider` exists in `ServiceInterface/Providers/`
*   [x] `GeminiAiProvider` implements `IAiProvider` interface
*   [x] Constructor accepts `AiServiceConfig` as first parameter
*   [x] All configuration is extracted from `AiServiceConfig`, not from global config
*   [x] Message mapping works bidirectionally (AI-agnostic ↔ Gemini)
*   [x] `ChatAsync` and `GenerateTextAsync` methods are fully implemented
*   [x] Existing Gemini functionality maintains feature parity
*   [x] Error handling and logging are implemented
*   [x] Code compiles without errors
*   [x] Provider can be instantiated by `AiProviderFactory`

## Clood Groups
Reference these files when implementing:
- **Interface to Implement**:
  - `AIInterviewer.ServiceInterface/Interfaces/IAiProvider.cs`
  - `AIInterviewer.ServiceInterface/Models/AiMessage.cs`
  - `AIInterviewer.ServiceInterface/Models/AiChatRequest.cs`
  - `AIInterviewer.ServiceInterface/Models/AiChatResponse.cs`
- **Factory Integration**:
  - `AIInterviewer.ServiceInterface/Factories/AiProviderFactory.cs` (add Gemini case)
- **Configuration**:
  - `AIInterviewer.ServiceModel/Tables/Configuration/AiServiceConfig.cs`
- **Related Tasks**:
  - Task 18: Extract AI Interface
  - Task 18a: Implement AI Provider Factory
  - Task 20: Implement OpenAI Adapter (similar pattern)
  - Task 26: Register AI Services & DI Logic

## Notes
- The provider should be stateless except for configuration
- Each request should be independent
- Consider implementing retry logic for transient Gemini API errors
- Log important events (API calls, errors) for debugging
