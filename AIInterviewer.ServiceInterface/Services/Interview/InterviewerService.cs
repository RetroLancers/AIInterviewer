using System;
using ServiceStack;
using ServiceStack.OrmLite;
using AIInterviewer.ServiceModel.Tables.Interview;
using AIInterviewer.ServiceModel.Types.Interview;
using AIInterviewer.ServiceModel.Types.Interview.ExtensionMethods;

namespace AIInterviewer.ServiceInterface.Services.Interview;

public class InterviewerService : Service
{
    public ListInterviewersResponse Any(ListInterviewers request)
    {
        return new ListInterviewersResponse
        {
            Interviewers = Db.Select<Interviewer>().ToDto()
        };
    }

    public InterviewerResponse Any(GetInterviewer request)
    {
        var interviewer = Db.SingleById<Interviewer>(request.Id);
        if (interviewer == null) throw HttpError.NotFound("Interviewer not found");
        return interviewer.ToDto();
    }

    public InterviewerResponse Any(CreateInterviewer request)
    {
        using var trans = Db.OpenTransaction();

        var interviewer = request.ToTable();
        interviewer.CreatedAt = DateTime.UtcNow;
        interviewer.UpdatedAt = DateTime.UtcNow;

        long id = Db.Insert(interviewer, selectIdentity: true);
        interviewer.Id = (int)id;

        trans.Commit();

        return interviewer.ToDto();
    }

    public InterviewerResponse Any(UpdateInterviewer request)
    {
        using var trans = Db.OpenTransaction();

        var interviewer = Db.SingleById<Interviewer>(request.Id);
        if (interviewer == null) throw HttpError.NotFound("Interviewer not found");

        interviewer.UpdateTable(request);
        interviewer.UpdatedAt = DateTime.UtcNow;

        Db.Update(interviewer);

        trans.Commit();

        return interviewer.ToDto();
    }

    public void Any(DeleteInterviewer request)
    {
        using var trans = Db.OpenTransaction();
        Db.DeleteById<Interviewer>(request.Id);
        trans.Commit();
    }
}
