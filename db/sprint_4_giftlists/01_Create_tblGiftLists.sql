-- Create GiftLists table
CREATE TABLE [dbo].[tblGiftLists] (
    GiftListID INT IDENTITY(1,1) PRIMARY KEY,
    GiftListUserID INT NOT NULL,
    GiftListTitle NVARCHAR(255) NOT NULL,
    GiftListDescription NVARCHAR(MAX) NULL,
    GiftListOccasionType NVARCHAR(100) NULL,
    GiftListIsSecret BIT NOT NULL DEFAULT 0,
    GiftListIsPublished BIT NOT NULL DEFAULT 0,
    GiftListEndDate DATETIME NULL,
    GiftListDateCreated DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_GiftLists_Users FOREIGN KEY (GiftListUserID) REFERENCES [dbo].[Users](UserID)
);
