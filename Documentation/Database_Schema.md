# Database Schema Reference

This document contains column names and structure for key database tables.

---

## Permissions Table

Stores admin panel permissions, menu items, and tabs.

| Column | Type | Description |
|--------|------|-------------|
| PermissionID | INT | Primary key, auto-increment |
| PermissionParentID | INT | Foreign key to parent permission (for hierarchy) |
| PermissionCaption | NVARCHAR | Caption text (Georgian) |
| PermissionCaptionEng | NVARCHAR | Caption text (English) |
| PermissionPagePath | NVARCHAR | Regex pattern for URL matching (e.g., `/admin/users/.*`) |
| PermissionCodeName | NVARCHAR | Route name constant (e.g., `AdminUserFollowersControllerFollowers`) |
| PermissionCode | NVARCHAR | Optional code identifier |
| PermissionIsMenuItem | BIT | 1 = shows as menu item/tab, 0 = permission only |
| PermissionMenuIcon | NVARCHAR | Font Awesome icon class (e.g., `fa-solid fa-users`) |
| PermissionSortIndex | INT | Display order within parent |
| PermissionMenuTitle | NVARCHAR | Menu/tab title (Georgian) |
| PermissionMenuTitleEng | NVARCHAR | Menu/tab title (English) |
| PermissionDateCreated | DATETIME | Creation timestamp |

### Permission Hierarchy

```
Root (NULL parent)
└── Admin Section (PermissionID=2)
    └── Users Sub-routes (PermissionID=9)
        ├── User Properties (PermissionID=17)
        └── User Followers (PermissionID=79)
```

### Key Permission Records

| PermissionID | Caption | CodeName | ParentID | IsMenuItem | Purpose |
|--------------|---------|----------|----------|------------|---------|
| 9 | Users Sub-routes | `020F4066-1451-4739-BC42-F6022EEB8881` | 2 | 0 | Tab parent for user detail pages |
| 13 | Users Grid | `AdminUsersControllerGrid` | 9 | 0 | Grid permission |
| 14 | Users Grid Add | `AdminUsersControllerGridAdd` | 9 | 0 | Add permission |
| 15 | Users Grid Update | `AdminUsersControllerGridUpdate` | 9 | 0 | Update permission |
| 16 | Users Grid Delete | `AdminUsersControllerGridDelete` | 9 | 0 | Delete permission |
| 17 | User Properties | `AdminUserPropertiesControllerProperties` | 9 | 1 | Properties tab |
| 79 | User Followers | `AdminUserFollowersControllerFollowers` | 9 | 1 | Followers tab |

**Note:** `IsMenuItem = 1` is required for a permission to appear as a tab.

### Tab Parent Identification

**Important:** For tabs to appear on detail pages, the parent permission must have a specific `PermissionCodeName` (GUID) that matches constants in `WebConstants.Permissions`:

| Detail Page | Parent CodeName (GUID) | C# Constant |
|-------------|------------------------|-------------|
| User tabs | `020F4066-1451-4739-BC42-F6022EEB8881` | `WebConstants.Permissions.User` |
| Page tabs | `EA1EAEBA-9D1B-40E3-99C6-99A46061C050` | `WebConstants.Permissions.Page` |

The code in `UserFilterAttribute.cs` (line 74) finds the tab parent by:
```csharp
var tabsParentID = _model.User.Permissions.FindLast(
    Item => Item.PermissionCodeName == WebConstants.Permissions.User
)?.PermissionID;
```

**Setup for User tabs:** The "Users Sub-routes" (PermissionID=9) must have this CodeName:
```sql
UPDATE Permissions
SET PermissionCodeName = '020F4066-1451-4739-BC42-F6022EEB8881'
WHERE PermissionID = 9
```

### Adding a New Tab

To add a tab to a user detail page:

