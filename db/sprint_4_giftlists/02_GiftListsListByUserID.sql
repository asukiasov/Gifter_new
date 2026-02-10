-- Table-Valued Function to get GiftLists by UserID
-- Column names must match GiftListsListDTO properties exactly (PascalCase)
CREATE FUNCTION [dbo].[GiftListsListByUserID](@UserID INT)
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
    INNER JOIN [dbo].[Users] u ON gl.GiftListUserID = u.UserID
    WHERE gl.GiftListUserID = @UserID;
