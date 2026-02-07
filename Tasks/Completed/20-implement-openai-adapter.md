# Implement OpenAI Adapter

## Objective
Implement the `IAiProvider` interface for OpenAI using the official OpenAI .NET SDK. This provider will work with the factory pattern (Task 18a) and accept `AiServiceConfig` for configuration.

## Context
With the interface extracted (Task 18) and factory pattern implemented (Task 18a), we need to add OpenAI as an alternative provider to Gemini. This enables multi-provider support and allows users to choose their preferred AI backend.

## Requirements

### 1. Create Provider Class
Create `OpenAiProvider` in `ServiceInterface/Providers/` implementing `IAiProvider`.

### 2. Constructor Requirements
The provider constructor must:
- Accept `AiServiceConfig` as the **first parameter** (required for `ActivatorUtilities.CreateInstance`)
- Accept any other dependencies via DI (e.g., `ILogger<OpenAiProvider>`)
- Extract configuration values from `AiServiceConfig`:
  - `config.ApiKey` → OpenAI API authentication
  - `config.ModelId` → OpenAI model to use (e.g., "gpt-4o", "gpt-4o-mini", "gpt-5.1")
  - `config.BaseUrl` → Optional custom endpoint (for Azure OpenAI or compatible APIs)

Example constructor:
```csharp
public OpenAiProvider(
    AiServiceConfig config,
    ILogger<OpenAiProvider> logger)
{
    _logger = logger;
    _modelId = config.ModelId;
    
    // Create OpenAI client with optional custom endpoint
    var options = new OpenAIClientOptions();
    if (!string.IsNullOrEmpty(config.BaseUrl))
    {
        options.Endpoint = new Uri(config.BaseUrl);
    }
    
    _chatClient = new ChatClient(_modelId, config.ApiKey, options);
}
```

### 3. OpenAI SDK Integration
Use the official **OpenAI .NET SDK** (`OpenAI` NuGet package):
```bash
dotnet add package OpenAI
```

Key classes to use:
- `ChatClient` - For chat completions
- `ChatCompletion` - Response object
- `ChatMessage` types: `UserChatMessage`, `AssistantChatMessage`, `SystemChatMessage`

### 4. Message Mapping
Implement bidirectional mapping:

**Input Mapping** (`AiMessage` → OpenAI SDK):
- `AiRole.User` → `new UserChatMessage(content)`
- `AiRole.Assistant` → `new AssistantChatMessage(content)`
- `AiRole.System` → `new SystemChatMessage(content)`

**Output Mapping** (OpenAI SDK → `AiChatResponse`):
- `completion.Content[0].Text` → `AiChatResponse.Content`
- `completion.FinishReason` → `AiChatResponse.FinishReason`
- Token usage from response → `AiChatResponse.TokenUsage` (if tracking)

## Implementation Steps

### 1. Install OpenAI SDK
```bash
cd AIInterviewer.ServiceInterface
dotnet add package OpenAI
```

### 2. Create `OpenAiProvider.cs` in `ServiceInterface/Providers/`
```csharp
using OpenAI.Chat;
using AIInterviewer.ServiceInterface.Interfaces;
using AIInterviewer.ServiceInterface.Models;
using AIInterviewer.ServiceModel.Tables.Configuration;

namespace AIInterviewer.ServiceInterface.Providers;

public class OpenAiProvider : IAiProvider
{
    private readonly ChatClient _chatClient;
    private readonly ILogger<OpenAiProvider> _logger;
    private readonly string _modelId;

    public OpenAiProvider(
        AiServiceConfig config,
        ILogger<OpenAiProvider> logger)
    {
        _logger = logger;
        _modelId = config.ModelId;

        var options = new OpenAIClientOptions();
        if (!string.IsNullOrEmpty(config.BaseUrl))
        {
            options.Endpoint = new Uri(config.BaseUrl);
        }

        _chatClient = new ChatClient(_modelId, config.ApiKey, options);
    }

    public async Task<AiChatResponse> ChatAsync(AiChatRequest request)
    {
        // Implementation below
    }

    public async Task<string> GenerateTextAsync(string prompt)
    {
        // Implementation below
    }
}
```

### 3. Implement `ChatAsync` Method
```csharp
public async Task<AiChatResponse> ChatAsync(AiChatRequest request)
{
    try
    {
        // Convert AI-agnostic messages to OpenAI format
        var messages = new List<ChatMessage>();
        foreach (var message in request.Messages)
        {
            messages.Add(ToOpenAiMessage(message));
        }

        // Call OpenAI API
        ChatCompletion completion = await _chatClient.CompleteChatAsync(messages);

        // Convert response
        return new AiChatResponse
        {
            Content = completion.Content[0].Text,
            FinishReason = completion.FinishReason.ToString(),
            // Add more mappings as needed
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error calling OpenAI ChatAsync");
        throw;
    }
}
```

### 4. Implement `GenerateTextAsync` Method
```csharp
public async Task<string> GenerateTextAsync(string prompt)
{
    try
    {
        ChatCompletion completion = await _chatClient.CompleteChatAsync(prompt);
        return completion.Content[0].Text;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error calling OpenAI GenerateTextAsync");
        throw;
    }
}
```

### 5. Implement Message Conversion Helper
```csharp
private ChatMessage ToOpenAiMessage(AiMessage message)
{
    return message.Role switch
    {
        AiRole.User => new UserChatMessage(message.Content),
        AiRole.Assistant => new AssistantChatMessage(message.Content),
        AiRole.System => new SystemChatMessage(message.Content),
        _ => throw new ArgumentException($"Unknown role: {message.Role}", nameof(message))
    };
}
```

### 6. Update Factory to Include OpenAI
In `AiProviderFactory.cs`, uncomment/add the OpenAI case:
```csharp
public IAiProvider GetProvider(AiServiceConfig config)
{
    return config.ProviderType switch
    {
        "Gemini" => ActivatorUtilities.CreateInstance<GeminiAiProvider>(serviceProvider, config),
        "OpenAI" => ActivatorUtilities.CreateInstance<OpenAiProvider>(serviceProvider, config),
        _ => throw new ArgumentException($"Unknown AI Provider Type: {config.ProviderType}", nameof(config))
    };
}
```

## OpenAI SDK Usage Examples

### Basic Chat Completion
```csharp
ChatClient client = new("gpt-4o", apiKey);
ChatCompletion completion = client.CompleteChat("Say 'this is a test.'");
Console.WriteLine(completion.Content[0].Text);
```

### Async Chat Completion
```csharp
ChatCompletion completion = await client.CompleteChatAsync("Say 'this is a test.'");
Console.WriteLine(completion.Content[0].Text);
```

### Multi-Message Conversation
```csharp
List<ChatMessage> messages = new()
{
    new SystemChatMessage("You are a helpful assistant."),
    new UserChatMessage("What's the weather like today?")
};

ChatCompletion completion = await client.CompleteChatAsync(messages);
```

### Using Custom Endpoint (Azure OpenAI or compatible)
```csharp
ChatClient client = new(
    model: "gpt-4o",
    credential: new ApiKeyCredential(apiKey),
    options: new OpenAIClientOptions()
    {
        Endpoint = new Uri("https://your-custom-endpoint.com")
    });
```

## OpenAI Model Support

The OpenAI SDK supports:
- **GPT-4 Series**: `gpt-4o`, `gpt-4o-mini`, `gpt-4-turbo`
- **GPT-5 Series**: `gpt-5.1` (as shown in docs)
- **Legacy**: `gpt-3.5-turbo` (not recommended for new projects)

Ensure the `ModelId` in `AiServiceConfig` matches supported models.

## Handling OpenAI Quirks

### System Messages
OpenAI supports system messages natively:
```csharp
new SystemChatMessage("You are a technical interviewer...")
```

### Streaming (Optional Enhancement)
If streaming is needed in the future:
```csharp
AsyncCollectionResult<StreamingChatCompletionUpdate> updates 
    = client.CompleteChatStreamingAsync(messages);

await foreach (var update in updates)
{
    if (update.ContentUpdate.Count > 0)
    {
        Console.Write(update.ContentUpdate[0].Text);
    }
}
```

### Error Handling
Common OpenAI errors:
- **401**: Invalid API key
- **429**: Rate limit exceeded
- **500**: Server error

Implement retry logic for transient errors (429, 500).

## Definition of Done
*   [ ] OpenAI NuGet package (`OpenAI`) is installed
*   [ ] `OpenAiProvider` exists in `ServiceInterface/Providers/`
*   [ ] `OpenAiProvider` implements `IAiProvider` interface
*   [ ] Constructor accepts `AiServiceConfig` as first parameter
*   [ ] All configuration is extracted from `AiServiceConfig`
*   [ ] `ChatAsync` method properly converts messages and calls OpenAI SDK
*   [ ] `GenerateTextAsync` method is implemented
*   [ ] Message mapping works bidirectionally (AI-agnostic ↔ OpenAI)
*   [ ] Error handling and logging are implemented
*   [ ] Factory updated to instantiate `OpenAiProvider` for `ProviderType == "OpenAI"`
*   [ ] Code compiles without errors
*   [ ] Successfully tested with OpenAI API (or mock)

## Testing Checklist
- [ ] Test with valid OpenAI API key
- [ ] Test with invalid API key (should fail gracefully)
- [ ] Test with different models (gpt-4o, gpt-4o-mini)
- [ ] Test with custom endpoint (if using Azure OpenAI)
- [ ] Verify error handling and logging
- [ ] Confirm factory can create OpenAiProvider instances

## Notes
- The provider should be stateless except for configuration
- Each request should be independent
- Consider implementing retry logic with exponential backoff for rate limits
- Log all API calls for debugging and monitoring
- OpenAI SDK handles connection pooling and HTTP client management automatically

## Clood Groups
Reference these files when implementing:
- **Interface to Implement**:
  - `AIInterviewer.ServiceInterface/Interfaces/IAiProvider.cs`
  - `AIInterviewer.ServiceInterface/Models/AiMessage.cs`
  - `AIInterviewer.ServiceInterface/Models/AiChatRequest.cs`
  - `AIInterviewer.ServiceInterface/Models/AiChatResponse.cs`
- **Factory Integration**:
  - `AIInterviewer.ServiceInterface/Factories/AiProviderFactory.cs` (add OpenAI case)
- **Configuration**:
  - `AIInterviewer.ServiceModel/Tables/Configuration/AiServiceConfig.cs`
- **Example Implementation** (reference pattern):
  - `AIInterviewer.ServiceInterface/Providers/GeminiAiProvider.cs` (Task 19)
- **Related Tasks**:
  - Task 18: Extract AI Interface
  - Task 18a: Implement AI Provider Factory
  - Task 19: Implement Gemini Adapter (similar pattern)
  - Task 26: Register AI Services & DI Logic

## References
- [OpenAI .NET SDK GitHub](https://github.com/openai/openai-dotnet)
- [OpenAI .NET SDK NuGet](https://www.nuget.org/packages/OpenAI)
- [OpenAI API Documentation](https://platform.openai.com/docs)
