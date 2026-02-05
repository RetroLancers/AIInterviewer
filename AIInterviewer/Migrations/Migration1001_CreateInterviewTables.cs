using ServiceStack;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace AIInterviewer.Migrations;

public class Migration1001_CreateInterviewTables : MigrationBase
{
    [Alias("interviews")]
    public class Interview
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [Required]
        public string Prompt { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public string? UserId { get; set; }
    }

    [Alias("interviewchathistories")]
    public class InterviewChatHistory
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [ForeignKey(typeof(Interview), OnDelete = "Cascade")]
        public int InterviewId { get; set; }

        [Required]
        public DateTime EntryDate { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string Content { get; set; }
    }

    [Alias("interviewresults")]
    public class InterviewResult
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [ForeignKey(typeof(Interview), OnDelete = "Cascade")]
        public int InterviewId { get; set; }

        [Required]
        public string ReportText { get; set; }

        public int Score { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }
    }

    public override void Up()
    {
        Db.CreateTable<Interview>();
        Db.CreateTable<InterviewChatHistory>();
        Db.CreateTable<InterviewResult>();
    }

    public override void Down()
    {
        Db.DropTable<InterviewResult>();
        Db.DropTable<InterviewChatHistory>();
        Db.DropTable<Interview>();
    }
}
