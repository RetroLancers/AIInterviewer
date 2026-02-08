using ServiceStack;
using ServiceStack.FluentValidation;
using AIInterviewer.ServiceModel.Types.Interview;

namespace AIInterviewer.ServiceInterface.Validators.Interview;

public class UpdateInterviewerValidator : AbstractValidator<UpdateInterviewer>
{
    public UpdateInterviewerValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.SystemPrompt).NotEmpty().MaximumLength(8000);
    }
}
