using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace AIInterviewer.Migrations;

public class Migration1004_UpdateSiteConfigForAi : MigrationBase
{
    [Alias("ai_service_config")]
    public class AiServiceConfig
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string ProviderType { get; set; } 

        [Required]
        [StringLength(2096)]
        public string ApiKey { get; set; }

        [Required]
        [StringLength(255)]
        public string ModelId { get; set; }

        [StringLength(2096)]
        public string? BaseUrl { get; set; }
    }

    [Alias("siteconfig")]
    public class SiteConfig
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [Required]
        [References(typeof(AiServiceConfig))]
        public int ActiveAiConfigId { get; set; }

        [StringLength(255)]
        public string? GlobalFallbackModel { get; set; }

        [StringLength(255)]
        public string? KokoroVoice { get; set; }

        [Required]
        [StringLength(64)]
        public string TranscriptionProvider { get; set; } = "Gemini";
    }

    public override void Up()
    {
        // Try to capture old data if table exists
        string oldApiKey = null;
        string oldModel = null;
        string oldFallback = null;
        string oldVoice = null;
        string oldProvider = "Gemini";

        try {
            var oldRow = Db.Single<dynamic>("SELECT * FROM siteconfig LIMIT 1");
            if (oldRow != null) {
                // Use dictionary access for dynamic if possible, or just cast
                var dict = (System.Collections.Generic.IDictionary<string, object>)oldRow;
                if (dict.ContainsKey("GeminiApiKey")) oldApiKey = dict["GeminiApiKey"]?.ToString();
                if (dict.ContainsKey("InterviewModel")) oldModel = dict["InterviewModel"]?.ToString();
                if (dict.ContainsKey("GlobalFallbackModel")) oldFallback = dict["GlobalFallbackModel"]?.ToString();
                if (dict.ContainsKey("KokoroVoice")) oldVoice = dict["KokoroVoice"]?.ToString();
                if (dict.ContainsKey("TranscriptionProvider")) oldProvider = dict["TranscriptionProvider"]?.ToString() ?? "Gemini";
            }
        } catch {
            // Table might not exist or columns missing in some states, ignore
        }

        Db.DropTable<SiteConfig>();
        Db.CreateTable<SiteConfig>();

        if (!string.IsNullOrEmpty(oldApiKey)) {
            var configId = (int)Db.Insert(new AiServiceConfig {
                Name = "Migrated Gemini Config",
                ProviderType = "Gemini",
                ApiKey = oldApiKey,
                ModelId = oldModel ?? "gemini-1.5-flash",
            }, selectIdentity: true);

            Db.Insert(new SiteConfig {
                ActiveAiConfigId = configId,
                GlobalFallbackModel = oldFallback,
                KokoroVoice = oldVoice,
                TranscriptionProvider = oldProvider
            });
        }
    }

    public override void Down()
    {
        Db.DropTable<SiteConfig>();
        // Re-creating with old schema would be complex here, usually Down is just Drop for dev migrations
    }
}
