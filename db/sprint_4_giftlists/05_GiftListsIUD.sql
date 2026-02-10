-- Stored Procedure for GiftLists Insert/Update/Delete
-- Action: 0 = INSERT, 1 = UPDATE, 2 = DELETE
CREATE PROCEDURE [dbo].[GiftListsIUD]
(
    @Action TINYINT,
    @GiftListID INT OUTPUT,
    @GiftListJson NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- INSERT
    IF @Action = 0
    BEGIN
        INSERT INTO [dbo].[tblGiftLists] (
            GiftListUserID,
            GiftListTitle,
            GiftListDescription,
            GiftListOccasionType,
            GiftListIsSecret,
            GiftListIsPublished,
            GiftListEndDate,
            GiftListDateCreated
        )
        VALUES (
            JSON_VALUE(@GiftListJson, '$.GiftListUserID'),
            JSON_VALUE(@GiftListJson, '$.GiftListTitle'),
            JSON_VALUE(@GiftListJson, '$.GiftListDescription'),
            JSON_VALUE(@GiftListJson, '$.GiftListOccasionType'),
            CASE WHEN JSON_VALUE(@GiftListJson, '$.GiftListIsSecret') = 'true' THEN 1 ELSE 0 END,
            CASE WHEN JSON_VALUE(@GiftListJson, '$.GiftListIsPublished') = 'true' THEN 1 ELSE 0 END,
            JSON_VALUE(@GiftListJson, '$.GiftListEndDate'),
            GETDATE()
        );

        SET @GiftListID = SCOPE_IDENTITY();
    END

    -- UPDATE
    IF @Action = 1
    BEGIN
        UPDATE [dbo].[tblGiftLists]
        SET
            GiftListTitle = COALESCE(JSON_VALUE(@GiftListJson, '$.GiftListTitle'), GiftListTitle),
            GiftListDescription = COALESCE(JSON_VALUE(@GiftListJson, '$.GiftListDescription'), GiftListDescription),
            GiftListOccasionType = COALESCE(JSON_VALUE(@GiftListJson, '$.GiftListOccasionType'), GiftListOccasionType),
            GiftListIsSecret = CASE
                WHEN JSON_VALUE(@GiftListJson, '$.GiftListIsSecret') IS NOT NULL
                THEN CASE WHEN JSON_VALUE(@GiftListJson, '$.GiftListIsSecret') = 'true' THEN 1 ELSE 0 END
                ELSE GiftListIsSecret
            END,
            GiftListIsPublished = CASE
                WHEN JSON_VALUE(@GiftListJson, '$.GiftListIsPublished') IS NOT NULL
                THEN CASE WHEN JSON_VALUE(@GiftListJson, '$.GiftListIsPublished') = 'true' THEN 1 ELSE 0 END
                ELSE GiftListIsPublished
            END,
            GiftListEndDate = COALESCE(JSON_VALUE(@GiftListJson, '$.GiftListEndDate'), GiftListEndDate)
        WHERE GiftListID = @GiftListID;
    END

    -- DELETE
    IF @Action = 2
    BEGIN
        DELETE FROM [dbo].[tblGiftLists]
        WHERE GiftListID = @GiftListID;
    END
END;
