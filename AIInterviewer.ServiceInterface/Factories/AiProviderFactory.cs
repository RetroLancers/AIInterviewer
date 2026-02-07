using AIInterviewer.ServiceInterface.Providers;
using AIInterviewer.ServiceModel.Tables.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AIInterviewer.ServiceInterface.Interfaces;

namespace AIInterviewer.ServiceInterface.Factories;

public class AiProviderFactory(IServiceProvider serviceProvider) : IAiProviderFactory
{
    public IAiProvider GetProvider(AiServiceConfig config)
    {
        return config.ProviderType switch
        {
            "Gemini" => ActivatorUtilities.CreateInstance<GeminiAiProvider>(serviceProvider, config),
            "OpenAI" => ActivatorUtilities.CreateInstance<OpenAiProvider>(serviceProvider, config),
            _ => throw new ArgumentException($"Unknown AI Provider Type: {config.ProviderType}", nameof(config))
        };
    }
}
