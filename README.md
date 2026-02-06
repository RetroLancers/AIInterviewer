# AI Interviewer

AI Interviewer is a full-stack, voice-enabled application designed to conduct simulated job interviews using advanced LLMs (e.g., Gemini). It provides a real-time, conversational practice experience and produces a detailed post-interview report.

## Table of Contents

- [Features](#features)
- [Architecture Overview](#architecture-overview)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Quick Start (Development)](#quick-start-development)
  - [Run the Frontend Only](#run-the-frontend-only)
  - [Run the Backend Only](#run-the-backend-only)
- [Configuration](#configuration)
  - [Environment Variables](#environment-variables)
  - [Database](#database)
  - [Auth & Identity](#auth--identity)
- [Key URLs](#key-urls)
- [Common Commands](#common-commands)
- [Development Workflow](#development-workflow)
  - [Regenerate TypeScript DTOs](#regenerate-typescript-dtos)
  - [Adding New API Endpoints](#adding-new-api-endpoints)
  - [Adding New Frontend Pages](#adding-new-frontend-pages)
  - [AutoQuery CRUD (okai)](#autoquery-crud-okai)
- [Testing](#testing)
- [Build & Deployment](#build--deployment)
- [Troubleshooting](#troubleshooting)
- [Additional Documentation](#additional-documentation)

## Features

- **Voice-first interaction**: Real-time voice conversations powered by LLM-based transcription and response.
- **Interview workflow**:
  - **Interviewer persona generation** to match a target role.
  - **Live interview** session driven by the generated context.
  - **Comprehensive reporting** with hiring recommendations, strengths/weaknesses, and answer analysis.
- **Modern full-stack architecture**:
  - ServiceStack-based APIs with ASP.NET Core Identity authentication.
  - Vue 3 + TypeScript SPA with Vite and Tailwind CSS.

## Architecture Overview

The application follows a hybrid development/production model:

- **Development**: `dotnet watch` runs the .NET backend and Vite dev server in tandem. ASP.NET Core proxies frontend requests to Vite for HMR.
- **Production**: Vite builds to `AIInterviewer.Client/dist`, and the backend serves the static assets from `AIInterviewer/wwwroot`.

## Tech Stack

**Backend**
- .NET 10 + ServiceStack v10
- ASP.NET Core Identity
- SQLite + dual ORM strategy (EF Core for Identity, OrmLite for app data)

**Frontend**
- Vue 3 + TypeScript
- Vite 7
- Tailwind CSS 4
- Pinia
- `@servicestack/vue`
- File-based routing (`unplugin-vue-router`)
- Markdown pages (`unplugin-vue-markdown`)

## Project Structure

```
AIInterviewer/                         # .NET Backend (hosts backend + built frontend)
├── Configure.*.cs             # Modular startup configuration
├── Migrations/                # EF Core Identity + OrmLite migrations
├── Pages/                     # Identity Razor Pages
└── wwwroot/                   # Production static files (from AIInterviewer.Client/dist)

AIInterviewer.Client/                  # Vue Frontend
├── src/
│   ├── pages/                 # File-based routes (Vue + Markdown)
│   ├── layouts/               # Layout components
│   ├── components/            # Reusable UI
│   ├── stores/                # Pinia stores
├── assets/
├── dtos.ts                    # Generated from C# DTOs
└── vite.config.ts

AIInterviewer.ServiceModel/            # API Contracts (DTOs)
AIInterviewer.ServiceInterface/        # Service implementations
AIInterviewer.Tests/                   # NUnit tests
```

## Getting Started

### Prerequisites

Ensure these are installed:

- **.NET 10 SDK**
- **Node.js** (LTS recommended)
- **npm**

### Quick Start (Development)

Run backend + Vite dev server together:

```bash
dotnet watch
```

This starts:
- **Backend** on https://localhost:5001
- **Vite** on https://127.0.0.1:5173 (proxied by the backend)

### Run the Frontend Only

```bash
cd AIInterviewer.Client
npm install
npm run dev
```

### Run the Backend Only

```bash
cd AIInterviewer
dotnet watch
```

## Configuration

### Environment Variables

- `KAMAL_DEPLOY_HOST`: Production hostname for Kamal deployments.

### Database

The app uses **SQLite** by default (`App_Data/app.db`). Connection strings live in `AIInterviewer/appsettings.json`.

Dual ORM strategy:
- **Entity Framework Core** for ASP.NET Core Identity tables
- **OrmLite** for application data

### Auth & Identity

ASP.NET Core Identity powers authentication at `/Identity/*`. ServiceStack integrates with Identity via `IdentityAuth.For<ApplicationUser>()` in `Configure.Auth.cs`.

## Key URLs

When running locally:

- **SPA**: https://127.0.0.1:5173 (dev) or https://localhost:5001 (backend-served)
- **ServiceStack API**: `/api/*`
- **Identity UI**: `/Identity/*`
- **API Explorer**: `/ui`
- **Admin UI** (requires Admin role): `/admin-ui`
- **Swagger**: `/swagger-ui`
- **Health**: `/up`

## Common Commands

### Development

```bash
dotnet watch
```

### Tailwind for Razor Pages

```bash
cd AIInterviewer
npm run ui:dev
```

### Database Migrations

```bash
cd AIInterviewer
npm run migrate
```

### Revert / Rerun last migration

```bash
cd AIInterviewer
npm run revert:last
npm run rerun:last
```

## Development Workflow

### Regenerate TypeScript DTOs

After changing C# DTOs in `AIInterviewer.ServiceModel/`, restart .NET and run:

```bash
cd AIInterviewer.Client
npm run dtos
```

### Adding New API Endpoints

1. Define request/response DTOs in `AIInterviewer.ServiceModel/`.
2. Implement service logic in `AIInterviewer.ServiceInterface/` (if not AutoQuery).
3. Restart .NET and regenerate DTOs (see above).

### Adding New Frontend Pages

Create a new file in `AIInterviewer.Client/src/pages/`:

```text
src/pages/my-page.vue  ->  /my-page
```

Optional layout:

```vue
<route lang="yaml">
meta:
  layout: admin
</route>
```

### AutoQuery CRUD (okai)

Quickly scaffold CRUD APIs from a `.d.ts` data model:

```bash
npx okai init Table
npx okai Table.d.ts
```

## Testing

```bash
cd AIInterviewer.Tests
dotnet test
```

## Build & Deployment

### Frontend Build

```bash
cd AIInterviewer.Client
npm run build
```

### Backend Publish

```bash
cd AIInterviewer
dotnet publish
```

The frontend build output is copied to `AIInterviewer/wwwroot` during publish.

### CI / Deployment

GitHub Actions workflows are located in `.github/workflows/` and include:

- `build.yml`: CI build & test
- `build-container.yml`: Container image build
- `release.yml`: Kamal deployment

Kamal configuration is in `config/deploy.yml`.

## Troubleshooting

- **Vite not responding**: Ensure `npm install` has been run in `AIInterviewer.Client`.
- **DTOs out of sync**: Restart .NET, then run `npm run dtos`.
- **Migrations failing**: Run `npm run migrate` from `AIInterviewer` and verify `App_Data/app.db` permissions.

## Additional Documentation

- [DocumentationQuickStart.md](./DocumentationQuickStart.md)
- [ServiceStack AutoQuery CRUD](https://react-templates.net/docs/autoquery/crud)
- [ServiceStack API Design](https://docs.servicestack.net/api-design)
