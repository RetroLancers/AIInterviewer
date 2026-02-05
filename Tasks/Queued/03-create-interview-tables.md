# Task: Create Interview Tables

## Objective
Define the database schema for the interview process using ServiceStack OrmLite.

## Requirements
Create the following tables in `AIInterviewer.ServiceModel/Tables/`:

### `Interview.cs`
- `int Id` (AutoIncrement, PrimaryKey)
- `string Prompt` (Required, StringLength(max or large))
- `DateTime CreatedDate` (Required)
- `string? UserId` (Optional, for future auth)

### `InterviewChatHistory.cs`
- `int Id` (AutoIncrement, PrimaryKey)
- `[ForeignKey(typeof(Interview), OnDelete = "Cascade")] int InterviewId`
- `[References(typeof(Interview))] Interview Interview`
- `DateTime EntryDate` (Required)
- `string Role` (Interviewer/User)
- `string Content` (Required)

### `InterviewResult.cs`
- `int Id` (AutoIncrement, PrimaryKey)
- `[ForeignKey(typeof(Interview), OnDelete = "Cascade")] int InterviewId`
- `[References(typeof(Interview))] Interview Interview`
- `string ReportText` (Required)
- `int Score` (0-100)
- `DateTime CreatedDate` (Required)

## Checklist
- [ ] Create table classes in `AIInterviewer.ServiceModel/Tables/`
- [ ] Add Migrations for the new tables
- [ ] Run migrations to verify schema
- [ ] Update clood files for the Database domain
