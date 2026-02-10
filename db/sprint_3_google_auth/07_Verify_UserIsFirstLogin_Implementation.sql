-- =============================================
-- Verification Script for UserIsFirstLogin Implementation
-- =============================================

PRINT 'Checking table [dbo].[Users] for column [UserIsFirstLogin]...'
IF EXISTS (
    SELECT 1 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID('[dbo].[Users]') 
    AND name = 'UserIsFirstLogin'
)
BEGIN
    PRINT 'SUCCESS: [UserIsIsFirstLogin] column exists in [dbo].[Users].'
    
    -- Check default value
    DECLARE @DefaultValue NVARCHAR(MAX)
    SELECT @DefaultValue = definition 
    FROM sys.default_constraints 
    WHERE parent_object_id = OBJECT_ID('[dbo].[Users]') 
    AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[Users]') AND name = 'UserIsFirstLogin')
    
    PRINT 'Default constraint: ' + ISNULL(@DefaultValue, 'None found')
END
ELSE
BEGIN
    PRINT 'FAILURE: [UserIsIsFirstLogin] column DOES NOT exist in [dbo].[Users].'
END
GO

PRINT ''
PRINT 'Checking Functions for UserIsFirstLogin inclusion...'

-- Test function UsersGetSingleByEmail (if exists)
IF OBJECT_ID('[dbo].[UsersGetSingleByEmail]', 'FN') IS NOT NULL
BEGIN
    PRINT 'Testing [dbo].[UsersGetSingleByEmail] output...'
    -- We assume there's at least one user or we just check if it compiles and runs
    -- Note: This is an inline check, it might return NULL if no user exists, but we want to see the JSON structure
    DECLARE @SampleEmail NVARCHAR(255) = (SELECT TOP 1 UserEmail FROM [dbo].[Users])
    IF @SampleEmail IS NOT NULL
    BEGIN
        DECLARE @Result NVARCHAR(MAX) = [dbo].[UsersGetSingleByEmail](@SampleEmail)
        IF @Result LIKE '%UserIsFirstLogin%'
        BEGIN
            PRINT 'SUCCESS: [dbo].[UsersGetSingleByEmail] returns [UserIsFirstLogin].'
            PRINT 'Sample output: ' + LEFT(@Result, 200) + '...'
        END
        ELSE
        BEGIN
            PRINT 'FAILURE: [dbo].[UsersGetSingleByEmail] DOES NOT return [UserIsFirstLogin].'
        END
    END
    ELSE
    BEGIN
        PRINT 'WARNING: No users found in table to test function [dbo].[UsersGetSingleByEmail].'
    END
END
ELSE
BEGIN
    PRINT 'FAILURE: Function [dbo].[UsersGetSingleByEmail] NOT FOUND.'
END
GO

-- Test function UsersGetSingleByGoogleID (if exists)
IF OBJECT_ID('[dbo].[UsersGetSingleByGoogleID]', 'FN') IS NOT NULL
BEGIN
    PRINT 'Testing [dbo].[UsersGetSingleByGoogleID] output...'
    DECLARE @SampleGoogleID NVARCHAR(255) = (SELECT TOP 1 UserGoogleID FROM [dbo].[Users] WHERE UserGoogleID IS NOT NULL)
    IF @SampleGoogleID IS NOT NULL
    BEGIN
        DECLARE @ResultGoogleID NVARCHAR(MAX) = [dbo].[UsersGetSingleByGoogleID](@SampleGoogleID)
        IF @ResultGoogleID LIKE '%UserIsFirstLogin%'
        BEGIN
            PRINT 'SUCCESS: [dbo].[UsersGetSingleByGoogleID] returns [UserIsFirstLogin].'
        END
        ELSE
        BEGIN
            PRINT 'FAILURE: [dbo].[UsersGetSingleByGoogleID] DOES NOT return [UserIsFirstLogin].'
        END
    END
    ELSE
    BEGIN
        PRINT 'NOTE: No users with GoogleID found to test [dbo].[UsersGetSingleByGoogleID].'
    END
END
GO

-- Test function UsersGetSingleByID (if exists)
IF OBJECT_ID('[dbo].[UsersGetSingleByID]', 'FN') IS NOT NULL
BEGIN
    PRINT 'Testing [dbo].[UsersGetSingleByID] output...'
    DECLARE @SampleUserID INT = (SELECT TOP 1 UserID FROM [dbo].[Users])
    IF @SampleUserID IS NOT NULL
    BEGIN
        DECLARE @ResultID NVARCHAR(MAX) = [dbo].[UsersGetSingleByID](@SampleUserID)
        IF @ResultID LIKE '%UserIsFirstLogin%'
        BEGIN
            PRINT 'SUCCESS: [dbo].[UsersGetSingleByID] returns [UserIsFirstLogin].'
        END
        ELSE
        BEGIN
            PRINT 'FAILURE: [dbo].[UsersGetSingleByID] DOES NOT return [UserIsFirstLogin].'
        END
    END
END
GO
