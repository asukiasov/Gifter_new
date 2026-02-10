-- Scalar-Valued Function to get a single GiftList by ID
-- Returns JSON matching GiftListsListDTO properties
CREATE FUNCTION [dbo].[GiftListsGetSingleByID](@GiftListID INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN (
        SELECT
            gl.GiftListID,
            gl.GiftListUserID,
            gl.GiftListTitle,
            gl.GiftListDescription,
            gl.GiftListOccasionType,
            gl.GiftListIsSecret,
            gl.GiftListIsPublished,
            gl.GiftListEndDate,
            gl.GiftListDateCreated,
            u.UserFirstname + ' ' + u.UserLastname AS OwnerFullname
        FROM [dbo].[tblGiftLists] gl
        INNER JOIN [dbo].[Users] u ON gl.GiftListUserID = u.UserID
        WHERE gl.GiftListID = @GiftListID
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    )
END;
