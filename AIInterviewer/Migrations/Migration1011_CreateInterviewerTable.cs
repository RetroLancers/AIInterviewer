using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;
using System;

namespace AIInterviewer.Migrations;

public class Migration1011_CreateInterviewerTable : MigrationBase
{
    [Alias("ai_service_config")]
    public class AiServiceConfig
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }
    }

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

        [ForeignKey(typeof(AiServiceConfig), OnDelete = "Set Null")]
        public int? AiConfigId { get; set; }

        [References(typeof(AiServiceConfig))]
        public AiServiceConfig AiConfig { get; set; }

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
