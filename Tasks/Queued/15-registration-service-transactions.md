# Task 15: Implement Transactions in RegisterService

## Objective
Ensure that the user registration process is atomic by wrapping it in a database transaction.

## Requirements
- Wrap the logic in `RegisterService.PostAsync` in a transaction.
- The transaction should cover:
    - `UserManager.CreateAsync`
    - `RegisterNewUserAsync` (which handles ServiceStack session and additional registration logic).
- If any part of the registration fails, the entire user creation should be rolled back (if using a compatible `IUserStore`).
- Note: `UserManager` often handles its own internal transactions, but wrapping the whole service call ensures that higher-level integrations are also atomic.

## Checklist
- [ ] Implement `Db.OpenTransactionAsync()` in `RegisterService.PostAsync`.
- [ ] Ensure `UserManager` operations are compatible with the explicit transaction.
- [ ] Test failure scenarios (e.g., `RegisterNewUserAsync` failing after user creation) to verify rollback.
- [ ] Update clood files.
