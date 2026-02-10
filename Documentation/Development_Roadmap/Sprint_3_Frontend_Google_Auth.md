# Sprint 3: Frontend Foundations & Google Authentication

**Goal:** Transform HTML/CSS from `/HTML New/` directory into functional ASP.NET Core Razor Views, implement Google-only authentication, and build the "Viewer" experience for wishlists.

**Duration:** ~2 weeks
**Status:** 🟡 In Progress (Phases 1-8 Complete - Add Product, Scraper functional)

**Source Files:** `/HTML New/` (Bootstrap 5 + jQuery + Ionicons)

### Phase Progress
| Phase | Description | Status |
|-------|-------------|--------|
| Phase 1 | Database: Google Auth Support | ✅ Complete |
| Phase 2 | Backend: Google OAuth Configuration | ✅ Complete |
| Phase 3 | Backend: Auth Controller & Repository | ✅ Complete |
| Phase 4 | Assets: Copy to wwwroot | ✅ Complete |
| Phase 5 | Frontend: Layout Shell | ✅ Complete |
| Phase 5.5 | First-Time Login Welcome Modal | ✅ Complete |
| Phase 6 | Frontend: Dashboard Page | ✅ Complete |
| Phase 7 | Frontend: Wishlist Details Page | ✅ Complete |
| Phase 8 | JavaScript Updates | 🟡 In Progress (Add Product + Scraper done) |
| Phase 9 | Google Cloud Console Setup | ✅ Complete |

---

## What This Sprint Covers

| Area | In Scope | Out of Scope |
|---|---|---|
| Layout & Shell | Main navigation, Footer, Toast notifications | Blog, complex animations |
| Authentication | **Google OAuth only** — no email/password | Email registration, password reset |
| Dashboard | "My Wishlists" grid with real data | Public profile page |
| Wishlist Details | Owner view (Add Product + Scraper) & Viewer view (Reservations) | Comments, ratings |
| Global Styles | Copy HTML New assets to wwwroot | Dark/Light theme toggle |

---

## Critical: 63BITS Constraints

1. **Don't Break the Scraper**: When porting the "Add Product" UI, ensure `app.js` scraper logic works with backend endpoint.
2. **No Direct Edits in HTML New Folder**: Treat `/HTML New/` as read-only. Copy to `/Views` or `/wwwroot`, then modify.
3. **Pathing**: Use `~/` for all local assets to ensure they load on SmarterASP.NET.
4. **Google Only**: The **only** authentication mechanism is Google OAuth. Remove email/password forms.

---

## Phase 1 — Database: Google Auth Support (3 Scripts) ✅

### Script 1 — Add Google ID Column to Users

```sql
ALTER TABLE [dbo].[Users]
ADD [UserGoogleID] NVARCHAR(255) NULL;
```

### Script 2 — Function to Get User by Google ID

```sql
CREATE FUNCTION [dbo].[UsersGetSingleByGoogleID](@GoogleID NVARCHAR(255))
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN (
        SELECT
            UserID,
            UserFullname,
            UserFirstname,
            UserLastname,
            UserBirthdate,
            UserEmail,
            UserPhoneNumberMobile,
            UserIsActive,
            UserAvatarFilename,
            UserDateCreated,
            UserGoogleID,
            RoleID
        FROM [dbo].[Users]
        WHERE UserGoogleID = @GoogleID
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    )
END;
```

### Script 3 — Function to Get User by Email (Migration Support)

```sql
CREATE FUNCTION [dbo].[UsersGetSingleByEmail](@Email NVARCHAR(255))
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN (
        SELECT
            UserID,
            UserFullname,
            UserFirstname,
            UserLastname,
            UserBirthdate,
            UserEmail,
            UserPhoneNumberMobile,
            UserIsActive,
            UserAvatarFilename,
            UserDateCreated,
            UserGoogleID,
            RoleID
        FROM [dbo].[Users]
        WHERE UserEmail = @Email
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    )
END;
```

---

## Phase 2 — Backend: Google OAuth Configuration ✅

> **Credentials:** See [Google_OAuth_Credentials.md](./Google_OAuth_Credentials.md) for Client ID and Secret.

### 2.1 NuGet Package

Add to `SixtyThreeBits.Web.csproj`:

```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.Google" Version="10.0.0" />
```

### 2.2 AppSettings

Add to `appsettings.json`:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "1049038610778-dnrsrqgmfh2s3mntd2ubg7j56vh9vtks.apps.googleusercontent.com",
      "ClientSecret": "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"
    }
  }
}
```

### 2.3 Program.cs / Startup.cs Configuration

Add to `ConfigureServices`:

```csharp
// Authentication - Google Only
services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
})
.AddGoogle(options =>
{
    options.ClientId = Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
    options.CallbackPath = "/signin-google";

    // Request additional scopes for profile picture
    options.Scope.Add("profile");
    options.Scope.Add("email");

    // Map the picture claim
    options.ClaimActions.MapJsonKey("picture", "picture");
});
```

Add to `Configure` (before `UseEndpoints`):

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

---

## Phase 3 — Backend: Auth Controller & Repository (63BITS Compliant) ✅

### 3.1 Route Names (ControllerActionRouteNames.cs)

Add to `Utilities/Web/ControllerActionRouteNames.cs`:

```csharp
#region Website Account

public const string WebAccountControllerLogin = "web-account-login";
public const string WebAccountControllerExternalLoginCallback = "web-account-external-callback";
public const string WebAccountControllerLogout = "web-account-logout";

#endregion

#region Website Dashboard

public const string WebDashboardControllerIndex = "web-dashboard-index";
public const string WebDashboardControllerCreate = "web-dashboard-create";

#endregion

#region Website Wishlist

public const string WebWishlistControllerDetail = "web-wishlist-detail";
public const string WebWishlistControllerAddProduct = "web-wishlist-add-product";
public const string WebWishlistControllerScrape = "web-wishlist-scrape";
public const string WebWishlistControllerReserve = "web-wishlist-reserve";
public const string WebWishlistControllerUnreserve = "web-wishlist-unreserve";

#endregion
```

### 3.2 View Names (ViewNames.cs)

Add to `Utilities/Web/ViewNames.cs`:

```csharp
#region Website Dashboard

public const string WebDashboardIndex = "~/Views/Website/Dashboard/IndexView.cshtml";

#endregion

#region Website Wishlist

public const string WebWishlistDetail = "~/Views/Website/Wishlist/DetailView.cshtml";

#endregion
```

### 3.3 UserDTO Update

Add to `UserDTO`:

```csharp
public string? UserGoogleID { get; init; }
```

### 3.4 UsersRepository Updates (with TryToReturn wrapper)

Add methods to `UsersRepository.cs`:

```csharp
#region Methods

public UserDTO? UsersGetSingleByGoogleID(string googleId)
{
    return TryToReturn(
        () =>
        {
            var json = DbContext.ScalarFunction<string>("UsersGetSingleByGoogleID", googleId);
            return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<UserDTO>(json);
        },
        $"{nameof(UsersGetSingleByGoogleID)} ({nameof(googleId)} = {googleId})"
    );
}

public UserDTO? UsersGetSingleByEmail(string email)
{
    return TryToReturn(
        () =>
        {
            var json = DbContext.ScalarFunction<string>("UsersGetSingleByEmail", email);
            return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<UserDTO>(json);
        },
        $"{nameof(UsersGetSingleByEmail)} ({nameof(email)} = {email})"
    );
}

public int UsersCreateFromGoogle(
    string googleId,
    string email,
    string fullname,
    string firstname,
    string lastname,
    string? avatarUrl)
{
    return TryToReturn(
        () =>
        {
            var userJson = JsonConvert.SerializeObject(new
            {
                UserGoogleID = googleId,
                UserEmail = email,
                UserFullname = fullname,
                UserFirstname = firstname,
                UserLastname = lastname,
                UserAvatarFilename = avatarUrl,
                UserIsActive = true,
                RoleID = 2
            });

            return DbContext.StoredProcedure<int>("UsersIUD", 0, userJson);
        },
        $"{nameof(UsersCreateFromGoogle)} ({nameof(googleId)} = {googleId}, {nameof(email)} = {email})"
    );
}

public void UsersUpdateGoogleID(int userId, string googleId, string? avatarUrl)
{
    TryToRun(
        () =>
        {
            var userJson = JsonConvert.SerializeObject(new
            {
                UserGoogleID = googleId,
                UserAvatarFilename = avatarUrl
            });
            DbContext.StoredProcedure("UsersIUD", 1, userId, userJson);
        },
        $"{nameof(UsersUpdateGoogleID)} ({nameof(userId)} = {userId})"
    );
}

public void UsersUpdateAvatar(int userId, string avatarUrl)
{
    TryToRun(
        () =>
        {
            var userJson = JsonConvert.SerializeObject(new
            {
                UserAvatarFilename = avatarUrl
            });
            DbContext.StoredProcedure("UsersIUD", 1, userId, userJson);
        },
        $"{nameof(UsersUpdateAvatar)} ({nameof(userId)} = {userId})"
    );
}

#endregion
```

### 3.5 AccountModel (Logic Only - No Properties)

Create `Models/Website/Account/AccountModel.cs`:

```csharp
namespace SixtyThreeBits.Web.Models.Website.Account
{
    public class AccountModel : ModelBase
    {
        #region Methods

        public async Task<UserDTO?> AuthenticateGoogleUserAsync(
            string googleId,
            string email,
            string? fullName,
            string? firstName,
            string? lastName,
            string? pictureUrl)
        {
            var user = UsersRepository.UsersGetSingleByGoogleID(googleId);

            if (user == null)
            {
                user = UsersRepository.UsersGetSingleByEmail(email);

                if (user != null)
                {
                    UsersRepository.UsersUpdateGoogleID(user.UserID!.Value, googleId, pictureUrl);
                    user = UsersRepository.UsersGetSingleByID(user.UserID!.Value);
                }
                else
                {
                    var userId = UsersRepository.UsersCreateFromGoogle(
                        googleId: googleId,
                        email: email,
                        fullname: fullName ?? email,
                        firstname: firstName ?? "",
                        lastname: lastName ?? "",
                        avatarUrl: pictureUrl
                    );
                    user = UsersRepository.UsersGetSingleByID(userId);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(pictureUrl) && user.UserAvatarFilename != pictureUrl)
                {
                    UsersRepository.UsersUpdateAvatar(user.UserID!.Value, pictureUrl);
                    user = UsersRepository.UsersGetSingleByID(user.UserID!.Value);
                }
            }

            return user;
        }

        public void StoreUserSession(UserDTO user)
        {
            SessionAssistance.SetObject(WebConstants.SessionKeys.User, user);
            CookieAssistance.SetObject(WebConstants.Cookies.User, user, 30);
        }

        public void ClearUserSession()
        {
            SessionAssistance.Remove(WebConstants.SessionKeys.User);
            CookieAssistance.Delete(WebConstants.Cookies.User);
        }

        #endregion
    }
}
```

### 3.6 AccountController (63BITS - Allman Braces, Route Names)

Create `Controllers/Website/Account/AccountController.cs`:

```csharp
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;

namespace SixtyThreeBits.Web.Controllers.Website.Account
{
    [Route("{language}/account")]
    [Route("account")]
    public class AccountController : WebsiteControllerBase<AccountModel>
    {
        #region Methods

        [HttpGet("login")]
        [Route(ControllerActionRouteNames.WebAccountControllerLogin)]
        public IActionResult Login(string? returnUrl = null)
        {
            var existingUser = Model.SessionAssistance.GetObject<UserDTO>(WebConstants.SessionKeys.User);

            if (existingUser != null)
            {
                return RedirectToRoute(ControllerActionRouteNames.WebDashboardControllerIndex, new { language = Model.LanguageCode });
            }

            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("ExternalLoginCallback", new { returnUrl }),
                Items = { { "returnUrl", returnUrl ?? "/" } }
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("external-login-callback")]
        [Route(ControllerActionRouteNames.WebAccountControllerExternalLoginCallback)]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal == null)
            {
                return Redirect("/?error=auth_failed");
            }

