# Sprint 1: Admin Panel — Database & Core Functionality

**Goal:** Wire the admin panel to the database. Login, dashboard, and Users / Roles / Permissions management must fully work end-to-end.

**Duration:** ~2 weeks
**Status:** ✅ Completed

---

## What This Sprint Covers

| Area | In Scope | Out of Scope |
|---|---|---|
| Auth | Login, session, logout | Social login |
| Dashboard | Loads without error | Live stats (hardcoded cards are fine) |
| Users | Full grid CRUD + properties page | Avatar upload |
| Roles | Full grid CRUD | — |
| Permissions | Full tree CRUD | — |
| Roles ↔ Permissions | Assignment screen | — |
| System Properties | Load & save config | Email/AWS test buttons |
| CMS (Products, Blog, Pages…) | — | Sprint 2 |
| Gifter domain (GiftLists, Gifts, Followers) | — | Sprint 2 |

---

## Critical: SmarterASP Constraints

SmarterASP's SQL editor **does not support `GO`**. Every script below must be executed as a **single, separate run**. You cannot paste the entire file at once — each numbered script is one execution.

Run the scripts **exactly in the order listed**. Foreign keys and function references create hard dependencies between them.

---

## Execution Order (26 scripts)

| # | Script | Type | Why this order |
|---|---|---|---|
| 1–5 | Tables | DDL | Everything else references these |
| 6–11 | Stored Procedures | DDL | IUD procs needed before seed |
| 12–22 | Functions | DDL | Read functions needed before app works |
| 23 | Seed: Admin Role | Data | RoleID = 1; Users table FK and SetupController both assume this |
| 24 | Seed: Permissions | Data | Must come after Roles (no FK, but logical dependency) |
| 25 | Seed: RolesPermissions | Data | Links Role 1 to all 25 permissions |
| 26 | Seed: SystemProperties | Data | Dashboard reads ProjectName on every page load |

After all 26 scripts: run the SetupController endpoint to create the admin user (see Phase 5).

---

## Phase 1 — Create Tables (Scripts 1–5)

### Script 1 — Roles

```sql
CREATE TABLE [dbo].[Roles] (
    [RoleID]         INT          IDENTITY(1,1) NOT NULL,
    [RoleName]       NVARCHAR(100) NOT NULL,
    [RoleCode]       INT           NOT NULL,
    [RoleDateCreated] DATETIME    NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY ([RoleID])
);
```

### Script 2 — Users

```sql
CREATE TABLE [dbo].[Users] (
    [UserID]                 INT           IDENTITY(1,1) NOT NULL,
    [RoleID]                 INT           NULL,
    [UserEmail]              NVARCHAR(255) NOT NULL UNIQUE,
    [UserPassword]           NVARCHAR(255) NOT NULL,
    [UserFirstname]          NVARCHAR(100) NULL,
    [UserLastname]           NVARCHAR(100) NULL,
    [UserFullname]           NVARCHAR(255) NULL,
    [UserBirthdate]          DATETIME      NULL,
    [UserPhoneNumberMobile]  NVARCHAR(50)  NULL,
    [UserPersonalNumber]     NVARCHAR(50)  NULL,
    [UserAvatarFilename]     NVARCHAR(255) NULL,
    [UserIsActive]           BIT           NOT NULL DEFAULT 1,
    [UserDateCreated]        DATETIME      NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY ([UserID]),
    FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Roles]([RoleID])
);
```

### Script 3 — Permissions

```sql
CREATE TABLE [dbo].[Permissions] (
    [PermissionID]          INT           IDENTITY(1,1) NOT NULL,
    [PermissionParentID]    INT           NULL,
    [PermissionCaption]     NVARCHAR(255) NOT NULL,
    [PermissionCaptionEng]  NVARCHAR(255) NULL,
    [PermissionPagePath]    NVARCHAR(500) NULL,
    [PermissionCodeName]    NVARCHAR(255) NULL,
    [PermissionCode]        NVARCHAR(255) NULL,
    [PermissionIsMenuItem]  BIT           NOT NULL DEFAULT 0,
    [PermissionMenuIcon]    NVARCHAR(100) NULL,
    [PermissionSortIndex]   INT           NOT NULL DEFAULT 0,
    [PermissionMenuTitle]   NVARCHAR(255) NULL,
    [PermissionMenuTitleEng] NVARCHAR(255) NULL,
    [PermissionDateCreated] DATETIME      NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY ([PermissionID]),
    FOREIGN KEY ([PermissionParentID]) REFERENCES [dbo].[Permissions]([PermissionID])
);
```

### Script 4 — RolesPermissions

```sql
CREATE TABLE [dbo].[RolesPermissions] (
    [RoleID]       INT NOT NULL,
    [PermissionID] INT NOT NULL,
    PRIMARY KEY ([RoleID], [PermissionID]),
    FOREIGN KEY ([RoleID])       REFERENCES [dbo].[Roles]([RoleID]),
    FOREIGN KEY ([PermissionID]) REFERENCES [dbo].[Permissions]([PermissionID])
);
```

### Script 5 — SystemProperties

Single-row config table. All columns nullable except ProjectName at runtime.

```sql
CREATE TABLE [dbo].[SystemProperties] (
    [ProjectName]            NVARCHAR(255)  NULL,
    [AdminEmails]            NVARCHAR(MAX)  NULL,
    [DeveloperEmails]        NVARCHAR(MAX)  NULL,
    [ContactEmail]           NVARCHAR(255)  NULL,
    [ContactPhone]           NVARCHAR(50)   NULL,
    [ContactAddress]         NVARCHAR(MAX)  NULL,
    [ContactAddressEng]      NVARCHAR(MAX)  NULL,
    [Facebook]               NVARCHAR(500)  NULL,
    [Instagram]              NVARCHAR(500)  NULL,
    [Twitter]                NVARCHAR(500)  NULL,
    [YouTube]                NVARCHAR(500)  NULL,
    [LinkedIn]               NVARCHAR(500)  NULL,
    [GoogleMapsIFrame]       NVARCHAR(MAX)  NULL,
    [ScriptHeader]           NVARCHAR(MAX)  NULL,
    [ScriptBodyStart]        NVARCHAR(MAX)  NULL,
    [ScriptBodyEnd]          NVARCHAR(MAX)  NULL,
    [SmtpEnabled]            BIT            NULL DEFAULT 0,
    [SmtpAddress]            NVARCHAR(255)  NULL,
    [SmtpPort]               INT            NULL,
    [SmtpUsername]            NVARCHAR(255)  NULL,
    [SmtpPassword]           NVARCHAR(255)  NULL,
    [SmtpIsSSL]              BIT            NULL DEFAULT 0,
    [SmtpFromAddress]        NVARCHAR(255)  NULL,
    [MailgunEnabled]         BIT            NULL DEFAULT 0,
    [MailgunBaseUrl]         NVARCHAR(500)  NULL,
    [MailgunApiKey]          NVARCHAR(500)  NULL,
    [MailgunDomain]          NVARCHAR(255)  NULL,
    [MailgunFromAddress]     NVARCHAR(255)  NULL,
    [MailgunSigningKey]      NVARCHAR(500)  NULL,
    [Office365Enabled]       BIT            NULL DEFAULT 0,
    [Office365TenantId]      NVARCHAR(255)  NULL,
    [Office365ClientId]      NVARCHAR(255)  NULL,
    [Office365ClientSecret]  NVARCHAR(500)  NULL,
    [Office365UserId]        NVARCHAR(255)  NULL,
    [AwsEnabled]             BIT            NULL DEFAULT 0,
    [AwsAccessKeyId]         NVARCHAR(255)  NULL,
    [AwsSecretAccessKey]     NVARCHAR(500)  NULL,
    [AwsRegion]              NVARCHAR(50)   NULL,
    [AwsBucketName]          NVARCHAR(255)  NULL,
    [AzureEnabled]           BIT            NULL DEFAULT 0,
    [AzureConnectionString]  NVARCHAR(MAX)  NULL,
    [AzureContainerName]     NVARCHAR(255)  NULL,
    [ReCaptchaEnabled]       BIT            NULL DEFAULT 0,
    [ReCaptchaSiteKey]       NVARCHAR(500)  NULL,
    [ReCaptchaSecretKey]     NVARCHAR(500)  NULL
);
```

---

## Phase 2 — Stored Procedures (Scripts 6–11)

All IUD procedures follow the 63BITS pattern: `@Action TINYINT` (0 = INSERT, 1 = UPDATE, 2 = DELETE), `@ID INT OUTPUT`, and a single `@Json NVARCHAR(MAX)` that carries the DTO fields. Boolean values arrive in JSON as `true`/`false` strings and must be converted via CASE, not CAST.

### Script 6 — UsersIUD

```sql
CREATE OR ALTER PROCEDURE [dbo].[UsersIUD]
(
    @Action   TINYINT,
    @UserID   INT OUTPUT,
    @UserJson NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 0 -- INSERT
    BEGIN
        INSERT INTO [dbo].[Users] (
            RoleID, UserEmail, UserPassword, UserFirstname, UserLastname, UserFullname,
            UserBirthdate, UserPhoneNumberMobile, UserPersonalNumber, UserAvatarFilename, UserIsActive
        )
        VALUES (
            CAST(JSON_VALUE(@UserJson, '$.RoleID') AS INT),
            JSON_VALUE(@UserJson, '$.UserEmail'),
            JSON_VALUE(@UserJson, '$.UserPassword'),
            JSON_VALUE(@UserJson, '$.UserFirstname'),
            JSON_VALUE(@UserJson, '$.UserLastname'),
            JSON_VALUE(@UserJson, '$.UserFullname'),
            CAST(JSON_VALUE(@UserJson, '$.UserBirthdate') AS DATETIME),
            JSON_VALUE(@UserJson, '$.UserPhoneNumberMobile'),
            JSON_VALUE(@UserJson, '$.UserPersonalNumber'),
            JSON_VALUE(@UserJson, '$.UserAvatarFilename'),
            CASE WHEN JSON_VALUE(@UserJson, '$.UserIsActive') = 'true' THEN 1 ELSE 0 END
        );
        SET @UserID = SCOPE_IDENTITY();
    END
    ELSE IF @Action = 1 -- UPDATE
    BEGIN
        UPDATE [dbo].[Users]
        SET
            RoleID                 = COALESCE(CAST(JSON_VALUE(@UserJson, '$.RoleID') AS INT), RoleID),
            UserEmail              = COALESCE(JSON_VALUE(@UserJson, '$.UserEmail'), UserEmail),
            UserPassword           = COALESCE(JSON_VALUE(@UserJson, '$.UserPassword'), UserPassword),
            UserFirstname          = COALESCE(JSON_VALUE(@UserJson, '$.UserFirstname'), UserFirstname),
            UserLastname           = COALESCE(JSON_VALUE(@UserJson, '$.UserLastname'), UserLastname),
            UserFullname           = COALESCE(JSON_VALUE(@UserJson, '$.UserFullname'), UserFullname),
            UserBirthdate          = COALESCE(CAST(JSON_VALUE(@UserJson, '$.UserBirthdate') AS DATETIME), UserBirthdate),
            UserPhoneNumberMobile  = COALESCE(JSON_VALUE(@UserJson, '$.UserPhoneNumberMobile'), UserPhoneNumberMobile),
            UserPersonalNumber     = COALESCE(JSON_VALUE(@UserJson, '$.UserPersonalNumber'), UserPersonalNumber),
            UserAvatarFilename     = COALESCE(JSON_VALUE(@UserJson, '$.UserAvatarFilename'), UserAvatarFilename),
            UserIsActive = CASE
                WHEN JSON_VALUE(@UserJson, '$.UserIsActive') IS NOT NULL
                THEN CASE WHEN JSON_VALUE(@UserJson, '$.UserIsActive') = 'true' THEN 1 ELSE 0 END
                ELSE UserIsActive
            END
        WHERE UserID = @UserID;
    END
    ELSE IF @Action = 2 -- DELETE
    BEGIN
        DELETE FROM [dbo].[Users] WHERE UserID = @UserID;
    END
END;
```

### Script 7 — RolesIUD

Deleting a role cascades to RolesPermissions first.

```sql
CREATE OR ALTER PROCEDURE [dbo].[RolesIUD]
(
    @Action   TINYINT,
    @RoleID   INT OUTPUT,
    @RoleJson NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 0 -- INSERT
    BEGIN
        INSERT INTO [dbo].[Roles] (RoleName, RoleCode)
        VALUES (
            JSON_VALUE(@RoleJson, '$.RoleName'),
            CAST(JSON_VALUE(@RoleJson, '$.RoleCode') AS INT)
        );
        SET @RoleID = SCOPE_IDENTITY();
    END
    ELSE IF @Action = 1 -- UPDATE
    BEGIN
        UPDATE [dbo].[Roles]
        SET
            RoleName = COALESCE(JSON_VALUE(@RoleJson, '$.RoleName'), RoleName),
            RoleCode = COALESCE(CAST(JSON_VALUE(@RoleJson, '$.RoleCode') AS INT), RoleCode)
        WHERE RoleID = @RoleID;
    END
    ELSE IF @Action = 2 -- DELETE
    BEGIN
        DELETE FROM [dbo].[RolesPermissions] WHERE RoleID = @RoleID;
        DELETE FROM [dbo].[Roles] WHERE RoleID = @RoleID;
    END
END;
```

### Script 8 — PermissionsIUD

Single-item delete only. For cascading (permission + all children), use `PermissionsDeleteRecursive` (Script 9).

```sql
CREATE OR ALTER PROCEDURE [dbo].[PermissionsIUD]
(
    @Action         TINYINT,
    @PermissionID   INT OUTPUT,
    @PermissionJson NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 0 -- INSERT
    BEGIN
        INSERT INTO [dbo].[Permissions] (
            PermissionParentID, PermissionCaption, PermissionCaptionEng,
            PermissionPagePath, PermissionCodeName, PermissionCode,
            PermissionIsMenuItem, PermissionMenuIcon, PermissionSortIndex,
            PermissionMenuTitle, PermissionMenuTitleEng
        )
        VALUES (
            CAST(JSON_VALUE(@PermissionJson, '$.PermissionParentID') AS INT),
            JSON_VALUE(@PermissionJson, '$.PermissionCaption'),
            JSON_VALUE(@PermissionJson, '$.PermissionCaptionEng'),
            JSON_VALUE(@PermissionJson, '$.PermissionPagePath'),
            JSON_VALUE(@PermissionJson, '$.PermissionCodeName'),
            JSON_VALUE(@PermissionJson, '$.PermissionCode'),
            CASE WHEN JSON_VALUE(@PermissionJson, '$.PermissionIsMenuItem') = 'true' THEN 1 ELSE 0 END,
            JSON_VALUE(@PermissionJson, '$.PermissionMenuIcon'),
            COALESCE(CAST(JSON_VALUE(@PermissionJson, '$.PermissionSortIndex') AS INT), 0),
            JSON_VALUE(@PermissionJson, '$.PermissionMenuTitle'),
            JSON_VALUE(@PermissionJson, '$.PermissionMenuTitleEng')
        );
        SET @PermissionID = SCOPE_IDENTITY();
    END
    ELSE IF @Action = 1 -- UPDATE
    BEGIN
        UPDATE [dbo].[Permissions]
        SET
            PermissionParentID  = CASE WHEN JSON_VALUE(@PermissionJson, '$.PermissionParentID') IS NOT NULL THEN CAST(JSON_VALUE(@PermissionJson, '$.PermissionParentID') AS INT) ELSE PermissionParentID END,
            PermissionCaption   = COALESCE(JSON_VALUE(@PermissionJson, '$.PermissionCaption'), PermissionCaption),
            PermissionCaptionEng = COALESCE(JSON_VALUE(@PermissionJson, '$.PermissionCaptionEng'), PermissionCaptionEng),
            PermissionPagePath  = COALESCE(JSON_VALUE(@PermissionJson, '$.PermissionPagePath'), PermissionPagePath),
            PermissionCodeName  = COALESCE(JSON_VALUE(@PermissionJson, '$.PermissionCodeName'), PermissionCodeName),
            PermissionCode      = COALESCE(JSON_VALUE(@PermissionJson, '$.PermissionCode'), PermissionCode),
            PermissionIsMenuItem = CASE
                WHEN JSON_VALUE(@PermissionJson, '$.PermissionIsMenuItem') IS NOT NULL
                THEN CASE WHEN JSON_VALUE(@PermissionJson, '$.PermissionIsMenuItem') = 'true' THEN 1 ELSE 0 END
                ELSE PermissionIsMenuItem
            END,
            PermissionMenuIcon    = COALESCE(JSON_VALUE(@PermissionJson, '$.PermissionMenuIcon'), PermissionMenuIcon),
            PermissionSortIndex   = COALESCE(CAST(JSON_VALUE(@PermissionJson, '$.PermissionSortIndex') AS INT), PermissionSortIndex),
            PermissionMenuTitle   = COALESCE(JSON_VALUE(@PermissionJson, '$.PermissionMenuTitle'), PermissionMenuTitle),
            PermissionMenuTitleEng = COALESCE(JSON_VALUE(@PermissionJson, '$.PermissionMenuTitleEng'), PermissionMenuTitleEng)
        WHERE PermissionID = @PermissionID;
    END
    ELSE IF @Action = 2 -- DELETE
    BEGIN
        DELETE FROM [dbo].[RolesPermissions] WHERE PermissionID = @PermissionID;
        DELETE FROM [dbo].[Permissions] WHERE PermissionID = @PermissionID;
    END
END;
```

### Script 9 — PermissionsDeleteRecursive

Deletes a permission and all its descendants. Uses a temp table because SmarterASP cannot reuse a CTE across two DELETE statements.

```sql
CREATE OR ALTER PROCEDURE [dbo].[PermissionsDeleteRecursive]
(
    @PermissionID INT
)
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #ToDelete (PermissionID INT);

    WITH cte AS
    (
        SELECT PermissionID FROM [dbo].[Permissions] WHERE PermissionID = @PermissionID
        UNION ALL
        SELECT p.PermissionID
        FROM [dbo].[Permissions] p
        INNER JOIN cte ON p.PermissionParentID = cte.PermissionID
    )
    INSERT INTO #ToDelete SELECT PermissionID FROM cte;

    DELETE FROM [dbo].[RolesPermissions] WHERE PermissionID IN (SELECT PermissionID FROM #ToDelete);
    DELETE FROM [dbo].[Permissions]      WHERE PermissionID IN (SELECT PermissionID FROM #ToDelete);

    DROP TABLE #ToDelete;
END;
```

### Script 10 — RolesPermissionsUpdate

Replaces all permission assignments for a role in one shot. Receives a JSON array of permission IDs (e.g. `[1,5,12,18]`).

```sql
CREATE OR ALTER PROCEDURE [dbo].[RolesPermissionsUpdate]
(
    @RoleID            INT,
    @PermissionIdsJson NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[RolesPermissions] WHERE RoleID = @RoleID;

    IF @PermissionIdsJson IS NOT NULL AND @PermissionIdsJson <> '[]'
    BEGIN
        INSERT INTO [dbo].[RolesPermissions] (RoleID, PermissionID)
        SELECT @RoleID, CAST([value] AS INT)
        FROM OPENJSON(@PermissionIdsJson)
        WHERE [value] IS NOT NULL;
    END
END;
```

### Script 11 — SystemPropertiesUpdate

Updates the single config row. Takes one JSON blob containing all fields.

```sql
CREATE OR ALTER PROCEDURE [dbo].[SystemPropertiesUpdate]
(
    @SystemPropertiesJson NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[SystemProperties]
    SET
        ProjectName          = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.ProjectName'), ProjectName),
        AdminEmails          = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.AdminEmails'), AdminEmails),
        DeveloperEmails      = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.DeveloperEmails'), DeveloperEmails),
        ContactEmail         = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.ContactEmail'), ContactEmail),
        ContactPhone         = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.ContactPhone'), ContactPhone),
        ContactAddress       = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.ContactAddress'), ContactAddress),
        ContactAddressEng    = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.ContactAddressEng'), ContactAddressEng),
        Facebook             = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.Facebook'), Facebook),
        Instagram            = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.Instagram'), Instagram),
        Twitter              = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.Twitter'), Twitter),
        YouTube              = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.YouTube'), YouTube),
        LinkedIn             = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.LinkedIn'), LinkedIn),
        GoogleMapsIFrame     = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.GoogleMapsIFrame'), GoogleMapsIFrame),
        ScriptHeader         = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.ScriptHeader'), ScriptHeader),
        ScriptBodyStart      = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.ScriptBodyStart'), ScriptBodyStart),
        ScriptBodyEnd        = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.ScriptBodyEnd'), ScriptBodyEnd),
        SmtpEnabled          = CASE WHEN JSON_VALUE(@SystemPropertiesJson, '$.SmtpEnabled')     IS NOT NULL THEN CASE WHEN JSON_VALUE(@SystemPropertiesJson, '$.SmtpEnabled')     = 'true' THEN 1 ELSE 0 END ELSE SmtpEnabled     END,
        SmtpAddress          = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.SmtpAddress'), SmtpAddress),
        SmtpPort             = COALESCE(CAST(JSON_VALUE(@SystemPropertiesJson, '$.SmtpPort') AS INT), SmtpPort),
        SmtpUsername         = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.SmtpUsername'), SmtpUsername),
        SmtpPassword         = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.SmtpPassword'), SmtpPassword),
        SmtpIsSSL            = CASE WHEN JSON_VALUE(@SystemPropertiesJson, '$.SmtpIsSSL')       IS NOT NULL THEN CASE WHEN JSON_VALUE(@SystemPropertiesJson, '$.SmtpIsSSL')       = 'true' THEN 1 ELSE 0 END ELSE SmtpIsSSL       END,
        SmtpFromAddress      = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.SmtpFromAddress'), SmtpFromAddress),
        MailgunEnabled       = CASE WHEN JSON_VALUE(@SystemPropertiesJson, '$.MailgunEnabled')   IS NOT NULL THEN CASE WHEN JSON_VALUE(@SystemPropertiesJson, '$.MailgunEnabled')   = 'true' THEN 1 ELSE 0 END ELSE MailgunEnabled   END,
        MailgunBaseUrl       = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.MailgunBaseUrl'), MailgunBaseUrl),
        MailgunApiKey        = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.MailgunApiKey'), MailgunApiKey),
        MailgunDomain        = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.MailgunDomain'), MailgunDomain),
        MailgunFromAddress   = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.MailgunFromAddress'), MailgunFromAddress),
        MailgunSigningKey    = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.MailgunSigningKey'), MailgunSigningKey),
        Office365Enabled     = CASE WHEN JSON_VALUE(@SystemPropertiesJson, '$.Office365Enabled') IS NOT NULL THEN CASE WHEN JSON_VALUE(@SystemPropertiesJson, '$.Office365Enabled') = 'true' THEN 1 ELSE 0 END ELSE Office365Enabled END,
        Office365TenantId    = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.Office365TenantId'), Office365TenantId),
        Office365ClientId    = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.Office365ClientId'), Office365ClientId),
        Office365ClientSecret = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.Office365ClientSecret'), Office365ClientSecret),
        Office365UserId      = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.Office365UserId'), Office365UserId),
        AwsEnabled           = CASE WHEN JSON_VALUE(@SystemPropertiesJson, '$.AwsEnabled')      IS NOT NULL THEN CASE WHEN JSON_VALUE(@SystemPropertiesJson, '$.AwsEnabled')      = 'true' THEN 1 ELSE 0 END ELSE AwsEnabled      END,
        AwsAccessKeyId       = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.AwsAccessKeyId'), AwsAccessKeyId),
        AwsSecretAccessKey   = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.AwsSecretAccessKey'), AwsSecretAccessKey),
        AwsRegion            = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.AwsRegion'), AwsRegion),
        AwsBucketName        = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.AwsBucketName'), AwsBucketName),
        AzureEnabled         = CASE WHEN JSON_VALUE(@SystemPropertiesJson, '$.AzureEnabled')    IS NOT NULL THEN CASE WHEN JSON_VALUE(@SystemPropertiesJson, '$.AzureEnabled')    = 'true' THEN 1 ELSE 0 END ELSE AzureEnabled    END,
        AzureConnectionString = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.AzureConnectionString'), AzureConnectionString),
        AzureContainerName   = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.AzureContainerName'), AzureContainerName),
        ReCaptchaEnabled     = CASE WHEN JSON_VALUE(@SystemPropertiesJson, '$.ReCaptchaEnabled') IS NOT NULL THEN CASE WHEN JSON_VALUE(@SystemPropertiesJson, '$.ReCaptchaEnabled') = 'true' THEN 1 ELSE 0 END ELSE ReCaptchaEnabled END,
        ReCaptchaSiteKey     = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.ReCaptchaSiteKey'), ReCaptchaSiteKey),
        ReCaptchaSecretKey   = COALESCE(JSON_VALUE(@SystemPropertiesJson, '$.ReCaptchaSecretKey'), ReCaptchaSecretKey);
END;
```

---

## Phase 3 — Functions (Scripts 12–22)

Two function types are used:
- **Scalar functions** — called as `SELECT dbo.FnName(…)`. Return a single `NVARCHAR(MAX)` containing JSON that the repository deserializes to a DTO.
- **Table-valued functions (TVF)** — called as `SELECT col1, col2 … FROM dbo.FnName(…)`. Column names **must** match the C# DTO property names exactly, because `SqlQueryBuilder` builds the SELECT from reflection on the DTO type.

### Script 12 — UsersGetSingleByID

Returns a full `UserDTO` as JSON, including the nested `Permissions` array for the user's role. `JSON_QUERY(COALESCE(…, '[]'))` ensures an empty array rather than null when the role has no permissions.

```sql
CREATE FUNCTION [dbo].[UsersGetSingleByID](@UserID INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN (
        SELECT
            u.UserID,
            u.UserFullname,
            u.UserFirstname,
            u.UserLastname,
            u.UserBirthdate,
            u.UserEmail,
            u.UserPassword,
            u.UserPhoneNumberMobile,
            u.UserIsActive,
            u.UserAvatarFilename,
            u.UserDateCreated,
            u.RoleID,
            r.RoleCode,
            r.RoleName,
            JSON_QUERY(
                COALESCE(
                    (SELECT
                        p.PermissionID,
                        p.PermissionParentID,
                        p.PermissionCaption,
                        p.PermissionCaptionEng,
                        p.PermissionPagePath,
                        p.PermissionCodeName,
                        p.PermissionCode,
                        p.PermissionIsMenuItem,
                        p.PermissionMenuIcon,
                        p.PermissionSortIndex,
                        p.PermissionMenuTitle,
                        p.PermissionMenuTitleEng
                    FROM [dbo].[RolesPermissions] rp
                    INNER JOIN [dbo].[Permissions] p ON rp.PermissionID = p.PermissionID
                    WHERE rp.RoleID = u.RoleID
                    ORDER BY p.PermissionSortIndex
                    FOR JSON PATH
                    ),
                    '[]'
                )
            ) AS Permissions
        FROM [dbo].[Users] u
        LEFT JOIN [dbo].[Roles] r ON u.RoleID = r.RoleID
        WHERE u.UserID = @UserID
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    )
END;
```

### Script 13 — UsersGetSingleByEmailAndPassword

Same structure as Script 12, but filtered by credentials. Only returns active users.

```sql
CREATE FUNCTION [dbo].[UsersGetSingleByEmailAndPassword](@UserEmail NVARCHAR(255), @UserPassword NVARCHAR(255))
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN (
        SELECT
            u.UserID,
            u.UserFullname,
            u.UserFirstname,
            u.UserLastname,
            u.UserBirthdate,
            u.UserEmail,
            u.UserPassword,
            u.UserPhoneNumberMobile,
            u.UserIsActive,
            u.UserAvatarFilename,
            u.UserDateCreated,
            u.RoleID,
            r.RoleCode,
            r.RoleName,
            JSON_QUERY(
                COALESCE(
                    (SELECT
                        p.PermissionID,
                        p.PermissionParentID,
                        p.PermissionCaption,
                        p.PermissionCaptionEng,
                        p.PermissionPagePath,
                        p.PermissionCodeName,
                        p.PermissionCode,
                        p.PermissionIsMenuItem,
                        p.PermissionMenuIcon,
                        p.PermissionSortIndex,
                        p.PermissionMenuTitle,
                        p.PermissionMenuTitleEng
                    FROM [dbo].[RolesPermissions] rp
                    INNER JOIN [dbo].[Permissions] p ON rp.PermissionID = p.PermissionID
                    WHERE rp.RoleID = u.RoleID
                    ORDER BY p.PermissionSortIndex
                    FOR JSON PATH
                    ),
                    '[]'
                )
            ) AS Permissions
        FROM [dbo].[Users] u
        LEFT JOIN [dbo].[Roles] r ON u.RoleID = r.RoleID
        WHERE u.UserEmail    = @UserEmail
          AND u.UserPassword = @UserPassword
          AND u.UserIsActive = 1
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    )
END;
```

### Script 14 — UsersIsEmailUnique

Returns 1 if no other user has this email (excluding the user being edited, if provided).

```sql
CREATE FUNCTION [dbo].[UsersIsEmailUnique](@UserEmail NVARCHAR(255), @UserID INT = NULL)
RETURNS BIT
AS
BEGIN
    DECLARE @IsUnique BIT;

    SELECT @IsUnique = CASE
        WHEN COUNT(*) = 0 THEN 1
        ELSE 0
    END
    FROM [dbo].[Users]
    WHERE UserEmail = @UserEmail
      AND (@UserID IS NULL OR UserID <> @UserID);

    RETURN @IsUnique;
END;
```

### Script 15 — UsersList

TVF. Column names match `UsersListDTO` properties exactly.

```sql
CREATE FUNCTION [dbo].[UsersList]()
RETURNS TABLE
AS
RETURN
    SELECT
        UserID,
        RoleID,
        UserEmail,
        UserPassword,
        UserFirstname,
        UserLastname,
        UserFullname,
        UserBirthdate,
        UserPhoneNumberMobile,
        UserPersonalNumber,
        UserAvatarFilename,
        UserIsActive,
        UserDateCreated
    FROM [dbo].[Users];
```

### Script 16 — RolesList

```sql
CREATE FUNCTION [dbo].[RolesList]()
RETURNS TABLE
AS
RETURN
    SELECT RoleID, RoleName, RoleCode, RoleDateCreated
    FROM [dbo].[Roles];
```

### Script 17 — RolesListAsKeyValueTuple

TVF returning `Key` / `Value` columns. Used to populate the Role dropdown on the Users grid.

```sql
CREATE FUNCTION [dbo].[RolesListAsKeyValueTuple](@IsRoleCodeAsKey BIT = 0)
RETURNS TABLE
AS
RETURN
    SELECT
        CASE WHEN @IsRoleCodeAsKey = 1 THEN RoleCode ELSE RoleID END AS [Key],
        RoleName AS [Value]
    FROM [dbo].[Roles];
```

### Script 18 — RolesListAsKeyValueSelectedTuple

Same as Script 17, plus an `IsSelected` flag for pre-selecting a value in dropdowns.

```sql
CREATE FUNCTION [dbo].[RolesListAsKeyValueSelectedTuple](@SelectedValue INT = NULL, @IsRoleCodeAsKey BIT = 0)
RETURNS TABLE
AS
RETURN
    SELECT
        CASE WHEN @IsRoleCodeAsKey = 1 THEN RoleCode ELSE RoleID END AS [Key],
        RoleName AS [Value],
        CASE
            WHEN (CASE WHEN @IsRoleCodeAsKey = 1 THEN RoleCode ELSE RoleID END) = @SelectedValue THEN 1
            ELSE 0
        END AS IsSelected
    FROM [dbo].[Roles];
```

### Script 19 — PermissionsList

```sql
CREATE FUNCTION [dbo].[PermissionsList]()
RETURNS TABLE
AS
RETURN
    SELECT
        PermissionID,
        PermissionParentID,
        PermissionCaption,
        PermissionCaptionEng,
        PermissionPagePath,
        PermissionCodeName,
        PermissionCode,
        PermissionIsMenuItem,
        PermissionMenuIcon,
        PermissionSortIndex,
        PermissionMenuTitle,
        PermissionMenuTitleEng,
        PermissionDateCreated
    FROM [dbo].[Permissions];
```

### Script 20 — PermissionsListByRoleID

Returns only the permission IDs assigned to a role. Matches `PermissionsListByRoleIDDTO` (single `PermissionID` column).

```sql
CREATE FUNCTION [dbo].[PermissionsListByRoleID](@RoleID INT)
RETURNS TABLE
AS
RETURN
    SELECT rp.PermissionID
    FROM [dbo].[RolesPermissions] rp
    WHERE rp.RoleID = @RoleID;
```

### Script 21 — PermissionsListForDeleteRecursive

Returns the permission and all its descendants (used by the UI to preview what a recursive delete will remove). Inline TVF with recursive CTE — supported in SQL Server 2016+.

```sql
CREATE FUNCTION [dbo].[PermissionsListForDeleteRecursive](@PermissionID INT)
RETURNS TABLE
AS
RETURN
    WITH cte AS
    (
        SELECT
            PermissionID, PermissionParentID, PermissionCaption, PermissionCaptionEng,
            PermissionPagePath, PermissionCodeName, PermissionCode, PermissionIsMenuItem,
            PermissionMenuIcon, PermissionSortIndex, PermissionMenuTitle, PermissionMenuTitleEng,
            PermissionDateCreated
        FROM [dbo].[Permissions]
        WHERE PermissionID = @PermissionID

        UNION ALL

        SELECT
            p.PermissionID, p.PermissionParentID, p.PermissionCaption, p.PermissionCaptionEng,
            p.PermissionPagePath, p.PermissionCodeName, p.PermissionCode, p.PermissionIsMenuItem,
            p.PermissionMenuIcon, p.PermissionSortIndex, p.PermissionMenuTitle, p.PermissionMenuTitleEng,
            p.PermissionDateCreated
        FROM [dbo].[Permissions] p
        INNER JOIN cte ON p.PermissionParentID = cte.PermissionID
    )
    SELECT * FROM cte;
```

### Script 22 — SystemPropertiesGet

Scalar function. Returns the single config row as JSON. Column names become JSON keys, which must match `SystemPropertiesDTO` properties.

```sql
CREATE FUNCTION [dbo].[SystemPropertiesGet]()
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN (
        SELECT
            ProjectName, AdminEmails, DeveloperEmails,
            ContactEmail, ContactPhone, ContactAddress, ContactAddressEng,
            Facebook, Instagram, Twitter, YouTube, LinkedIn,
            GoogleMapsIFrame,
            ScriptHeader, ScriptBodyStart, ScriptBodyEnd,
            SmtpEnabled, SmtpAddress, SmtpPort, SmtpUsername, SmtpPassword, SmtpIsSSL, SmtpFromAddress,
            MailgunEnabled, MailgunBaseUrl, MailgunApiKey, MailgunDomain, MailgunFromAddress, MailgunSigningKey,
            Office365Enabled, Office365TenantId, Office365ClientId, Office365ClientSecret, Office365UserId,
            AwsEnabled, AwsAccessKeyId, AwsSecretAccessKey, AwsRegion, AwsBucketName,
            AzureEnabled, AzureConnectionString, AzureContainerName,
            ReCaptchaEnabled, ReCaptchaSiteKey, ReCaptchaSecretKey
        FROM [dbo].[SystemProperties]
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    )
END;
```

---

## Phase 4 — Seed Data (Scripts 23–26)

### Script 23 — Admin Role

This INSERT produces `RoleID = 1`. The SetupController and the permission seed both hardcode this value.

```sql
INSERT INTO [dbo].[Roles] (RoleName, RoleCode)
VALUES ('Admin', 1);
```

### Script 24 — Permissions (25 rows)

Three layers:
1. **Menu items** (rows 1–8) — visible in the sidebar. Groups 3 & 4 have no `PagePath`; their `PermissionCode` is `#` so MetisMenu treats them as toggle headers.
2. **URL catch-alls** (rows 9–12) — hidden. `PagePath` ends with `.` which, combined with the `*` appended by the `HasPermission` regex, produces `.*` — matching all sub-routes under that prefix.
3. **CodeName entries** (rows 13–25) — hidden. `PermissionCodeName` matches the route-name constants used in Model code for button-level permission checks (e.g. `ShowAddNewButton`).

This script uses `IDENTITY_INSERT` to set explicit IDs so that the RolesPermissions seed (Script 25) can reference them by number. Run the entire block as one execution.

```sql
SET IDENTITY_INSERT [dbo].[Permissions] ON;

INSERT INTO [dbo].[Permissions] (
    PermissionID, PermissionParentID,
    PermissionCaption, PermissionCaptionEng,
    PermissionPagePath, PermissionCodeName, PermissionCode,
    PermissionIsMenuItem, PermissionMenuIcon, PermissionSortIndex,
    PermissionMenuTitle, PermissionMenuTitleEng
)
VALUES
-- 1-4: Top-level menu items
(1,  NULL, 'Dashboard',            'Dashboard',            '/admin',                   'AdminHomeControllerIndex',                              NULL, 1, 'fas fa-home',       10, 'Dashboard',            'Dashboard'),
(2,  NULL, 'Users',                'Users',                '/admin/users',             'AdminUsersControllerUsers',                             NULL, 1, 'fas fa-users',      20, 'Users',                'Users'),
(3,  NULL, 'Roles & Permissions',  'Roles & Permissions',  NULL,                       NULL,                                                    '#',  1, 'fas fa-shield-alt', 30, 'Roles & Permissions',  'Roles & Permissions'),
(4,  NULL, 'System',               'System',               NULL,                       NULL,                                                    '#',  1, 'fas fa-cog',        40, 'System',               'System'),

-- 5-8: Sub-menu items (children of groups 3 and 4)
(5,  3,    'Roles',                'Roles',                '/admin/roles',             'AdminRolesControllerRoles',                             NULL, 1, 'fas fa-list',       10, 'Roles',                'Roles'),
(6,  3,    'Permissions',          'Permissions',          '/admin/permissions',       'AdminPermissionsControllerPermissions',                 NULL, 1, 'fas fa-key',        20, 'Permissions',          'Permissions'),
(7,  3,    'Roles-Permissions',    'Roles-Permissions',    '/admin/roles-permissions', 'AdminRolePermissionsControllerRolesPermissions',        NULL, 1, 'fas fa-users-cog',  30, 'Roles-Permissions',    'Roles-Permissions'),
(8,  4,    'System Properties',    'System Properties',    '/admin/system-properties', 'AdminSystemPropertiesControllerSystemProperties',      NULL, 1, 'fas fa-sliders-h',  10, 'System Properties',    'System Properties'),

-- 9-12: URL catch-all permissions (hidden, PagePath ends with . for .* regex)
(9,  2,    'Users Sub-routes',       'Users Sub-routes',       '/admin/users/.',                NULL, NULL, 0, NULL, 0, NULL, NULL),
(10, 5,    'Roles Sub-routes',       'Roles Sub-routes',       '/admin/roles/.',                NULL, NULL, 0, NULL, 0, NULL, NULL),
(11, 6,    'Permissions Sub-routes', 'Permissions Sub-routes', '/admin/permissions/.',          NULL, NULL, 0, NULL, 0, NULL, NULL),
(12, 7,    'RP Sub-routes',          'RP Sub-routes',          '/admin/roles-permissions/.',    NULL, NULL, 0, NULL, 0, NULL, NULL),

-- 13-17: Users CRUD CodeNames
(13, 9,  'Users Grid',        'Users Grid',        NULL, 'AdminUsersControllerGrid',        NULL, 0, NULL, 0, NULL, NULL),
(14, 9,  'Users Grid Add',    'Users Grid Add',    NULL, 'AdminUsersControllerGridAdd',     NULL, 0, NULL, 0, NULL, NULL),
(15, 9,  'Users Grid Update', 'Users Grid Update', NULL, 'AdminUsersControllerGridUpdate',  NULL, 0, NULL, 0, NULL, NULL),
(16, 9,  'Users Grid Delete', 'Users Grid Delete', NULL, 'AdminUsersControllerGridDelete',  NULL, 0, NULL, 0, NULL, NULL),
(17, 9,  'User Properties',   'User Properties',   NULL, 'AdminUserPropertiesControllerProperties', NULL, 0, NULL, 0, NULL, NULL),

-- 18-21: Roles CRUD CodeNames
(18, 10, 'Roles Grid',        'Roles Grid',        NULL, 'AdminRolesControllerGrid',        NULL, 0, NULL, 0, NULL, NULL),
(19, 10, 'Roles Grid Add',    'Roles Grid Add',    NULL, 'AdminRolesControllerGridAdd',     NULL, 0, NULL, 0, NULL, NULL),
(20, 10, 'Roles Grid Update', 'Roles Grid Update', NULL, 'AdminRolesControllerGridUpdate',  NULL, 0, NULL, 0, NULL, NULL),
(21, 10, 'Roles Grid Delete', 'Roles Grid Delete', NULL, 'AdminRolesControllerGridDelete',  NULL, 0, NULL, 0, NULL, NULL),

-- 22-25: Permissions CRUD CodeNames
(22, 11, 'Permissions Tree',        'Permissions Tree',        NULL, 'AdminPermissionsControllerTree',        NULL, 0, NULL, 0, NULL, NULL),
(23, 11, 'Permissions Tree Add',    'Permissions Tree Add',    NULL, 'AdminPermissionsControllerTreeAdd',     NULL, 0, NULL, 0, NULL, NULL),
(24, 11, 'Permissions Tree Update', 'Permissions Tree Update', NULL, 'AdminPermissionsControllerTreeUpdate',  NULL, 0, NULL, 0, NULL, NULL),
(25, 11, 'Permissions Tree Delete', 'Permissions Tree Delete', NULL, 'AdminPermissionsControllerTreeDelete',  NULL, 0, NULL, 0, NULL, NULL);

SET IDENTITY_INSERT [dbo].[Permissions] OFF;
```

### Script 25 — RolesPermissions

Links every permission (1–25) to the Admin role (ID 1).

```sql
INSERT INTO [dbo].[RolesPermissions] (RoleID, PermissionID)
VALUES
(1,1),(1,2),(1,3),(1,4),(1,5),(1,6),(1,7),(1,8),
(1,9),(1,10),(1,11),(1,12),(1,13),(1,14),(1,15),
(1,16),(1,17),(1,18),(1,19),(1,20),(1,21),(1,22),
(1,23),(1,24),(1,25);
```

### Script 26 — SystemProperties

Seeds the single config row. Only `ProjectName` is populated; everything else defaults to NULL.

```sql
INSERT INTO [dbo].[SystemProperties] (ProjectName)
VALUES ('Gifter');
```

---

## Phase 5 — Seed Admin User (via App)

No SQL needed here. The existing [SetupController](../../SixtyThreeBits.Web/Controllers/Admin/Auth/SetupController.cs) handles this.

1. Deploy the app (or confirm it is deployed and pointing at `db_ac3c37_gifter`).
2. Open a browser and navigate to:

```
https://<your-domain>/admin/setup/seed-admin
```

3. The page returns one of:
   - `"Admin user created successfully with ID: 1…"` — success.
   - `"Admin user already exists."` — already seeded, skip.
   - `"Failed to create admin user…"` — check the error log or DB.

Credentials after seeding: **`admin@gifter.com` / `asdf`**

---

## Phase 6 — Verification Checklist

Run through these in order after all scripts and the seed user are done:

- [x] `/admin/login` — login page renders
- [x] Login with `admin@gifter.com` / `asdf` — redirects to `/admin`
- [x] Dashboard (`/admin`) — page loads, sidebar menu shows: Dashboard, Users, Roles & Permissions (group), System (group)
- [x] Expand "Roles & Permissions" group — shows Roles, Permissions, Roles-Permissions
- [x] Expand "System" group — shows System Properties
- [x] `/admin/users` — Users grid loads with the seeded admin user; Add / Update / Delete buttons visible
- [x] Add a test user via the grid — succeeds
- [x] Update that user — succeeds
- [x] Delete that user — succeeds
- [x] `/admin/users/{id}/properties` — User detail page loads (required DROP + CREATE of `UsersGetSingleByID` — see Issues Fixed below)
- [x] `/admin/roles` — Roles grid loads with Admin, User, and Viewer roles
- [x] `/admin/permissions` — Permissions tree loads with all 25 entries
- [x] `/admin/roles-permissions` — roles grid and permissions tree both load; selecting Admin shows all 25 checked
- [x] `/admin/system-properties` — form loads, ProjectName shows "Gifter"
- [x] Logout — redirects to login

---

## Issues Fixed During Execution

### UsersGetSingleByID — pre-existing function with wrong column name
The function already existed on the server with `UserRoleID` instead of `RoleID`. `CREATE FUNCTION` silently failed (object already exists), so the stale version stayed in place. Fix: `DROP FUNCTION` first, then re-run Script 12. This caused `/admin/users/{id}/properties` to 404 because `UserFilterAttribute` loads the user by ID and returns NotFound when the result is null.

### Admin user seeded with ID 2
The `seed-admin` endpoint created the admin user with `UserID = 2` (not 1). A prior row likely existed from an earlier test run. Both the app and permission system are ID-agnostic — this has no effect.

### Local dev requires `ASPNETCORE_ENVIRONMENT=Development`
Without it, `Startup.cs` adds `AddRedirectToHttpsPermanent()` which sends a 301 + HSTS header. Chrome caches HSTS permanently for `localhost`, breaking all subsequent HTTP requests even after restart. To avoid this entirely, use a different port each time or always set the environment variable:

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run --project SixtyThreeBits.Web/SixtyThreeBits.Web.csproj
```

---

## Additional Roles Seeded

Two roles were added beyond the original Admin seed (Script 23):

| RoleID | RoleName | RoleCode | Purpose |
|---|---|---|---|
| 1 | Admin | 1 | Full admin panel access |
| 2 | User | 2 | Registered user — owns wishlists, scraping, privacy controls, social |
| 3 | Viewer | 3 | Guest/follower — reservations, public + follower-only content |

User and Viewer have **no permissions assigned yet** in `RolesPermissions`. Their permission sets are Sprint 2 work.

---

## Stub Functions Created for Sprint 2 (Replace These)

The website layout filter (`WebsiteFilterAttribute`) and the catch-all page router call four database objects that do not exist yet. Empty stub functions were created so the app runs without errors while the CMS tables are absent. **These must be replaced with real implementations in Sprint 2** once the underlying tables (`MenuHeader`, `MenuFooter`, `Redirects`, `Pages`) are created.

| Stub function | Type | Parameter | Returns | Replace when |
|---|---|---|---|---|
| `dbo.RedirectsList` | TVF | — | 0 rows | `Redirects` table exists |
| `dbo.MenuHeaderList` | TVF | `@MenuHeaderIsPublished BIT` | 0 rows | `MenuHeader` table exists |
| `dbo.MenuFooterList` | TVF | `@MenuFooterIsPublished BIT` | 0 rows | `MenuFooter` table exists |
| `dbo.PagesGetSingleBySlug` | Scalar | `@PageSlug NVARCHAR(500)` | NULL | `Pages` table exists |

Each stub TVF returns the correct column set (matching its DTO) with `WHERE 1 = 0`. The scalar returns NULL. The existing null-checks in `WebsiteFilterAttribute` handle empty/null gracefully.

---

## Troubleshooting

| Symptom | Likely cause | Fix |
|---|---|---|
| Login page shows but credentials fail | `UsersGetSingleByEmailAndPassword` returns NULL | Verify the admin user row exists in Users table; check `UserIsActive = 1` |
| Dashboard loads but sidebar is empty | Permissions not linked to role | Verify RolesPermissions has 25 rows for RoleID = 1 |
| Grid buttons (Add/Update/Delete) are hidden but grid loads | CodeName permissions missing or mismatched | Compare `PermissionCodeName` values in the Permissions table against the route name constants. They must be exact string matches (e.g. `AdminUsersControllerGridAdd`). |
| Sub-route returns 404 (e.g. `/admin/users/grid/add`) | Catch-all PagePath permission missing | Verify permission rows 9–12 exist with PagePath values ending in `/.` |
| `SystemPropertiesGet` returns NULL | SystemProperties table is empty | Re-run Script 26 |
| `Object 'dbo.FnName' cannot be found` | Function was not created | Re-run the relevant script; check for syntax errors in the SmarterASP editor output |
| Column name mismatch error at runtime | TVF column names don't match DTO property names | Column names in TVF must match C# property names exactly (PascalCase). Check `SqlQueryBuilder` debug output |

---

## How Permission Matching Works

Two independent mechanisms run for every admin request:

1. **URL check** (AdminFilterAttribute) — passes the current URL path to `HasPermission`. A permission matches if its `PagePath` produces a regex that covers the URL. Ending `PagePath` with `.` creates `.*` (match-anything) in the regex, which is how catch-all permissions (rows 9–12) cover all sub-routes.

2. **Route-name check** (Model code) — passes a route-name constant (e.g. `AdminUsersControllerGridAdd`) to `HasPermission`. A permission matches if its `PermissionCodeName` equals that string exactly. This controls UI visibility (Add/Update/Delete buttons).

Both checks look through the same flat list of permissions assigned to the user's role. A permission only needs to satisfy one check to be useful — URL-catchers handle access, CodeName entries handle button visibility.
