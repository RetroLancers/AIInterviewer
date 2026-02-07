using ServiceStack;
using AIInterviewer.ServiceModel.Types.AI;
using System.Linq;
using System.Threading.Tasks;
using AIInterviewer.ServiceModel.Tables.Configuration;
using System.Collections.Generic;
using AIInterviewer.ServiceInterface;
using Microsoft.Extensions.Logging;

namespace AIInterviewer.ServiceInterface.Services.AI;

public class GeminiModelsService(SiteConfigHolder siteConfigHolder, ILogger<GeminiModelsService> logger) : Service
{
    public async Task<GetGeminiModelsResponse> Get(GetGeminiModels request)
    {
        logger.LogInformation("Fetching available Gemini models.");
        GeminiClient client;

        if (!string.IsNullOrEmpty(request.ApiKey))
        {
            client = new GeminiClient(request.ApiKey, "gemini-2.5-flash");
        }
        else
        {
            client = siteConfigHolder.GetGeminiClient();
        }

        var modelsPager = await client.GetModels(null);
        var models = new List<string>();

        await foreach (var model in modelsPager)
        {
            if (model.Name != null && model.Name.StartsWith("models/gemini"))
            {
                models.Add(model.Name.Replace("models/", ""));
            }
        }

        var response = new GetGeminiModelsResponse
        {
            Models = models
        };
        
        logger.LogInformation("Fetched {Count} Gemini models.", models.Count);
        return response;
    }
}