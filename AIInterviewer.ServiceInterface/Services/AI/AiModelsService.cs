using ServiceStack;
using ServiceStack.OrmLite;
using AIInterviewer.ServiceModel.Types.AI;
using AIInterviewer.ServiceModel.Tables.Configuration;
using AIInterviewer.ServiceInterface.Interfaces;
using Microsoft.Extensions.Logging;

namespace AIInterviewer.ServiceInterface.Services.AI;

public class AiModelsService(IAiProviderFactory aiProviderFactory, SiteConfigHolder siteConfigHolder, ILogger<AiModelsService> logger) : Service
{
    public async Task<GetAiModelsResponse> Get(GetAiModels request)
    {
        logger.LogInformation("Fetching available {ProviderType} models.", request.ProviderType ?? "active");
        
        AiServiceConfig? config = null;

        if (!string.IsNullOrEmpty(request.ApiKey) && !string.IsNullOrEmpty(request.ProviderType))
        {
            config = new AiServiceConfig 
            { 
                ProviderType = request.ProviderType, 
                ApiKey = request.ApiKey,
                ModelId = request.ProviderType == "Gemini" ? "gemini-1.5-flash" : "gpt-4o"
            };
        }
        else
        {
            var siteConfig = siteConfigHolder.SiteConfig;
            if (siteConfig is { ActiveAiConfigId: > 0 })
            {
                config = await Db.SingleByIdAsync<AiServiceConfig>(siteConfig.ActiveAiConfigId);
                if (!string.IsNullOrEmpty(request.ProviderType) && config?.ProviderType != request.ProviderType) 
                {
                    config = null; 
                }
            }
        }


        if (config == null)
        {
            throw new HttpError(400, "ConfigurationError", "No valid AI configuration found.");
        }

        var provider = aiProviderFactory.GetProvider(config);
        var models = await provider.ListModelsAsync();

        return new GetAiModelsResponse
        {
            Models = models.ToList()
        };
    }
}
