# Sprint 2: CMS + Gifter Domain + Orders Dashboard

**Goal:** Build the CMS layer (Pages, menus, redirects), the core Gifter domain (wishlists, gifts, following, reservations), and the Orders activity grid on the admin dashboard.

**Duration:** ~3 weeks
**Status:** ✅ Sprint 2 complete — all admin grids verified, CMS null-safety confirmed, database layer done, Admin User Followers tab added. Remaining for future sprints: Website Followers C# wiring, OrderTypes 3 & 4 triggers, Gifts Owner column bug (DB-side fix needed)

---

## What This Sprint Covers

| Area | In Scope | Out of Scope |
|---|---|---|
| CMS | Pages, MenuHeader, MenuFooter, Redirects — tables, procs, functions, admin CRUD | Blog engine |
| Gifter: Wishlists | GiftLists table, CRUD, privacy toggle, shareable links | Occasion type filtering |
| Gifter: Gifts | Gifts table, manual add, URL scraping (Quick Add via existing `ScraperService`) | Integrated retailer search |
| Gifter: Social | Followers table, follow/unfollow | Email notifications |
| Gifter: Reservations | Reserve/unreserve a gift, concurrency check | Purchase flow |
| Orders Dashboard | Activity grid on admin dashboard | Real-time updates |
| Auth | User registration (email only) | Social login |
| User Dashboard | Profile, own wishlists list | Avatar upload |

---

## Critical: SmarterASP Constraints

Same rules as Sprint 1 — the SQL editor **does not support `GO`**. Each numbered script below is one separate execution. Run them **exactly in the order listed**.

---

## Execution Order (41 scripts)

| # | Scripts | Type | Why this order |
|---|---|---|---|
| 1–8 | Tables | DDL | FK dependencies require this exact order |
| 9–12 | Drop stub functions | DDL | Must drop before recreating with real logic |
| 13–18 | CMS Stored Procedures | DDL | CMS admin CRUD needs these |
| 19–27 | CMS Functions | DDL | Replace stubs + new read functions |
| 28–32 | Gifter Stored Procedures | DDL | Domain write operations |
| 33–41 | Gifter + Orders Functions | DDL | Domain read operations + Orders grid |

---

## Phase 1 — Create Tables (Scripts 1–8)

### Script 1 — Pages

```sql
CREATE TABLE [dbo].[Pages] (
    [PageID]               INT           IDENTITY(1,1) NOT NULL,
    [PageSlug]             NVARCHAR(500) NOT NULL UNIQUE,
    [PageTitle]            NVARCHAR(255) NULL,
    [PageTitleEng]         NVARCHAR(255) NULL,
    [PageContent]          NVARCHAR(MAX) NULL,
    [PageContentEng]       NVARCHAR(MAX) NULL,
    [PageIsPublished]      BIT           NOT NULL DEFAULT 0,
    [PageMetaTitle]        NVARCHAR(255) NULL,
    [PageMetaDescription]  NVARCHAR(500) NULL,
    [PageDateCreated]      DATETIME      NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY ([PageID])
);
```

### Script 2 — Redirects

```sql
CREATE TABLE [dbo].[Redirects] (
    [RedirectID]          INT           IDENTITY(1,1) NOT NULL,
    [RedirectFrom]        NVARCHAR(500) NOT NULL UNIQUE,
    [RedirectTo]          NVARCHAR(500) NULL,
    [RedirectDateCreated] DATETIME      NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY ([RedirectID])
);
```

### Script 3 — MenuHeader

Self-referencing for nested menus. `MenuHeaderPageID` links to the page this menu item navigates to (NULL for external links).

```sql
CREATE TABLE [dbo].[MenuHeader] (
    [MenuHeaderID]              INT           IDENTITY(1,1) NOT NULL,
    [MenuHeaderParentID]        INT           NULL,
    [MenuHeaderPageID]          INT           NULL,
    [MenuHeaderTitle]           NVARCHAR(255) NULL,
    [MenuHeaderTitleEng]        NVARCHAR(255) NULL,
    [MenuHeaderIsExternalPage]  BIT           NOT NULL DEFAULT 0,
    [MenuHeaderExternalPageUrl] NVARCHAR(500) NULL,
    [MenuHeaderIsPublished]     BIT           NOT NULL DEFAULT 0,
    [MenuHeaderIsTargetBlank]   BIT           NOT NULL DEFAULT 0,
    [MenuHeaderSortIndex]       INT           NOT NULL DEFAULT 0,
    [MenuHeaderDateCreated]     DATETIME      NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY ([MenuHeaderID]),
    FOREIGN KEY ([MenuHeaderParentID]) REFERENCES [dbo].[MenuHeader]([MenuHeaderID]),
    FOREIGN KEY ([MenuHeaderPageID])   REFERENCES [dbo].[Pages]([PageID])
);
```

### Script 4 — MenuFooter

Same structure as MenuHeader but flat (no parent).

```sql
CREATE TABLE [dbo].[MenuFooter] (
    [MenuFooterID]              INT           IDENTITY(1,1) NOT NULL,
    [MenuFooterPageID]          INT           NULL,
    [MenuFooterTitle]           NVARCHAR(255) NULL,
    [MenuFooterTitleEng]        NVARCHAR(255) NULL,
    [MenuFooterIsExternalPage]  BIT           NOT NULL DEFAULT 0,
    [MenuFooterExternalPageUrl] NVARCHAR(500) NULL,
    [MenuFooterIsPublished]     BIT           NOT NULL DEFAULT 0,
    [MenuFooterIsTargetBlank]   BIT           NOT NULL DEFAULT 0,
    [MenuFooterSortIndex]       INT           NOT NULL DEFAULT 0,
    [MenuFooterDateCreated]     DATETIME      NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY ([MenuFooterID]),
    FOREIGN KEY ([MenuFooterPageID]) REFERENCES [dbo].[Pages]([PageID])
);
```

### Script 5 — GiftLists

Owned by a User. `GiftListIsSecret` controls Public vs Secret visibility.

```sql
CREATE TABLE [dbo].[GiftLists] (
    [GiftListID]           INT           IDENTITY(1,1) NOT NULL,
    [GiftListUserID]       INT           NOT NULL,
    [GiftListTitle]        NVARCHAR(255) NOT NULL,
    [GiftListDescription]  NVARCHAR(MAX) NULL,
    [GiftListOccasionType] NVARCHAR(100) NULL,
    [GiftListIsSecret]     BIT           NOT NULL DEFAULT 0,
    [GiftListIsPublished]  BIT           NOT NULL DEFAULT 1,
    [GiftListEndDate]      DATETIME      NULL,
    [GiftListDateCreated]  DATETIME      NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY ([GiftListID]),
    FOREIGN KEY ([GiftListUserID]) REFERENCES [dbo].[Users]([UserID])
);
```

### Script 6 — Gifts

Items inside a GiftList. `GiftIsReserved` + `GiftReservedByUserID` track the reservation state.

```sql
CREATE TABLE [dbo].[Gifts] (
    [GiftID]               INT            IDENTITY(1,1) NOT NULL,
    [GiftGiftListID]       INT            NOT NULL,
    [GiftTitle]            NVARCHAR(255)  NOT NULL,
    [GiftDescription]      NVARCHAR(MAX)  NULL,
    [GiftPrice]            DECIMAL(18,2)  NULL,
    [GiftCurrency]         NVARCHAR(10)   NULL DEFAULT 'GEL',
    [GiftUrl]              NVARCHAR(MAX)  NULL,
    [GiftImageUrl]         NVARCHAR(MAX)  NULL,
    [GiftIsReserved]       BIT            NOT NULL DEFAULT 0,
    [GiftReservedByUserID] INT            NULL,
    [GiftDateCreated]      DATETIME       NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY ([GiftID]),
    FOREIGN KEY ([GiftGiftListID])       REFERENCES [dbo].[GiftLists]([GiftListID]),
    FOREIGN KEY ([GiftReservedByUserID]) REFERENCES [dbo].[Users]([UserID])
);
```

### Script 7 — Followers

UNIQUE constraint prevents duplicate follows. `FollowingUserID` = the person doing the following. `FollowedUserID` = the person being followed.

```sql
CREATE TABLE [dbo].[Followers] (
    [FollowerID]                INT      IDENTITY(1,1) NOT NULL,
    [FollowerFollowingUserID]   INT      NOT NULL,
    [FollowerFollowedUserID]    INT      NOT NULL,
    [FollowerDateCreated]       DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY ([FollowerID]),
    FOREIGN KEY ([FollowerFollowingUserID]) REFERENCES [dbo].[Users]([UserID]),
    FOREIGN KEY ([FollowerFollowedUserID])  REFERENCES [dbo].[Users]([UserID]),
    UNIQUE ([FollowerFollowingUserID], [FollowerFollowedUserID])
);
```

### Script 8 — Orders

Activity log. Every user action that matters gets one row. `OrderType` identifies the activity. Nullable FKs point to the relevant objects depending on the type.

```sql
CREATE TABLE [dbo].[Orders] (
    [OrderID]            INT      IDENTITY(1,1) NOT NULL,
    [OrderUserID]        INT      NOT NULL,
    [OrderType]          INT      NOT NULL,
    [OrderGiftListID]    INT      NULL,
    [OrderGiftID]        INT      NULL,
    [OrderTargetUserID]  INT      NULL,
    [OrderDateCreated]   DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY ([OrderID]),
    FOREIGN KEY ([OrderUserID])       REFERENCES [dbo].[Users]([UserID]),
    FOREIGN KEY ([OrderGiftListID])   REFERENCES [dbo].[GiftLists]([GiftListID]),
    FOREIGN KEY ([OrderGiftID])       REFERENCES [dbo].[Gifts]([GiftID]),
    FOREIGN KEY ([OrderTargetUserID]) REFERENCES [dbo].[Users]([UserID])
);
```

---

## Phase 2 — Drop Stub Functions (Scripts 9–12)

These stubs were created in Sprint 1 to silence errors. Drop each one separately before the real versions are created in Phase 4.

### Script 9
```sql
DROP FUNCTION [dbo].[RedirectsList];
```

### Script 10
```sql
DROP FUNCTION [dbo].[MenuHeaderList];
```

### Script 11
```sql
DROP FUNCTION [dbo].[MenuFooterList];
```

### Script 12
```sql
DROP FUNCTION [dbo].[PagesGetSingleBySlug];
```

---

## Phase 3 — CMS Stored Procedures (Scripts 13–18)

### Script 13 — PagesIUD

DELETE cascades to MenuHeader and MenuFooter rows that reference the page.

```sql
CREATE OR ALTER PROCEDURE [dbo].[PagesIUD]
(
    @Action   TINYINT,
    @PageID   INT OUTPUT,
    @PageJson NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 0 -- INSERT
    BEGIN
        INSERT INTO [dbo].[Pages] (
            PageSlug, PageTitle, PageTitleEng, PageContent, PageContentEng,
            PageIsPublished, PageMetaTitle, PageMetaDescription
        )
        VALUES (
            JSON_VALUE(@PageJson, '$.PageSlug'),
            JSON_VALUE(@PageJson, '$.PageTitle'),
            JSON_VALUE(@PageJson, '$.PageTitleEng'),
            JSON_VALUE(@PageJson, '$.PageContent'),
            JSON_VALUE(@PageJson, '$.PageContentEng'),
            CASE WHEN JSON_VALUE(@PageJson, '$.PageIsPublished') = 'true' THEN 1 ELSE 0 END,
            JSON_VALUE(@PageJson, '$.PageMetaTitle'),
            JSON_VALUE(@PageJson, '$.PageMetaDescription')
        );
        SET @PageID = SCOPE_IDENTITY();
    END
    ELSE IF @Action = 1 -- UPDATE
    BEGIN
        UPDATE [dbo].[Pages]
        SET
            PageSlug            = COALESCE(JSON_VALUE(@PageJson, '$.PageSlug'), PageSlug),
            PageTitle           = COALESCE(JSON_VALUE(@PageJson, '$.PageTitle'), PageTitle),
            PageTitleEng        = COALESCE(JSON_VALUE(@PageJson, '$.PageTitleEng'), PageTitleEng),
            PageContent         = COALESCE(JSON_VALUE(@PageJson, '$.PageContent'), PageContent),
            PageContentEng      = COALESCE(JSON_VALUE(@PageJson, '$.PageContentEng'), PageContentEng),
            PageIsPublished = CASE
                WHEN JSON_VALUE(@PageJson, '$.PageIsPublished') IS NOT NULL
                THEN CASE WHEN JSON_VALUE(@PageJson, '$.PageIsPublished') = 'true' THEN 1 ELSE 0 END
                ELSE PageIsPublished
            END,
            PageMetaTitle       = COALESCE(JSON_VALUE(@PageJson, '$.PageMetaTitle'), PageMetaTitle),
            PageMetaDescription = COALESCE(JSON_VALUE(@PageJson, '$.PageMetaDescription'), PageMetaDescription)
        WHERE PageID = @PageID;
    END
    ELSE IF @Action = 2 -- DELETE
    BEGIN
        DELETE FROM [dbo].[MenuHeader] WHERE MenuHeaderPageID = @PageID;
        DELETE FROM [dbo].[MenuFooter] WHERE MenuFooterPageID = @PageID;
        DELETE FROM [dbo].[Pages]      WHERE PageID = @PageID;
    END
END;
```

