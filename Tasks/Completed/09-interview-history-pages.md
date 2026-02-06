# Task: Interview history pages + restart from prompt

## Objective
Provide frontend pages for viewing chat/interview history and allow users to start a new interview from a historical entry by reusing its prompt.

## Requirements
- Add a history list page that shows previous chat/interview sessions with key metadata (title, date, status, etc.).
- Add a history detail page that shows the full transcript and metadata for a selected session.
- Provide a “Start new interview from this prompt” action from history entries (list and/or detail view).
- Reuse the stored prompt content from the historical session when starting a new interview.
- Ensure navigation integrates with existing routing/layouts.
- Add relevant navigation links for the history pages in the app navbar.

## Notes
- Confirm whether history data is already available in the API; if not, define the needed DTOs and endpoints.
- Align UI styling with the existing Tailwind/Vue patterns in `AIInterviewer.Client/src/pages`.

## Checklist
- [ ] Review existing interview/chat DTOs and data sources.
- [ ] Define API/DTO additions if history data is missing.
- [ ] Implement history list page.
- [ ] Implement history detail page.
- [ ] Implement “start new interview” action that reuses prompt.
- [ ] Add navigation links to history pages.
