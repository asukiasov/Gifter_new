-- 003_seed_rolespermissions.sql
-- Link seeded permissions to the Admin role (RoleName = 'Admin').
-- Idempotent: inserts role-permission rows only if not present.

SET XACT_ABORT ON;
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Admin')
    BEGIN
        PRINT 'Admin role not found. Run 001_seed_roles.sql first.';
    END
    ELSE
    BEGIN
        DECLARE @RoleID INT = (SELECT RoleID FROM Roles WHERE RoleName = 'Admin');

        DECLARE @PermID INT;

        -- Link a curated list of admin permissions to the Admin role (idempotent set-based operation)
        DECLARE @Paths TABLE (Path NVARCHAR(255));
        INSERT INTO @Paths (Path) VALUES
        ('/admin'),('/admin/.'),('/admin/users'),('/admin/users/.'),
        ('/admin/roles'),('/admin/permissions'),('/admin/rolespermissions'),
        ('/admin/orders'),('/admin/orders/.'),('/admin/giftlists'),('/admin/giftlists/.'),('/admin/gifts'),('/admin/gifts/.'),
        ('/admin/pages'),('/admin/pages/.'),('/admin/redirects'),('/admin/menuheader'),('/admin/menufooter'),
        ('/admin/products'),('/admin/productcategories'),('/admin/brands'),
        ('/admin/news'),('/admin/blog'),('/admin/emailtemplates'),('/admin/systemproperties'),
        ('/admin/filemanager'),('/admin/dictionaries'),('/admin/teammembers'),('/admin/errorlog');

        INSERT INTO RolesPermissions (RoleID, PermissionID)
        SELECT @RoleID, p.PermissionID
        FROM Permissions p
        INNER JOIN @Paths ps ON p.PermissionPagePath = ps.Path
        WHERE NOT EXISTS (SELECT 1 FROM RolesPermissions rp WHERE rp.RoleID = @RoleID AND rp.PermissionID = p.PermissionID);

        -- Report newly linked count
        DECLARE @InsertedCount INT = @@ROWCOUNT;
        PRINT CONCAT('Linked ', @InsertedCount, ' permission(s) to Admin role (new links added).');

        -- Show missing permission paths (if any) so maintainer can add them to 002_seed_permissions.sql
        IF EXISTS (SELECT 1 FROM @Paths ps WHERE NOT EXISTS (SELECT 1 FROM Permissions p WHERE p.PermissionPagePath = ps.Path))
        BEGIN
            PRINT 'Warning: Some permission paths from the list were not found in Permissions table. Check 002_seed_permissions.sql or add missing entries.';
            SELECT Path FROM @Paths ps WHERE NOT EXISTS (SELECT 1 FROM Permissions p WHERE p.PermissionPagePath = ps.Path);
        END
    END
END TRY
BEGIN CATCH
    PRINT 'Error: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
