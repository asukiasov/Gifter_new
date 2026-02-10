-- Sprint 3: Allow NULL passwords for Google OAuth users
-- Google OAuth users authenticate via Google, so they don't need passwords

ALTER TABLE Users ALTER COLUMN UserPassword NVARCHAR(255) NULL;
GO
