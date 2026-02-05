using ServiceStack;
using ServiceStack.OrmLite;
using AIInterviewer.ServiceModel.Types.Interview;
using AIInterviewer.ServiceModel.Tables.Interview;
using AIInterviewer.ServiceModel.Types.Interview.ExtensionMethods;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace AIInterviewer.ServiceInterface.Services.Interview;

public class InterviewService : Service
{
    public GeminiClient Gemini { get; set; }

    public async Task<GenerateInterviewPromptResponse> Post(GenerateInterviewPrompt request)
    {
        var prompt = $"Create a system prompt for an AI interviewer interviewing a candidate for the role of '{request.TargetRole}'.";
        if (!string.IsNullOrEmpty(request.Context))
        {
            prompt += $" Context: {request.Context}.";
        }
        prompt += " The output should be the raw system prompt text that defines the persona and rules for the AI. Do not include markdown code blocks.";

        var result = await Gemini.GenerateTextAsync(prompt);
        return new GenerateInterviewPromptResponse { SystemPrompt = result?.Trim() ?? "Failed to generate prompt." };
    }

    public async Task<CreateInterviewResponse> Post(CreateInterview request)
    {
        var interview = new AIInterviewer.ServiceModel.Tables.Interview.Interview
        {
            Prompt = request.SystemPrompt,
            CreatedDate = DateTime.UtcNow,
            UserId = request.UserId
        };

        await Db.SaveAsync(interview);

        return new CreateInterviewResponse { Id = interview.Id };
    }

    public async Task<GetInterviewResponse> Get(GetInterview request)
    {
        var interview = await Db.SingleByIdAsync<AIInterviewer.ServiceModel.Tables.Interview.Interview>(request.Id);
        if (interview == null) throw HttpError.NotFound("Interview not found");

        var history = await Db.SelectAsync<InterviewChatHistory>(x => x.InterviewId == request.Id);

        return new GetInterviewResponse
        {
            Interview = interview.ToDto(),
            History = history.OrderBy(x => x.EntryDate).ToDto()
        };
    }

    public async Task<AddChatMessageResponse> Post(AddChatMessage request)
    {
        var interview = await Db.SingleByIdAsync<AIInterviewer.ServiceModel.Tables.Interview.Interview>(request.InterviewId);
        if (interview == null) throw HttpError.NotFound("Interview not found");

        // 1. Save User Message
        await Db.SaveAsync(new InterviewChatHistory
        {
            InterviewId = interview.Id,
            Role = "User",
            Content = request.Message,
            EntryDate = DateTime.UtcNow
        });

        // 2. Get Context for AI
        var history = await Db.SelectAsync<InterviewChatHistory>(x => x.InterviewId == request.InterviewId);
        
        // Construct string prompt from history
        var historyText = string.Join("\n", history.OrderBy(x => x.EntryDate).Select(h => $"{h.Role}: {h.Content}"));
        var fullPrompt = historyText + "\nModel:"; 
        
        var aiResponse = await Gemini.GenerateTextAsync(
            prompt: fullPrompt, 
            systemInstruction: interview.Prompt 
        );

        if (!string.IsNullOrEmpty(aiResponse))
        {
            await Db.SaveAsync(new InterviewChatHistory
            {
                InterviewId = interview.Id,
                Role = "Model",
                Content = aiResponse,
                EntryDate = DateTime.UtcNow
            });
        }

        // Return updated history
        var updatedHistory = await Db.SelectAsync<InterviewChatHistory>(x => x.InterviewId == request.InterviewId);
        return new AddChatMessageResponse
        {
            History = updatedHistory.OrderBy(x => x.EntryDate).ToDto()
        };
    }
}
