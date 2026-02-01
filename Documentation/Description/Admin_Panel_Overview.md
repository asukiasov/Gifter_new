# Admin Panel — Overview & Status ✅

**Purpose:** A concise description for developers and maintainers explaining what the Admin Panel does, what is implemented, and what remains to be completed.

---

## 1. Quick Status Summary
- **Implemented (done):**
  - Admin theme and assets: `html/admin_borex_html` (Borex template) ✅
  - Core admin controllers: `SixtyThreeBits.Web/Controllers/Admin/*` (Users, Orders, GiftLists, Pages, Roles, Permissions, etc.) ✅
  - DevExtreme grids for server-side CRUD (Users, GiftLists, Gifts, Orders dashboard read-only) ✅
  - Setup endpoint to seed an admin account: `admin/setup/seed-admin` (`SetupController`) ✅
  - Roadmap & sprint docs: `Documentation/Development_Roadmap/Admin_Panel_Roadmap.md`, `Sprint_1_Admin_Panel.md`, `Sprint_2_CMS_Gifter_Domain.md` ✅

- **Requires verification / In progress:**
  - Confirm `AdminFilterAttribute` redirects unauthenticated users to `/admin/login` and `HasPermission()` mapping works as expected for catch-all paths (`.` logic). See `SixtyThreeBits.Web/Filters/Admin/AdminFilterAttribute.cs`. ⚠️
  - Ensure permissions seeding includes catch-all entries (e.g., `/admin/orders/.`) and is idempotent. ⚠️
  - Integration tests for stored procs / TVFs used by admin (CMS, Gifter domain) — still needed. ⚠️

- **Planned / To do:**
  - Add idempotent SQL seed scripts to create Admin role, Permissions (including `/.` catch-alls), and Superadmin user. ⏳
  - Add automated end-to-end tests for admin login + permissions and DevExtreme grid actions. ⏳
  - Create a short runbook for first-time setup (ordering of DB script execution, seed steps). ⏳

- **Recent changes (done):**
  - `db/seeds` added with idempotent scripts: `001_seed_roles.sql`, `002_seed_permissions.sql` (expanded), `003_seed_rolespermissions.sql` (updated to link a curated set of permissions to Admin role idempotently), `004_seed_superadmin.sql`. ✅
  - `003_seed_rolespermissions.sql` now performs a set-based insert and prints any missing permission paths to help maintainers add them to `002`. ✅

- **Blockers / Known issues:**
  - App logs show missing DB objects (stored procedures / TVFs) used by Products/ProductCategories (e.g., `ProductCategoriesList`, `ProductsList`, `ProductCategoriesIUD`, `ProductCategoriesSyncParentsAndSortIndexes`). These must be applied from Sprint DB scripts before Products admin pages will work. ⚠️

---

## 2. How it works (high level) 🔍
1. Request enters at an admin route (e.g., `/admin/users`).
2. Controller inherits from `AdminControllerBase` which applies `[TypeFilter(typeof(AdminFilterAttribute))]`.
3. `AdminFilterAttribute`:
   - Checks authentication (`_model.User != null`).
   - Calls permission check `_model.User.HasPermission(_model.UrlCurrentPageWithoutDomain)` — permission rows are matched by `PermissionPagePath` (ending with `.` becomes catch-all regex). If not allowed, returns a NotFound-like admin view. 
   - If allowed and not an AJAX request, it sets up `AdminLayoutViewModel` (menu, breadcrumbs, plugins, language, sidebar state, toasts).
4. Controllers call Models → Repositories → TVFs/Stored Procedures for data (example: `GiftListsRepository` uses `GiftListsList` TVF and `GiftListsIUd` proc).
5. Client-side: DevExtreme grids call server endpoints (grid, add, update, delete) and use DevExtreme UI inside the admin theme (`html/admin_borex_html`).

---

## 3. Inventory (selected files & locations) 📁
- Filters & Layouts:
  - `SixtyThreeBits.Web/Filters/Admin/AdminFilterAttribute.cs`
  - `SixtyThreeBits.Web/Controllers/Admin/Base/AdminControllerBase.cs`
- Seed / Setup:
  - `SixtyThreeBits.Web/Controllers/Admin/Auth/SetupController.cs` (`/admin/setup/seed-admin`)
- Controllers (examples):
  - `SixtyThreeBits.Web/Controllers/Admin/Users/UsersControllers.cs` (`/admin/users`) — grid endpoints
  - `SixtyThreeBits.Web/Controllers/Admin/Orders/OrdersController.cs` (`/admin/orders`) — dashboard grid (read-only)
  - `SixtyThreeBits.Web/Controllers/Admin/GiftLists/GiftListsController.cs` (`/admin/giftlists`) — grid + update/delete
  - `SixtyThreeBits.Web/Controllers/Admin/Gifts/GiftsController.cs` (`/admin/gifts`) — grid + update/delete
  - `SixtyThreeBits.Web/Controllers/Admin/Permissions/PermissionsController.cs` and `Roles` controllers
- Front-end admin theme:
  - `html/admin_borex_html` — theme assets, SCSS/JS, pages
- Documentation references:
  - `Documentation/Development_Roadmap/Admin_Panel_Roadmap.md`
  - `Documentation/Development_Roadmap/Sprint_1_Admin_Panel.md`
  - `Documentation/Development_Roadmap/Sprint_2_CMS_Gifter_Domain.md`

---

## 4. Setup & Run notes ⚙️
1. Ensure DB schema and stored procs/TVFs from `Sprint_1_Admin_Panel.md` are applied (scripts included in that doc).
2. Seed roles & permissions (create Admin role = RoleID 1). Make sure seeded permissions include both page routes and `/.` catch-all rows as noted in the sprint docs.
3. Run `/admin/setup/seed-admin` once (or use idempotent SQL seed) to create initial Superadmin (email `admin@gifter.com` in code example). Update credentials after first login.
4. Check `appsettings.*` and `SystemProperties.AdminEmails` (see `SixtyThreeBits.Core/Properties/Resources.resx`) if any admin email notifications are expected.

---

## 5. Verification checklist / Tests to add 🧪
- [ ] Unit test for `AdminFilterAttribute` — redirection when unauthenticated.
- [ ] Integration test: permission match behavior for exact path and `/.` catch-all.
- [ ] E2E test: Admin login → open `/admin/users` grid → perform CRUD via grid actions.
- [ ] DB seed tests: run seed script twice to confirm idempotency.

---

## 6. Short-term next steps (priority) 📌
1. Create idempotent SQL seed scripts for Admin role, Permissions (include `/.`), and Superadmin user. (High)
2. Add tests for `AdminFilterAttribute` and a basic E2E for the Users grid. (High)
3. Add a one-page runbook and link it from `Documentation/Development_Roadmap/README.md`. (Medium)

---

## 7. References & Links 🔗
- `SixtyThreeBits.Web/Filters/Admin/AdminFilterAttribute.cs`
- `SixtyThreeBits.Web/Controllers/Admin/Users/UsersControllers.cs`
- `SixtyThreeBits.Web/Controllers/Admin/Auth/SetupController.cs`
- `html/admin_borex_html` (admin UI theme)
- `Documentation/Development_Roadmap/Admin_Panel_Roadmap.md`

---
