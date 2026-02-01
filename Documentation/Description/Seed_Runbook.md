# Admin Panel Seed Runbook

This runbook explains the order and how to run the idempotent seed scripts in `db/seeds/`.

## Files

- `001_seed_roles.sql` — Create Admin role (if not exists).
- `002_seed_permissions.sql` — Insert page-level `Permissions` rows for all admin routes. Covers:
  - **Gifter domain**: `/admin/orders`, `/admin/giftlists`, `/admin/gifts` (+ `/.` catch-alls)
  - **CMS**: `/admin/pages`, `/admin/redirects`, `/admin/menuheader`, `/admin/menufooter`
  - **System**: `/admin/users`, `/admin/roles`, `/admin/permissions`, `/admin/systemproperties`, `/admin/errorlog`, etc.
  - **Boilerplate** (not part of Gifter domain, kept for completeness): `/admin/products`, `/admin/productcategories`, `/admin/brands`, `/admin/news`, `/admin/blog`
- `003_seed_rolespermissions.sql` — Link all seeded permissions to the Admin role. Set-based and idempotent. Prints a warning if any paths in its curated list are missing from `Permissions`.
- `004_seed_superadmin.sql` — Create superadmin user `admin@gifter.com` (password `asdf`) if not present.

> All scripts are idempotent — safe to run multiple times.

## Recommended order

1. `001_seed_roles.sql`
2. `002_seed_permissions.sql`
3. `003_seed_rolespermissions.sql`
4. `004_seed_superadmin.sql` (or use `GET /admin/setup/seed-admin` endpoint)

## How to run

**SSMS or SmarterASP SQL editor:** Open each file and execute in the correct database. Scripts do not use `GO` batch separators, so they run as a single batch in any SQL editor.

**sqlcmd:**
```
sqlcmd -S <server> -d <database> -i db/seeds/001_seed_roles.sql
```
Repeat for each file in order.

## Post-seed checks

- `Roles` has an `Admin` row (RoleID = 1 by convention).
- `Permissions` contains rows for the Gifter admin routes: `/admin/orders`, `/admin/orders/.`, `/admin/giftlists`, `/admin/giftlists/.`, `/admin/gifts`, `/admin/gifts/.`.
- `003` output confirms how many new links were added and flags any missing permission paths.
- `RolesPermissions` maps Admin role to all seeded permissions.
- `Users` contains `admin@gifter.com`. Log in to `/admin/login` and change the password.

## Notes & Caveats

- `002` seeds page-level permissions only. Action-level button visibility (Update/Delete buttons on grids) requires separate `PermissionCodeName` rows (e.g., `AdminGiftsControllerGridUpdate`). These are currently seeded manually on SmarterASP — see the Gifts Admin Grid section in `Sprint_2_CMS_Gifter_Domain.md` for the exact statements.
- Products/ProductCategories/Brands are boilerplate from the SixtyThreeBits template and are **not part of the Gifter domain**. Their permissions are included in the seed for completeness but no further work is planned on them.
- `004` inserts a plaintext password to match `SetupController`. If password hashing is added, prefer the endpoint or update the script.
- Always run seeds against a backup or staging database first.

## Next steps

- Add action-level CodeName permissions to `002` (or a separate `002b` script) so button visibility is also seed-managed.
- Add a PowerShell/Bash wrapper to run all seeds in order with fail-fast on errors.