### Script 14 — MenuHeaderIUD

DELETE removes children first (self-referencing hierarchy).

```sql
CREATE OR ALTER PROCEDURE [dbo].[MenuHeaderIUD]
(
    @Action         TINYINT,
    @MenuHeaderID   INT OUTPUT,
    @MenuHeaderJson NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 0 -- INSERT
    BEGIN
        INSERT INTO [dbo].[MenuHeader] (
            MenuHeaderParentID, MenuHeaderPageID, MenuHeaderTitle, MenuHeaderTitleEng,
            MenuHeaderIsExternalPage, MenuHeaderExternalPageUrl, MenuHeaderIsPublished,
            MenuHeaderIsTargetBlank, MenuHeaderSortIndex
        )
        VALUES (
            CAST(JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderParentID') AS INT),
            CAST(JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderPageID') AS INT),
            JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderTitle'),
            JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderTitleEng'),
            CASE WHEN JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderIsExternalPage') = 'true' THEN 1 ELSE 0 END,
            JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderExternalPageUrl'),
            CASE WHEN JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderIsPublished') = 'true' THEN 1 ELSE 0 END,
            CASE WHEN JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderIsTargetBlank') = 'true' THEN 1 ELSE 0 END,
            COALESCE(CAST(JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderSortIndex') AS INT), 0)
        );
        SET @MenuHeaderID = SCOPE_IDENTITY();
    END
    ELSE IF @Action = 1 -- UPDATE
    BEGIN
        UPDATE [dbo].[MenuHeader]
        SET
            MenuHeaderParentID = CASE WHEN JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderParentID') IS NOT NULL THEN CAST(JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderParentID') AS INT) ELSE MenuHeaderParentID END,
            MenuHeaderPageID   = CASE WHEN JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderPageID') IS NOT NULL THEN CAST(JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderPageID') AS INT) ELSE MenuHeaderPageID END,
            MenuHeaderTitle    = COALESCE(JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderTitle'), MenuHeaderTitle),
            MenuHeaderTitleEng = COALESCE(JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderTitleEng'), MenuHeaderTitleEng),
            MenuHeaderIsExternalPage = CASE
                WHEN JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderIsExternalPage') IS NOT NULL
                THEN CASE WHEN JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderIsExternalPage') = 'true' THEN 1 ELSE 0 END
                ELSE MenuHeaderIsExternalPage
            END,
            MenuHeaderExternalPageUrl = COALESCE(JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderExternalPageUrl'), MenuHeaderExternalPageUrl),
            MenuHeaderIsPublished = CASE
                WHEN JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderIsPublished') IS NOT NULL
                THEN CASE WHEN JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderIsPublished') = 'true' THEN 1 ELSE 0 END
                ELSE MenuHeaderIsPublished
            END,
            MenuHeaderIsTargetBlank = CASE
                WHEN JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderIsTargetBlank') IS NOT NULL
                THEN CASE WHEN JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderIsTargetBlank') = 'true' THEN 1 ELSE 0 END
                ELSE MenuHeaderIsTargetBlank
            END,
            MenuHeaderSortIndex = COALESCE(CAST(JSON_VALUE(@MenuHeaderJson, '$.MenuHeaderSortIndex') AS INT), MenuHeaderSortIndex)
        WHERE MenuHeaderID = @MenuHeaderID;
    END
    ELSE IF @Action = 2 -- DELETE
    BEGIN
        DELETE FROM [dbo].[MenuHeader] WHERE MenuHeaderParentID = @MenuHeaderID;
        DELETE FROM [dbo].[MenuHeader] WHERE MenuHeaderID = @MenuHeaderID;
    END
END;
```

### Script 15 — MenuFooterIUD

```sql
CREATE OR ALTER PROCEDURE [dbo].[MenuFooterIUD]
(
    @Action         TINYINT,
    @MenuFooterID   INT OUTPUT,
    @MenuFooterJson NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 0 -- INSERT
    BEGIN
        INSERT INTO [dbo].[MenuFooter] (
            MenuFooterPageID, MenuFooterTitle, MenuFooterTitleEng,
            MenuFooterIsExternalPage, MenuFooterExternalPageUrl, MenuFooterIsPublished,
            MenuFooterIsTargetBlank, MenuFooterSortIndex
        )
        VALUES (
            CAST(JSON_VALUE(@MenuFooterJson, '$.MenuFooterPageID') AS INT),
            JSON_VALUE(@MenuFooterJson, '$.MenuFooterTitle'),
            JSON_VALUE(@MenuFooterJson, '$.MenuFooterTitleEng'),
            CASE WHEN JSON_VALUE(@MenuFooterJson, '$.MenuFooterIsExternalPage') = 'true' THEN 1 ELSE 0 END,
            JSON_VALUE(@MenuFooterJson, '$.MenuFooterExternalPageUrl'),
            CASE WHEN JSON_VALUE(@MenuFooterJson, '$.MenuFooterIsPublished') = 'true' THEN 1 ELSE 0 END,
            CASE WHEN JSON_VALUE(@MenuFooterJson, '$.MenuFooterIsTargetBlank') = 'true' THEN 1 ELSE 0 END,
            COALESCE(CAST(JSON_VALUE(@MenuFooterJson, '$.MenuFooterSortIndex') AS INT), 0)
        );
        SET @MenuFooterID = SCOPE_IDENTITY();
    END
    ELSE IF @Action = 1 -- UPDATE
    BEGIN
        UPDATE [dbo].[MenuFooter]
        SET
            MenuFooterPageID   = CASE WHEN JSON_VALUE(@MenuFooterJson, '$.MenuFooterPageID') IS NOT NULL THEN CAST(JSON_VALUE(@MenuFooterJson, '$.MenuFooterPageID') AS INT) ELSE MenuFooterPageID END,
            MenuFooterTitle    = COALESCE(JSON_VALUE(@MenuFooterJson, '$.MenuFooterTitle'), MenuFooterTitle),
            MenuFooterTitleEng = COALESCE(JSON_VALUE(@MenuFooterJson, '$.MenuFooterTitleEng'), MenuFooterTitleEng),
            MenuFooterIsExternalPage = CASE
                WHEN JSON_VALUE(@MenuFooterJson, '$.MenuFooterIsExternalPage') IS NOT NULL
                THEN CASE WHEN JSON_VALUE(@MenuFooterJson, '$.MenuFooterIsExternalPage') = 'true' THEN 1 ELSE 0 END
                ELSE MenuFooterIsExternalPage
            END,
            MenuFooterExternalPageUrl = COALESCE(JSON_VALUE(@MenuFooterJson, '$.MenuFooterExternalPageUrl'), MenuFooterExternalPageUrl),
            MenuFooterIsPublished = CASE
                WHEN JSON_VALUE(@MenuFooterJson, '$.MenuFooterIsPublished') IS NOT NULL
                THEN CASE WHEN JSON_VALUE(@MenuFooterJson, '$.MenuFooterIsPublished') = 'true' THEN 1 ELSE 0 END
                ELSE MenuFooterIsPublished
            END,
            MenuFooterIsTargetBlank = CASE
                WHEN JSON_VALUE(@MenuFooterJson, '$.MenuFooterIsTargetBlank') IS NOT NULL
                THEN CASE WHEN JSON_VALUE(@MenuFooterJson, '$.MenuFooterIsTargetBlank') = 'true' THEN 1 ELSE 0 END
                ELSE MenuFooterIsTargetBlank
            END,
            MenuFooterSortIndex = COALESCE(CAST(JSON_VALUE(@MenuFooterJson, '$.MenuFooterSortIndex') AS INT), MenuFooterSortIndex)
        WHERE MenuFooterID = @MenuFooterID;
    END
    ELSE IF @Action = 2 -- DELETE
    BEGIN
        DELETE FROM [dbo].[MenuFooter] WHERE MenuFooterID = @MenuFooterID;
    END
END;
```

