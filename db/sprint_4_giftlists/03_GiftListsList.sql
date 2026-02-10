-- Table-Valued Function to get all GiftLists
-- Column names must match GiftListsListDTO properties exactly (PascalCase)
CREATE FUNCTION [dbo].[GiftListsList]()
RETURNS TABLE
AS
RETURN
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
    INNER JOIN [dbo].[Users] u ON gl.GiftListUserID = u.UserID;
