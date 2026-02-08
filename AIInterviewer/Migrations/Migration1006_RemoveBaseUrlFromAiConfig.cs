using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace AIInterviewer.Migrations;

public class Migration1006_RemoveBaseUrlFromAiConfig : MigrationBase
{
    [Alias("ai_service_config")]
    public class AiServiceConfig
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [StringLength(2096)]
        public string? BaseUrl { get; set; }
    }

    public override void Up()
    {
        Db.DropColumn<AiServiceConfig>(nameof(AiServiceConfig.BaseUrl));
    }

    public override void Down()
    {
        Db.AddColumn<AiServiceConfig>(x => x.BaseUrl);
    }
}