```sql
INSERT INTO [dbo].[Permissions] (
    PermissionParentID,
    PermissionCaption,
    PermissionCaptionEng,
    PermissionPagePath,
    PermissionCodeName,
    PermissionIsMenuItem,
    PermissionMenuIcon,
    PermissionSortIndex,
    PermissionMenuTitle,
    PermissionMenuTitleEng
)
VALUES (
    9,                                          -- Parent: Users Sub-routes
    'User Followers',                           -- Caption (Georgian)
    'User Followers',                           -- Caption (English)
    '/admin/users/.*/followers',                -- URL regex pattern
    'AdminUserFollowersControllerFollowers',    -- Route name from C# code
    1,                                          -- Is menu item (tab)
    'fa-solid fa-users',                        -- Font Awesome icon
    1,                                          -- Sort order
    'Followers',                                -- Menu title (Georgian)
    'Followers'                                 -- Menu title (English)
)
```

---

## Users Table

| Column | Type | Description |
|--------|------|-------------|
| UserID | INT | Primary key |
| UserFullname | NVARCHAR | Full name |
| UserEmail | NVARCHAR | Email address |
| UserPassword | NVARCHAR | Hashed password |
| UserFirstname | NVARCHAR | First name |
| UserLastname | NVARCHAR | Last name |
| UserBirthdate | DATE | Birth date |
| UserPhoneNumberMobile | NVARCHAR | Mobile phone |
| UserIsActive | BIT | Active status |
| RoleID | INT | Foreign key to Roles |
| UserDateCreated | DATETIME | Creation timestamp |

---

## Followers Table

| Column | Type | Description |
|--------|------|-------------|
| FollowerID | INT | Primary key, auto-increment |
| FollowerFollowingUserID | INT | User doing the following (FK to Users) |
| FollowerFollowedUserID | INT | User being followed (FK to Users) |
| FollowerDateCreated | DATETIME | When the follow was created |

**Unique constraint:** `(FollowerFollowingUserID, FollowerFollowedUserID)` - prevents duplicate follows.

---

## GiftLists Table

| Column | Type | Description |
|--------|------|-------------|
| GiftListID | INT | Primary key |
| GiftListUserID | INT | Owner (FK to Users) |
| GiftListTitle | NVARCHAR | List title |
| GiftListIsSecret | BIT | Secret list flag |
| GiftListIsPublished | BIT | Published flag |
| GiftListEndDate | DATE | End date |
| GiftListDateCreated | DATETIME | Creation timestamp |

---

## Gifts Table

| Column | Type | Description |
|--------|------|-------------|
| GiftID | INT | Primary key |
| GiftGiftListID | INT | FK to GiftLists |
| GiftTitle | NVARCHAR | Gift title |
| GiftDescription | NVARCHAR | Description |
| GiftPrice | DECIMAL | Price |
| GiftCurrency | NVARCHAR | Currency code |
| GiftUrl | NVARCHAR | Link to product |
| GiftImageUrl | NVARCHAR | Image URL |
| GiftIsReserved | BIT | Reserved flag |
| GiftReservedByUserID | INT | FK to Users (who reserved) |
| GiftDateCreated | DATETIME | Creation timestamp |

---

## Orders Table

Activity log for user actions.

| Column | Type | Description |
|--------|------|-------------|
| OrderID | INT | Primary key |
| OrderUserID | INT | User who performed action (FK to Users) |
| OrderType | INT | Action type (1=Wishlist Created, 2=Gift Added, 3=User Followed, 4=Gift Reserved) |
| OrderGiftListID | INT | Related gift list (nullable) |
| OrderGiftID | INT | Related gift (nullable) |
| OrderTargetUserID | INT | Target user (for follows, nullable) |
| OrderDateCreated | DATETIME | When action occurred |

---

## Roles Table

| Column | Type | Description |
|--------|------|-------------|
| RoleID | INT | Primary key |
| RoleName | NVARCHAR | Role name (Georgian) |
| RoleNameEng | NVARCHAR | Role name (English) |
| RoleSortIndex | INT | Display order |
| RoleDateCreated | DATETIME | Creation timestamp |

---

## RolesPermissions Table

Links roles to permissions (many-to-many).

| Column | Type | Description |
|--------|------|-------------|
| RolePermissionID | INT | Primary key |
| RoleID | INT | FK to Roles |
| PermissionID | INT | FK to Permissions |

**Note:** For a user to see a tab/menu item, their role must have that permission assigned:
```sql
INSERT INTO RolesPermissions (RoleID, PermissionID)
VALUES (1, {NEW_PERMISSION_ID})  -- Assign to Admin role
```
