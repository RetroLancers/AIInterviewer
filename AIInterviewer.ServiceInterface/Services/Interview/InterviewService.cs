using ServiceStack;
using ServiceStack.OrmLite;
using AIInterviewer.ServiceModel.Types.Interview;
using AIInterviewer.ServiceModel.Tables.Interview;
using AIInterviewer.ServiceModel.Types.Interview.ExtensionMethods;
using AIInterviewer.ServiceModel.Tables.Configuration;
using AIInterviewer.ServiceInterface.Interfaces;
using AIInterviewer.ServiceModel.Types.Ai;
using Microsoft.Extensions.Logging;
using AIInterviewer.ServiceInterface.Extensions;

namespace AIInterviewer.ServiceInterface.Services.Interview;

public class InterviewService(IAiProviderFactory aiProviderFactory, SiteConfigHolder siteConfigHolder, ILogger<InterviewService> logger) : Service
{
    private async Task<IAiProvider> GetAiProviderAsync()
    {
        var activeConfigId = siteConfigHolder.SiteConfig?.ActiveAiConfigId ?? 0;
        AiServiceConfig? config = null;

        if (activeConfigId > 0)
        {
             config = await Db.SingleByIdAsync<AiServiceConfig>(activeConfigId);
        }
        
        // Fallback: Get first Gemini provider if active one not found or not set
        if (config == null)
        {
             config = await Db.SingleAsync<AiServiceConfig>(x => x.ProviderType == "Gemini");
        }

        if (config == null)
        {
            throw new Exception("No AI Service Configuration found. Please configure AiServiceConfig table.");
        }

        return aiProviderFactory.GetProvider(config);
    }
    private const string BaseInterviewRules = """
                                              Base Interview Rules (Mandatory):
                                              - Start the interview with the exact opening line: "We’ll proceed with the interview. Answer concisely but thoroughly."
                                              - Ask exactly one question at a time and wait for the candidate's response before continuing.
                                              - Ask 8–12 questions total, increasing difficulty as the interview progresses.
                                              - Maintain a firm, professional tone.
                                              - Do not provide feedback, evaluation, or hints during the interview.
                                              - After the final question and the candidate's response, end with: "The interview is complete. Evaluation follows. Please press the end interview button".
                                              """;

    private static string ApplyBaseInterviewRules(string? systemPrompt)
    {
        if (string.IsNullOrWhiteSpace(systemPrompt))
        {
            return BaseInterviewRules;
        }

        return systemPrompt.Contains("Base Interview Rules (Mandatory):", StringComparison.Ordinal)
            ? systemPrompt
            : $"{systemPrompt.Trim()}\n\n{BaseInterviewRules}";
    }

    public async Task<GenerateInterviewPromptResponse> Post(GenerateInterviewPrompt request)
    {
        logger.LogInformation("Generating interview prompt for role: {TargetRole}", request.TargetRole);
        var prompt =
            $"Create a system prompt for an AI interviewer interviewing a candidate for the role of '{request.TargetRole}'.";
        if (!string.IsNullOrEmpty(request.Context))
        {
            prompt += $" Context: {request.Context}.";
        }

        prompt +=
            " The output should be the raw system prompt text that defines the persona and rules for the AI. Do not include markdown code blocks.";

        var provider = await GetAiProviderAsync();
        var result = await provider.GenerateTextAsync(prompt);
        var generatedPrompt = result?.Trim() ?? "Failed to generate prompt.";
        return new GenerateInterviewPromptResponse { SystemPrompt = ApplyBaseInterviewRules(generatedPrompt) };
    }