            var googleId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var fullName = result.Principal.FindFirstValue(ClaimTypes.Name);
            var firstName = result.Principal.FindFirstValue(ClaimTypes.GivenName);
            var lastName = result.Principal.FindFirstValue(ClaimTypes.Surname);
            var pictureUrl = result.Principal.FindFirstValue("picture");

            if (string.IsNullOrEmpty(googleId) || string.IsNullOrEmpty(email))
            {
                return Redirect("/?error=missing_claims");
            }

            var user = await Model.AuthenticateGoogleUserAsync(
                googleId,
                email,
                fullName,
                firstName,
                lastName,
                pictureUrl
            );

            if (user == null)
            {
                return Redirect("/?error=user_creation_failed");
            }

            Model.StoreUserSession(user);

            var redirectUrl = returnUrl ?? Url.RouteUrl(ControllerActionRouteNames.WebDashboardControllerIndex, new { language = Model.LanguageCode });
            return Redirect(redirectUrl);
        }

        [HttpGet("logout")]
        [Route(ControllerActionRouteNames.WebAccountControllerLogout)]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Model.ClearUserSession();
            return Redirect("/");
        }

        [HttpGet("register")]
        public IActionResult Register() => NotFound();

        [HttpGet("forgot-password")]
        public IActionResult ForgotPassword() => NotFound();

        #endregion
    }
}
```

### 3.7 Cleanup: Remove Local Authentication

**Disable/Remove these components** (Gifter is 100% Google-dependent):

| Component | Action |
|-----------|--------|
| `Register` action/view | DELETE or return `NotFound()` |
| `ForgotPassword` action/view | DELETE or return `NotFound()` |
| `ResetPassword` action/view | DELETE or return `NotFound()` |
| `ChangePassword` action/view | DELETE or return `NotFound()` |
| Email/Password login form | REMOVE from UI |
| Password fields in UserDTO | Keep for admin, hide in website |

```csharp
// Stub out local auth endpoints
[HttpGet("register")]
public IActionResult Register() => NotFound();

[HttpGet("forgot-password")]
public IActionResult ForgotPassword() => NotFound();

[HttpPost("register")]
public IActionResult RegisterPost() => NotFound();
```

---

## Phase 4 — Assets: Copy to wwwroot ✅

### 4.1 Folder Structure

```
/wwwroot/
├── css/
│   └── website/
│       ├── gifter.css         (from HTML New/css/website/)
│       └── style.css          (from HTML New/css/website/)
├── js/
│   ├── app.js                 (from HTML New/js/ - modify for backend)
│   ├── auth.js                (from HTML New/js/ - simplify for Google)
│   ├── components.js          (from HTML New/js/)
│   └── layout.js              (from HTML New/js/)
└── plugins/
    ├── bootstrap/             (from HTML New/plugins/)
    ├── jquery/                (from HTML New/plugins/)
    ├── 63bits-fonts/          (from HTML New/plugins/)
    ├── virtual-select/        (from HTML New/plugins/)
    └── air-datepicker-3/      (from HTML New/plugins/)
```

### 4.2 Design Tokens (gifter.css)

```css
:root {
    --neon-blue: #00aaff;
    --accent-purple: #cd32ff;
    --rich-black: #080808;
    --deep-obsidian: #1a1a1a;
    --foreground: #fafafa;
    --muted: #8c8c8c;
    --border-color: rgba(255, 255, 255, 0.1);
    --radius-bento: 28px;
    --radius-md: 16px;
}
```

---

## Phase 5 — Frontend: Layout Shell ✅

### 5.1 Website Layout (_Layout.cshtml)

Port structure from `HTML New/index.html`:

```html
<!DOCTYPE html>
<html lang="@Model.LanguageCode">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>@ViewBag.Title - Gifter</title>

    <!-- Fonts -->
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&display=swap" rel="stylesheet" />

    <!-- Plugins -->
    <link href="~/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="~/css/website/gifter.css" rel="stylesheet" />
    <link href="~/css/website/style.css" rel="stylesheet" />

    <!-- Ionicons -->
    <script type="module" src="https://unpkg.com/ionicons@7.1.0/dist/ionicons/ionicons.esm.js"></script>
</head>
<body class="bg-rich-black text-foreground">

    <div class="ambient-light"></div>

    <div class="app-container">
        <!-- Header -->
        @await Html.PartialAsync("_Header")

        <!-- Main Content -->
        @RenderBody()

        <!-- Footer -->
        @await Html.PartialAsync("_Footer")
    </div>

    <!-- Toast Container -->
    <div id="toast-container" class="toast-container position-fixed top-0 end-0 p-3"></div>

    <!-- Scripts -->
    <script src="~/plugins/jquery/jquery.min.js"></script>
    <script src="~/plugins/bootstrap/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/layout.js"></script>
    <script src="~/js/components.js"></script>
    @RenderSection("Scripts", required: false)
</body>
</html>
```

### 5.2 Header Partial (_Header.cshtml)

```html
@{
    var user = ViewData[WebConstants.ViewData.User] as UserDTO;
    var isAuthenticated = user != null;

    // Avatar Safe-Load: Generate fallback URL using user initials
    string avatarUrl = "";
    string fallbackAvatar = "";
    if (isAuthenticated)
    {
        avatarUrl = user.UserAvatarFilename ?? "";
        var initials = string.IsNullOrEmpty(user.UserFirstname)
            ? user.UserEmail?.Substring(0, 1).ToUpper() ?? "U"
            : user.UserFirstname.Substring(0, 1).ToUpper();
        if (!string.IsNullOrEmpty(user.UserLastname))
            initials += user.UserLastname.Substring(0, 1).ToUpper();

        // Use ui-avatars.com as fallback (generates avatar from initials)
        fallbackAvatar = $"https://ui-avatars.com/api/?name={initials}&background=1a1a1a&color=00aaff&size=32&bold=true";
    }
}

