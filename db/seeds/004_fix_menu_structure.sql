-- 004_fix_menu_structure.sql
-- ONE-TIME migration: fixes the admin sidebar menu after duplicate permissions were created.
-- Run this ONCE on the existing database, then re-login to see changes.
--
-- Desired menu:
--   1. Dashboard
--   2. System  >  System Properties, Dictionaries, Error Log
--   3. Roles & Permissions  >  Users, Roles, Permissions
--   4. Activity Log
--   5. Gift Lists
--   6. Gifts
--
-- Problem: seed 002 created NEW parent groups (IDs 70, 74) that duplicate the
-- OLD parents (IDs 4, 3) already used by the app. The UPDATE statements in 002
-- targeted PermissionCode which only matched the new entries, leaving old ones
-- unchanged. This script cleans that up.

SET XACT_ABORT ON;
BEGIN TRANSACTION;
BEGIN TRY

    -- =================================================================
    -- Step 1: Re-parent children of NEW parents before deleting them
    -- =================================================================

    -- Children of ID=70 (new ADMIN_SYSTEM) → move to ID=4 (old System)
    UPDATE Permissions SET PermissionParentID = 4 WHERE PermissionParentID = 70;

    -- Children of ID=74 (new ADMIN_USERS_MANAGEMENT) → move to ID=3 (old R&P)
    UPDATE Permissions SET PermissionParentID = 3 WHERE PermissionParentID = 74;

    PRINT 'Re-parented children of duplicate parent groups';

    -- =================================================================
    -- Step 2: Delete duplicate NEW parent groups (no page path, no use)
    -- =================================================================

    DELETE FROM RolesPermissions WHERE PermissionID IN (70, 74);
    DELETE FROM Permissions WHERE PermissionID IN (70, 74);

    PRINT 'Deleted duplicate parent groups (ADMIN_SYSTEM=70, ADMIN_USERS_MANAGEMENT=74)';

    -- =================================================================
    -- Step 3: Top-level menu items — set sort order
    -- =================================================================

    -- 1. Dashboard (ID=1)
    UPDATE Permissions
    SET PermissionSortIndex = 1, PermissionParentID = NULL,
        PermissionIsMenuItem = 1, PermissionMenuIcon = 'mdi-view-dashboard'
    WHERE PermissionID = 1;

    -- 2. System (ID=4, old parent, code='#')
    UPDATE Permissions
    SET PermissionSortIndex = 2, PermissionParentID = NULL,
        PermissionIsMenuItem = 1, PermissionMenuIcon = 'mdi-cogs'
    WHERE PermissionID = 4;

    -- 3. Roles & Permissions (ID=3, old parent, code='#')
    UPDATE Permissions
    SET PermissionSortIndex = 3, PermissionParentID = NULL,
        PermissionIsMenuItem = 1, PermissionMenuIcon = 'mdi-shield-account'
    WHERE PermissionID = 3;

    -- 4. Activity Log (ID=26)
    UPDATE Permissions
    SET PermissionSortIndex = 4, PermissionParentID = NULL,
        PermissionIsMenuItem = 1, PermissionMenuIcon = 'mdi-history'
    WHERE PermissionID = 26;

    -- 5. Gift Lists (ID=31)
    UPDATE Permissions
    SET PermissionSortIndex = 5, PermissionParentID = NULL,
        PermissionIsMenuItem = 1, PermissionMenuIcon = 'mdi-format-list-bulleted'
    WHERE PermissionID = 31;

    -- 6. Gifts (ID=51)
    UPDATE Permissions
    SET PermissionSortIndex = 6, PermissionParentID = NULL,
        PermissionIsMenuItem = 1, PermissionMenuIcon = 'mdi-gift'
    WHERE PermissionID = 51;

    PRINT 'Set top-level menu sort order';

    -- =================================================================
    -- Step 4: System children (parent = ID 4)
    -- =================================================================

    -- 2.1 System Properties (ID=8, old, path=/admin/system-properties)
    UPDATE Permissions
    SET PermissionSortIndex = 1, PermissionParentID = 4, PermissionIsMenuItem = 1
    WHERE PermissionID = 8;

    -- 2.2 Dictionaries (ID=72, was under 70, path=/admin/dictionaries)
    UPDATE Permissions
    SET PermissionSortIndex = 2, PermissionParentID = 4, PermissionIsMenuItem = 1
    WHERE PermissionID = 72;

    -- 2.3 Error Log (ID=27, was top-level, path=/admin/error-log) → move under System
    UPDATE Permissions
    SET PermissionSortIndex = 3, PermissionParentID = 4, PermissionIsMenuItem = 1
    WHERE PermissionID = 27;

    PRINT 'Set System children (System Properties, Dictionaries, Error Log)';

    -- =================================================================
    -- Step 5: Roles & Permissions children (parent = ID 3)
    -- =================================================================

    -- 3.1 Users (ID=2, was top-level) → move under R&P
    UPDATE Permissions
    SET PermissionSortIndex = 1, PermissionParentID = 3, PermissionIsMenuItem = 1
    WHERE PermissionID = 2;

    -- 3.2 Roles (ID=5, already under 3)
    UPDATE Permissions
    SET PermissionSortIndex = 2, PermissionParentID = 3, PermissionIsMenuItem = 1
    WHERE PermissionID = 5;

    -- 3.3 Permissions (ID=6, already under 3)
    UPDATE Permissions
    SET PermissionSortIndex = 3, PermissionParentID = 3, PermissionIsMenuItem = 1
    WHERE PermissionID = 6;

    -- 3.4 Roles-Permissions (ID=7, already under 3)
    UPDATE Permissions
    SET PermissionSortIndex = 4, PermissionParentID = 3, PermissionIsMenuItem = 1
    WHERE PermissionID = 7;

    PRINT 'Set Roles & Permissions children (Users, Roles, Permissions, Roles-Permissions)';

    -- =================================================================
    -- Step 6: Hide items NOT in the sidebar menu
    -- =================================================================

    -- Page Management group + children (IDs 53, 56, 54, 55) — hide
    UPDATE Permissions SET PermissionIsMenuItem = 0 WHERE PermissionID IN (53, 56, 54, 55);

    -- New duplicate entries — hide from menu (keep for access control of alternate routes)
    -- ID=71: /admin/systemproperties  (duplicate of ID=8 /admin/system-properties)
    -- ID=73: /admin/errorlog          (duplicate of ID=27 /admin/error-log)
    -- ID=75: /admin/rolespermissions   (duplicate of ID=7 /admin/roles-permissions)
    UPDATE Permissions SET PermissionIsMenuItem = 0 WHERE PermissionID IN (71, 73, 75);

    -- Duplicate Users entry (ID=33) — already hidden, ensure
    UPDATE Permissions SET PermissionIsMenuItem = 0 WHERE PermissionID = 33;

    PRINT 'Hidden non-menu items';

    -- =================================================================
    -- Step 7: Ensure RolesPermissions links for old parent groups
    -- =================================================================

    DECLARE @RoleID INT = (SELECT RoleID FROM Roles WHERE RoleName = 'Admin');

    IF @RoleID IS NOT NULL
    BEGIN
        INSERT INTO RolesPermissions (RoleID, PermissionID)
        SELECT @RoleID, p.PermissionID
        FROM Permissions p
        WHERE p.PermissionID IN (3, 4)
        AND NOT EXISTS (
            SELECT 1 FROM RolesPermissions rp
            WHERE rp.RoleID = @RoleID AND rp.PermissionID = p.PermissionID
        );

        PRINT 'Ensured RolesPermissions links for old parent groups';
    END

    COMMIT TRANSACTION;
    PRINT '';
    PRINT '=== Menu structure fix completed successfully! ===';
    PRINT 'Please re-login to see the updated sidebar menu.';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Error: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
