using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace AIInterviewer.Migrations;

public class Migration1010_AddDefaultVoiceToSiteConfig : MigrationBase
{
    [Alias("siteconfig")]
    public class SiteConfig
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        public string? DefaultVoice { get; set; }
    }

    public override void Up()
    {
        Db.AddColumn<SiteConfig>(x => x.DefaultVoice);
    }

    public override void Down()
    {
        Db.DropColumn<SiteConfig>(nameof(SiteConfig.DefaultVoice));
    }
}
