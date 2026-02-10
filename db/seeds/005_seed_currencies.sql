-- 005 — Dictionaries infrastructure + Currency seed data
-- Run each script SEPARATELY on SmarterASP (no GO support).
-- Order: Table → IUD proc → DeleteRecursive proc → DictionariesList func → DictionariesListByLevelCodeIsVisible func → Seed data

-- ============================================================
-- Script 1 — Dictionaries table
-- ============================================================

CREATE TABLE [dbo].[Dictionaries] (
    [DictionaryID]           INT            IDENTITY(1,1) NOT NULL,
    [DictionaryParentID]     INT            NULL,
    [DictionaryCaption]      NVARCHAR(255)  NOT NULL,
    [DictionaryCaptionEng]   NVARCHAR(255)  NULL,
    [DictionaryLevel]        INT            NULL DEFAULT 1,
    [DictionaryStringCode]   NVARCHAR(100)  NULL,
    [DictionaryIntCode]      INT            NULL,
    [DictionaryDecimalValue] DECIMAL(18,4)  NULL,
    [DictionaryCode]         INT            NULL,
    [DictionaryIsDefault]    BIT            NOT NULL DEFAULT 0,
    [DictionaryIsVisible]    BIT            NOT NULL DEFAULT 1,
    [DictionarySortIndex]    INT            NULL DEFAULT 0,
    [DictionaryDateCreated]  DATETIME       NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY ([DictionaryID]),
    FOREIGN KEY ([DictionaryParentID]) REFERENCES [dbo].[Dictionaries]([DictionaryID])
);

-- ============================================================
-- Script 2 — DictionariesIUD stored procedure
-- ============================================================

