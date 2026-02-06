# Task: Revert Conversation to Prior Point

## Objective
Allow users to revert the chat to any previous interviewer message so they can retry answers or re-run questions.

## Status
Not started.

## Requirements
- Add a hover action on each interviewer message labeled "Revert to this point".
    - Only show the action on hover to keep the UI clean.
    - Provide a confirmation prompt to avoid accidental deletion.
- Reverting should delete all messages after the selected interviewer message.
- Add backend support to truncate conversation history from a chosen message onward.
    - Consider new API endpoint(s) for deleting messages after a message ID or sequence index.
    - Ensure permissions and interview ownership checks are enforced.
- Update client state to reflect deleted messages and reset any input/recording state as needed.

## Checklist
- [ ] Implement hover action and button styling for interviewer messages.
- [ ] Add confirmation flow before revert.
- [ ] Add backend endpoint to truncate conversation history from a selected message.
- [ ] Update frontend store/state after revert to match server state.
- [ ] Verify interview session continues correctly after revert.
- [ ] Update relevant clood file(s) after implementation.
