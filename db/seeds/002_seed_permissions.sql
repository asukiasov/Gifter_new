-- 002_seed_permissions.sql
-- Idempotent script to insert key admin Permissions (page & catch-all entries).
-- Expanded to cover all detected admin controllers.

SET XACT_ABORT ON;
BEGIN TRY
    -- Dashboard
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

    -- Users
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

    -- Roles & Permissions
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/roles')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/roles', 'ADMIN_ROLES', 'Roles', 1, 'mdi-account-key');
        PRINT 'Inserted /admin/roles permission';
    END
    ELSE
        PRINT '/admin/roles permission exists';

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/permissions')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/permissions', 'ADMIN_PERMISSIONS', 'Permissions', 1, 'mdi-shield-key');
        PRINT 'Inserted /admin/permissions permission';
    END
    ELSE
        PRINT '/admin/permissions permission exists';

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/rolespermissions')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/rolespermissions', 'ADMIN_ROLES_PERMISSIONS', 'Roles & Permissions', 0, NULL);
        PRINT 'Inserted /admin/rolespermissions permission';
    END
    ELSE
        PRINT '/admin/rolespermissions permission exists';

    -- Orders
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/orders')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/orders', 'ADMIN_ORDERS', 'Orders', 1, 'mdi-cart');
        PRINT 'Inserted /admin/orders permission';
    END
    ELSE
        PRINT '/admin/orders permission exists';

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/orders/.')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/orders/.', 'ADMIN_ORDERS_CATCHALL', 'Orders (catch-all)', 0, NULL);
        PRINT 'Inserted /admin/orders/. permission (catch-all)';
    END
    ELSE
        PRINT '/admin/orders/. permission exists';

    -- GiftLists
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

    -- Gifts
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

    -- CMS: Pages, Redirects, Menu
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/pages')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/pages', 'ADMIN_PAGES', 'Pages (CMS)', 1, 'mdi-file-document');
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

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/redirects')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/redirects', 'ADMIN_REDIRECTS', 'Redirects', 1, 'mdi-share-variant');
        PRINT 'Inserted /admin/redirects permission';
    END
    ELSE
        PRINT '/admin/redirects permission exists';

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/menuheader')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/menuheader', 'ADMIN_MENU_HEADER', 'Menu Header', 0, NULL);
        PRINT 'Inserted /admin/menuheader permission';
    END
    ELSE
        PRINT '/admin/menuheader permission exists';

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/menufooter')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/menufooter', 'ADMIN_MENU_FOOTER', 'Menu Footer', 0, NULL);
        PRINT 'Inserted /admin/menufooter permission';
    END
    ELSE
        PRINT '/admin/menufooter permission exists';

    -- Products and categories
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/products')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/products', 'ADMIN_PRODUCTS', 'Products', 1, 'mdi-package-variant');
        PRINT 'Inserted /admin/products permission';
    END
    ELSE
        PRINT '/admin/products permission exists';

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/productcategories')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/productcategories', 'ADMIN_PRODUCT_CATEGORIES', 'Product Categories', 1, 'mdi-shape');
        PRINT 'Inserted /admin/productcategories permission';
    END
    ELSE
        PRINT '/admin/productcategories permission exists';

    -- Brands
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/brands')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/brands', 'ADMIN_BRANDS', 'Brands', 1, 'mdi-tag');
        PRINT 'Inserted /admin/brands permission';
    END
    ELSE
        PRINT '/admin/brands permission exists';

    -- News & Blog
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/news')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/news', 'ADMIN_NEWS', 'News', 1, 'mdi-newspaper-variant');
        PRINT 'Inserted /admin/news permission';
    END
    ELSE
        PRINT '/admin/news permission exists';

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/blog')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/blog', 'ADMIN_BLOG', 'Blog Posts', 1, 'mdi-rss');
        PRINT 'Inserted /admin/blog permission';
    END
    ELSE
        PRINT '/admin/blog permission exists';

    -- Email templates & system props
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/emailtemplates')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/emailtemplates', 'ADMIN_EMAIL_TEMPLATES', 'Email Templates', 0, 'mdi-email');
        PRINT 'Inserted /admin/emailtemplates permission';
    END
    ELSE
        PRINT '/admin/emailtemplates permission exists';

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/systemproperties')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/systemproperties', 'ADMIN_SYSTEM_PROPERTIES', 'System Properties', 0, 'mdi-cog');
        PRINT 'Inserted /admin/systemproperties permission';
    END
    ELSE
        PRINT '/admin/systemproperties permission exists';

    -- File manager
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/filemanager')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/filemanager', 'ADMIN_FILE_MANAGER', 'File Manager', 0, 'mdi-folder');
        PRINT 'Inserted /admin/filemanager permission';
    END
    ELSE
        PRINT '/admin/filemanager permission exists';

    -- Dictionaries, TeamMembers, ErrorLog
    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/dictionaries')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/dictionaries', 'ADMIN_DICTIONARIES', 'Dictionaries', 0, 'mdi-dictionary');
        PRINT 'Inserted /admin/dictionaries permission';
    END
    ELSE
        PRINT '/admin/dictionaries permission exists';

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/teammembers')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/teammembers', 'ADMIN_TEAM_MEMBERS', 'Team Members', 0, 'mdi-account-group');
        PRINT 'Inserted /admin/teammembers permission';
    END
    ELSE
        PRINT '/admin/teammembers permission exists';

    IF NOT EXISTS (SELECT 1 FROM Permissions WHERE PermissionPagePath = '/admin/errorlog')
    BEGIN
        INSERT INTO Permissions (PermissionPagePath, PermissionCode, PermissionCaption, PermissionIsMenuItem, PermissionMenuIcon)
        VALUES ('/admin/errorlog', 'ADMIN_ERROR_LOG', 'Error Log', 0, 'mdi-bug');
        PRINT 'Inserted /admin/errorlog permission';
    END
    ELSE
        PRINT '/admin/errorlog permission exists';

END TRY
BEGIN CATCH
    PRINT 'Error: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
