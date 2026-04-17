# SmartCareApp

SmartCareApp is a macOS-friendly ASP.NET Core MVC showcase for doctor, patient, appointment, and medical history workflows. It is designed to demo well in a job interview or client presentation: the app highlights triage prioritization, no-show risk, and a patient timeline instead of stopping at CRUD tables.

## What is set up
- ASP.NET Core MVC on `.NET 8`
- VS Code friendly workflow with `dotnet watch run`
- EF Core with SQLite provider
- File-based database with no separate database server requirement
- Local `dotnet-ef` tool manifest
- Sample seed data for dashboard and patient detail flows
- Razor ViewComponent for the patient timeline
- Custom TagHelper for risk badges

## Project pitch
- **Triage Risk Score**: assigns a live 0-100 urgency score from intake signals.
- **No-Show Signal**: estimates slot risk using missed appointment count and recent portal activity.
- **Medical Timeline**: renders appointments and care events as a vertical timeline for doctor review.

## Local macOS workflow
1. Open the folder in VS Code.
2. If HTTPS is not trusted yet, optionally run:

```bash
dotnet dev-certs https --trust
```

3. Restore and run:

```bash
dotnet restore
dotnet watch run
```

4. Open the app at the URL printed by the terminal. The default launch profile includes:
- `https://localhost:7123`
- `http://localhost:5144`

## Database file
By default the application uses:

```bash
App_Data/smartcareapp.db
```

You can override it with an environment variable:

```bash
export ConnectionStrings__SmartCareDb="Data Source=/absolute/path/to/smartcareapp.db"
```

On Dokploy or another server, mount a persistent volume to the app directory and keep the SQLite file inside that mounted path.

## EF Core commands
The repository uses a local tool manifest, so migrations are consistent across machines.

Create a migration:

```bash
dotnet tool run dotnet-ef migrations add InitialCreate --output-dir Data/Migrations
```

Apply migrations to the SQLite database:

```bash
dotnet tool run dotnet-ef database update
```

## Suggested implementation phases
1. Foundation: tighten models, validation, dashboard, and navigation.
2. Scheduling: doctor availability, booking conflicts, cancellation policies.
3. Clinical flow: visit notes, prescriptions, richer patient detail pages.
4. Demo polish: analytics, alerts, next-best-action cards, printable summaries.

## VS Code extensions
- `ms-dotnettools.csharp`
- `ms-dotnettools.csdevkit`
- `ms-dotnettools.vscodeintellicode-csharp`