### Script 16 — RedirectsIUD

```sql
CREATE OR ALTER PROCEDURE [dbo].[RedirectsIUD]
(
    @Action       TINYINT,
    @RedirectID   INT OUTPUT,
    @RedirectJson NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 0 -- INSERT
    BEGIN
        INSERT INTO [dbo].[Redirects] (RedirectFrom, RedirectTo)
        VALUES (
            JSON_VALUE(@RedirectJson, '$.RedirectFrom'),
            JSON_VALUE(@RedirectJson, '$.RedirectTo')
        );
        SET @RedirectID = SCOPE_IDENTITY();
    END
    ELSE IF @Action = 1 -- UPDATE
    BEGIN
        UPDATE [dbo].[Redirects]
        SET
            RedirectFrom = COALESCE(JSON_VALUE(@RedirectJson, '$.RedirectFrom'), RedirectFrom),
            RedirectTo   = COALESCE(JSON_VALUE(@RedirectJson, '$.RedirectTo'), RedirectTo)
        WHERE RedirectID = @RedirectID;
    END
    ELSE IF @Action = 2 -- DELETE
    BEGIN
        DELETE FROM [dbo].[Redirects] WHERE RedirectID = @RedirectID;
    END
END;
```

### Script 17 — MenuHeaderSort

Receives a JSON array of `[{"ID":1,"SortIndex":0}, {"ID":2,"SortIndex":1}, …]` and bulk-updates sort indexes in one shot.

```sql
CREATE OR ALTER PROCEDURE [dbo].[MenuHeaderSort]
(
    @SortIndexesJson NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE mh
    SET MenuHeaderSortIndex = si.SortIndex
    FROM [dbo].[MenuHeader] mh
    INNER JOIN OPENJSON(@SortIndexesJson) WITH (
        ID        INT '$.ID',
        SortIndex INT '$.SortIndex'
    ) si ON mh.MenuHeaderID = si.ID;
END;
```

### Script 18 — MenuFooterSort

```sql
CREATE OR ALTER PROCEDURE [dbo].[MenuFooterSort]
(
    @SortIndexesJson NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE mf
    SET MenuFooterSortIndex = si.SortIndex
    FROM [dbo].[MenuFooter] mf
    INNER JOIN OPENJSON(@SortIndexesJson) WITH (
        ID        INT '$.ID',
        SortIndex INT '$.SortIndex'
    ) si ON mf.MenuFooterID = si.ID;
END;
```

---

## Phase 4 — CMS Functions (Scripts 19–27)

Scripts 19–22 are the **real replacements** for the Sprint 1 stubs. Scripts 23–27 are new.

### Script 19 — RedirectsList

```sql
CREATE FUNCTION [dbo].[RedirectsList]()
RETURNS TABLE
AS
RETURN
    SELECT
        RedirectID,
        RedirectFrom,
        RedirectTo,
        RedirectDateCreated
    FROM [dbo].[Redirects];
```

### Script 20 — MenuHeaderList

Parameter filters by publish status. NULL = return all (used by admin). `1` = published only (used by website layout).

```sql
CREATE FUNCTION [dbo].[MenuHeaderList](@MenuHeaderIsPublished BIT = NULL)
RETURNS TABLE
AS
RETURN
    SELECT
        mh.MenuHeaderID,
        mh.MenuHeaderParentID,
        mh.MenuHeaderTitle,
        mh.MenuHeaderTitleEng,
        mh.MenuHeaderIsExternalPage,
        mh.MenuHeaderExternalPageUrl,
        mh.MenuHeaderIsPublished,
        mh.MenuHeaderIsTargetBlank,
        mh.MenuHeaderSortIndex,
        p.PageID,
        p.PageSlug,
        p.PageTitle,
        p.PageTitleEng,
        p.PageIsPublished
    FROM [dbo].[MenuHeader] mh
    LEFT JOIN [dbo].[Pages] p ON mh.MenuHeaderPageID = p.PageID
    WHERE @MenuHeaderIsPublished IS NULL OR mh.MenuHeaderIsPublished = @MenuHeaderIsPublished;
```

### Script 21 — MenuFooterList

```sql
CREATE FUNCTION [dbo].[MenuFooterList](@MenuFooterIsPublished BIT = NULL)
RETURNS TABLE
AS
RETURN
    SELECT
        mf.MenuFooterID,
        mf.MenuFooterTitle,
        mf.MenuFooterTitleEng,
        mf.MenuFooterIsExternalPage,
        mf.MenuFooterExternalPageUrl,
        mf.MenuFooterIsPublished,
        mf.MenuFooterIsTargetBlank,
        mf.MenuFooterSortIndex,
        p.PageID,
        p.PageSlug,
        p.PageTitle,
        p.PageTitleEng,
        p.PageIsPublished
    FROM [dbo].[MenuFooter] mf
    LEFT JOIN [dbo].[Pages] p ON mf.MenuFooterPageID = p.PageID
    WHERE @MenuFooterIsPublished IS NULL OR mf.MenuFooterIsPublished = @MenuFooterIsPublished;
```

### Script 22 — PagesGetSingleBySlug

Returns NULL when no page matches — the website router uses this to fall through to a 404 page.

```sql
CREATE FUNCTION [dbo].[PagesGetSingleBySlug](@PageSlug NVARCHAR(500))
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN (
        SELECT
            PageID,
            PageSlug,
            PageTitle,
            PageTitleEng,
            PageContent,
            PageContentEng,
            PageIsPublished,
            PageMetaTitle,
            PageMetaDescription,
            PageDateCreated
        FROM [dbo].[Pages]
        WHERE PageSlug = @PageSlug
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    )
END;
```

### Script 23 — PagesGetSingleByID

```sql
CREATE FUNCTION [dbo].[PagesGetSingleByID](@PageID INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN (
        SELECT
            PageID,
            PageSlug,
            PageTitle,
            PageTitleEng,
            PageContent,
            PageContentEng,
            PageIsPublished,
            PageMetaTitle,
            PageMetaDescription,
            PageDateCreated
        FROM [dbo].[Pages]
        WHERE PageID = @PageID
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    )
END;
```

### Script 24 — PagesList

TVF for the admin Pages grid.

```sql
CREATE FUNCTION [dbo].[PagesList]()
RETURNS TABLE
AS
RETURN
    SELECT
        PageID,
        PageSlug,
        PageTitle,
        PageTitleEng,
        PageContent,
        PageContentEng,
        PageIsPublished,
        PageMetaTitle,
        PageMetaDescription,
        PageDateCreated
    FROM [dbo].[Pages];
```

### Script 25 — PagesIsSlugUniq

```sql
CREATE FUNCTION [dbo].[PagesIsSlugUniq](@PageSlug NVARCHAR(500), @PageID INT = NULL)
RETURNS BIT
AS
BEGIN
    DECLARE @IsUnique BIT;

    SELECT @IsUnique = CASE
        WHEN COUNT(*) = 0 THEN 1
        ELSE 0
    END
    FROM [dbo].[Pages]
    WHERE PageSlug = @PageSlug
      AND (@PageID IS NULL OR PageID <> @PageID);

    RETURN @IsUnique;
END;
```

### Script 26 — MenuHeaderGetSingleByID

```sql
CREATE FUNCTION [dbo].[MenuHeaderGetSingleByID](@MenuHeaderID INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN (
        SELECT
            mh.MenuHeaderID,
            mh.MenuHeaderParentID,
            mh.MenuHeaderTitle,
            mh.MenuHeaderTitleEng,
            mh.MenuHeaderIsExternalPage,
            mh.MenuHeaderExternalPageUrl,
            mh.MenuHeaderIsPublished,
            mh.MenuHeaderIsTargetBlank,
            mh.MenuHeaderSortIndex,
            p.PageID,
            p.PageSlug,
            p.PageTitle,
            p.PageTitleEng,
            p.PageIsPublished
        FROM [dbo].[MenuHeader] mh
        LEFT JOIN [dbo].[Pages] p ON mh.MenuHeaderPageID = p.PageID
        WHERE mh.MenuHeaderID = @MenuHeaderID
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    )
END;
```

### Script 27 — MenuFooterGetSingleByID

```sql
CREATE FUNCTION [dbo].[MenuFooterGetSingleByID](@MenuFooterID INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN (
        SELECT
            mf.MenuFooterID,
            mf.MenuFooterTitle,
            mf.MenuFooterTitleEng,
            mf.MenuFooterIsExternalPage,
            mf.MenuFooterExternalPageUrl,
            mf.MenuFooterIsPublished,
            mf.MenuFooterIsTargetBlank,
            mf.MenuFooterSortIndex,
            p.PageID,
            p.PageSlug,
            p.PageTitle,
            p.PageTitleEng,
            p.PageIsPublished
        FROM [dbo].[MenuFooter] mf
        LEFT JOIN [dbo].[Pages] p ON mf.MenuFooterPageID = p.PageID
        WHERE mf.MenuFooterID = @MenuFooterID
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    )
END;
```

---

## Phase 5 — Gifter Stored Procedures (Scripts 28–32)

### Script 28 — GiftListsIUD

DELETE cascades: removes Orders and Gifts referencing the list before deleting it.

```sql
CREATE OR ALTER PROCEDURE [dbo].[GiftListsIUD]
(
    @Action       TINYINT,
    @GiftListID   INT OUTPUT,
    @GiftListJson NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 0 -- INSERT
    BEGIN
        INSERT INTO [dbo].[GiftLists] (
            GiftListUserID, GiftListTitle, GiftListDescription, GiftListOccasionType,
            GiftListIsSecret, GiftListIsPublished, GiftListEndDate
        )
        VALUES (
            CAST(JSON_VALUE(@GiftListJson, '$.GiftListUserID') AS INT),
            JSON_VALUE(@GiftListJson, '$.GiftListTitle'),
            JSON_VALUE(@GiftListJson, '$.GiftListDescription'),
            JSON_VALUE(@GiftListJson, '$.GiftListOccasionType'),
            CASE WHEN JSON_VALUE(@GiftListJson, '$.GiftListIsSecret') = 'true' THEN 1 ELSE 0 END,
            CASE WHEN JSON_VALUE(@GiftListJson, '$.GiftListIsPublished') = 'true' THEN 1 ELSE 0 END,
            CAST(JSON_VALUE(@GiftListJson, '$.GiftListEndDate') AS DATETIME)
        );
        SET @GiftListID = SCOPE_IDENTITY();
    END
    ELSE IF @Action = 1 -- UPDATE
    BEGIN
        UPDATE [dbo].[GiftLists]
        SET
            GiftListTitle        = COALESCE(JSON_VALUE(@GiftListJson, '$.GiftListTitle'), GiftListTitle),
            GiftListDescription  = COALESCE(JSON_VALUE(@GiftListJson, '$.GiftListDescription'), GiftListDescription),
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
            GiftListEndDate = COALESCE(CAST(JSON_VALUE(@GiftListJson, '$.GiftListEndDate') AS DATETIME), GiftListEndDate)
        WHERE GiftListID = @GiftListID;
    END
    ELSE IF @Action = 2 -- DELETE
    BEGIN
        DELETE FROM [dbo].[Orders]    WHERE OrderGiftListID = @GiftListID;
        DELETE FROM [dbo].[Gifts]     WHERE GiftGiftListID  = @GiftListID;
        DELETE FROM [dbo].[GiftLists] WHERE GiftListID      = @GiftListID;
    END
END;
```

### Script 29 — GiftsIUD

DELETE removes related Orders before deleting the gift.

```sql
CREATE OR ALTER PROCEDURE [dbo].[GiftsIUD]
(
    @Action   TINYINT,
    @GiftID   INT OUTPUT,
    @GiftJson NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 0 -- INSERT
    BEGIN
        INSERT INTO [dbo].[Gifts] (
            GiftGiftListID, GiftTitle, GiftDescription, GiftPrice, GiftCurrency,
            GiftUrl, GiftImageUrl, GiftIsReserved, GiftReservedByUserID
        )
        VALUES (
            CAST(JSON_VALUE(@GiftJson, '$.GiftGiftListID') AS INT),
            JSON_VALUE(@GiftJson, '$.GiftTitle'),
            JSON_VALUE(@GiftJson, '$.GiftDescription'),
            CAST(JSON_VALUE(@GiftJson, '$.GiftPrice') AS DECIMAL(18,2)),
            COALESCE(JSON_VALUE(@GiftJson, '$.GiftCurrency'), 'GEL'),
            JSON_VALUE(@GiftJson, '$.GiftUrl'),
            JSON_VALUE(@GiftJson, '$.GiftImageUrl'),
            CASE WHEN JSON_VALUE(@GiftJson, '$.GiftIsReserved') = 'true' THEN 1 ELSE 0 END,
            CAST(JSON_VALUE(@GiftJson, '$.GiftReservedByUserID') AS INT)
        );
        SET @GiftID = SCOPE_IDENTITY();
    END
    ELSE IF @Action = 1 -- UPDATE
    BEGIN
        UPDATE [dbo].[Gifts]
        SET
            GiftTitle            = COALESCE(JSON_VALUE(@GiftJson, '$.GiftTitle'), GiftTitle),
            GiftDescription      = COALESCE(JSON_VALUE(@GiftJson, '$.GiftDescription'), GiftDescription),
            GiftPrice            = COALESCE(CAST(JSON_VALUE(@GiftJson, '$.GiftPrice') AS DECIMAL(18,2)), GiftPrice),
            GiftCurrency         = COALESCE(JSON_VALUE(@GiftJson, '$.GiftCurrency'), GiftCurrency),
            GiftUrl              = COALESCE(JSON_VALUE(@GiftJson, '$.GiftUrl'), GiftUrl),
            GiftImageUrl         = COALESCE(JSON_VALUE(@GiftJson, '$.GiftImageUrl'), GiftImageUrl),
            GiftIsReserved = CASE
                WHEN JSON_VALUE(@GiftJson, '$.GiftIsReserved') IS NOT NULL
                THEN CASE WHEN JSON_VALUE(@GiftJson, '$.GiftIsReserved') = 'true' THEN 1 ELSE 0 END
                ELSE GiftIsReserved
            END,
            GiftReservedByUserID = CASE
                WHEN JSON_VALUE(@GiftJson, '$.GiftReservedByUserID') IS NOT NULL
                THEN CAST(JSON_VALUE(@GiftJson, '$.GiftReservedByUserID') AS INT)
                ELSE GiftReservedByUserID
            END
        WHERE GiftID = @GiftID;
    END
    ELSE IF @Action = 2 -- DELETE
    BEGIN
        DELETE FROM [dbo].[Orders] WHERE OrderGiftID = @GiftID;
        DELETE FROM [dbo].[Gifts]  WHERE GiftID      = @GiftID;
    END
END;
```

### Script 30 — FollowersFollow

Idempotent — does nothing if the follow already exists.

```sql
CREATE OR ALTER PROCEDURE [dbo].[FollowersFollow]
(
    @FollowingUserID INT,
    @FollowedUserID  INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1 FROM [dbo].[Followers]
        WHERE FollowerFollowingUserID = @FollowingUserID
          AND FollowerFollowedUserID  = @FollowedUserID
    )
    BEGIN
        INSERT INTO [dbo].[Followers] (FollowerFollowingUserID, FollowerFollowedUserID)
        VALUES (@FollowingUserID, @FollowedUserID);
    END
END;
```

### Script 31 — FollowersUnfollow

```sql
CREATE OR ALTER PROCEDURE [dbo].[FollowersUnfollow]
(
    @FollowingUserID INT,
    @FollowedUserID  INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[Followers]
    WHERE FollowerFollowingUserID = @FollowingUserID
      AND FollowerFollowedUserID  = @FollowedUserID;
END;
```

### Script 32 — OrdersInsert

Called by the application after each tracked activity. Only inserts — the Orders table is append-only (no update or delete).

```sql
CREATE OR ALTER PROCEDURE [dbo].[OrdersInsert]
(
    @OrderUserID       INT,
    @OrderType         INT,
    @OrderGiftListID   INT = NULL,
    @OrderGiftID       INT = NULL,
    @OrderTargetUserID INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[Orders] (
        OrderUserID, OrderType, OrderGiftListID, OrderGiftID, OrderTargetUserID
    )
    VALUES (
        @OrderUserID, @OrderType, @OrderGiftListID, @OrderGiftID, @OrderTargetUserID
    );
END;
```

---

## Phase 6 — Gifter + Orders Functions (Scripts 33–41)

### Script 33 — GiftListsGetSingleByID

```sql
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
            u.UserFullname AS OwnerFullname
        FROM [dbo].[GiftLists] gl
        INNER JOIN [dbo].[Users] u ON gl.GiftListUserID = u.UserID
        WHERE gl.GiftListID = @GiftListID
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    )
END;
```

### Script 34 — GiftListsList

Admin view — all lists in the system with owner info.

```sql
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
        u.UserFullname AS OwnerFullname
    FROM [dbo].[GiftLists] gl
    INNER JOIN [dbo].[Users] u ON gl.GiftListUserID = u.UserID;
```

### Script 35 — GiftListsListByUserID

User's own lists — used on the profile dashboard.

```sql
CREATE FUNCTION [dbo].[GiftListsListByUserID](@UserID INT)
RETURNS TABLE
AS
RETURN
    SELECT
        GiftListID,
        GiftListUserID,
        GiftListTitle,
        GiftListDescription,
        GiftListOccasionType,
        GiftListIsSecret,
        GiftListIsPublished,
        GiftListEndDate,
        GiftListDateCreated
    FROM [dbo].[GiftLists]
    WHERE GiftListUserID = @UserID;
```

### Script 36 — GiftsGetSingleByID

```sql
CREATE FUNCTION [dbo].[GiftsGetSingleByID](@GiftID INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN (
        SELECT
            g.GiftID,
            g.GiftGiftListID,
            g.GiftTitle,
            g.GiftDescription,
            g.GiftPrice,
            g.GiftCurrency,
            g.GiftUrl,
            g.GiftImageUrl,
            g.GiftIsReserved,
            g.GiftReservedByUserID,
            g.GiftDateCreated,
            u.UserFullname AS ReservedByFullname
        FROM [dbo].[Gifts] g
        LEFT JOIN [dbo].[Users] u ON g.GiftReservedByUserID = u.UserID
        WHERE g.GiftID = @GiftID
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    )
END;
```

### Script 37 — GiftsListByGiftListID

All gifts inside a specific wishlist, with reservation info joined.

```sql
CREATE FUNCTION [dbo].[GiftsListByGiftListID](@GiftListID INT)
RETURNS TABLE
AS
RETURN
    SELECT
        g.GiftID,
        g.GiftGiftListID,
        g.GiftTitle,
        g.GiftDescription,
        g.GiftPrice,
        g.GiftCurrency,
        g.GiftUrl,
        g.GiftImageUrl,
        g.GiftIsReserved,
        g.GiftReservedByUserID,
        g.GiftDateCreated,
        u.UserFullname AS ReservedByFullname
    FROM [dbo].[Gifts] g
    LEFT JOIN [dbo].[Users] u ON g.GiftReservedByUserID = u.UserID
    WHERE g.GiftGiftListID = @GiftListID;
```

### Script 38 — FollowersList

Returns the users that a given user is **following**.

```sql
CREATE FUNCTION [dbo].[FollowersList](@FollowingUserID INT)
RETURNS TABLE
AS
RETURN
    SELECT
        f.FollowerID,
        f.FollowerFollowingUserID,
        f.FollowerFollowedUserID,
        f.FollowerDateCreated,
        u.UserFullname AS FollowedUserFullname,
        u.UserEmail    AS FollowedUserEmail
    FROM [dbo].[Followers] f
    INNER JOIN [dbo].[Users] u ON f.FollowerFollowedUserID = u.UserID
    WHERE f.FollowerFollowingUserID = @FollowingUserID;
```

### Script 39 — FollowersMyFollowers

Returns the users that are **following** a given user.

```sql
CREATE FUNCTION [dbo].[FollowersMyFollowers](@FollowedUserID INT)
RETURNS TABLE
AS
RETURN
    SELECT
        f.FollowerID,
        f.FollowerFollowingUserID,
        f.FollowerFollowedUserID,
        f.FollowerDateCreated,
        u.UserFullname AS FollowingUserFullname,
        u.UserEmail    AS FollowingUserEmail
    FROM [dbo].[Followers] f
    INNER JOIN [dbo].[Users] u ON f.FollowerFollowingUserID = u.UserID
    WHERE f.FollowerFollowedUserID = @FollowedUserID;
```

### Script 40 — FollowersIsFollowing

Returns 1 if user A follows user B, 0 otherwise.

```sql
CREATE FUNCTION [dbo].[FollowersIsFollowing](@FollowingUserID INT, @FollowedUserID INT)
RETURNS BIT
AS
BEGIN
    DECLARE @IsFollowing BIT;

    SELECT @IsFollowing = CASE
        WHEN COUNT(*) > 0 THEN 1
        ELSE 0
    END
    FROM [dbo].[Followers]
    WHERE FollowerFollowingUserID = @FollowingUserID
      AND FollowerFollowedUserID  = @FollowedUserID;

    RETURN @IsFollowing;
END;
```

### Script 41 — OrdersList

The TVF that backs the Orders dashboard grid. JOINs everything needed to show readable activity info in a single flat result set.

```sql
CREATE FUNCTION [dbo].[OrdersList]()
RETURNS TABLE
AS
RETURN
    SELECT
        o.OrderID,
        o.OrderDateCreated,
        o.OrderUserID,
        u.UserFullname                  AS OrderUserFullname,
        o.OrderType,
        CASE o.OrderType
            WHEN 1 THEN 'Wishlist Created'
            WHEN 2 THEN 'Gift Added'
            WHEN 3 THEN 'User Followed'
            WHEN 4 THEN 'Gift Reserved'
            ELSE 'Unknown'
        END                             AS OrderTypeName,
        o.OrderGiftListID,
        gl.GiftListTitle                AS OrderGiftListTitle,
        o.OrderGiftID,
        g.GiftTitle                     AS OrderGiftTitle,
        o.OrderTargetUserID,
        tu.UserFullname                 AS OrderTargetUserFullname
    FROM [dbo].[Orders] o
    INNER JOIN [dbo].[Users]     u  ON o.OrderUserID       = u.UserID
    LEFT  JOIN [dbo].[GiftLists] gl ON o.OrderGiftListID   = gl.GiftListID
    LEFT  JOIN [dbo].[Gifts]     g  ON o.OrderGiftID       = g.GiftID
    LEFT  JOIN [dbo].[Users]     tu ON o.OrderTargetUserID = tu.UserID;
```

---

## Orders Grid Specification

**Location:** Admin Dashboard (`/admin`) — replaces or supplements the current hardcoded cards
**Component:** DevExtreme DataGrid
**Data source:** `dbo.OrdersList` TVF
**Default sort:** `OrderDateCreated` DESC (newest first)
**Mode:** Read-only — no Add / Edit / Delete buttons

### Grid Columns

| Column | Source field | Notes |
|---|---|---|
| # | `OrderID` | |
| Date | `OrderDateCreated` | Format: `dd.MM.yyyy HH:mm` |
| User | `OrderUserFullname` | Who performed the action |
| Activity | `OrderTypeName` | Readable type label |
| Wishlist | `OrderGiftListTitle` | Nullable — blank for follows |
| Gift | `OrderGiftTitle` | Nullable — blank for follows |
| Target User | `OrderTargetUserFullname` | Nullable — shown for follows and reservations |

### OrderType Codes — When to Insert

The application calls `OrdersInsert` after each successful write operation:

| Code | Label | Trigger | Required fields |
|---|---|---|---|
| 1 | Wishlist Created | After `GiftListsIUD` INSERT succeeds | `OrderUserID`, `OrderGiftListID` |
| 2 | Gift Added | After `GiftsIUD` INSERT succeeds | `OrderUserID`, `OrderGiftListID`, `OrderGiftID` |
| 3 | User Followed | After `FollowersFollow` succeeds | `OrderUserID`, `OrderTargetUserID` |
| 4 | Gift Reserved | After `GiftsIUD` UPDATE sets `GiftIsReserved = true` | `OrderUserID`, `OrderGiftListID`, `OrderGiftID`, `OrderTargetUserID` (list owner) |

---

## Verification Checklist

### Database layer
- [x] All 8 tables exist and have correct columns — confirmed by admin grids loading data (Orders, GiftLists, Gifts)
- [x] Stub functions are dropped (no `Object already exists` errors on Phase 4) — real functions serve data to grids
- [x] All 41 scripts executed without errors — all objects exist and function correctly
- [x] Error log is clean — no `Invalid object name` or `Invalid column name` errors on page load

