-- 004_seed_superadmin.sql
-- Idempotent script to create a Superadmin user (linked to Admin role).
-- WARNING: This inserts a user with password 'asdf' (matching SetupController example).
-- Change the password after first login or adapt to your hashing requirements.

SET XACT_ABORT ON;
BEGIN TRY
    IF NOT EXISTS (SELECT 1 FROM Users WHERE UserEmail = 'admin@gifter.com')
    BEGIN
        INSERT INTO Users (UserEmail, UserPassword, UserFirstname, UserLastname, UserFullname, UserIsActive, RoleID)
        VALUES (
            'admin@gifter.com', -- email
            'asdf',             -- password (plain text in DB example; adapt to your hashing if necessary)
            'System',
            'Administrator',
            'System Administrator',
            1,
            (SELECT RoleID FROM Roles WHERE RoleName = 'Admin')
        );
        PRINT 'Inserted Superadmin user admin@gifter.com (password: asdf). Please change the password after first login.';
    END
    ELSE
    BEGIN
        PRINT 'Superadmin user admin@gifter.com already exists';
    END
END TRY
BEGIN CATCH
    PRINT 'Error: ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO
