<div align="center">

# ✳ CareerCraft Studio

### Professional CV Writing & Career Services

**Your next chapter deserves a beautiful introduction.**

A premium ASP.NET Core MVC website for a planned South African CV-writing and career-support business.

**Phase 1: website + service catalogue + email-based enquiry workflow.**  
**Not yet accepting online payments or CV uploads.**

![C#](https://img.shields.io/badge/C%23-.NET_10-512BD4?logo=dotnet)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET_Core-MVC-4A5278)
![Status](https://img.shields.io/badge/Status-Phase_1_Preview-D8BA89)
![License](https://img.shields.io/badge/Location-South_Africa-14263A)

</div>

---

## About

CareerCraft Studio is being developed as a real CV-writing service. Customers will eventually be able to select a service, securely upload their existing CV, specify target jobs, pay online, check their order status and receive finished documents.

The first release focuses on an original, responsive user interface, a **configurable services catalogue** and a low-risk enquiry process. This release does **not** accept payment, store orders, process CV files or host customer accounts.

## Phase 1 features

- Premium responsive homepage with original CSS CV illustration
- Dedicated services and pricing page
- Configurable prices and turnaround-time descriptions
- Multi-field enquiry form with selected service and target job title
- Optional vacancy link and career goals
- Browser validation and a prefilled email enquiry workflow
- Detailed FAQs and privacy/launch-status page
- Mobile navigation and accessible form labels
- No database, document uploads, payment processing or customer data persistence yet

### Configurable service prices

| Service | Draft price |
|---|---:|
| CV Refresh | **R200** |
| Professional CV | **R350** |
| Cover Letter | **R150** |
| Career Bundle | **R500** |

All are in **South African Rand (ZAR)** and configurable in `CareerCraft.Web/appsettings.json`, under `Studio.Services`. Service details, timelines and prices are read via the `ServiceCatalogue` class.

**Note:** Prices, timelines and scope must be confirmed with the customer before accepting a real order.

## Tech stack

- C# / .NET 10
- ASP.NET Core MVC + Razor Views
- HTML, custom CSS and vanilla JavaScript
- Options pattern for configurable catalogue data
- Responsive and accessible web forms
- Email draft generated client-side with `mailto:`

## Run locally

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```bash
git clone https://github.com/ST10458151/careercraft-studio.git
cd careercraft-studio
dotnet restore CareerCraft.Web/CareerCraft.Web.csproj
dotnet run --project CareerCraft.Web
```

Open the localhost URL displayed in the terminal.

**GitHub Pages cannot run ASP.NET Core.** This application needs a .NET hosting provider to run online.

## Project structure

```text
careercraft-studio/
├── CareerCraft.Web/
│   ├── Controllers/HomeController.cs
│   ├── Models/
│   │   ├── StudioSettings.cs
│   │   └── EnquiryViewModel.cs
│   ├── Views/Home/
│   │   ├── Index.cshtml
│   │   ├── Services.cshtml
│   │   ├── Request.cshtml
│   │   ├── Privacy.cshtml
│   │   └── Error.cshtml
│   ├── Views/Shared/_Layout.cshtml
│   ├── wwwroot/css/studio.css
│   ├── wwwroot/js/site.js
│   ├── appsettings.json
│   ├── Program.cs
│   └── CareerCraft.Web.csproj
└── README.md
```

## Next development phases

1. **Orders & authentication** — ASP.NET Core Identity, customer/admin roles, PostgreSQL and EF Core migrations.
2. **Secure document processing** — private PDF/DOCX uploads, validation, virus/malware scanning, controlled downloads, retention policy and audit trails.
3. **Payments** — select a South African gateway, implement hosted checkout, validate webhooks server-side and handle payment lifecycle events idempotently.
4. **Customer experience** — order status, document delivery, revisions, transactional emails and support.
5. **Production hardening** — tests, CI, deployments, HTTPS, monitoring, backups, business policies and POPIA compliance review.

### Real-business safeguards

Before accepting real customer CVs or money, implement authentication and authorisation, secure file storage, webhook verification, database backups, privacy and retention policies, accurate commercial terms and an appropriate hosting environment. **Never commit actual customer documents, credentials or payment keys to a public repository.**

The current enquiry form opens a draft in the visitor's local email application. The user must explicitly send it, and the website does not record the request. This is not a secure file-transfer channel; do not request identity documents or CV attachments through this preview.

---

Developed by **Shakeelah Khan** · [Developer Portfolio](https://st10458151.github.io/developer-portfolio/) · [GitHub](https://github.com/ST10458151)
