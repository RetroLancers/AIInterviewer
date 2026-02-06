using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;
using System;

namespace AIInterviewer.Migrations;

public class Migration1001_CreateInterviewTables : MigrationBase
{
    [Alias("interview")]
    public class Interview
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [Required]
        [StringLength(8000)]
        public string Prompt { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [StringLength(255)]
        public string? UserId { get; set; }
    }


    [Alias("interview_chat_history")]
    public class InterviewChatHistory
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [ForeignKey(typeof(Interview), OnDelete = "Cascade")]
        public int InterviewId { get; set; }

        [References(typeof(Interview))] 
        public Interview Interview { get; set; }

        [Required]
        public DateTime EntryDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Role { get; set; }

        [Required]
        public string Content { get; set; }
    }

  
    [Alias("interview_result")]
    public class InterviewResult
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [ForeignKey(typeof(Interview), OnDelete = "Cascade")]
        public int InterviewId { get; set; }

        [References(typeof(Interview))]
        public Interview Interview { get; set; }

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