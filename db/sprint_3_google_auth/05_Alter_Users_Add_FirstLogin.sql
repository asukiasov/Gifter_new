-- Add UserIsFirstLogin column to Users table
-- Default to 1 (true) for new users, existing users will also be set to 1
ALTER TABLE [dbo].[Users]
ADD [UserIsFirstLogin] BIT NOT NULL DEFAULT 1;
GO
