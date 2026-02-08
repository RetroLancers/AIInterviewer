using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace AIInterviewer.Migrations;

public class Migration1008_RemoveVoiceFromSiteConfig : MigrationBase
{
    [Alias("siteconfig")]
    public class SiteConfig
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [StringLength(255)]
        public string? KokoroVoice { get; set; }
    }

    public override void Up()
    {
        Db.DropColumn<SiteConfig>(nameof(SiteConfig.KokoroVoice));
    }

    public override void Down()
    {
        Db.AddColumn<SiteConfig>(x => x.KokoroVoice);
    }
}
