using ServiceStack;
using AIInterviewer.ServiceModel.Types.AI;

namespace AIInterviewer.ServiceInterface.Services.AI;

public class InterviewService : Service
{
    public object Post(GenerateInterviewPrompt request)
    {
        return new GenerateInterviewPromptResponse
        {
            Prompt = $"Stub prompt for: {request.Context}\n\nYou are an interviewer..."
        };
    }
}
