using ServiceStack;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace AIInterviewer.Migrations;

public class Migration1000_CreateSiteConfig : MigrationBase
{
    [Alias("siteconfig")]
    public class SiteConfig
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [Required]
        [StringLength(2096)]
        public string GeminiApiKey { get; set; }

        [Required]
        [StringLength(255)]
        public string InterviewModel { get; set; }

        [StringLength(255)]
        public string? GlobalFallbackModel { get; set; }
    }

    public override void Up()
    {
        Db.CreateTable<SiteConfig>();
    }

    public override void Down()
    {
        Db.DropTable<SiteConfig>();
    }
}
