#pragma warning disable OPENAI001

using OpenAI.Chat;
using AIInterviewer.ServiceInterface.Interfaces;
using AIInterviewer.ServiceModel.Types.Ai;
using AIInterviewer.ServiceModel.Tables.Configuration;
using AIInterviewer.ServiceInterface.Providers.Generators;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;
using OpenAI;
using System.ClientModel;
using OpenAI.Models;

namespace AIInterviewer.ServiceInterface.Providers;

public class OpenAiProvider : IAiProvider
{
    private readonly OpenAIModelClient _modelClient;
    private readonly ChatClient _chatClient;
    private readonly ILogger<OpenAiProvider> _logger;
    private readonly AiServiceConfig _config;

    public string ProviderName => "OpenAI";

    public OpenAiProvider(AiServiceConfig config, ILogger<OpenAiProvider> logger)
    {
        _config = config;
        _logger = logger;

        var options = new OpenAIClientOptions();


        _modelClient = new OpenAIModelClient(new ApiKeyCredential(config.ApiKey), options);
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

            sdkMessages.AddRange(messages.Select(MapMessage));

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

    public async Task<T?> GenerateJsonAsync<T>(string prompt, AiSchemaDefinition schema, string schemaName, string? systemPrompt = null) where T : class
    {
        try
        {
            var messages = new List<ChatMessage>();
            if (!string.IsNullOrEmpty(systemPrompt))
            {
                messages.Add(new SystemChatMessage(systemPrompt));
            }
            messages.Add(new UserChatMessage(prompt));

            var openAiSchema = OpenAiSchemaGenerator.MapSchema(schema);
            var schemaJson = JsonConvert.SerializeObject(openAiSchema, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            });

            var options = new ChatCompletionOptions
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: schemaName,
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


    public async Task<IEnumerable<string>> ListModelsAsync()
    {
        var clientResult = await _modelClient.GetModelsAsync();
        return clientResult.Value.Select(model => model.Id).ToList();
    }
}
