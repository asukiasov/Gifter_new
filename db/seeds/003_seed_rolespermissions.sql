-- 003_seed_rolespermissions.sql
-- Link seeded permissions to the Admin role (RoleName = 'Admin').
-- Idempotent: inserts role-permission rows only if not present.
-- Updated: 2026-02-04 — Handles both fresh and legacy databases

SET XACT_ABORT ON;
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Admin')
    BEGIN
        PRINT 'Admin role not found. Run 001_seed_roles.sql first.';
    END
    ELSE
    BEGIN
        DECLARE @RoleID INT = (SELECT RoleID FROM Roles WHERE RoleName = 'Admin');

        -- Link permissions by page path (idempotent set-based operation)
        DECLARE @Paths TABLE (Path NVARCHAR(255));
        INSERT INTO @Paths (Path) VALUES
        ('/admin'),('/admin/.'),('/admin/users'),('/admin/users/.'),
        ('/admin/roles'),('/admin/permissions'),('/admin/rolespermissions'),
        ('/admin/orders'),('/admin/orders/.'),
        ('/admin/giftlists'),('/admin/giftlists/.'),('/admin/gifts'),('/admin/gifts/.'),
        ('/admin/pages'),('/admin/pages/.'),('/admin/page-management'),
        ('/admin/redirects'),('/admin/menu-header'),('/admin/menu-footer'),
        ('/admin/emailtemplates'),('/admin/systemproperties'),
        ('/admin/filemanager'),('/admin/dictionaries'),('/admin/errorlog');

        INSERT INTO RolesPermissions (RoleID, PermissionID)
        SELECT @RoleID, p.PermissionID
        FROM Permissions p
        INNER JOIN @Paths ps ON p.PermissionPagePath = ps.Path
        WHERE NOT EXISTS (SELECT 1 FROM RolesPermissions rp WHERE rp.RoleID = @RoleID AND rp.PermissionID = p.PermissionID);

        DECLARE @InsertedByPath INT = @@ROWCOUNT;

        -- Link parent group permissions (NULL page paths, matched by code)
        -- Handles both fresh DBs (ADMIN_SYSTEM) and legacy DBs (code normalized by 002)
        INSERT INTO RolesPermissions (RoleID, PermissionID)
        SELECT @RoleID, p.PermissionID
        FROM Permissions p
        WHERE p.PermissionPagePath IS NULL
        AND p.PermissionCode IN ('ADMIN_SYSTEM', 'ADMIN_USERS_MANAGEMENT')
        AND NOT EXISTS (SELECT 1 FROM RolesPermissions rp WHERE rp.RoleID = @RoleID AND rp.PermissionID = p.PermissionID);

        DECLARE @InsertedByCode INT = @@ROWCOUNT;

        -- Report newly linked count
        PRINT CONCAT('Linked ', @InsertedByPath + @InsertedByCode, ' permission(s) to Admin role (new links added).');

        -- Show missing permission paths (if any)
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