<header class="d-flex justify-content-between align-items-center py-3 mb-4">
    <a href="~/" class="d-flex align-items-center text-decoration-none">
        <span class="fs-4 fw-bold text-white">🎁 Gifter</span>
    </a>

    <div class="d-flex align-items-center gap-3">
        @if (isAuthenticated)
        {
            <a href="~/@Model.LanguageCode/dashboard" class="btn btn-link text-white">
                My Wishlists
            </a>
            <div class="dropdown">
                <button class="btn btn-link dropdown-toggle d-flex align-items-center gap-2"
                        data-bs-toggle="dropdown">
                    <!-- Avatar with onerror fallback -->
                    <img src="@(string.IsNullOrEmpty(avatarUrl) ? fallbackAvatar : avatarUrl)"
                         alt="@user.UserFirstname"
                         class="rounded-circle user-avatar"
                         width="32" height="32"
                         onerror="this.onerror=null; this.src='@fallbackAvatar';" />
                    <span class="text-white">@user.UserFirstname</span>
                </button>
                <ul class="dropdown-menu dropdown-menu-end">
                    <li><a class="dropdown-item" href="~/@Model.LanguageCode/account/logout">Logout</a></li>
                </ul>
            </div>
        }
        else
        {
            <a href="~/@Model.LanguageCode/account/login" class="btn btn-primary btn-squish">
                <ion-icon name="logo-google" class="me-2"></ion-icon>
                Sign in with Google
            </a>
        }
    </div>
</header>
```

### 5.3 Avatar Safe-Load CSS (gifter.css)

```css
/* Avatar fallback styling */
.user-avatar {
    object-fit: cover;
    background-color: var(--deep-obsidian);
}

/* Fallback icon if image fails completely */
.user-avatar-fallback {
    width: 32px;
    height: 32px;
    border-radius: 50%;
    background: var(--deep-obsidian);
    display: flex;
    align-items: center;
    justify-content: center;
    color: var(--neon-blue);
    font-weight: 600;
    font-size: 14px;
}
```

---

## Phase 5.5 — First-Time Login Welcome Modal ✅

### 5.5.1 Database Changes

**Script 5 — Add UserIsFirstLogin Column**

```sql
-- db/sprint_3_google_auth/05_Alter_Users_Add_FirstLogin.sql
ALTER TABLE [dbo].[Users]
ADD [UserIsFirstLogin] BIT NOT NULL DEFAULT 1;
GO
```

**Script 6 — Update User Functions to Include UserIsFirstLogin**

```sql
-- db/sprint_3_google_auth/06_Update_UserFunctions_AddFirstLogin.sql
-- Updates UsersGetSingleByGoogleID, UsersGetSingleByEmail, UsersGetSingleByID
-- to include UserIsFirstLogin in JSON output
```

### 5.5.2 DTO Updates

Add to `UserDTO.cs`:
```csharp
public bool UserIsFirstLogin { get; init; }
```

Add to `UserIudDTO.cs`:
```csharp
public bool? UserIsFirstLogin { get; init; }
```

### 5.5.3 Repository Method

Add to `UsersRepository.cs`:
```csharp
public async Task<int?> UsersCompleteOnboarding(int userID)
{
    return await UsersIUD(
        databaseAction: Enums.DatabaseActions.UPDATE,
        userID: userID,
        user: new UserIudDTO
        {
            UserIsFirstLogin = false
        }
    );
}
```

### 5.5.4 DashboardModel

Update `Models/Website/Dashboard/DashboardModel.cs`:
```csharp
public class DashboardModel : ModelBase
{
    #region Properties
    public bool ShowWelcomeModal => User?.UserIsFirstLogin ?? false;
    #endregion

    #region Methods
    public async Task<bool> CompleteOnboardingAsync()
    {
        if (User?.UserID == null) return false;

        var usersRepository = RepositoriesFactory.CreateUsersRepository();
        await usersRepository.UsersCompleteOnboarding(User.UserID.Value);

        // Refresh user session with updated data
        var updatedUser = await usersRepository.UsersGetSingleByID(User.UserID.Value);
        if (updatedUser != null)
        {
            SessionAssistance.Set(WebConstants.SessionKeys.User, updatedUser);
        }
        return true;
    }
    #endregion
}
```

### 5.5.5 API Endpoint

Add to `DashboardController.cs`:
```csharp
[HttpPost]
[Route("api/users/complete-onboarding")]
public async Task<IActionResult> CompleteOnboarding()
{
    if (Model.User == null)
        return Unauthorized(new { success = false, message = "Not authenticated" });

    var result = await Model.CompleteOnboardingAsync();
    return result
        ? Ok(new { success = true })
        : BadRequest(new { success = false, message = "Failed to complete onboarding" });
}
```

### 5.5.6 Welcome Modal UI (Dashboard/IndexView.cshtml)

Bootstrap modal with:
- Welcome icon with gradient background
- Personalized greeting using user's first name
- Feature highlights (create wishlists, share, notifications)
- "Get Started" button that calls `/api/users/complete-onboarding`
- `data-bs-backdrop="static"` to prevent accidental dismissal

### 5.5.7 Verification Checklist

- [x] `UserIsFirstLogin` column added to Users table
- [x] SQL functions updated to include `UserIsFirstLogin`
- [x] Welcome modal appears on first login
- [x] Modal displays personalized greeting
- [x] "Get Started" button calls API
- [x] API sets `UserIsFirstLogin = 0`
- [x] Modal doesn't appear on subsequent logins
- [x] User session refreshed after onboarding complete

---

## Phase 6 — Frontend: Dashboard Page

### 6.1 Dashboard Controller

Create `Controllers/Website/Dashboard/DashboardController.cs`:

```csharp
[WebsiteFilter]
[Route("{language}/dashboard")]
public class DashboardController : WebsiteControllerBase<DashboardModel>
{
    [HttpGet]
    [Route(ControllerActionRouteNames.WebDashboardControllerIndex)]
    public async Task<IActionResult> Index()
    {
        var user = Model.SessionAssistance.GetObject<UserDTO>(WebConstants.SessionKeys.User);
        if (user == null)
        {
            return RedirectToRoute(ControllerActionRouteNames.WebAccountControllerLogin, new { language = Model.LanguageCode });
        }

        var viewModel = await Model.GetViewModelAsync(user.UserID.Value);
        return View("IndexView", viewModel);
    }

