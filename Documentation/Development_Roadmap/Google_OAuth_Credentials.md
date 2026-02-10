# Google OAuth Credentials

**Project:** Gifter
**Provider:** Google Cloud Console
**Type:** OAuth 2.0 Client ID (Web Application)

---

## Production Credentials

| Field | Value |
|-------|-------|
| **Client ID** | `1049038610778-dnrsrqgmfh2s3mntd2ubg7j56vh9vtks.apps.googleusercontent.com` |
| **Client Secret** | `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` |

---

## Authorized Redirect URIs

Configure these in [Google Cloud Console](https://console.cloud.google.com/apis/credentials):

| Environment | Redirect URI |
|-------------|--------------|
| Development | `http://localhost:5001/signin-google` |
| Production | `https://gifter.ge/signin-google` |
| SmarterASP | `https://your-smarterasp-domain.com/signin-google` |

---

## appsettings.json Configuration

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

---

## Security Notes

1. **Never commit credentials to public repositories** - Use environment variables or user secrets in production
2. **Restrict API keys** - In Google Cloud Console, restrict the OAuth client to specific domains
3. **Rotate secrets** - If credentials are compromised, regenerate them immediately in Google Cloud Console

---

## Google Cloud Console Setup

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Select or create project: **Gifter**
3. Navigate to **APIs & Services** → **Credentials**
4. Click **Create Credentials** → **OAuth 2.0 Client ID**
5. Application type: **Web application**
6. Name: `Gifter Web Client`
7. Add **Authorized redirect URIs** (see table above)
8. Click **Create**

---

## Required OAuth Scopes

| Scope | Purpose |
|-------|---------|
| `email` | Get user's email address |
| `profile` | Get user's name and profile picture |
| `openid` | OpenID Connect authentication |

---

## Claims Mapping

| Google Claim | Maps To | Usage |
|--------------|---------|-------|
| `sub` | `UserGoogleID` | Unique Google identifier |
| `email` | `UserEmail` | User email address |
| `name` | `UserFullname` | Full display name |
| `given_name` | `UserFirstname` | First name |
| `family_name` | `UserLastname` | Last name |
| `picture` | `UserAvatarFilename` | Profile photo URL |
