using ServiceStack;
using AIInterviewer.ServiceModel.Tables.Configuration;

namespace AIInterviewer.ServiceInterface;

public static class SiteConfigHolderExtensions
{
    public static GeminiClient GetGeminiClient(this SiteConfigHolder siteConfigHolder)
    {
        var config = siteConfigHolder.SiteConfig;
        if (config == null || string.IsNullOrEmpty(config.GeminiApiKey))
            throw new HttpError(400, "ConfigurationError", "Site configuration is missing or API Key is not set.");

        return new GeminiClient(
            config.GeminiApiKey, 
            config.InterviewModel ?? "gemini-2.5-flash", 
            config.GlobalFallbackModel ?? "gemini-2.5-flash");
    }
}
