-- 002_seed_permissions.sql
-- Idempotent script to insert key admin Permissions (page & catch-all entries).
-- Updated: 2026-02-04 — Restructured admin sidebar menu with groups and sort order

SET XACT_ABORT ON;
BEGIN TRY

    -----------------------------------------------------------------------
    -- 1. Dashboard
    -----------------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin', 'ADMIN_DASHBOARD', 'Dashboard', 1, 'mdi-view-dashboard');
        PRINT 'Inserted /admin permission (Dashboard)';
    END
    ELSE
        PRINT '/admin permission exists';

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/.')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/.', 'ADMIN_DASHBOARD_CATCHALL', 'Admin (catch-all)', 0, NULL);
        PRINT 'Inserted /admin/. permission (catch-all)';
    END
    ELSE
        PRINT '/admin/. permission exists';

    -----------------------------------------------------------------------
    -- 2. System (parent group)
    -----------------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'ADMIN_SYSTEM')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES (NULL, 'ADMIN_SYSTEM', 'System', 1, 'mdi-cogs');
        PRINT 'Inserted ADMIN_SYSTEM permission (System group)';
    END
    ELSE
        PRINT 'ADMIN_SYSTEM permission exists';

    -- 2.1 System Properties
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/systemproperties')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/systemproperties', 'ADMIN_SYSTEM_PROPERTIES', 'System Properties', 1, 'mdi-cog');
        PRINT 'Inserted /admin/systemproperties permission';
    END
    ELSE
        PRINT '/admin/systemproperties permission exists';

    -- 2.2 Dictionaries
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/dictionaries')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/dictionaries', 'ADMIN_DICTIONARIES', 'Dictionaries', 1, 'mdi-dictionary');
        PRINT 'Inserted /admin/dictionaries permission';
    END
    ELSE
        PRINT '/admin/dictionaries permission exists';

    -- 2.3 Error Log
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/errorlog')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/errorlog', 'ADMIN_ERROR_LOG', 'Error Log', 1, 'mdi-bug');
        PRINT 'Inserted /admin/errorlog permission';
    END
    ELSE
        PRINT '/admin/errorlog permission exists';

    -----------------------------------------------------------------------
    -- 3. Roles & Permissions (parent group)
    -----------------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionCode = 'ADMIN_USERS_MANAGEMENT')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES (NULL, 'ADMIN_USERS_MANAGEMENT', 'Roles & Permissions', 1, 'mdi-shield-account');
        PRINT 'Inserted ADMIN_USERS_MANAGEMENT permission (Roles & Permissions group)';
    END
    ELSE
        PRINT 'ADMIN_USERS_MANAGEMENT permission exists';

    -- 3.1 Users (child of Roles & Permissions)
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/users')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/users', 'ADMIN_USERS', 'Users', 1, 'mdi-account-multiple');
        PRINT 'Inserted /admin/users permission';
    END
    ELSE
        PRINT '/admin/users permission exists';

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/users/.')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/users/.', 'ADMIN_USERS_CATCHALL', 'Users (catch-all)', 0, NULL);
        PRINT 'Inserted /admin/users/. permission (catch-all)';
    END
    ELSE
        PRINT '/admin/users/. permission exists';

    -- 3.2 Roles (child of Roles & Permissions)
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/roles')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/roles', 'ADMIN_ROLES', 'Roles', 1, 'mdi-account-key');
        PRINT 'Inserted /admin/roles permission';
    END
    ELSE
        PRINT '/admin/roles permission exists';

    -- 3.3 Permissions (child of Roles & Permissions)
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/permissions')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/permissions', 'ADMIN_PERMISSIONS', 'Permissions', 1, 'mdi-shield-key');
        PRINT 'Inserted /admin/permissions permission';
    END
    ELSE
        PRINT '/admin/permissions permission exists';

    -- Roles & Permissions page (hidden, access control only)
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/rolespermissions')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/rolespermissions', 'ADMIN_ROLES_PERMISSIONS', 'Roles & Permissions', 0, NULL);
        PRINT 'Inserted /admin/rolespermissions permission';
    END
    ELSE
        PRINT '/admin/rolespermissions permission exists';

    -----------------------------------------------------------------------
    -- 4. Activity Log (was "Orders")
    -----------------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/orders')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/orders', 'ADMIN_ORDERS', 'Activity Log', 1, 'mdi-history');
        PRINT 'Inserted /admin/orders permission (Activity Log)';
    END
    ELSE
        PRINT '/admin/orders permission exists';

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/orders/.')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/orders/.', 'ADMIN_ORDERS_CATCHALL', 'Activity Log (catch-all)', 0, NULL);
        PRINT 'Inserted /admin/orders/. permission (catch-all)';
    END
    ELSE
        PRINT '/admin/orders/. permission exists';

    -----------------------------------------------------------------------
    -- 5. Gift Lists
    -----------------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/giftlists')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/giftlists', 'ADMIN_GIFTLISTS', 'Gift Lists', 1, 'mdi-format-list-bulleted');
        PRINT 'Inserted /admin/giftlists permission';
    END
    ELSE
        PRINT '/admin/giftlists permission exists';

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/giftlists/.')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/giftlists/.', 'ADMIN_GIFTLISTS_CATCHALL', 'Gift Lists (catch-all)', 0, NULL);
        PRINT 'Inserted /admin/giftlists/. permission (catch-all)';
    END
    ELSE
        PRINT '/admin/giftlists/. permission exists';

    -----------------------------------------------------------------------
    -- 6. Gifts
    -----------------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/gifts')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/gifts', 'ADMIN_GIFTS', 'Gifts', 1, 'mdi-gift');
        PRINT 'Inserted /admin/gifts permission';
    END
    ELSE
        PRINT '/admin/gifts permission exists';

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/gifts/.')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/gifts/.', 'ADMIN_GIFTS_CATCHALL', 'Gifts (catch-all)', 0, NULL);
        PRINT 'Inserted /admin/gifts/. permission (catch-all)';
    END
    ELSE
        PRINT '/admin/gifts/. permission exists';

    -----------------------------------------------------------------------
    -- Hidden permissions (not in sidebar but still needed for access control)
    -----------------------------------------------------------------------

    -- Page Management
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/page-management')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/page-management', 'ADMIN_PAGE_MANAGEMENT', 'Page Management', 0, NULL);
        PRINT 'Inserted /admin/page-management permission';
    END
    ELSE
        PRINT '/admin/page-management permission exists';

    -- CMS: Pages
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/pages')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/pages', 'ADMIN_PAGES', 'Pages (CMS)', 0, 'mdi-file-document');
        PRINT 'Inserted /admin/pages permission';
    END
    ELSE
        PRINT '/admin/pages permission exists';

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/pages/.')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/pages/.', 'ADMIN_PAGES_CATCHALL', 'Pages (catch-all)', 0, NULL);
        PRINT 'Inserted /admin/pages/. permission (catch-all)';
    END
    ELSE
        PRINT '/admin/pages/. permission exists';

    -- CMS: Redirects
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/redirects')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/redirects', 'ADMIN_REDIRECTS', 'Redirects', 0, 'mdi-share-variant');
        PRINT 'Inserted /admin/redirects permission';
    END
    ELSE
        PRINT '/admin/redirects permission exists';

    -- CMS: Menu Header
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/menu-header')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/menu-header', 'ADMIN_MENU_HEADER', 'Menu Header', 0, NULL);
        PRINT 'Inserted /admin/menu-header permission';
    END
    ELSE
        PRINT '/admin/menu-header permission exists';

    -- CMS: Menu Footer
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/menu-footer')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/menu-footer', 'ADMIN_MENU_FOOTER', 'Menu Footer', 0, NULL);
        PRINT 'Inserted /admin/menu-footer permission';
    END
    ELSE
        PRINT '/admin/menu-footer permission exists';

    -- Email Templates
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/emailtemplates')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/emailtemplates', 'ADMIN_EMAIL_TEMPLATES', 'Email Templates', 0, 'mdi-email');
        PRINT 'Inserted /admin/emailtemplates permission';
    END
    ELSE
        PRINT '/admin/emailtemplates permission exists';

    -- File Manager
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/filemanager')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/filemanager', 'ADMIN_FILE_MANAGER', 'File Manager', 0, 'mdi-folder');
        PRINT 'Inserted /admin/filemanager permission';
    END
    ELSE
        PRINT '/admin/filemanager permission exists';

    -----------------------------------------------------------------------
    -- Normalize legacy parent codes
    -- Old databases have parent groups with PermissionCode = '#'.
    -- Give them proper codes so INSERT checks and UPDATE targeting work.
    -- Safe no-op on fresh databases (no '#' entries exist).
    -----------------------------------------------------------------------
    UPDATE Permissions SET PermissionCode = 'ADMIN_SYSTEM'
    WHERE PermissionCode = '#' AND PermissionCaption = 'System' AND PermissionPagePath IS NULL;

    UPDATE Permissions SET PermissionCode = 'ADMIN_USERS_MANAGEMENT'
    WHERE PermissionCode = '#' AND PermissionCaption = 'Roles & Permissions' AND PermissionPagePath IS NULL;

    -----------------------------------------------------------------------
    -- Menu structure: sort order, parent-child, visibility
    -- Only runs on FRESH databases (no legacy paths like /admin/system-properties).
    -- Legacy databases are structured by 004_fix_menu_structure.sql instead.
    -----------------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/system-properties')
    BEGIN
        -- 1. Dashboard (top-level, sort=1)
        UPDATE Permissions SET PermissionSortIndex = 1, PermissionParentID = NULL, PermissionIsMenuItem = 1, PermissionMenuIcon = 'mdi-view-dashboard' WHERE PermissionCode = 'ADMIN_DASHBOARD';

        -- 2. System group (top-level, sort=2)
        UPDATE Permissions SET PermissionSortIndex = 2, PermissionParentID = NULL, PermissionIsMenuItem = 1, PermissionMenuIcon = 'mdi-cogs', PermissionCaption = 'System' WHERE PermissionCode = 'ADMIN_SYSTEM';

        -- 2.1 System Properties (child of System)
        UPDATE Permissions SET PermissionSortIndex = 1, PermissionParentID = (SELECT PermissionID FROM Permissions WHERE PermissionCode = 'ADMIN_SYSTEM'), PermissionIsMenuItem = 1 WHERE PermissionCode = 'ADMIN_SYSTEM_PROPERTIES';
        -- 2.2 Dictionaries (child of System)
        UPDATE Permissions SET PermissionSortIndex = 2, PermissionParentID = (SELECT PermissionID FROM Permissions WHERE PermissionCode = 'ADMIN_SYSTEM'), PermissionIsMenuItem = 1 WHERE PermissionCode = 'ADMIN_DICTIONARIES';
        -- 2.3 Error Log (child of System)
        UPDATE Permissions SET PermissionSortIndex = 3, PermissionParentID = (SELECT PermissionID FROM Permissions WHERE PermissionCode = 'ADMIN_SYSTEM'), PermissionIsMenuItem = 1 WHERE PermissionCode = 'ADMIN_ERROR_LOG';

        -- 3. Roles & Permissions group (top-level, sort=3)
        UPDATE Permissions SET PermissionSortIndex = 3, PermissionParentID = NULL, PermissionIsMenuItem = 1, PermissionMenuIcon = 'mdi-shield-account', PermissionCaption = 'Roles & Permissions' WHERE PermissionCode = 'ADMIN_USERS_MANAGEMENT';

        -- 3.1 Users (child of Roles & Permissions)
        UPDATE Permissions SET PermissionSortIndex = 1, PermissionParentID = (SELECT PermissionID FROM Permissions WHERE PermissionCode = 'ADMIN_USERS_MANAGEMENT'), PermissionIsMenuItem = 1 WHERE PermissionCode = 'ADMIN_USERS';
        -- 3.2 Roles (child of Roles & Permissions)
        UPDATE Permissions SET PermissionSortIndex = 2, PermissionParentID = (SELECT PermissionID FROM Permissions WHERE PermissionCode = 'ADMIN_USERS_MANAGEMENT'), PermissionIsMenuItem = 1 WHERE PermissionCode = 'ADMIN_ROLES';
        -- 3.3 Permissions (child of Roles & Permissions)
        UPDATE Permissions SET PermissionSortIndex = 3, PermissionParentID = (SELECT PermissionID FROM Permissions WHERE PermissionCode = 'ADMIN_USERS_MANAGEMENT'), PermissionIsMenuItem = 1 WHERE PermissionCode = 'ADMIN_PERMISSIONS';
        -- 3.4 Roles-Permissions (child of Roles & Permissions)
        UPDATE Permissions SET PermissionSortIndex = 4, PermissionParentID = (SELECT PermissionID FROM Permissions WHERE PermissionCode = 'ADMIN_USERS_MANAGEMENT'), PermissionIsMenuItem = 1 WHERE PermissionCode = 'ADMIN_ROLES_PERMISSIONS';

        -- 4. Activity Log (top-level, sort=4)
        UPDATE Permissions SET PermissionSortIndex = 4, PermissionParentID = NULL, PermissionIsMenuItem = 1, PermissionMenuIcon = 'mdi-history' WHERE PermissionCode = 'ADMIN_ORDERS';

        -- 5. Gift Lists (top-level, sort=5)
        UPDATE Permissions SET PermissionSortIndex = 5, PermissionParentID = NULL, PermissionIsMenuItem = 1, PermissionMenuIcon = 'mdi-format-list-bulleted' WHERE PermissionCode = 'ADMIN_GIFTLISTS';

        -- 6. Gifts (top-level, sort=6)
        UPDATE Permissions SET PermissionSortIndex = 6, PermissionParentID = NULL, PermissionIsMenuItem = 1, PermissionMenuIcon = 'mdi-gift' WHERE PermissionCode = 'ADMIN_GIFTS';

        -- Hidden items: NOT in the sidebar (access control only)
        UPDATE Permissions SET PermissionIsMenuItem = 0 WHERE PermissionCode = 'ADMIN_PAGES';
        UPDATE Permissions SET PermissionIsMenuItem = 0 WHERE PermissionCode = 'ADMIN_REDIRECTS';
        UPDATE Permissions SET PermissionIsMenuItem = 0 WHERE PermissionCode = 'ADMIN_PAGE_MANAGEMENT';

        PRINT 'Menu structure updated successfully (fresh database)';
    END
    ELSE
        PRINT 'Legacy database detected — menu structure managed by 004_fix_menu_structure.sql';

END TRY
BEGIN CATCH
    PRINT 'Error: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
