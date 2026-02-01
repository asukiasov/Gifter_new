-- 001_seed_roles.sql
-- Idempotent script to ensure Admin role exists.
-- Adjust database context if needed before running.

SET XACT_ABORT ON;
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Admin')
    BEGIN
        INSERT INTO Roles (RoleName, RoleCode, RoleIsActive)
        VALUES ('Admin', 'ADMIN', 1);
        PRINT 'Inserted Admin role';
    END
    ELSE
    BEGIN
        PRINT 'Admin role already exists';
    END
END TRY
BEGIN CATCH
    PRINT 'Error: ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO
