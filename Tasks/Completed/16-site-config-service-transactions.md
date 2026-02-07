# Task 16: Implement Transactions in SiteConfigService

## Objective
Ensure that site configuration updates are consistent by wrapping them in a database transaction.

## Requirements
- Wrap `SiteConfigService.Put` in a transaction.
- While the secondary operation is an in-memory update (`holder.SiteConfig = config`), ensuring the database write is fully committed before or during the update is good practice, especially if future configuration updates touch multiple tables (e.g., separate tables for different provider settings).

## Checklist
- [x] Implement `Db.OpenTransaction()` in `SiteConfigService.Put`.
- [x] Verify that the in-memory holder is only updated after a successful DB commit.
- [x] Update clood files.