    [HttpPost("create")]
    [Route(ControllerActionRouteNames.WebDashboardControllerCreate)]
    public async Task<IActionResult> CreateWishlist([FromBody] CreateWishlistRequest request)
    {
        var user = Model.SessionAssistance.GetObject<UserDTO>(WebConstants.SessionKeys.User);
        if (user == null) 
        {
            return Unauthorized();
        }

        var id = await Model.CreateWishlistAsync(user.UserID.Value, request);
        return Json(new { success = true, giftListId = id });
    }
}
```

### 6.2 Dashboard View (IndexView.cshtml)

Port from `HTML New/index.html`:

```html
@model DashboardViewModel

<div class="dashboard">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h1 class="h4 fw-bold">My Wishlists</h1>
        <button class="btn btn-primary btn-squish" data-bs-toggle="modal" data-bs-target="#createWishlistModal">
            <ion-icon name="add-outline" class="me-1"></ion-icon>
            Create Wishlist
        </button>
    </div>

    @if (Model.Wishlists.Any())
    {
        <div class="row g-3">
            @foreach (var wishlist in Model.Wishlists)
            {
                <div class="col-12 col-sm-6">
                    <a href="~/@Model.LanguageCode/wishlist/@wishlist.GiftListID"
                       class="bento-card d-block text-decoration-none p-4">
                        <div class="d-flex justify-content-between align-items-start mb-3">
                            <span class="fs-2">@wishlist.CategoryEmoji</span>
                            <span class="badge bg-secondary">@wishlist.GiftCount items</span>
                        </div>
                        <h3 class="h6 fw-semibold text-white mb-1">@wishlist.GiftListTitle</h3>
                        <p class="small text-muted mb-0">@wishlist.GiftListDescription</p>
                    </a>
                </div>
            }
        </div>
    }
    else
    {
        <div class="text-center py-5">
            <div class="fs-1 mb-3">🎁</div>
            <h3 class="h5">No wishlists yet</h3>
            <p class="text-muted">Create your first wishlist to get started!</p>
        </div>
    }
</div>

<!-- Create Wishlist Modal (Google auth already done, skip auth steps) -->
<div class="modal fade" id="createWishlistModal" data-bs-backdrop="static">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content glass">
            <div class="modal-body p-4">
                <div class="text-center mb-4">
                    <span class="badge bg-success-subtle text-success px-3 py-2 rounded-pill">
                        <span class="pulse-dot"></span> Signed in
                    </span>
                </div>
                <h5 class="text-center mb-4">✨ New Wishlist</h5>

                <div class="mb-4">
                    <input type="text" id="wishlistName" class="form-control form-control-lg"
                           placeholder="Wishlist name" />
                </div>

                <div class="category-grid mb-4">
                    <!-- 10 categories -->
                    @foreach (var cat in Model.Categories)
                    {
                        <button type="button" class="category-btn" data-category="@cat.Value">
                            <span class="fs-4">@cat.Emoji</span>
                            <span class="small">@cat.Label</span>
                        </button>
                    }
                </div>

                <button type="button" class="btn btn-primary w-100 btn-squish" id="createWishlistBtn" disabled>
                    Create Wishlist
                </button>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <script src="~/js/app.js"></script>
    <script>
        // Initialize dashboard
        initCreateWishlistModal();
    </script>
}
```

---

## Phase 7 — Frontend: Wishlist Details Page

### 7.1 Wishlist Controller

Create `Controllers/Website/Wishlist/WishlistController.cs`:

```csharp
[WebsiteFilter]
[Route("{language}/wishlist")]
public class WishlistController : WebsiteControllerBase<WishlistModel>
{
    [HttpGet("{id:int}")]
    [Route(ControllerActionRouteNames.WebWishlistControllerDetail)]
    public async Task<IActionResult> Detail(int id)
    {
        var currentUser = Model.SessionAssistance.GetObject<UserDTO>(WebConstants.SessionKeys.User);
        var viewModel = await Model.GetDetailViewModelAsync(id, currentUser?.UserID);

        if (viewModel == null) 
        {
            return NotFound();
        }
        return View("DetailView", viewModel);
    }

    [HttpPost("{id:int}/add-product")]
    [Route(ControllerActionRouteNames.WebWishlistControllerAddProduct)]
    public async Task<IActionResult> AddProduct(int id, [FromBody] AddProductRequest request)
    {
        var user = Model.SessionAssistance.GetObject<UserDTO>(WebConstants.SessionKeys.User);
        if (user == null) 
        {
            return Unauthorized();
        }

        var wishlist = Model.GiftListsRepository.GiftListsGetSingleByID(id);
        if (wishlist?.GiftListUserID != user.UserID) 
        {
            return Forbid();
        }

        var giftId = await Model.AddProductAsync(id, request);
        return Json(new { success = true, giftId });
    }

    [HttpPost("{id:int}/scrape")]
    [Route(ControllerActionRouteNames.WebWishlistControllerScrape)]
    public async Task<IActionResult> ScrapeUrl([FromBody] ScrapeRequest request)
    {
        var result = await Model.ScraperService.ScrapeAsync(request.Url);
        return Json(result);
    }

    [HttpPost("{id:int}/reserve/{giftId:int}")]
    [Route(ControllerActionRouteNames.WebWishlistControllerReserve)]
    public async Task<IActionResult> Reserve(int id, int giftId)
    {
        var user = Model.SessionAssistance.GetObject<UserDTO>(WebConstants.SessionKeys.User);
        if (user == null) 
        {
            return Unauthorized();
        }

        var result = await Model.ReserveGiftAsync(giftId, user.UserID.Value);
        return Json(result);
    }

    [HttpPost("{id:int}/unreserve/{giftId:int}")]
    [Route(ControllerActionRouteNames.WebWishlistControllerUnreserve)]
    public async Task<IActionResult> Unreserve(int id, int giftId)
    {
        var user = Model.SessionAssistance.GetObject<UserDTO>(WebConstants.SessionKeys.User);
        if (user == null) 
        {
            return Unauthorized();
        }

        var result = await Model.UnreserveGiftAsync(giftId, user.UserID.Value);
        return Json(result);
    }
}
```

### 7.2 Wishlist Detail View (DetailView.cshtml)

Port from `HTML New/wishlist-detail.html`:

```html
@model WishlistDetailViewModel

@{
    var isOwner = Model.CurrentUserID == Model.Wishlist.GiftListUserID;
}

<div class="wishlist-detail">
    <!-- Header -->
    <div class="d-flex align-items-center gap-3 mb-4">
        <a href="~/@Model.LanguageCode/dashboard" class="btn btn-link text-white p-0">
            <ion-icon name="arrow-back-outline" size="large"></ion-icon>
        </a>
        <div class="flex-grow-1">
            <h1 class="h5 fw-bold mb-0">@Model.Wishlist.GiftListTitle</h1>
            <small class="text-muted">@Model.Gifts.Count items · $@Model.TotalValue Total</small>
        </div>
        @if (isOwner)
        {
            <button class="btn btn-primary btn-squish" data-bs-toggle="modal" data-bs-target="#addProductModal">
                <ion-icon name="add-outline" class="me-1"></ion-icon>
                Add Product
            </button>
        }
    </div>

    <!-- Products Grid -->
    @if (Model.Gifts.Any())
    {
        <div class="row g-3">
            @foreach (var gift in Model.Gifts)
            {
                <div class="col-12 col-sm-6">
                    <div class="bento-card p-3 @(gift.GiftIsReserved ? "opacity-50" : "")">
                        <div class="position-relative mb-3">
                            <img src="@gift.GiftImageUrl" alt="@gift.GiftTitle"
                                 class="w-100 rounded-3" style="aspect-ratio: 4/3; object-fit: cover;" />
                            <span class="badge bg-primary position-absolute bottom-0 start-0 m-2">
                                @gift.GiftPrice @gift.GiftCurrency
                            </span>
                            @if (gift.GiftIsReserved)
                            {
                                <span class="badge bg-success position-absolute top-0 end-0 m-2">
                                    Purchased
                                </span>
                            }
                        </div>
                        <h4 class="h6 fw-semibold mb-2">@gift.GiftTitle</h4>

                        @if (!isOwner && !gift.GiftIsReserved)
                        {
                            <button class="btn btn-outline-success btn-sm w-100 btn-reserve"
                                    data-gift-id="@gift.GiftID"
                                    data-gift-title="@gift.GiftTitle"
                                    data-bs-toggle="modal"
                                    data-bs-target="#reserveConfirmModal">
                                <ion-icon name="gift-outline" class="me-1"></ion-icon>
                                I'll Buy This
                            </button>
                        }
                        else if (!isOwner && gift.GiftIsReserved && gift.GiftReservedByUserID == Model.CurrentUserID)
                        {
                            <button class="btn btn-outline-secondary btn-sm w-100 btn-unreserve"
                                    data-gift-id="@gift.GiftID">
                                Cancel Reservation
                            </button>
                        }
                    </div>
                </div>
            }
        </div>
    }
    else
    {
        <div class="text-center py-5">
            <div class="fs-1 mb-3">📦</div>
            <h3 class="h5">No products yet</h3>
            @if (isOwner)
            {
                <p class="text-muted">Add your first product to this wishlist!</p>
            }
        </div>
    }
</div>

<!-- Reservation Assurance Modal (for viewers) -->
@if (!isOwner)
{
    <div class="modal fade" id="reserveConfirmModal" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-sm">
            <div class="modal-content glass">
                <div class="modal-body p-4 text-center">
                    <div class="fs-1 mb-3">🎁</div>
                    <h5 class="mb-3">Reserve this gift?</h5>
                    <p class="text-muted mb-4">
                        Are you sure you want to reserve
                        <strong id="reserveGiftTitle"></strong>
                        for <strong>@Model.OwnerFullname</strong>?
                    </p>
                    <input type="hidden" id="reserveGiftId" />
                    <div class="d-grid gap-2">
                        <button type="button" class="btn btn-success btn-squish" id="confirmReserveBtn">
                            <ion-icon name="checkmark-outline" class="me-1"></ion-icon>
                            Yes, I'll buy it
                        </button>
                        <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">
                            Cancel
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
}

@if (isOwner)
{
    <!-- Add Product Modal -->
    <div class="modal fade" id="addProductModal" data-bs-backdrop="static">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content glass">
                <div class="modal-body p-4">
                    <h5 class="text-center mb-4">Add Product</h5>

                    <!-- URL Scraper -->
                    <div class="mb-4">
                        <div class="input-group">
                            <input type="url" id="productUrl" class="form-control"
                                   placeholder="Paste product URL..." />
                            <button class="btn btn-primary" id="scrapeBtn">
                                <ion-icon name="sync-outline" class="me-1"></ion-icon>
                                Scrape
                            </button>
                        </div>
                    </div>

                    <!-- Scraped/Manual Data -->
                    <div id="productForm" style="display: none;">
                        <div class="mb-3">
                            <img id="productImage" src="" class="w-100 rounded-3 mb-3" style="display: none;" />
                        </div>
                        <div class="mb-3">
                            <input type="text" id="productTitle" class="form-control" placeholder="Product title" />
                        </div>
                        <div class="row g-2 mb-3">
                            <div class="col-8">
                                <input type="number" id="productPrice" class="form-control" placeholder="Price" />
                            </div>
                            <div class="col-4">
                                <select id="productCurrency" class="form-select">
                                    <option value="USD">USD</option>
                                    <option value="EUR">EUR</option>
                                    <option value="GEL">GEL</option>
                                
                                </select>
                            </div>
                        </div>
                        <button type="button" class="btn btn-primary w-100 btn-squish" id="addProductBtn">
                            Add to Wishlist
                        </button>
                    </div>

                    <!-- Loading State -->
                    <div id="scrapingLoader" class="text-center py-4" style="display: none;">
                        <div class="spinner-border text-primary mb-2"></div>
                        <p class="text-muted mb-0">Scraping product data...</p>
                    </div>
                </div>
            </div>
        </div>
    </div>
}

