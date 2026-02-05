using ServiceStack;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace AIInterviewer.Migrations;

public class Migration1002_AddTranscriptionProviderToSiteConfig : MigrationBase
{
    [Alias("siteconfig")]
    public class SiteConfig
    {
        [StringLength(64)]
        public string? TranscriptionProvider { get; set; }
    }

    public override void Up()
    {
        Db.AddColumn<SiteConfig>(x => x.TranscriptionProvider);
        Db.ExecuteSql("UPDATE siteconfig SET TranscriptionProvider = 'Gemini' WHERE TranscriptionProvider IS NULL");
    }

    public override void Down()
    {
        Db.DropColumn<SiteConfig>(x => x.TranscriptionProvider);
    }
}
