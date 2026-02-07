using AIInterviewer.ServiceModel.Types.Ai;

namespace AIInterviewer.ServiceInterface.Interfaces;

public interface IAiProvider
{
    string ProviderName { get; }
    
    Task<string?> GenerateTextAsync(string prompt, string? systemPrompt = null, double? temperature = null, int? maxOutputTokens = null);
    
    Task<string?> GenerateTextAsync(IEnumerable<AiMessage> messages, string? systemPrompt = null, double? temperature = null, int? maxOutputTokens = null);
    
    Task<string?> GenerateTextFromAudioAsync(string prompt, byte[] audioData, string mimeType, string? systemPrompt = null);
    
    Task<T?> GenerateJsonAsync<T>(string prompt, string? systemPrompt = null) where T : class;
}