    public async Task<CreateInterviewResponse> Post(CreateInterview request)
    {
        var interview = new AIInterviewer.ServiceModel.Tables.Interview.Interview
        {
            Prompt = ApplyBaseInterviewRules(request.SystemPrompt),
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
        var result = await Db.SingleAsync<InterviewResult>(x => x.InterviewId == request.Id);

        return new GetInterviewResponse
        {
            Interview = interview.ToDto(),
            History = history.OrderBy(x => x.EntryDate).ToDto(),
            Result = result.ToDto()
        };
    }

    public async Task<GetInterviewHistoryResponse> Get(GetInterviewHistory request)
    {
        var query = Db.From<AIInterviewer.ServiceModel.Tables.Interview.Interview>()
            .OrderByDescending(x => x.CreatedDate);

        if (request.Offset.HasValue)
        {
            query.Skip(request.Offset.Value);
        }

        if (request.Limit.HasValue)
        {
            query.Take(request.Limit.Value);
        }

        var interviews = await Db.SelectAsync(query);

        return new GetInterviewHistoryResponse
        {
            Interviews = interviews.Select(interview => interview.ToDto()).ToList()
        };
    }

    public async Task<StartInterviewResponse> Post(StartInterview request)
    {
        var interview =
            await Db.SingleByIdAsync<AIInterviewer.ServiceModel.Tables.Interview.Interview>(request.InterviewId);
        if (interview == null) throw HttpError.NotFound("Interview not found");

        var history = await Db.SelectAsync<InterviewChatHistory>(x => x.InterviewId == request.InterviewId);
        if (history.Any())
        {
            return new StartInterviewResponse
            {
                History = history.OrderBy(x => x.EntryDate).ToDto()
            };
        }

        using var trans = Db.OpenTransaction();
        try
        {
            var provider = await GetAiProviderAsync();
            var aiResponse = await provider.GenerateTextAsync(
                "Begin the interview now.",
                systemPrompt: interview.Prompt
            );

            if (string.IsNullOrWhiteSpace(aiResponse))
            {
                logger.LogWarning("Failed to start interview: AI returned empty response for InterviewId: {InterviewId}", request.InterviewId);
                throw HttpError.BadRequest("Failed to start interview.");
            }

            logger.LogInformation("Interview started successfully for InterviewId: {InterviewId}", request.InterviewId);

            await Db.SaveAsync(new InterviewChatHistory
            {
                InterviewId = interview.Id,
                Role = "Interviewer",
                Content = aiResponse,
                EntryDate = DateTime.UtcNow
            });

            trans.Commit();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error starting interview for InterviewId: {InterviewId}", request.InterviewId);
            trans.Rollback();
            throw;
        }

        var updatedHistory = await Db.SelectAsync<InterviewChatHistory>(x => x.InterviewId == request.InterviewId);
        return new StartInterviewResponse
        {
            History = updatedHistory.OrderBy(x => x.EntryDate).ToDto()
        };
    }

    public async Task<AddChatMessageResponse> Post(AddChatMessage request)
    {
        var interview =
            await Db.SingleByIdAsync<AIInterviewer.ServiceModel.Tables.Interview.Interview>(request.InterviewId);
        if (interview == null) throw HttpError.NotFound("Interview not found");

        using var trans = Db.OpenTransaction();
        try
        {
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

            var orderedHistory = history.OrderBy(x => x.EntryDate).ToList();
            var messages = orderedHistory.Select(entry => new AiMessage
            {
                Role = entry.Role == "Interviewer" ? AiRole.Model : AiRole.User,
                Content = entry.Content
            }).ToList();

            var provider = await GetAiProviderAsync();
            var aiResponse = await provider.GenerateTextAsync(
                messages: messages,
                systemPrompt: interview.Prompt
            );

            if (!string.IsNullOrEmpty(aiResponse))
            {
                var aiChatHistory = new InterviewChatHistory
                {
                    InterviewId = interview.Id,
                    Role = "Interviewer",
                    Content = aiResponse,
                    EntryDate = DateTime.UtcNow
                };
                await Db.SaveAsync(aiChatHistory);
                orderedHistory.Add(aiChatHistory);
            }

            trans.Commit();
            return new AddChatMessageResponse
            {
                History = orderedHistory.ToDto()
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding chat message for InterviewId: {InterviewId}", request.InterviewId);
            trans.Rollback();
            throw;
        }
    }

    public async Task<FinishInterviewResponse> Post(FinishInterview request)
    {
        var interview = await Db.SingleByIdAsync<AIInterviewer.ServiceModel.Tables.Interview.Interview>(request.Id);
        if (interview == null) throw HttpError.NotFound("Interview not found");

        var history = await Db.SelectAsync<InterviewChatHistory>(x => x.InterviewId == request.Id);
        if (!history.Any()) throw HttpError.BadRequest("No conversation to evaluate");

        var conversationParams =
            string.Join("\n", history.OrderBy(x => x.EntryDate).Select(x => $"{x.Role}: {x.Content}"));

        var evaluationPrompt = $@"
Evaluate the following interview based on the candidate's responses.
Role: Interviewer (AI) vs Candidate (User).

Conversation:
{conversationParams}

Provide a JSON output with the following schema:
{{
  ""Score"": (integer 0-100),
  ""Feedback"": (string, comprehensive markdown report)
}}

The ""Feedback"" must be markdown and include a section titled ""Final Evaluation"" with the following subsections:
- ""Summary""
- ""Strengths""
- ""Areas for Improvement""
- ""Role Fit""
- ""Hiring Recommendation""
- ""Next Steps""
 
";

        using var trans = Db.OpenTransaction();
        InterviewResult result;
        try
        {
            var provider = await GetAiProviderAsync();
            var evaluation = await provider.GenerateJsonAsync<EvaluationResponse>(evaluationPrompt);

            if (evaluation == null) throw new Exception("Failed to generate evaluation");

            result = new InterviewResult
            {
                InterviewId = interview.Id,
                ReportText = string.IsNullOrWhiteSpace(evaluation.Feedback)
                    ? "No feedback provided."
                    : evaluation.Feedback.Trim(),
                Score = evaluation.Score,
                CreatedDate = DateTime.UtcNow
            };

            await Db.SaveAsync(result);

            trans.Commit();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error finishing interview for InterviewId: {Id}", request.Id);
            trans.Rollback();
            throw;
        }

        return new FinishInterviewResponse
        {
            Result = result.ToDto()
        };
    }
}
