<div align="center">

# ✳ CareerCraft Studio

### Professional CV Writing & Career Services

*Your next chapter deserves a beautiful introduction.*

A full-stack **ASP.NET Core MVC** application for a planned South African CV writing business.

**Development stage: Customer accounts, CV requests, private local documents and admin workflow. No live payments.**

![C#](https://img.shields.io/badge/C%23-.NET_10-512BD4?logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC-223A52)
![Identity](https://img.shields.io/badge/Auth-ASP.NET_Identity-55798B)
![SQLite](https://img.shields.io/badge/Database-SQLite-003B57?logo=sqlite)
![Status](https://img.shields.io/badge/Status-Local_Development_Only-B68B6A)

</div>

---

## Overview

CareerCraft Studio aims to let customers select a CV service, describe their work history and target vacancies, securely share supporting documents and follow the progress of their CV request. Administrators can manage cases and their files.

**This implementation is a development MVP. Do not deploy publicly or accept genuine customers, CVs or payments yet.** Significant production-security work remains, detailed below.

### Configurable services (ZAR)

| Service | Draft price |
| --- | ---: |
| CV Refresh | **R200** |
| Professional CV | **R350** |
| Cover Letter | **R150** |
| Career Bundle | **R500** |

Prices and descriptions are configured centrally in `CareerCraft.Web/appsettings.json` under `Studio.Services`. When a customer submits a request, the selected service price is recorded with the order. Orders do not count as payments.

## Current features

- Responsive editorial landing page, service pages and FAQs
- Customer registration and login using ASP.NET Core Identity
- Password strength policy, lockout support and cookie authentication
- Customer-only views of CV requests and attached documents
- Detailed career briefing: education, experience, skills, achievements and career goals
- Target positions, vacancy links and job descriptions for tailored CV requests
- **My Documents**: attach an existing CV, cover letter, qualifications or vacancy documents to a specific request
- PDF/DOCX extension and file-signature checks, 8 MB per-file limit
- Random server-side filenames stored **outside `wwwroot`**
- Authorised download and file removal endpoints
- Admin-only dashboard for all requests, status changes and document access
- Revision request facility when an order is marked Ready for Review
- Payments shown as unavailable/pending: **there is no checkout and no automatic marking of orders as paid**

## Tech stack

**C# / .NET 10 · ASP.NET Core MVC · ASP.NET Core Identity · Entity Framework Core · SQLite (local development) · Razor · CSS · Vanilla JavaScript**

A production release should move to a managed database, schema migrations, production-grade private object storage, secure authentication workflows and verified payment integration.

## Get started locally

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```bash
git clone https://github.com/ST10458151/careercraft-studio.git
cd careercraft-studio
dotnet restore CareerCraft.Web/CareerCraft.Web.csproj
dotnet run --project CareerCraft.Web --launch-profile http
```

Open the localhost address shown in Terminal. The `http` launch profile sets `ASPNETCORE_ENVIRONMENT=Development`. The app creates its local SQLite database via `EnsureCreated()` in this environment. Never commit the database or customer files.

**If you saw `no such table: AspNetUsers` or `AspNetRoles`:** the previous launch ran in Production mode and never initialized Identity. Pull the updated launch profile, then run using `--launch-profile http`. If an older local database has an incomplete schema, stop the app, back up your database file before making changes, and only replace it with a fresh local test database when you know it contains no important records. This MVP doesn't support migrations yet. Never delete genuine customer data.

### Create a local admin account

There is **no public admin registration page**. The role and a development-only administrator can be provisioned explicitly from Terminal:

```bash
export CAREERCRAFT_ADMIN_EMAIL="admin@example.test"
export CAREERCRAFT_ADMIN_PASSWORD="Choose-a-long-unique-password!"
dotnet run --project CareerCraft.Web --launch-profile http -- --seed-admin
unset CAREERCRAFT_ADMIN_EMAIL CAREERCRAFT_ADMIN_PASSWORD
```

Replace the example email and password with your own development-only credentials. Don't paste a real password into chat, logs, or screenshots; if a password is ever exposed, rotate it immediately. Use a **unique password** that is not used on any other site. The `--seed-admin` bootstrap is only accepted while the app runs in Development and requires environment variables.

To see the customer flow, register a separate customer account and create a CV request.

## Project layout

```text
CareerCraft.Web/
├── Controllers/
│   ├── AccountController.cs     # Register, login, logout
│   ├── AdminController.cs       # Admin order management
│   ├── DocumentsController.cs   # Authenticated upload / download / delete
│   ├── OrdersController.cs      # Customer requests and revisions
│   └── HomeController.cs        # Public marketing site
├── Data/
│   └── StudioDbContext.cs       # Identity and order models
├── Models/
│   ├── OrderModels.cs           # Orders, files, revisions and intake form
│   └── StudioSettings.cs        # Configurable service catalogue
├── Views/
│   ├── Account/                # Customer authentication
│   ├── Orders/                 # Request wizard and order detail
│   ├── Documents/              # My Documents
│   ├── Admin/                  # Private administration
│   └── Home/                   # Public website
├── wwwroot/                    # Public styles and scripts ONLY
└── appsettings.json            # Development configuration
```

Private documents are saved to `CareerCraftPrivateFiles/`, a sibling of the ASP.NET project folder, and are excluded from Git. File downloads go through an authenticated controller rather than a publicly served URL.

## Security status and launch blockers

The current safeguards (Identity, authenticated routes, role checks, anti-forgery validation, size limits and upload file signatures) are **not sufficient for accepting real customer CVs**.

Before production you must implement and test:

1. **Verified email, password resets, MFA for admins**, account recovery and production cookie/security settings.
2. **EF Core migrations and a managed database** (PostgreSQL is planned), with backups and audited schema changes.
3. **Hardened private file storage** with malware scanning, safe DOCX ZIP inspection, file quarantine, quota and rate controls, access logging, secure deletion and retention.
4. **Verified South African payment gateway integration**, server-to-server notifications/webhooks, replay handling and idempotent order-payment transactions.
5. **Security testing** for role escalation, cross-account access, upload abuse, request size limits, privacy leaks, brute-force attempts and CSRF.
6. **POPIA-related operating procedures**: accurate privacy notices, lawful processing, retention, breach response, appropriate service agreements and access controls.
7. **Commercial and operational rules** for revision limits, cancellations, refunds, order acceptance and delivery.
8. **Operational hosting** with HTTPS, protected secrets, structured logging, monitoring, disaster recovery and environment isolation.

`EnsureCreated()` is used only for the local development MVP and will be replaced with EF migrations. The application does not send email verification messages. No payment provider or backend order payment confirmation exists yet. Never collect real customer documents before addressing these blockers.

## Next roadmap milestone

Prepare a staging environment, migrate to PostgreSQL with migrations, integrate a verified email provider and admin MFA, and implement a private cloud upload service with malware scanning. After these security features and tests are complete, integrate hosted checkout via a South African payment gateway.

---

Developed by **Shakeelah Khan** · [Developer Portfolio](https://st10458151.github.io/developer-portfolio/) · [GitHub](https://github.com/ST10458151)
