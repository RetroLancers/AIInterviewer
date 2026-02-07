using OpenAI.Chat;
using AIInterviewer.ServiceInterface.Interfaces;
using AIInterviewer.ServiceModel.Types.Ai;
using AIInterviewer.ServiceModel.Tables.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AIInterviewer.ServiceInterface.Providers;

public class OpenAiProvider(AiServiceConfig config, ILogger<OpenAiProvider> logger) : IAiProvider
{
    public string ProviderName => "OpenAI";

    private ChatClient GetClient()
    {
        var apiKey = config.ApiKey;
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("OpenAI API Key is not configured in AiServiceConfig.");
        }

        var options = new OpenAIClientOptions();
        if (!string.IsNullOrEmpty(config.BaseUrl))
        {
            options.Endpoint = new Uri(config.BaseUrl);
        }
        
        return new ChatClient(config.ModelId ?? "gpt-4o", apiKey, options);
    }

    public async Task<string?> GenerateTextAsync(string prompt, string? systemPrompt = null, double? temperature = null, int? maxOutputTokens = null)
    {
        var messages = new List<ChatMessage>();
        if (!string.IsNullOrEmpty(systemPrompt))
        {
            messages.Add(new SystemChatMessage(systemPrompt));
        }
        messages.Add(new UserChatMessage(prompt));

        var options = new ChatCompletionOptions();
        if (temperature.HasValue) options.Temperature = (float)temperature.Value;
        if (maxOutputTokens.HasValue) options.MaxOutputTokenCount = maxOutputTokens.Value;

        var response = await GetClient().CompleteChatAsync(messages, options);
        return response.Value.Content[0].Text;
    }

    public async Task<string?> GenerateTextAsync(IEnumerable<AiMessage> messages, string? systemPrompt = null, double? temperature = null, int? maxOutputTokens = null)
    {
        var chatMessages = new List<ChatMessage>();
        if (!string.IsNullOrEmpty(systemPrompt))
        {
            chatMessages.Add(new SystemChatMessage(systemPrompt));
        }

        foreach (var msg in messages)
        {
            chatMessages.Add(MapRole(msg));
        }

        var options = new ChatCompletionOptions();
        if (temperature.HasValue) options.Temperature = (float)temperature.Value;
        if (maxOutputTokens.HasValue) options.MaxOutputTokenCount = maxOutputTokens.Value;

        var response = await GetClient().CompleteChatAsync(chatMessages, options);
        return response.Value.Content[0].Text;
    }

    public async Task<string?> GenerateTextFromAudioAsync(string prompt, byte[] audioData, string mimeType, string? systemPrompt = null)
    {
        // OpenAI GPT-4o multimodal input for audio is coming, but for now we might use Whisper for transcription 
        // or wait for official SDK support for audio parts in ChatMessage.
        // For the purposes of this adapter, we will throw NotSupported until verified.
        logger.LogWarning("GenerateTextFromAudioAsync is not yet implemented for OpenAI.");
        throw new NotSupportedException("Audio input is not yet supported in the OpenAI provider.");
    }

    public async Task<T?> GenerateJsonAsync<T>(string prompt, string? systemPrompt = null) where T : class
    {
        var client = GetClient();
        var messages = new List<ChatMessage>();
        if (!string.IsNullOrEmpty(systemPrompt))
        {
            messages.Add(new SystemChatMessage(systemPrompt));
        }
        messages.Add(new UserChatMessage(prompt));
        
        var options = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
        };

        var response = await client.CompleteChatAsync(messages, options);
        var text = response.Value.Content[0].Text;
        
        if (string.IsNullOrEmpty(text)) return null;
        
        return JsonSerializer.Deserialize<T>(text, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    private ChatMessage MapRole(AiMessage message)
    {
        return message.Role switch
        {
            AiRole.User => new UserChatMessage(message.Content),
            AiRole.Model => new AssistantChatMessage(message.Content),
            AiRole.System => new SystemChatMessage(message.Content),
            _ => new UserChatMessage(message.Content)
        };
    }
}
