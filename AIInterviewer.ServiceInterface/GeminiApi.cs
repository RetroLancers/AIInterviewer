namespace TyphoonSharp.ServiceInterface;

using Google.GenAI.Types;
using Google.GenAI;
using Newtonsoft.Json;
 

public class GeminiClient(string apiKey, string model, string fallbackModel = "gemini-2.5-flash")
{
    private Client _client = new(apiKey: apiKey, httpOptions: new HttpOptions()
    {
        Timeout = 180 * 1000 * 2
    });

    private string _model = model;
    private string _primaryModel = model;
    private string _fallbackModel = fallbackModel;

    public GeminiClient Clone()
    {
        return new GeminiClient(apiKey, _primaryModel, _fallbackModel);
    }

    public async Task<Pager<Model, ListModelsConfig, ListModelsResponse>> GetModels(string? pageToken)
    {
        return await _client.Models.ListAsync(new ListModelsConfig()
        {
            PageToken = pageToken
        });
    }

    public void ChangeModel(string model)
    {
        _model = model;
    }

    public void SetModels(string primaryModel, string fallbackModel)
    {
        _primaryModel = primaryModel;
        _fallbackModel = fallbackModel;
        _model = primaryModel;
    }

    public async Task<string?> GenerateTextAsync(string prompt, string? systemInstruction = null, double? temperature = null, int? maxOutputTokens = null, int retryCount = 0)
    {
        // Try to switch back to primary model if we are on fallback and this is not a retry
        if (retryCount == 0 && _model == _fallbackModel)
        {
            _model = _primaryModel;
        }

        try
        {
            var config = new GenerateContentConfig();
            var hasConfig = false;

            if (systemInstruction != null)
            {
                config.SystemInstruction = new Content
                {
                    Parts = new List<Part> { new Part { Text = systemInstruction } }
                };
                hasConfig = true;
            }
            if (temperature.HasValue)
            {
                config.Temperature = temperature.Value;
                hasConfig = true;
            }
            if (maxOutputTokens.HasValue)
            {
                config.MaxOutputTokens = maxOutputTokens.Value;
                hasConfig = true;
            }

            var response = await _client.Models.GenerateContentAsync(
                model: _model,
                contents: prompt,
                config: hasConfig ? config : null
            );

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
        catch (Google.GenAI.ServerError e) when (e.Message.Contains("overloaded"))
        {
            if (retryCount >= 3) throw;
            ChangeModel(_fallbackModel);
            return await GenerateTextAsync(prompt, systemInstruction, temperature, maxOutputTokens, retryCount: retryCount + 1);
        }
    }

    public async Task<T?> GenerateJsonAsync<T>(string prompt, Schema schema, string? systemInstruction = null, int retryCount = 0) where T : class
    {
        
        if (retryCount == 0 && _model == _fallbackModel)
        {
            _model = _primaryModel;
        }
        
        try
        {
            var config = new GenerateContentConfig
            {
                ResponseMimeType = "application/json",
                ResponseSchema = schema
            };

            if (systemInstruction != null)
            {
                config.SystemInstruction = new Content
                {
                    Parts = new List<Part> { new Part { Text = systemInstruction } }
                };
            }

            var response = await _client.Models.GenerateContentAsync(
                model: _model,
                contents: prompt,
                config: config
            );

            if (response.Candidates == null || response.Candidates.Count == 0) return null;
            var candidate = response.Candidates[0];
            if (candidate.Content == null || candidate.Content.Parts == null ||
                candidate.Content.Parts.Count == 0) return null;
            var text = candidate.Content.Parts[0].Text;
            if (text != null)
            {
                return JsonConvert.DeserializeObject<T>(text);
            }
            return null;
        }
        catch (Google.GenAI.ServerError e) when (e.Message.Contains("overloaded"))
        {
            if (retryCount >= 3) throw;
            ChangeModel(_fallbackModel);
            await Task.Delay(5000);
            return await GenerateJsonAsync<T>(prompt, schema, systemInstruction, retryCount: retryCount + 1);
        }
    }

public class GroundedJsonResult<T> where T : class
    {
        public T? Data { get; set; }
        public GenerateContentResponse Response { get; set; }
    }

    public async Task<GroundedJsonResult<T>?> GenerateJsonWithGroundedSearchAsync<T>(string prompt, Schema schema, string? systemInstruction = null, int retryCount = 0) where T : class
    {
        if (retryCount == 0 && _model == _fallbackModel)
        {
            _model = _primaryModel;
        }

        try
        {
            var config = new GenerateContentConfig
            {
                ResponseMimeType = "application/json",
                ResponseSchema = schema,
                Tools = new List<Tool>
                {
                    new() { GoogleSearch = new() }
                }
            };

            if (systemInstruction != null)
            {
                config.SystemInstruction = new Content
                {
                    Parts = new List<Part> { new Part { Text = systemInstruction } }
                };
            }

            var response = await _client.Models.GenerateContentAsync(
                model: _model,
                contents: prompt,
                config: config
            );

            if (response.Candidates == null || response.Candidates.Count == 0) return null;
            var candidate = response.Candidates[0];
            if (candidate.Content == null || candidate.Content.Parts == null ||
                candidate.Content.Parts.Count == 0) return null;
            var text = candidate.Content.Parts[0].Text;
            
            T? deserializedData = null;
            if (text != null)
            {
                deserializedData = JsonConvert.DeserializeObject<T>(text);
            }

            return new GroundedJsonResult<T>
            {
                Data = deserializedData,
                Response = response
            };
        }
        catch (Google.GenAI.ServerError e) when (e.Message.Contains("overloaded"))
        {
            if (retryCount >= 3) throw;
            ChangeModel(_fallbackModel);
            return await GenerateJsonWithGroundedSearchAsync<T>(prompt, schema, systemInstruction, retryCount: retryCount + 1);
        }
    }

    /// <summary>
    /// Generates content with Google search grounding enabled
    /// </summary>
    /// <param name="prompt">The user prompt</param>
    /// <param name="systemInstruction">Optional system instruction</param>
    /// <param name="responseSchema">Optional JSON schema for structured output</param>
    /// <param name="retryCount">Current retry count</param>
    /// <returns>Response with grounding metadata</returns>
    public async Task<GenerateContentResponse> GenerateWithGroundedSearchAsync(
        string prompt, 
        string? systemInstruction = null,
        Schema? responseSchema = null,
        int retryCount = 0)
    {
        // Try to switch back to primary model if we are on fallback and this is not a retry
        if (retryCount == 0 && _model == _fallbackModel)
        {
            _model = _primaryModel;
        }

        try
        {
            var config = new GenerateContentConfig
            {
                Tools = new List<Tool>
                {
                    new() { GoogleSearch = new() }
                }
            };

            if (systemInstruction != null)
            {
                config.SystemInstruction = new Content
                {
                    Parts = new List<Part> { new Part { Text = systemInstruction } }
                };
            }

            if (responseSchema != null)
            {
                config.ResponseMimeType = "application/json";
                config.ResponseSchema = responseSchema;
            }

            var response = await _client.Models.GenerateContentAsync(
                model: _model,
                contents: prompt,
                config: config
            );

            return response;
        }
        catch (Google.GenAI.ServerError e) when (e.Message.Contains("overloaded"))
        {
            if (retryCount >= 3) throw;
            ChangeModel(_fallbackModel);
            return await GenerateWithGroundedSearchAsync(prompt, systemInstruction, responseSchema, retryCount: retryCount + 1);
        }
    }

    /// <summary>
    /// Generates content with Google search grounding, restricting search to specified URLs/domains
    /// </summary>
    /// <param name="query">The user query</param>
    /// <param name="urls">List of URLs to restrict search to (extracts domains and builds site: restrictions)</param>
    /// <param name="systemInstruction">Optional system instruction</param>
    /// <param name="responseSchema">Optional JSON schema for structured output</param>
    /// <param name="retryCount">Current retry count</param>
    /// <returns>Response with grounding metadata</returns>
    public async Task<GenerateContentResponse> GenerateWithGroundedSearchAsync(
        string query,
        List<string> urls,
        string? systemInstruction = null,
        Schema? responseSchema = null,
        int retryCount = 0)
    {
        // Build site restrictions from URLs
        var siteRestrictions = BuildSiteRestrictions(urls);
        
        // Append site restrictions to query
        var fullQuery = string.IsNullOrEmpty(siteRestrictions) 
            ? query 
            : $"{query} {siteRestrictions}";

        return await GenerateWithGroundedSearchAsync(fullQuery, systemInstruction, responseSchema, retryCount);
    }

    /// <summary>
    /// Builds site restriction query from URLs
    /// Example: (site:example.com OR site:another.com)
    /// </summary>
    private string BuildSiteRestrictions(List<string>? urls)
    {
        if (urls == null || !urls.Any())
            return "";

        var domains = urls
            .Select(url => new Uri(url).Host)
            .Distinct()
            .Select(domain => $"site:{domain}");

        return $"({string.Join(" OR ", domains)})";
    }
}