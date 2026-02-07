using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace AIInterviewer.Migrations;

public class Migration1004_AddActiveAiConfigToSiteConfig : MigrationBase
{
    [Alias("siteconfig")]
    public class SiteConfig
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        public int? ActiveAiConfigId { get; set; }

        [StringLength(2096)]
        public string? GeminiApiKey { get; set; }

        [StringLength(255)]
        public string? InterviewModel { get; set; }

        [StringLength(255)]
        public string? GlobalFallbackModel { get; set; }

        [StringLength(255)]
        public string? KokoroVoice { get; set; }

        [StringLength(64)]
        public string? TranscriptionProvider { get; set; }
    }

    public override void Up()
    {
        Db.AddColumn<SiteConfig>(x => x.ActiveAiConfigId);
    }

    public override void Down()
    {
        Db.DropColumn<SiteConfig>(x => x.ActiveAiConfigId);
    }
}
