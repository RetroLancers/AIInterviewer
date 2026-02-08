using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;
using System;

namespace AIInterviewer.Migrations;

public class Migration1011_CreateInterviewerTable : MigrationBase
{
    [Alias("interviewer")]
    public class Interviewer
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(8000)]
        public string SystemPrompt { get; set; }

        public int? AiConfigId { get; set; }

        [StringLength(255)]
        public string? UserId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public override void Up()
    {
        Db.CreateTable<Interviewer>();
    }

    public override void Down()
    {
        Db.DropTable<Interviewer>();
    }
}