@section Scripts {
    <script>
        const wishlistId = @Model.Wishlist.GiftListID;
        const isOwner = @(isOwner ? "true" : "false");
        const ownerName = "@Model.OwnerFullname";
    </script>
    <script src="~/js/app.js"></script>
    <script>
        initWishlistDetail();
        initReserveConfirmModal(); // Initialize reservation confirmation
    </script>
}
```

---

## Phase 8 — JavaScript Updates (app.js)

### 8.1 Replace Mock Data with API Calls

```javascript
// Scraper - call backend instead of mock
async function scrapeProduct(url) {
    const response = await fetch(`/${languageCode}/wishlist/${wishlistId}/scrape`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ url })
    });
    return response.json();
}

// Reserve gift (called after confirmation modal)
async function reserveGift(giftId) {
    const response = await fetch(`/${languageCode}/wishlist/${wishlistId}/reserve/${giftId}`, {
        method: 'POST'
    });
    return response.json();
}

// Initialize Reservation Confirmation Modal
function initReserveConfirmModal() {
    const modal = document.getElementById('reserveConfirmModal');
    if (!modal) return;

    // When modal opens, populate with gift data
    modal.addEventListener('show.bs.modal', function(event) {
        const button = event.relatedTarget;
        const giftId = button.getAttribute('data-gift-id');
        const giftTitle = button.getAttribute('data-gift-title');

        document.getElementById('reserveGiftId').value = giftId;
        document.getElementById('reserveGiftTitle').textContent = giftTitle;
    });

    // Confirm reservation button
    document.getElementById('confirmReserveBtn').addEventListener('click', async function() {
        const giftId = document.getElementById('reserveGiftId').value;
        const btn = this;

        btn.disabled = true;
        btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span> Reserving...';

        try {
            const result = await reserveGift(giftId);
            if (result.success) {
                showToast('Gift reserved! The owner won\'t see who reserved it.', 'success');
                bootstrap.Modal.getInstance(modal).hide();
                // Reload page to update UI
                setTimeout(() => location.reload(), 1000);
            } else {
                showToast(result.message || 'Failed to reserve gift', 'error');
            }
        } catch (error) {
            showToast('An error occurred', 'error');
        } finally {
            btn.disabled = false;
            btn.innerHTML = '<ion-icon name="checkmark-outline" class="me-1"></ion-icon> Yes, I\'ll buy it';
        }
    });
}

// Create wishlist
async function createWishlist(name, category) {
    const response = await fetch(`/${languageCode}/dashboard/create`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ title: name, occasionType: category })
    });
    return response.json();
}

// Add product
async function addProduct(data) {
    const response = await fetch(`/${languageCode}/wishlist/${wishlistId}/add-product`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    });
    return response.json();
}
```

### 8.2 Toast Notifications

```javascript
function showToast(message, type = 'success') {
    const container = document.getElementById('toast-container');
    const toast = document.createElement('div');
    toast.className = `toast show align-items-center text-white bg-${type === 'success' ? 'success' : 'danger'}`;
    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">${message}</div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
    `;
    container.appendChild(toast);
    setTimeout(() => toast.remove(), 3000);
}
```

---

## Phase 9 — Google Cloud Console Setup

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create project or select existing
3. Enable **Google+ API** or **Google Identity Services**
4. Go to **Credentials** → Create **OAuth 2.0 Client ID**
5. Application type: **Web application**
6. Authorized redirect URIs:
   - Dev: `http://localhost:5001/signin-google`
   - Prod: `https://your-domain.com/signin-google`
7. Copy **Client ID** and **Client Secret** to `appsettings.json`

---

## File Structure Summary

```
/SixtyThreeBits.Web/
├── Controllers/Website/
│   ├── Account/
│   │   └── AccountController.cs         (Google OAuth - 63BITS Compliant)
│   ├── Dashboard/
│   │   └── DashboardController.cs       (User wishlists)
│   └── Wishlist/
│       └── WishlistController.cs        (Detail & reservations)
├── Models/Website/
│   ├── Account/
│   │   └── AccountModel.cs
│   ├── Dashboard/
│   │   └── DashboardModel.cs
│   └── Wishlist/
│       └── WishlistModel.cs
├── Views/Website/
│   ├── Shared/
│   │   ├── Layout.cshtml
│   │   ├── _Header.cshtml
│   │   └── _Footer.cshtml
│   ├── Dashboard/IndexView.cshtml
│   └── Wishlist/DetailView.cshtml
└── wwwroot/
    ├── css/website/gifter.css
    ├── js/app.js
    └── plugins/bootstrap/, jquery/, etc.
```

---

## Verification Checklist

### Google Authentication
- [x] "Sign in with Google" button redirects to Google consent screen
- [x] After consent, user created/updated in Users table with `UserGoogleID`
- [x] Google profile picture (`picture` claim) saved to `UserAvatarFilename`
- [x] User session persists across page refreshes
- [x] Logout clears session and redirects home
- [x] Guest sees "Sign in with Google", authenticated user sees avatar
- [x] Existing email users are linked to Google ID on first Google login
- [x] Avatar updates if user changes their Google profile picture

### First-Time Login Welcome Modal
- [x] Welcome modal appears on first login only
- [x] Modal shows personalized greeting with user's first name
- [x] "Get Started" button calls `/api/users/complete-onboarding`
- [x] `UserIsFirstLogin` flag set to false after clicking
- [x] Modal doesn't appear on subsequent logins

### Cleanup Verification
- [x] `/account/register` returns 404 (disabled)
- [x] `/account/forgot-password` returns 404 (disabled)
- [x] No email/password form visible in UI
- [x] Gifter is 100% Google-dependent for authentication

### Dashboard
- [x] User sees their wishlists (not mock data)
- [x] "Create Wishlist" modal works with category picker (emoji-based)
- [x] New wishlist appears after creation (redirects to detail page)
- [x] Empty state shows prompt

### Wishlist Details (Owner)
- [x] Owner sees "Add Product" button
- [x] Detail page loads wishlist data from database
- [x] Empty state shows "This list is empty" message
- [x] Plus button opens modal with product fields
- [x] Product appears after adding

### Scraper
- [x] scraper test page create - /scraper
- [x] Scraper calls backend and populates form
- [x] Scraper brings information from URLs
- [x] Generic scraper (JSON-LD, Open Graph, meta tags)
- [x] Site-specific: Amazon.com (URL normalization - auto-add https://)
- [x] Site-specific: eBay.com (@type array handling in JSON-LD)
- [x] Site-specific: jomashop.com (SPA fallbacks - title tag, state patterns)
- [x] Site-specific: jysk.ge (.price-current selector for Georgian prices)
- [x] Site-specific: veli.store, extra.ge
- [x] Site-specific: zoommer.ge, ee.ge

### Wishlist Details (Viewer)
- [x] Non-owner does NOT see "Add Product"
- [ ] "I'll Buy This" opens confirmation modal (not direct reservation)
- [ ] Confirmation modal shows gift title and owner name
- [ ] Only "Yes, I'll buy it" click actually reserves the gift
- [ ] Reserved items show "Purchased" badge with opacity
- [ ] Reserver can cancel own reservation

### Avatar Safe-Load
- [ ] User avatar displays Google profile picture when available
- [ ] If Google picture fails to load, fallback to ui-avatars.com with initials
- [ ] Initials generated from first name + last name (or email first char)
- [ ] Fallback avatar uses brand colors (neon-blue on deep-obsidian)

### Mobile
- [ ] Grid collapses to 1-2 columns
- [ ] Navigation works on mobile
- [ ] Buttons have 44px+ tap targets

---

## Dependencies

### From Sprint 2 (Required)
- [x] Users table + `UsersIUD` proc
- [x] GiftLists table + `GiftListsIUD` proc
- [x] Gifts table + `GiftsIUD` proc
- [x] `GiftListsListByUserID` function
- [x] `GiftsListByGiftListID` function
- [x] ScraperService (existing)

### New for Sprint 3
- [x] `UserGoogleID` column in Users table ✅ (Phase 1)
- [x] `UsersGetSingleByGoogleID` function ✅ (Phase 1)
- [x] `UsersGetSingleByEmail` function (for migration) ✅ (Phase 1)
- [x] Google OAuth NuGet package (`Microsoft.AspNetCore.Authentication.Google`) ✅ (Phase 2)
- [x] Regular user role (RoleID = 2) in Roles table ✅ (Phase 3)
- [x] Google OAuth credentials configured in `appsettings.json` ✅ (Phase 2)
- [x] Redirect URIs registered in Google Cloud Console (Phase 9) ✅

### Phase 5.5 — First-Time Login Welcome Modal ✅
- [x] `UserIsFirstLogin` column added to Users table
- [x] SQL functions updated to include `UserIsFirstLogin`
- [x] `UserDTO` and `UserIudDTO` updated with `UserIsFirstLogin` property
- [x] `UsersRepository.UsersCompleteOnboarding()` method added
- [x] `DashboardModel.ShowWelcomeModal` property added
- [x] `DashboardModel.CompleteOnboardingAsync()` method added
- [x] `POST /api/users/complete-onboarding` endpoint added
- [x] Welcome modal UI in Dashboard/IndexView.cshtml

### Phase 4 — Assets Copied to wwwroot ✅
- [x] `gifter.css` → `/wwwroot/css/website/gifter.css`
- [x] `style.css` → `/wwwroot/css/website/style.css`
- [x] `app.js` → `/wwwroot/js/website/app.js`
- [x] `auth.js` → `/wwwroot/js/website/auth.js`
- [x] `components.js` → `/wwwroot/js/website/components.js`
- [x] `gifter-layout.js` → `/wwwroot/js/website/gifter-layout.js`
- [x] Bootstrap plugin already exists in `/wwwroot/plugins/bootstrap/`
- [x] jQuery plugin already exists in `/wwwroot/plugins/jquery/`
- [x] 63bits-fonts already exists in `/wwwroot/plugins/63bits-fonts/`

### Phase 6 — Dashboard Complete ✅
- [x] `DashboardModel.LoadWishlistsAsync()` loads wishlists by UserID
- [x] `DashboardModel.CreateWishlistAsync()` creates new wishlist via GiftListsIUD
- [x] `DashboardController.Index()` now async with wishlist loading
- [x] `POST /api/dashboard/create-wishlist` endpoint added
- [x] IndexView displays wishlists grid with emoji categories
- [x] Create Wishlist modal with 10 emoji-based occasion types
- [x] Navigation to wishlist detail page after creation
- [x] Empty state displays when no wishlists exist

### Phase 7 — Wishlist Detail Page ✅
- [x] `WishlistDetailModel.cs` created with LoadAsync method
- [x] `WishlistController.cs` created with Detail action
- [x] `DetailView.cshtml` created with product grid and owner/viewer UI
- [x] `GiftListsRepository.GiftListsGetSingleByID()` method added
- [x] `GiftsRepository.GiftsListByGiftListID()` method added
- [x] `GiftListsGetSingleByID` scalar function created
- [x] `GiftsListByGiftListID` table-valued function created

**SQL Scripts (Executed):**

```sql
-- GiftListsGetSingleByID: Scalar function returning wishlist JSON with owner info
-- GiftsListByGiftListID: Table-valued function returning gifts with reserved user info
```

### Phase 5 — Frontend Layout Shell ✅
- [x] Updated `WebsiteLayoutViewModel.cs` with `User`, `IsAuthenticated`, `LanguageCode` properties
- [x] Updated `WebsiteFilterAttribute.cs` to populate User from session
- [x] Created `/Views/Website/Shared/Layout.cshtml` with Gifter dark theme
- [x] Created `/Views/Website/Shared/_HeaderPartial.cshtml` with Google auth + avatar fallback
- [x] Created `/Views/Website/Shared/_FooterPartial.cshtml` with social links
- [x] Added avatar safe-load CSS styles to `gifter.css`
- [x] Added dropdown, toast, form control, button, badge, modal dark theme overrides
- [x] Build succeeds with 0 errors
