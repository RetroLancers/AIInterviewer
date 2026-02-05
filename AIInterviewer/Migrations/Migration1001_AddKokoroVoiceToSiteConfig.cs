using ServiceStack;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace AIInterviewer.Migrations;

public class Migration1001_AddKokoroVoiceToSiteConfig : MigrationBase
{
    [Alias("siteconfig")]
    public class SiteConfig
    {
        [StringLength(255)]
        public string? KokoroVoice { get; set; }
    }

    public override void Up()
    {
        Db.AddColumn<SiteConfig>(x => x.KokoroVoice);
    }

    public override void Down()
    {
        Db.DropColumn<SiteConfig>(x => x.KokoroVoice);
    }
}