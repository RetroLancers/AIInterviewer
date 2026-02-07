# Task Management Workflow

We follow a file-based task management system to keep track of our progress.

Important -> when creating a new branch you have to create the App_Data folder in the same folder as program.cs and then run npm run migrage

## Project Layout

The repository is structured such that the main branch and project files are located in a subfolder within the workspace root.

*   **Workspace Root**: `c:\DevCurrent\AIInterviewer` (Where you start)
*   **Main Repository**: `c:\DevCurrent\AIInterviewer\AIInterviewer` (Where the `.git` folder, `.sln`, and `Tasks/` folder reside)
*   **Worktrees**: Should be created in the **Workspace Root** (parallel to the `AIInterviewer` folder).

## Directory Structure (Inside Main Repository)

*   **`Tasks/Queued/`**: Contains markdown files for tasks that are planned but not yet started.
*   **`Tasks/in_progress/`**: Contains markdown files for tasks currently being worked on.
*   **`Tasks/completed/`**: Contains markdown files for tasks that have been finished.
*   **`Tasks/Notes/`**: General project notes, ideas, and documentation not tied to specific immediate tasks.

## Workflow

1.  **Define a Task**:
    *   Create a new markdown file in `Tasks/Queued/` with a descriptive name (e.g., `01-setup-basic-structure.md`).
    *   Move the file to `Tasks/in_progress/` when starting.
    *   **From the Main Repository folder** (`AIInterviewer/`), create a new **git worktree** for the feature branch. This will place the worktree folder in the **Workspace Root**:
        ```bash
        # Run this from c:\DevCurrent\AIInterviewer\AIInterviewer
        git worktree add ../AIInterviewer-<task-name> -b feature/<task-name>
        ```
2.  **Document**: Inside the task file, outline the objective, requirements, and checklist of items to complete.
3.  **Execute**:
    *   Perform the work inside the newly created worktree directory in the Workspace Root (`c:\DevCurrent\AIInterviewer\AIInterviewer-<task-name>`).
    *   As you work, check off items and add notes if the plan changes.
    *   Commit changes to the feature branch regularly from within the worktree.

## Development Standards

### Database & DTOs
*   **Database Migrations**: Always use `npm run migrate` from the `AIInterviewer/AIInterviewer` project directory to apply database changes. Do NOT run the full server for migrations.
*   **Running the Server**: Never start the server directly using `dotnet run` solely to update DTOs. 
*   **Port Selection**: When working in a task-specific worktree, use a port number derived from the task number to avoid conflicts with other worktrees.
    *   **Rule**: `50` + `<Task Number>`. (e.g., Task `01` uses port `5001`, Task `12` uses `5012`).
    *   **Fallback Port**: `51` + `<Task Number>`.
*   **Updating DTOs**: Use the `Update-Dtos.ps1` script at the root. It handles starting the server on the correct task port, updating the TypeScript definitions, and shutting the server down.
    *   **Command**: `.\Update-Dtos.ps1 -TaskNumber <Number>`
    *   Example: For task `04`, run `.\Update-Dtos.ps1 -TaskNumber 4`.

4.  **Complete**: Once satisfied, move the file from `Tasks/in_progress/` to `Tasks/completed/` and commit the change in the main repository or the worktree.

5.  **Merge & Cleanup**:
    *   **Return to the Main Repository directory** (`c:\DevCurrent\AIInterviewer\AIInterviewer`).
    *   Merge the feature branch: `git merge feature/<task-name>`.
    *   Remove the worktree folder from the Workspace Root: `git worktree remove ../AIInterviewer-<task-name>`.
    *   Delete the local branch: `git branch -d feature/<task-name>`.

## Coding Standards

*   **One Struct/Function/Class Per File**: We adhere to a strict policy of one struct or function per file unless absolutely needed otherwise.

## Clood Files (Code Domains)

We use "clood files" to track code domains helping us maintain context of related files.

*   **Definition**: A clood file is a JSON file that tracks all files related to a specific code domain.
*   **Location**:Stored in the `clood-groups/` folder.
*   **Usage Rule**: When a task is worked on, you MUST update or create a clood file related to the domain you worked on.
    *   This helps assist future tasks.
    *   Overlap between groups is ok.
    *   It should basically be a list of file paths that are relevant to that specific "clood" or domain.
