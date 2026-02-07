using AIInterviewer.ServiceInterface.Interfaces;
using AIInterviewer.ServiceModel.Tables.Configuration;
using ServiceStack.OrmLite;
using System.Data;

namespace AIInterviewer.ServiceInterface.Extensions;

public static class AiProviderExtensions
{
    public static async Task<IAiProvider> GetActiveProviderAsync(
        this IAiProviderFactory factory,
        SiteConfigHolder configHolder,
        IDbConnection db)
    {
        var activeConfigId = configHolder.SiteConfig?.ActiveAiConfigId;
        
        AiServiceConfig? aiConfig = null;
        if (activeConfigId != null)
        {
            aiConfig = await db.SingleByIdAsync<AiServiceConfig>(activeConfigId.Value);
        }

        if (aiConfig == null)
        {
            // Fallback: Try to find any Gemini config if nothing is explicitly set
            aiConfig = await db.SingleAsync<AiServiceConfig>(x => x.ProviderType == "Gemini");
        }

        if (aiConfig == null)
        {
            // Last resort: find any config
            aiConfig = await db.SingleAsync<AiServiceConfig>(x => true);
        }

        if (aiConfig == null)
        {
            throw new InvalidOperationException("No AI configurations found in the database.");
        }

        return factory.GetProvider(aiConfig);
    }
}
