# Task 14: Implement Transactions in InterviewService

## Objective
Ensure that multi-step database operations in `InterviewService` are wrapped in database transactions to maintain data integrity.

## Requirements
- Identify methods in `InterviewService.cs` that perform multiple database writes or related logical units of work.
- Wrap these operations in `Db.OpenTransaction()` or `Db.OpenTransactionAsync()` blocks.
- Specifically focus on:
    - `Post(AddChatMessage)`: While the AI call is long-running, we should consider if the User message and the Subsequent AI response should be linked or if other data integrity checks are needed.
    - `Post(StartInterview)`: Ensure consistency if multiple writes are added in the future.
    - Future `Delete` or `Revert` operations.
- Ensure proper error handling and rollbacks on failure.

## Checklist
- [ ] Audit `InterviewService.cs` for all write operations.
- [ ] Implement transactions for `AddChatMessage` (considering the AI call timeout/failure).
- [ ] Implement transactions for any new or updated service methods.
- [ ] Verify that transactions work as expected during failures (e.g., DB disconnect, validation errors).
- [ ] Update clood files for the interview domain.
