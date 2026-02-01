# Development Roadmap: Admin Panel

This document outlines the steps required to build and stabilize the Admin Panel for the Gifter project.

## 1. Authentication & Authorization Stability
The infrastructure for authentication is in place, but needs verification and minor fixes to ensure stability.
- [ ] **Verification**: Confirm `AdminFilterAttribute` correctly redirects unauthenticated users to `/admin/login`.
- [ ] **Fix**: Ensure `AuthController` correctly handles session persistence and cookie encryption for "Remember Me".
- [ ] **Permissions**: Verify that the `hasUserPermission()` logic in `AdminFilterAttribute` correctly maps routes to the `Permissions` table in the database.

## 2. Initial System Setup (Superadmin)
To access the admin panel, a superadmin user must exist in the database with appropriate permissions.
- [ ] **DB Script**: Create a SQL script to insert a superadmin user:
  - **Email**: `admin@gifter.ge` (Example)
  - **Password**: `asdf` (To be hashed/stored as per project standards)
  - **Role**: Admin
- [ ] **Permissions Seeding**: Ensure the `Permissions` table is populated with all admin routes and linked to the Admin role.

## 3. Users Management (Grid + CRUD)
Manage system users, their roles, and status.
- [ ] **Grid Implementation**: Use DevExtreme Grid in `/Admin/Users/Index` view.
- [ ] **Repository Integration**: Connect grid to `UsersRepository.UsersList()`.
- [ ] **CRUD Operations**:
  - [ ] Implement **Create** using `UsersIUD` stored procedure.
  - [ ] Implement **Update** with validation (unique email check).
  - [ ] Implement **Delete/Deactivate** functionality.

## 4. Orders Dashboard (Read-only Grid)
Activity log grid showing all tracked user actions across the platform.
- [x] **Controller**: `OrdersController` at `/admin/orders` with `Orders` (page) and `Grid` (AJAX data) actions
- [x] **Repository**: `OrdersRepository` connected to `dbo.OrdersList` TVF
- [x] **Grid**: DevExtreme DataGrid — read-only (no Add/Edit/Delete)
- [x] **Columns**: #, Date, User, Activity, Wishlist, Gift, Target User
- [x] **Permissions**: Menu item (`/admin/orders`) + catch-all (`/admin/orders/.`) seeded for Admin role
- [x] **Verified**: Grid loads and displays test data correctly

## 5. Gift Lists Management (Grid + Edit/Delete)
Manage user-created gift lists (Wishlists, Registries). Read and edit existing lists; creation is a user-facing action.
- [x] **Controller**: `GiftListsController` at `/admin/giftlists` with page, Grid, GridUpdate, GridDelete actions
- [x] **Repository**: `GiftListsRepository` with `GiftListsList()` TVF and `GiftListsIUD()` proc
- [x] **Grid**: DevExtreme DataGrid — Update and Delete enabled; Add disabled
- [x] **Columns**: #, Title (editable), Owner (read-only), Secret (checkbox), Published (checkbox), End Date, Created
- [x] **Permissions**: Menu item (`/admin/giftlists`) + catch-all (`/admin/giftlists/.`) seeded for Admin role
- [x] **Verified**: Grid loads, displays GiftLists, sort DESC confirmed

## 6. Products Management ~~(Grid + CRUD)~~
> **Deprecated** — Products are boilerplate from the SixtyThreeBits template and are not part of the Gifter domain. The Gifter hierarchy is User → GiftLists (Wishlists) → Gifts. No further work planned here.

## 7. Gifts Management (Grid + Edit/Delete)
All gifts across all wishlists — admin overview with inline edit and delete. Creation is a user-facing action (within their wishlist), so Add is disabled.
- [x] **Controller**: `GiftsController` at `/admin/gifts` with page, Grid, GridUpdate, GridDelete actions
- [x] **Repository**: `GiftsRepository` with `GiftsList()` TVF and `GiftsIUD()` proc
- [x] **Grid**: DevExtreme DataGrid — Update and Delete enabled; Add disabled
- [x] **Columns**: #, Title (editable, required), Wishlist (read-only), Owner (read-only), Price, Currency, URL, Reserved (checkbox, read-only), Created (read-only)
- [x] **Permissions**: Menu item (`/admin/gifts`) + catch-all (`/admin/gifts/.`) + action CodeNames (`AdminGiftsControllerGridUpdate`, `AdminGiftsControllerGridDelete`) seeded for Admin role
- [x] **Verified**: Grid loads and displays gifts with Update/Delete buttons
- [ ] **Pending**: Test Update and Delete end-to-end; investigate empty Owner column (possible NULL in TVF join)

---
**Status:** ✅ Sections 4, 5, and 7 complete; Section 6 (Products) deprecated
**Target Completion:** TBD
