-- Update UsersGetSingleByGoogleID to include UserIsFirstLogin
ALTER FUNCTION [dbo].[UsersGetSingleByGoogleID](@GoogleID NVARCHAR(255))
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN (
        SELECT
            UserID,
            UserFullname,
            UserFirstname,
            UserLastname,
            UserBirthdate,
            UserEmail,
            UserPhoneNumberMobile,
            UserIsActive,
            UserAvatarFilename,
            UserDateCreated,
            UserGoogleID,
            UserIsFirstLogin,
            RoleID
        FROM [dbo].[Users]
        WHERE UserGoogleID = @GoogleID
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    )
END;
GO

-- Update UsersGetSingleByEmail to include UserIsFirstLogin
ALTER FUNCTION [dbo].[UsersGetSingleByEmail](@Email NVARCHAR(255))
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN (
        SELECT
            UserID,
            UserFullname,
            UserFirstname,
            UserLastname,
            UserBirthdate,
            UserEmail,
            UserPhoneNumberMobile,
            UserIsActive,
            UserAvatarFilename,
            UserDateCreated,
            UserGoogleID,
            UserIsFirstLogin,
            RoleID
        FROM [dbo].[Users]
        WHERE UserEmail = @Email
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    )
END;
GO

-- Update UsersGetSingleByID to include UserIsFirstLogin (if it exists)
-- Note: Check if this function exists in your database and update accordingly
IF OBJECT_ID('dbo.UsersGetSingleByID', 'FN') IS NOT NULL
BEGIN
    EXEC('
    ALTER FUNCTION [dbo].[UsersGetSingleByID](@UserID INT)
    RETURNS NVARCHAR(MAX)
    AS
    BEGIN
        RETURN (
            SELECT
                u.UserID,
                u.UserFullname,
                u.UserFirstname,
                u.UserLastname,
                u.UserBirthdate,
                u.UserEmail,
                u.UserPhoneNumberMobile,
                u.UserIsActive,
                u.UserAvatarFilename,
                u.UserDateCreated,
                u.UserGoogleID,
                u.UserIsFirstLogin,
                u.RoleID,
                r.RoleCode,
                r.RoleName,
                (
                    SELECT p.PermissionID, p.PermissionParentID, p.PermissionCode, p.PermissionCodeName, p.PermissionCaption, p.PermissionPagePath
                    FROM RolesPermissions rp
                    INNER JOIN Permissions p ON p.PermissionID = rp.PermissionID
                    WHERE rp.RoleID = u.RoleID
                    FOR JSON PATH
                ) AS Permissions
            FROM Users u
            LEFT JOIN Roles r ON r.RoleID = u.RoleID
            WHERE u.UserID = @UserID
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        )
    END
    ')
END
GO