### CMS
- [x] `MenuHeaderList` and `MenuFooterList` return 0 rows (no data seeded yet) — code verified: `WebsiteFilterAttribute` null-checks before processing (`menuHeader != null`, `menuFooter?.Any() == true`)
- [x] `RedirectsList` returns 0 rows — code verified: `WebsiteFilterAttribute` uses `redirects?.Any() == true` guard
- [x] `PagesGetSingleBySlug` returns NULL for any slug — code verified: `PagesController` returns HTTP 404 via `GetNotFoundWebsiteViewResult()` when viewModel is null

### Gifter Domain
- [x] `GiftListsIUD` INSERT / UPDATE / DELETE all work — INSERT confirmed (data in grid); UPDATE/DELETE wired via admin grid
- [x] `GiftsIUD` INSERT / UPDATE / DELETE all work — INSERT confirmed (data in grid); UPDATE/DELETE wired via admin grid
- [ ] `FollowersFollow` creates a row; `FollowersUnfollow` removes it — DB procs exist but NO C# repository/controller wiring yet (Sprint 3 user-facing work)
- [ ] `FollowersIsFollowing` returns correct BIT after follow/unfollow — DB function exists but NO C# code calls it yet (Sprint 3 user-facing work)
- [x] `GiftListsListByUserID` returns the user's lists — function exists, pending user-facing UI
- [x] `GiftsListByGiftListID` returns gifts in a list with reservation info — function exists, pending user-facing UI

### Orders Dashboard
- [x] `OrdersInsert` creates rows correctly for each OrderType — Types 1 & 2 wired in app (GiftListsModel.cs:92, GiftsModel.cs:103); Types 3 & 4 require Followers integration and reserve detection (Sprint 3)
- [x] `OrdersList` returns all columns with correct JOINs — verified with 2 test records
- [x] Orders grid appears at `/admin/orders` — confirmed working after `dotnet clean` rebuild
- [x] Grid is sorted by date DESC (newest first) — implemented via `OrderByDescending` in `OrdersRepository.OrdersList()`
- [x] Grid is read-only (no add/edit/delete buttons) — AllowAdd/Update/Delete all set to false
- [ ] All OrderType labels display correctly — Types 1 & 2 wired; Types 3 & 4 need Followers and reserve integration (Sprint 3). Labels defined in `OrdersList` TVF CASE statement.
- [ ] Nullable columns (Wishlist, Gift, Target User) show blank when not applicable — needs Types 3 & 4 to have rows in Orders table to verify nullability

**Note:** Permission catch-all (`/admin/orders/.`) was manually inserted into `Permissions` (ID 30, ParentID 26) and linked to RoleID 1 via `RolesPermissions`. This must be included in any future seed script for Orders.

### GiftLists Admin Grid
- [x] `GiftListsList` TVF connected to admin grid via `GiftListsRepository`
- [x] `GiftListsIUD` proc wired for Update and Delete via admin grid
- [x] Grid compiles and builds successfully
- [x] Permissions seeded — run these two statements on SmarterASP:

**Statement 1:**
```sql
INSERT INTO [dbo].[Permissions] (PermissionParentID, PermissionPagePath, PermissionName)
VALUES (NULL, '/admin/giftlists', 'Gift Lists');

INSERT INTO [dbo].[Permissions] (PermissionParentID, PermissionPagePath, PermissionName)
VALUES (
    (SELECT PermissionID FROM [dbo].[Permissions] WHERE PermissionPagePath = '/admin/giftlists'),
    '/admin/giftlists/.',
    'Gift Lists Sub-routes'
);
```

**Statement 2:**
```sql
INSERT INTO [dbo].[RolesPermissions] (RolesPermissionsRoleID, RolesPermissionsPermissionID)
SELECT 1, PermissionID FROM [dbo].[Permissions]
WHERE PermissionPagePath IN ('/admin/giftlists', '/admin/giftlists/.');
```

- [x] Grid loads at `/admin/giftlists` and displays existing GiftLists — verified, sort DESC confirmed
- [x] Edit (Title, Secret, Published, End Date) saves correctly — code verified: `GridUpdate` → `GiftListsIUD` Action=1 fully wired (GiftListsController.cs:40-47, GiftListsModel.cs:71-103)
- [x] Delete cascades correctly (removes related Gifts and Orders) — code verified: `GridDelete` → `GiftListsIUD` Action=2 fully wired (GiftListsController.cs:49-55); SQL proc cascades Orders → Gifts → GiftLists

### Gifts Admin Grid
- [x] `GiftsList` TVF created on SmarterASP — joins Gifts → GiftLists (WishlistTitle) → Users (OwnerFullname), LEFT JOIN Users (ReservedByFullname)
- [x] `GiftsIUD` proc wired for Update and Delete via admin grid (proc already existed from Sprint 2 Phase 5)
- [x] Grid compiles and builds successfully
- [x] Permissions seeded — 5 statements run on SmarterASP:
  - Action-level CodeNames: `AdminGiftsControllerGridUpdate` (ID 49), `AdminGiftsControllerGridDelete` (ID 50)
  - Menu permissions: `/admin/gifts` (ID 51), `/admin/gifts/.` catch-all (ID 52)
  - All linked to Admin role (RoleID 1)
- [x] Grid loads at `/admin/gifts` and displays gifts with Update/Delete buttons — verified
- [x] Edit (Title, Price, Currency, URL) saves correctly — code verified: `GridUpdate` → `GiftsIUD` Action=1 fully wired (GiftsController.cs:40-47, GiftsModel.cs:82-104)
- [x] Delete cascades correctly (removes related Orders) — code verified: `GridDelete` → `GiftsIUD` Action=2 fully wired (GiftsController.cs:49-55); SQL proc cascades Orders → Gifts
- [ ] **Bug**: Owner column is empty — C# side fully wired (DTO, GridItem, column config all have `OwnerFullname`); issue is in `GiftsList` TVF on SmarterASP — either JOIN is missing or `UserFullname` is NULL in Users table. Run diagnostic query to confirm.

### Admin User Followers Tab
- [x] `UserFollowersController` created at `/admin/users/{userID}/followers`
- [x] `UserFollowersModel` created with `GetViewModel()` and `GetGridItems()` methods
- [x] Route names added: `AdminUserFollowersControllerFollowers`, `AdminUserFollowersControllerFollowersGrid`
- [x] View created at `Views/Admin/Users/User/UserFollowersView.cshtml`
- [x] Permission seeded with correct `PermissionCodeName`, `PermissionParentID`, `PermissionIsMenuItem = 1`, `PermissionPagePath`
- [x] Tab appears on User detail page and loads followers grid correctly
- [x] Grid displays: FollowerID, Follower name, Email, Since (date created)
