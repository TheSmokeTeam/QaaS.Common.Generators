# Copilot Instructions — QaaS.Common.Generators

Read `AGENTS.md` at the repo root first — it details the generator hook contract, lazy
evaluation requirements, assembly scanning rules, tier constraints, and process requirements.

## Essentials
- **TFM**: net10.0; `<Nullable>enable</Nullable>`; `<ImplicitUsings>enable</ImplicitUsings>`
- **Test framework**: NUnit 4.x; mock file I/O with `System.IO.Abstractions`; coverage gate 70%
- **Build**: `dotnet build` | **Test**: `dotnet test`
- **Hook contract**: every generator inherits `BaseGenerator<TConfig>` (from `QaaS.Framework.SDK`);
  yields `IEnumerable<Data<object>>` lazily - never materialise all records into a list
- **Tier-1 rule**: no dependencies on QaaS.Runner, QaaS.Mocker, or any Tier-2+ package
- **Top gotcha**: assembly/namespace prefix must remain `QaaS.*` or `Common.*` - renaming
  breaks runtime hook discovery silently
- **Commit style**: conventional commits (`feat:`, `fix:`, `chore(release):`) + `dotnet format` clean