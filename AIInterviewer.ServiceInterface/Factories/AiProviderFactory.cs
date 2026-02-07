using AIInterviewer.ServiceInterface.Interfaces;
using AIInterviewer.ServiceInterface.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace AIInterviewer.ServiceInterface.Factories;

public class AiProviderFactory(IServiceProvider serviceProvider) : IAiProviderFactory
{
    public IAiProvider GetProvider(string providerName)
    {
        return providerName switch
        {
            "Gemini" => serviceProvider.GetRequiredService<GeminiAiProvider>(),
            // Future providers like OpenAI can be added here
            _ => throw new ArgumentException($"Unknown AI Provider: {providerName}", nameof(providerName))
        };
    }
}
