using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace AIInterviewer.Migrations;

public class Migration1007_UpdateAiConfigAddVoice : MigrationBase
{
    [Alias("ai_service_config")]
    public class AiServiceConfig
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [StringLength(100)]
        public string? Voice { get; set; }
    }

    public override void Up()
    {
        Db.AddColumn<AiServiceConfig>(x => x.Voice);
    }

    public override void Down()
    {
        Db.DropColumn<AiServiceConfig>(nameof(AiServiceConfig.Voice));
    }
}
