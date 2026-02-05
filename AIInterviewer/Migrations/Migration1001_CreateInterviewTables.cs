<<<<<<< HEAD
using ServiceStack;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace AIInterviewer.Migrations;

public class Migration1001_CreateInterviewTables : MigrationBase
{
    [Alias("interviews")]
=======
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;
using System;

public class Migration1001_CreateInterviewTables : MigrationBase
{
    [Alias("interview")]
>>>>>>> feature/interview-completion-and-report
    public class Interview
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [Required]
<<<<<<< HEAD
=======
        [StringLength(8000)]
>>>>>>> feature/interview-completion-and-report
        public string Prompt { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

<<<<<<< HEAD
        public string? UserId { get; set; }
    }

    [Alias("interviewchathistories")]
=======
        [StringLength(255)]
        public string? UserId { get; set; }
    }

    [Alias("interview_chat_history")]
>>>>>>> feature/interview-completion-and-report
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
<<<<<<< HEAD
=======
        [StringLength(50)]
>>>>>>> feature/interview-completion-and-report
        public string Role { get; set; }

        [Required]
        public string Content { get; set; }
    }

<<<<<<< HEAD
    [Alias("interviewresults")]
=======
    [Alias("interview_result")]
>>>>>>> feature/interview-completion-and-report
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
