using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace AIInterviewer.Migrations;

public class Migration1009_AddAiConfigIdToInterview : MigrationBase
{
    [Alias("interview")]
    public class Interview
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        public int? AiConfigId { get; set; }
    }

    public override void Up()
    {
        Db.AddColumn<Interview>(x => x.AiConfigId);
    }

    public override void Down()
    {
        Db.DropColumn<Interview>(nameof(Interview.AiConfigId));
    }
}