CREATE OR ALTER PROCEDURE [dbo].[DictionariesIUD]
(
    @Action         TINYINT,
    @DictionaryID   INT OUTPUT,
    @DictionaryJson NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 0 -- INSERT
    BEGIN
        INSERT INTO [dbo].[Dictionaries] (
            DictionaryParentID, DictionaryCaption, DictionaryCaptionEng,
            DictionaryCode, DictionaryIntCode, DictionaryStringCode,
            DictionaryDecimalValue, DictionaryIsVisible, DictionaryIsDefault,
            DictionarySortIndex, DictionaryLevel
        )
        VALUES (
            CAST(JSON_VALUE(@DictionaryJson, '$.DictionaryParentID') AS INT),
            JSON_VALUE(@DictionaryJson, '$.DictionaryCaption'),
            JSON_VALUE(@DictionaryJson, '$.DictionaryCaptionEng'),
            CAST(JSON_VALUE(@DictionaryJson, '$.DictionaryCode') AS INT),
            CAST(JSON_VALUE(@DictionaryJson, '$.DictionaryIntCode') AS INT),
            JSON_VALUE(@DictionaryJson, '$.DictionaryStringCode'),
            CAST(JSON_VALUE(@DictionaryJson, '$.DictionaryDecimalValue') AS DECIMAL(18,4)),
            CASE WHEN JSON_VALUE(@DictionaryJson, '$.DictionaryIsVisible') = 'true' THEN 1 ELSE 0 END,
            CASE WHEN JSON_VALUE(@DictionaryJson, '$.DictionaryIsDefault') = 'true' THEN 1 ELSE 0 END,
            COALESCE(CAST(JSON_VALUE(@DictionaryJson, '$.DictionarySortIndex') AS INT), 0),
            COALESCE(CAST(JSON_VALUE(@DictionaryJson, '$.DictionaryLevel') AS INT), 1)
        );
        SET @DictionaryID = SCOPE_IDENTITY();
    END
    ELSE IF @Action = 1 -- UPDATE
    BEGIN
        UPDATE [dbo].[Dictionaries]
        SET
            DictionaryParentID   = CASE WHEN JSON_VALUE(@DictionaryJson, '$.DictionaryParentID') IS NOT NULL THEN CAST(JSON_VALUE(@DictionaryJson, '$.DictionaryParentID') AS INT) ELSE DictionaryParentID END,
            DictionaryCaption    = COALESCE(JSON_VALUE(@DictionaryJson, '$.DictionaryCaption'), DictionaryCaption),
            DictionaryCaptionEng = COALESCE(JSON_VALUE(@DictionaryJson, '$.DictionaryCaptionEng'), DictionaryCaptionEng),
            DictionaryCode       = COALESCE(CAST(JSON_VALUE(@DictionaryJson, '$.DictionaryCode') AS INT), DictionaryCode),
            DictionaryIntCode    = CASE WHEN JSON_VALUE(@DictionaryJson, '$.DictionaryIntCode') IS NOT NULL THEN CAST(JSON_VALUE(@DictionaryJson, '$.DictionaryIntCode') AS INT) ELSE DictionaryIntCode END,
            DictionaryStringCode = COALESCE(JSON_VALUE(@DictionaryJson, '$.DictionaryStringCode'), DictionaryStringCode),
            DictionaryDecimalValue = CASE WHEN JSON_VALUE(@DictionaryJson, '$.DictionaryDecimalValue') IS NOT NULL THEN CAST(JSON_VALUE(@DictionaryJson, '$.DictionaryDecimalValue') AS DECIMAL(18,4)) ELSE DictionaryDecimalValue END,
            DictionaryIsVisible  = CASE
                WHEN JSON_VALUE(@DictionaryJson, '$.DictionaryIsVisible') IS NOT NULL
                THEN CASE WHEN JSON_VALUE(@DictionaryJson, '$.DictionaryIsVisible') = 'true' THEN 1 ELSE 0 END
                ELSE DictionaryIsVisible
            END,
            DictionaryIsDefault  = CASE
                WHEN JSON_VALUE(@DictionaryJson, '$.DictionaryIsDefault') IS NOT NULL
                THEN CASE WHEN JSON_VALUE(@DictionaryJson, '$.DictionaryIsDefault') = 'true' THEN 1 ELSE 0 END
                ELSE DictionaryIsDefault
            END,
            DictionarySortIndex  = COALESCE(CAST(JSON_VALUE(@DictionaryJson, '$.DictionarySortIndex') AS INT), DictionarySortIndex)
        WHERE DictionaryID = @DictionaryID;
    END
    ELSE IF @Action = 2 -- DELETE
    BEGIN
        DELETE FROM [dbo].[Dictionaries] WHERE DictionaryID = @DictionaryID;
    END
END;

-- ============================================================
-- Script 3 — DictionariesDeleteRecursive stored procedure
-- ============================================================

CREATE OR ALTER PROCEDURE [dbo].[DictionariesDeleteRecursive]
(
    @DictionaryID INT
)
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #ToDelete (DictionaryID INT);

    ;WITH CTE AS (
        SELECT DictionaryID FROM [dbo].[Dictionaries] WHERE DictionaryID = @DictionaryID
        UNION ALL
        SELECT d.DictionaryID FROM [dbo].[Dictionaries] d INNER JOIN CTE c ON d.DictionaryParentID = c.DictionaryID
    )
    INSERT INTO #ToDelete SELECT DictionaryID FROM CTE;

    DELETE FROM [dbo].[Dictionaries] WHERE DictionaryID IN (SELECT DictionaryID FROM #ToDelete) AND DictionaryID <> @DictionaryID;
    DELETE FROM [dbo].[Dictionaries] WHERE DictionaryID = @DictionaryID;

    DROP TABLE #ToDelete;
END;

-- ============================================================
-- Script 4 — DictionariesList table-valued function
-- ============================================================

CREATE FUNCTION [dbo].[DictionariesList]()
RETURNS TABLE
AS
RETURN
    SELECT
        DictionaryID, DictionaryParentID, DictionaryCaption, DictionaryCaptionEng,
        DictionaryLevel, DictionaryStringCode, DictionaryIntCode, DictionaryDecimalValue,
        DictionaryCode, DictionaryIsDefault, DictionaryIsVisible, DictionarySortIndex,
        DictionaryDateCreated
    FROM [dbo].[Dictionaries];

-- ============================================================
-- Script 5 — DictionariesListByLevelCodeIsVisible table-valued function
-- ============================================================

CREATE FUNCTION [dbo].[DictionariesListByLevelCodeIsVisible]
(
    @DictionaryLevel     INT,
    @DictionaryCode      INT,
    @DictionaryIsVisible BIT
)
RETURNS TABLE
AS
RETURN
    SELECT
        DictionaryID, DictionaryParentID, DictionaryCaption, DictionaryCaptionEng,
        DictionaryLevel, DictionaryStringCode, DictionaryIntCode, DictionaryDecimalValue,
        DictionaryCode, DictionaryIsDefault, DictionaryIsVisible, DictionarySortIndex,
        DictionaryDateCreated
    FROM [dbo].[Dictionaries]
    WHERE (@DictionaryLevel IS NULL OR DictionaryLevel = @DictionaryLevel)
      AND (@DictionaryCode  IS NULL OR DictionaryCode  = @DictionaryCode)
      AND (@DictionaryIsVisible IS NULL OR DictionaryIsVisible = @DictionaryIsVisible);

-- ============================================================
-- Script 6 — Seed currency data (DictionaryCode = 4)
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM Dictionaries WHERE DictionaryCode = 4 AND DictionaryStringCode = 'GEL')
    INSERT INTO Dictionaries (DictionaryCaption, DictionaryCaptionEng, DictionaryLevel, DictionaryStringCode, DictionaryIntCode, DictionaryCode, DictionaryIsDefault, DictionaryIsVisible, DictionarySortIndex)
    VALUES ('GEL', 'GEL', 1, 'GEL', 1, 4, 1, 1, 1);

IF NOT EXISTS (SELECT 1 FROM Dictionaries WHERE DictionaryCode = 4 AND DictionaryStringCode = 'USD')
    INSERT INTO Dictionaries (DictionaryCaption, DictionaryCaptionEng, DictionaryLevel, DictionaryStringCode, DictionaryIntCode, DictionaryCode, DictionaryIsDefault, DictionaryIsVisible, DictionarySortIndex)
    VALUES ('USD', 'USD', 1, 'USD', 2, 4, 0, 1, 2);

IF NOT EXISTS (SELECT 1 FROM Dictionaries WHERE DictionaryCode = 4 AND DictionaryStringCode = 'EUR')
    INSERT INTO Dictionaries (DictionaryCaption, DictionaryCaptionEng, DictionaryLevel, DictionaryStringCode, DictionaryIntCode, DictionaryCode, DictionaryIsDefault, DictionaryIsVisible, DictionarySortIndex)
    VALUES ('EUR', 'EUR', 1, 'EUR', 3, 4, 0, 1, 3);
