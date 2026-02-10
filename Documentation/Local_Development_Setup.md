# Local Development Setup

How to run the Gifter project locally on macOS.

---

## Daily Startup

### 1. Open terminal in the project folder

```bash
cd "/Users/aliksukiasov/Library/CloudStorage/GoogleDrive-asukiasov@gmail.com/My Drive/Gifter/Gifter_New/SixtyThreeBits.Boilerplate"
```

### 2. Run the project

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run --project SixtyThreeBits.Web/SixtyThreeBits.Web.csproj --urls http://localhost:5001
```

### 3. Open in browser

| Page | URL |
|------|-----|
| Website (home) | http://localhost:5001 |
| Admin panel | http://localhost:5001/admin |
| Admin login | http://localhost:5001/admin/login |

### 4. Stop the server

Press `Ctrl+C` in the terminal.

---

## Why these flags are needed

### `ASPNETCORE_ENVIRONMENT=Development`

The app's `Startup.cs` adds an HTTPS redirect (`AddRedirectToHttpsPermanent`) in non-Development environments. Since localhost has no SSL certificate, the browser gets `ERR_CONNECTION_REFUSED` when the redirect fires.

Setting the environment variable to `Development` skips the HTTPS redirect, so HTTP works normally.

### `--urls http://localhost:5001`

macOS ControlCenter occupies port 5000 by default. Using port 5001 avoids the conflict.

---

## Prerequisites

- .NET 10 SDK installed (`dotnet --version` to check)
- Internet connection (the database is hosted on SmarterASP: `SQL1001.site4now.net`)
- Connection string is in `appsettings.json` — no local DB setup needed

---

## Troubleshooting

| Problem | Cause | Fix |
|---------|-------|-----|
| `ERR_CONNECTION_REFUSED` on `/admin` | Missing `ASPNETCORE_ENVIRONMENT=Development` | Add the env variable before `dotnet run` |
| Port already in use | Previous server still running | Check with `lsof -i :5001` then `kill <PID>` |
| Build errors after pulling changes | Stale build cache | Run `dotnet clean` then `dotnet build` |
| Database connection timeout | SmarterASP server down or network issue | Try again later |
