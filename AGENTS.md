# SmartCareApp Agent Rules

You are working inside `SmartCareApp`, a macOS-first ASP.NET Core MVC healthcare demo built to run from VS Code with `dotnet` CLI commands.

## Environment
- Target `.NET 8`.
- The developer works on macOS in VS Code.
- Do not add a new top-level project folder. Work inside the existing repository root.
- The app uses `Microsoft.EntityFrameworkCore.Sqlite` and stores data in a local SQLite file.

## Architecture
- Use a single ASP.NET Core MVC app unless a split is justified by real complexity.
- Keep controllers thin and async.
- Put business rules in focused services only when the logic is non-trivial or reused.
- Use EF Core with Code-First models.
- Preserve the SQLite-first workflow: by default the database file lives at `App_Data/smartcareapp.db`.
- Preserve the design-time factory so `dotnet ef` migrations keep working on macOS and on Dokploy.

## Required domain concepts
- `Patient`
- `Doctor`
- `Appointment`
- `MedicalHistory`

## Demo differentiators to preserve
- Triage risk score based on age, symptoms, and chronic conditions.
- No-show prediction signal based on attendance history and portal usage.
- Reusable patient medical timeline using a Razor ViewComponent.
- Reusable risk badge using a custom TagHelper.

## Coding style
- Prefer explicit, readable code over heavy abstraction.
- Use strongly typed views and view models.
- Keep demo language realistic: smart workflow support is fine, fake medical certainty is not.
- Avoid adding JavaScript-heavy frontends or SPA frameworks.

## Expected delivery from an agent
- If you add or change data models, update EF Core mappings and mention the exact `dotnet tool run dotnet-ef ...` command needed.
- If you add UI, keep it server-rendered Razor and consistent with the existing visual language.
- If you change business rules, verify both dashboard and patient detail flows still compile.
