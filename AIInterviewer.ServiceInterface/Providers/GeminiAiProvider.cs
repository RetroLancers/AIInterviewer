using AIInterviewer.ServiceInterface.Interfaces;
using AIInterviewer.ServiceModel.Types.Ai;
using AIInterviewer.ServiceModel.Tables.Configuration;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;

namespace AIInterviewer.ServiceInterface.Providers;

public class GeminiAiProvider(SiteConfigHolder siteConfigHolder, ILogger<GeminiAiProvider> logger) : IAiProvider
{
    public string ProviderName => "Gemini";

    private Client GetClient()
    {
        var apiKey = siteConfigHolder.SiteConfig?.GeminiApiKey;
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("Gemini API Key is not configured.");
        }
        
        return new Client(apiKey: apiKey, httpOptions: new HttpOptions()
        {
            Timeout = 180 * 1000 * 2
        });
    }

    private string GetModel()
    {
         var model = siteConfigHolder.SiteConfig?.InterviewModel;
         if (string.IsNullOrEmpty(model)) return "gemini-2.0-flash-exp"; // Default or fallback
         return model;
    }
    
    private string GetFallbackModel()
    {
        // Could be configured, but hardcoding reasonable fallback for now based on GeminiClient
        return "gemini-2.0-flash";
    }

    public async Task<string?> GenerateTextAsync(string prompt, string? systemPrompt = null, double? temperature = null, int? maxOutputTokens = null)
    {
        return await ExecuteWithRetryAsync(async (model) =>
        {
            var config = BuildGenerateContentConfig(systemPrompt, temperature, maxOutputTokens);
            var response = await GetClient().Models.GenerateContentAsync(
                model: model,
                contents: prompt,
                config: config
            );
            return ExtractText(response);
        });
    }


    public async Task<string?> GenerateTextAsync(IEnumerable<AiMessage> messages, string? systemPrompt = null, double? temperature = null, int? maxOutputTokens = null)
    {
        return await ExecuteWithRetryAsync(async (model) =>
        {
            var config = BuildGenerateContentConfig(systemPrompt, temperature, maxOutputTokens);
            var contentList = messages.Select(m => new Content
            {
                Role = MapRole(m.Role),
                Parts = new List<Part> { new Part { Text = m.Content } }
            }).ToList();

            var response = await GetClient().Models.GenerateContentAsync(
                model: model,
                contents: contentList,
                config: config
            );
            return ExtractText(response);
        });
    }

    public async Task<string?> GenerateTextFromAudioAsync(string prompt, byte[] audioData, string mimeType, string? systemPrompt = null)
    {
        return await ExecuteWithRetryAsync(async (model) =>
        {
            var config = BuildGenerateContentConfig(systemPrompt, null, null);
            var content = new Content
            {
                Parts = new List<Part>
                {
                    new Part { Text = prompt },
                    new Part 
                    { 
                        InlineData = new Blob 
                        { 
                            Data = audioData, 
                            MimeType = mimeType 
                        } 
                    }
                }
            };

            var response = await GetClient().Models.GenerateContentAsync(
                model: model,
                contents: new List<Content> { content },
                config: config
            );
            return ExtractText(response);
        });
    }

    public async Task<T?> GenerateJsonAsync<T>(string prompt, string? systemPrompt = null) where T : class
    {
        return await ExecuteWithRetryAsync(async (model) =>
        {
            var schema = GenerateSchema(typeof(T));
            var config = new GenerateContentConfig
            {
                ResponseMimeType = "application/json",
                ResponseSchema = schema
            };

            if (systemPrompt != null)
            {
                config.SystemInstruction = new Content
                {
                    Parts = new List<Part> { new Part { Text = systemPrompt } }
                };
            }

            var response = await GetClient().Models.GenerateContentAsync(
                model: model,
                contents: prompt,
                config: config
            );

            var text = ExtractText(response);
            if (!string.IsNullOrEmpty(text))
            {
                return JsonConvert.DeserializeObject<T>(text);
            }
            return null;
        });
    }

    // Helper methods

    private async Task<TResult?> ExecuteWithRetryAsync<TResult>(Func<string, Task<TResult?>> action)
    {
        var model = GetModel();
        var fallback = GetFallbackModel();
        int retryCount = 0;

        while (true)
        {
            try
            {
                return await action(model);
            }
            catch (Google.GenAI.ServerError e) when (e.Message.Contains("overloaded"))
            {
                if (retryCount >= 3) throw;
                retryCount++;
                model = fallback;
                logger.LogWarning("Gemini overloaded, switching to fallback model {FallbackModel}. Retry {RetryCount}", fallback, retryCount);
                await Task.Delay(1000 * retryCount); // Simple backoff
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    private GenerateContentConfig BuildGenerateContentConfig(string? systemPrompt, double? temperature, int? maxOutputTokens)
    {
        var config = new GenerateContentConfig();
        if (systemPrompt != null)
        {
            config.SystemInstruction = new Content
            {
                Parts = new List<Part> { new Part { Text = systemPrompt } }
            };
        }
        if (temperature.HasValue) config.Temperature = temperature.Value;
        if (maxOutputTokens.HasValue) config.MaxOutputTokens = maxOutputTokens.Value;
        return config;
    }

    private string? ExtractText(GenerateContentResponse response)
    {
        if (response.Candidates != null && response.Candidates.Any())
        {
            var candidate = response.Candidates[0];
            if (candidate.Content != null && candidate.Content.Parts != null && candidate.Content.Parts.Any())
            {
                return candidate.Content.Parts[0].Text;
            }
        }
        return null;
    }
    
    private string MapRole(AiRole role)
    {
        return role switch
        {
            AiRole.User => "user",
            AiRole.Model => "model",
            // System role is usually handled via SystemInstruction, but if passed in history, generic 'user' or 'model' mapping might be needed or ignored depending on API check.
            // Gemini doesn't support 'system' role in contents list usually.
            AiRole.System => "user", 
            _ => "user"
        };
    }

    private Schema GenerateSchema(System.Type type)
    {
        if (type == typeof(int) || type == typeof(long)) return new Schema { Type = Google.GenAI.Types.Type.INTEGER };
        if (type == typeof(string)) return new Schema { Type = Google.GenAI.Types.Type.STRING };
        if (type == typeof(bool)) return new Schema { Type = Google.GenAI.Types.Type.BOOLEAN };
        if (type == typeof(double) || type == typeof(float) || type == typeof(decimal)) return new Schema { Type = Google.GenAI.Types.Type.NUMBER };
        
        if (type.IsArray)
        {
             return new Schema 
             { 
                 Type = Google.GenAI.Types.Type.ARRAY, 
                 Items = GenerateSchema(type.GetElementType()!)
             };
        }
        
        if (type.IsGenericType && typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
        {
             var args = type.GetGenericArguments();
             if (args.Length > 0)
             {
                 return new Schema 
                 { 
                     Type = Google.GenAI.Types.Type.ARRAY, 
                     Items = GenerateSchema(args[0])
                 };
             }
        }

        if (type.IsClass && type != typeof(string))
        {
             var schema = new Schema { Type = Google.GenAI.Types.Type.OBJECT, Properties = new Dictionary<string, Schema>(), Required = new List<string>() };
             foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
             {
                 schema.Properties[prop.Name] = GenerateSchema(prop.PropertyType);
                 schema.Required.Add(prop.Name);
             }
             return schema;
        }
        return new Schema { Type = Google.GenAI.Types.Type.STRING }; // Fallback
    }
}
