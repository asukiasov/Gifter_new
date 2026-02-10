CREATE FUNCTION [dbo].[UsersGetSingleByGoogleID](@GoogleID NVARCHAR(255))
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
        WHERE UserGoogleID = @GoogleID
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    )
END;
