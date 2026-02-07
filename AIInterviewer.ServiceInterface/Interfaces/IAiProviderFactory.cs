using AIInterviewer.ServiceInterface.Interfaces;

namespace AIInterviewer.ServiceInterface.Interfaces;

public interface IAiProviderFactory
{
    IAiProvider GetProvider(string providerName);
}
