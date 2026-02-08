# Create Interviewer Table and Migration

## Objective
Create a new database table to store saved interviewer configurations. An interviewer consists of a name, system prompt, and optional AI config override.

## Context
Users should be able to save interviewer configurations for reuse. This table will store the interviewer definitions that can be selected when creating new interviews.

## Files to Create/Modify

### Backend
- `AIInterviewer.ServiceModel/Tables/Interview/Interviewer.cs` (New POCO)
- `AIInterviewer/Migrations/Migration****_CreateInterviewerTable.cs` (New migration)

## Clood Groups
This task affects files tracked in the following clood groups:
- **`clood-groups/Interview.json`** - Interview-related tables and migrations

## Implementation Steps

1. **Create Worktree**:
    - Create a new worktree for this task:
        ```bash
        git worktree add ../AIInterviewer-interviewer-table -b feature/interviewer-table
        ```
    - Navigate to the worktree and create the `App_Data` folder, then run `npm run migrate`

2. **Create Interviewer POCO**:
    - Create file: `AIInterviewer.ServiceModel/Tables/Interview/Interviewer.cs`
    - Define the table structure:
        ```csharp
        using ServiceStack.DataAnnotations;

        namespace AIInterviewer.ServiceModel.Tables.Interview;

        [Alias("interviewer")]
        public class Interviewer
        {
            [AutoIncrement]
            [PrimaryKey]
            public int Id { get; set; }

            [Required]
            [StringLength(255)]
            public string Name { get; set; }

            [Required]
            [StringLength(8000)]
            public string SystemPrompt { get; set; }

            /// <summary>
            /// Optional AI config override. If null, uses site default.
            /// </summary>
            public int? AiConfigId { get; set; }

            /// <summary>
            /// Optional user ID if we want to support per-user interviewers in the future
            /// </summary>
            [StringLength(255)]
            public string? UserId { get; set; }

            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }
        ```

3. **Create Migration**:
    - Create file: `AIInterviewer/Migrations/Migration****_CreateInterviewerTable.cs`
    - Use appropriate migration number (check existing migrations)
    - Define the table structure inside the migration class:
        ```csharp
        using ServiceStack.OrmLite;
        using ServiceStack.DataAnnotations;

        public class Migration****_CreateInterviewerTable : MigrationBase
        {
            [Alias("interviewer")]
            public class Interviewer
            {
                [AutoIncrement]
                [PrimaryKey]
                public int Id { get; set; }

                [Required]
                [StringLength(255)]
                public string Name { get; set; }

                [Required]
                [StringLength(8000)]
                public string SystemPrompt { get; set; }

                public int? AiConfigId { get; set; }

                [StringLength(255)]
                public string? UserId { get; set; }

                public DateTime CreatedAt { get; set; }
                public DateTime UpdatedAt { get; set; }
            }

            public override void Up()
            {
                Db.CreateTable<Interviewer>();
            }

            public override void Down()
            {
                Db.DropTable<Interviewer>();
            }
        }
        ```

4. **Run Migration**:
    - From the worktree directory, run:
        ```bash
        npm run migrate
        ```
    - Verify the table is created successfully

5. **Update Clood Files**:
    - Update `clood-groups/Interview.json` to include:
        - `AIInterviewer.ServiceModel/Tables/Interview/Interviewer.cs`
        - `AIInterviewer/Migrations/Migration****_CreateInterviewerTable.cs`

6. **Finalize**:
    - Commit changes with descriptive message
    - Return to main repository and merge:
        ```bash
        cd c:\DevCurrent\AIInterviewer\AIInterviewer
        git merge feature/interviewer-table
        ```
    - Remove worktree:
        ```bash
        git worktree remove ../AIInterviewer-interviewer-table
        git branch -d feature/interviewer-table
        ```
    - Move this task to `Tasks/completed/`

## Notes
- The `AiConfigId` is nullable to allow using the site default
- `UserId` is included for future multi-user support but is optional for now
- `CreatedAt` and `UpdatedAt` timestamps help track when interviewers were created/modified
- The system prompt has a generous length (8000) to accommodate detailed prompts
