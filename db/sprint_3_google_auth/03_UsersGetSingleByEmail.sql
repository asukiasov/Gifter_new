CREATE FUNCTION [dbo].[UsersGetSingleByEmail](@Email NVARCHAR(255))
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
            RoleID
        FROM [dbo].[Users]
        WHERE UserEmail = @Email
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    )
END;
