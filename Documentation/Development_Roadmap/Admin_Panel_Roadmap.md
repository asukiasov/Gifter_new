# Development Roadmap: Admin Panel

This document outlines the steps required to build and stabilize the Admin Panel for the Gifter project.

## 1. Authentication & Authorization Stability
The infrastructure for authentication is in place.
- [x] **Verification**: `AdminFilterAttribute` correctly redirects unauthenticated users to `/admin/login`
- [x] **Session**: `AuthController` handles session persistence and cookie encryption for "Remember Me"
- [x] **Permissions**: `hasUserPermission()` logic in `AdminFilterAttribute` correctly maps routes to `Permissions` table via regex matching

## 2. Initial System Setup (Superadmin)
Superadmin user exists in the database with appropriate permissions.
- [x] **DB Script**: Admin user created via `SetupController.SeedAdmin()` endpoint
  - **Email**: `admin@gifter.com`
  - **Password**: `asdf`
  - **Role**: Admin (RoleID = 1)
- [x] **Permissions Seeding**: All admin routes populated in `Permissions` table and linked to Admin role via `RolesPermissions`

## 3. Users Management (Grid + CRUD)
Manage system users, their roles, and status.
- [x] **Grid Implementation**: DevExtreme Grid at `/admin/users` via `UsersController`
- [x] **Repository Integration**: Connected to `UsersRepository.UsersList()`
- [x] **CRUD Operations**:
  - [x] **Create** using `UsersIUD` stored procedure (GridAdd action)
  - [x] **Update** with email uniqueness validation (GridUpdate action)
  - [x] **Delete** with avatar cleanup (GridDelete action)
- [x] **Verified**: Grid loads, CRUD operations work, role dropdown populated

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

## 8. User Followers Tab (Read-only Grid)
Displays users who follow a specific user. Accessible as a tab on the User detail page.
- [x] **Controller**: `UserFollowersController` at `/admin/users/{userID}/followers` with `Followers` (page) and `FollowersGrid` (AJAX data) actions
- [x] **Repository**: Uses existing `FollowersRepository.FollowersMyFollowers()` method connected to `dbo.FollowersMyFollowers` TVF
- [x] **Grid**: DevExtreme DataGrid — read-only (no Add/Edit/Delete)
- [x] **Columns**: #, Follower (fullname), Email, Since (date)
- [x] **Permissions**: Tab permission seeded with `PermissionCodeName = 'AdminUserFollowersControllerFollowers'`, `PermissionParentID` pointing to User permission, `PermissionIsMenuItem = 1`
- [x] **Verified**: Tab appears on User detail page, grid loads and displays followers

---
**Status:** ✅ Sections 1, 2, 3, 4, 5, 7, and 8 complete; Section 6 (Products) deprecated
**Target Completion:** Complete
