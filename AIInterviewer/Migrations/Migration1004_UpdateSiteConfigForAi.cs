using ServiceStack;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace AIInterviewer.Migrations;

public class Migration1004_UpdateSiteConfigForAi : MigrationBase
{
    [Alias("siteconfig")]
    public class SiteConfig
    {
        [StringLength(2096)]
        public string GeminiApiKey { get; set; }

        [StringLength(255)]
        public string InterviewModel { get; set; }

        [ForeignKey(typeof(AiServiceConfig))]
        public int? ActiveAiConfigId { get; set; }
    }

    [Alias("ai_service_config")]
    public class AiServiceConfig
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }
    }

    public override void Up()
    {
        Db.AddColumn<SiteConfig>(x => x.ActiveAiConfigId);

        // Optional: Attempt to link existing config if possible
        // This logic is best-effort
        Db.ExecuteSql(@"
            UPDATE siteconfig 
            SET ActiveAiConfigId = (SELECT Id FROM ai_service_config WHERE ModelId = siteconfig.InterviewModel LIMIT 1)
            WHERE ActiveAiConfigId IS NULL AND InterviewModel IS NOT NULL AND EXISTS(SELECT 1 FROM ai_service_config WHERE ModelId = siteconfig.InterviewModel)");

        Db.DropColumn<SiteConfig>(x => x.GeminiApiKey);
        Db.DropColumn<SiteConfig>(x => x.InterviewModel);
    }

    public override void Down()
    {
        Db.AddColumn<SiteConfig>(x => x.GeminiApiKey);
        Db.AddColumn<SiteConfig>(x => x.InterviewModel);
        Db.DropColumn<SiteConfig>(x => x.ActiveAiConfigId);
    }
}
