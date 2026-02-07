---
description: Implement NLog logging across all services
---

# Implement NLog Logging

We need to add proper logging to our services using NLog. This will help with debugging and monitoring the application. The logging should be file-based.

## Relevant Files

*   **Project**: `AIInterviewer/AIInterviewer/AIInterviewer.csproj`
*   **Startup**: `AIInterviewer/AIInterviewer/Program.cs`
*   **Config**: `AIInterviewer/AIInterviewer/nlog.config` (to be created)
*   **Services**: All files in `AIInterviewer.ServiceInterface/Services/`

## Objectives

1.  **Add NLog Packages**:
    *   Add `NLog.Web.AspNetCore` to the `AIInterviewer` project.
2.  **Create NLog Configuration**:
    *   Create `nlog.config` in the root of the `AIInterviewer` project.
    *   Configure it to write to `Logs/app-${shortdate}.log`.
    *   Ensure the config is copied to the output directory.
3.  **Initialize NLog in Program.cs**:
    *   Configure the WebHost to use NLog.
4.  **Update Services**:
    *   Inject `ILogger<T>` into major services (e.g., `InterviewService`, `SiteConfigService`, `TtsService`).
    *   Add logging calls for important events (e.g., API calls, database transactions, errors).
5.  **Verification**:
    *   Ensure logs are being written to the file system.
    *   Verify that different log levels are correctly captured.

## Checklist

- [x] Add `NLog.Web.AspNetCore` NuGet package
- [x] Create and configure `nlog.config`
- [x] Update `Program.cs` to use NLog
- [ ] Inject `ILogger` into Services
- [ ] Add logging to `InterviewService`
- [ ] Add logging to `TtsService`
- [ ] Add logging to `SiteConfigService`
- [ ] Verify log file creation and content
- [x] Create `Infrastructure.json` clood-group
