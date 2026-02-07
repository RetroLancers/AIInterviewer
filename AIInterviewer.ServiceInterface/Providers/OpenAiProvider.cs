#pragma warning disable OPENAI001

using OpenAI.Chat;
using AIInterviewer.ServiceInterface.Interfaces;
using AIInterviewer.ServiceModel.Types.Ai;
using AIInterviewer.ServiceModel.Tables.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;
using OpenAI;
using System.ClientModel;

namespace AIInterviewer.ServiceInterface.Providers;

public class OpenAiProvider : IAiProvider
{
    private readonly ChatClient _chatClient;
    private readonly ILogger<OpenAiProvider> _logger;
    private readonly AiServiceConfig _config;

    public string ProviderName => "OpenAI";

    public OpenAiProvider(AiServiceConfig config, ILogger<OpenAiProvider> logger)
    {
        _config = config;
        _logger = logger;

        var options = new OpenAIClientOptions();
        if (!string.IsNullOrEmpty(config.BaseUrl))
        {
            options.Endpoint = new Uri(config.BaseUrl);
        }

        _chatClient = new ChatClient(config.ModelId ?? "gpt-4o", new ApiKeyCredential(config.ApiKey), options);
    }

    public async Task<string?> GenerateTextAsync(string prompt, string? systemPrompt = null, double? temperature = null, int? maxOutputTokens = null)
    {
        try
        {
            var messages = new List<ChatMessage>();
            if (!string.IsNullOrEmpty(systemPrompt))
            {
                messages.Add(new SystemChatMessage(systemPrompt));
            }
            messages.Add(new UserChatMessage(prompt));

            var options = new ChatCompletionOptions();
            if (temperature.HasValue) options.Temperature = (float?)temperature.Value;
            if (maxOutputTokens.HasValue) options.MaxOutputTokenCount = maxOutputTokens.Value;

            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);
            return completion.Content[0].Text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OpenAI GenerateTextAsync (string prompt)");
            throw;
        }
    }

    public async Task<string?> GenerateTextAsync(IEnumerable<AiMessage> messages, string? systemPrompt = null, double? temperature = null, int? maxOutputTokens = null)
    {
        try
        {
            var sdkMessages = new List<ChatMessage>();
            if (!string.IsNullOrEmpty(systemPrompt))
            {
                sdkMessages.Add(new SystemChatMessage(systemPrompt));
            }

            foreach (var msg in messages)
            {
                sdkMessages.Add(MapMessage(msg));
            }

            var options = new ChatCompletionOptions();
            if (temperature.HasValue) options.Temperature = (float?)temperature.Value;
            if (maxOutputTokens.HasValue) options.MaxOutputTokenCount = maxOutputTokens.Value;

            ChatCompletion completion = await _chatClient.CompleteChatAsync(sdkMessages, options);
            return completion.Content[0].Text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OpenAI GenerateTextAsync (IEnumerable<AiMessage> messages)");
            throw;
        }
    }

    public async Task<string?> GenerateTextFromAudioAsync(string prompt, byte[] audioData, string mimeType, string? systemPrompt = null)
    {
        try
        {
            var messages = new List<ChatMessage>();
            if (!string.IsNullOrEmpty(systemPrompt))
            {
                messages.Add(new SystemChatMessage(systemPrompt));
            }

            // Note: GPT-4o supports audio input. 
            // The official SDK uses ChatMessageContentPart for multi-modal content.
            var userMessage = new UserChatMessage(
                ChatMessageContentPart.CreateTextPart(prompt),
                ChatMessageContentPart.CreateInputAudioPart(BinaryData.FromBytes(audioData), MapMimeToFormat(mimeType))
            );
            messages.Add(userMessage);

            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages);
            return completion.Content[0].Text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OpenAI GenerateTextFromAudioAsync");
            throw;
        }
    }

    private ChatInputAudioFormat MapMimeToFormat(string mimeType)
    {
        return mimeType.ToLower() switch
        {
            "audio/wav" => ChatInputAudioFormat.Wav,
            "audio/mp3" => ChatInputAudioFormat.Mp3,
            "audio/mpeg" => ChatInputAudioFormat.Mp3,
            _ => ChatInputAudioFormat.Wav
        };
    }

    public async Task<T?> GenerateJsonAsync<T>(string prompt, string? systemPrompt = null) where T : class
    {
        try
        {
            var messages = new List<ChatMessage>();
            if (!string.IsNullOrEmpty(systemPrompt))
            {
                messages.Add(new SystemChatMessage(systemPrompt));
            }
            messages.Add(new UserChatMessage(prompt));

            var schema = GenerateOpenAiSchema(typeof(T));
            var schemaJson = JsonConvert.SerializeObject(schema);

            var options = new ChatCompletionOptions
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: typeof(T).Name,
                    jsonSchema: BinaryData.FromString(schemaJson),
                    jsonSchemaIsStrict: true
                )
            };

            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);
            var text = completion.Content[0].Text;
            
            if (!string.IsNullOrEmpty(text))
            {
                return JsonConvert.DeserializeObject<T>(text);
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OpenAI GenerateJsonAsync<{Type}>", typeof(T).Name);
            throw;
        }
    }

    public async Task<IEnumerable<string>> ListModelsAsync()
    {
        // For now, return a common set of OpenAI models as listing them via API is different
        return new List<string> { "gpt-4o", "gpt-4o-mini", "gpt-4-turbo", "gpt-3.5-turbo" };
    }

    private ChatMessage MapMessage(AiMessage msg)

    {
        return msg.Role switch
        {
            AiRole.User => new UserChatMessage(msg.Content),
            AiRole.System => new SystemChatMessage(msg.Content),
            AiRole.Model => new AssistantChatMessage(msg.Content),
            _ => new UserChatMessage(msg.Content)
        };
    }

    private object GenerateOpenAiSchema(Type type)
    {
        if (type == typeof(string)) return new { type = "string" };
        if (type == typeof(int) || type == typeof(long)) return new { type = "integer" };
        if (type == typeof(bool)) return new { type = "boolean" };
        if (type == typeof(double) || type == typeof(float) || type == typeof(decimal)) return new { type = "number" };

        if (type.IsArray)
        {
            return new { type = "array", items = GenerateOpenAiSchema(type.GetElementType()!) };
        }

        if (type.IsGenericType && typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
        {
            var args = type.GetGenericArguments();
            if (args.Length > 0)
            {
                return new { type = "array", items = GenerateOpenAiSchema(args[0]) };
            }
        }

        if (type.IsClass && type != typeof(string))
        {
            var properties = new Dictionary<string, object>();
            var required = new List<string>();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                properties[prop.Name] = GenerateOpenAiSchema(prop.PropertyType);
                required.Add(prop.Name);
            }

            return new
            {
                type = "object",
                properties = properties,
                required = required,
                additionalProperties = false
            };
        }

        return new { type = "string" };
    }

    public async Task<IEnumerable<string>> ListModelsAsync()
    {
        // For now return a few common models. 
        // In a real implementation, we might want to query the OpenAI API.
        return await Task.FromResult(new List<string> { "gpt-4o", "gpt-4o-mini", "o1-preview", "o1-mini" });
    }
}
