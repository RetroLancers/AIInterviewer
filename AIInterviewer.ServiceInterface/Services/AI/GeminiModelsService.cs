using ServiceStack;
using ServiceStack.OrmLite;
using AIInterviewer.ServiceModel.Types.AI;
using AIInterviewer.ServiceModel.Tables.Configuration;
using AIInterviewer.ServiceInterface.Interfaces;
using Microsoft.Extensions.Logging;

namespace AIInterviewer.ServiceInterface.Services.AI;

public class GeminiModelsService(IAiProviderFactory aiProviderFactory, SiteConfigHolder siteConfigHolder, ILogger<GeminiModelsService> logger) : Service
{
    public async Task<GetGeminiModelsResponse> Get(GetGeminiModels request)
    {
        logger.LogInformation("Fetching available Gemini models.");
        
        AiServiceConfig? config = null;

        if (!string.IsNullOrEmpty(request.ApiKey))
        {
            config = new AiServiceConfig 
            { 
                ProviderType = "Gemini", 
                ApiKey = request.ApiKey,
                ModelId = "gemini-2.0-flash-exp"
            };
        }
        else
        {
            config = await Db.SingleAsync<AiServiceConfig>(x => x.ProviderType == "Gemini");
            
            if (config == null)
            {
                var siteConfig = siteConfigHolder.SiteConfig;
                if (siteConfig != null && !string.IsNullOrEmpty(siteConfig.GeminiApiKey))
                {
                    config = new AiServiceConfig
                    {
                        ProviderType = "Gemini",
                        ApiKey = siteConfig.GeminiApiKey,
                        ModelId = siteConfig.InterviewModel ?? "gemini-2.0-flash-exp"
                    };
                }
            }
        }

        if (config == null)
        {
            throw new HttpError(400, "ConfigurationError", "No Gemini configuration found.");
        }

        var provider = aiProviderFactory.GetProvider(config);
        var models = await provider.ListModelsAsync();

        return new GetGeminiModelsResponse
        {
            Models = models.ToList()
        };
    }
}