# AGENTS.md — QaaS.Common.Generators
Guidance for AI agents working in this repository.

## What this repo is
`QaaS.Common.Generators` is a Tier-1 shared library providing 11 concrete `IGenerator` hook
implementations that feed lazily-evaluated test-data pipelines inside `QaaS.Runner`. Every
class inherits from `BaseGenerator<TConfig>` (from `QaaS.Framework.SDK`) and yields
`IEnumerable<Data<object>>` lazily to downstream processors and assertions. The package is
published as `QaaS.Common.Generators` (NuGet, net10.0, version prefix `0.0.0`, tag-driven in
CI) and consumed by Runner and user test projects.

## Projects / Layout

| Project | Purpose |
|---|---|
| `QaaS.Common.Generators/` | Main library — 11 generator hooks + config models |
| `QaaS.Common.Generators.Tests/` | NUnit 4.x; uses `System.IO.Abstractions` mock filesystem |

Generator families:
| Family | Classes |
|---|---|
| Structured Payloads | `Json`, `JsonSchemaDraft4` |
| Existing Sources | `FromDataSources`, `FromLettuceDataSources`, `FromSessionDataDataSources`, `Stacking` |
| File Sourced | `FromCSV` (RFC-4180), `FromFileSystem`, `LettuceFromFileSystem` |
| Cloud Sourced | `FromS3` (AWS S3), `FromDataLake` (Trino) |

Config models live under `ConfigurationObjects/` with sub-folders:
`FromDataLakeConfigurations/`, `FromDataSourcesConfigurations/`,
`FromExternalSourceConfigurations/`, `JsonConfigurations/`.

## Build & test
```
dotnet build
dotnet test
dotnet test --collect:"XPlat Code Coverage"
```
CI runs on `windows-latest`. Coverage via `dotnet-coverage collect`; minimum 70%.

## Critical gotchas
- **Assembly scanning contract**: generator classes MUST be public, non-abstract, and implement
  `IGenerator`. Assembly/namespace prefix must be `QaaS.*` or `Common.*` for the scanner.
- **BaseGenerator<TConfig>**: always inherit this; direct `IGenerator` implementation bypasses
  config deserialization and silently fails.
- **Lazy evaluation**: generators yield `IEnumerable<Data<object>>` — never buffer all records
  into a list; downstream runners rely on lazy streaming for memory-bounded pipelines.
- **Tier-1 position**: sits between `QaaS.Framework.*` (Tier 0) and Runner/Mocker (Tier 2).
  Never add a dependency on Runner, Mocker, or any Tier-2+ package.
- **Key dependencies**: `QaaS.Framework.SDK` + `Configurations` + `Protocols` + `Serialization`
  (v1.5.1); `System.IO.Abstractions` (v22.1.0) for testable file I/O.
- **FromCSV**: strict RFC-4180 compliance with double-quote handling — test edge cases when
  modifying CSV parsing logic.
- **Version pinning**: `0.0.0` prefix; actual version injected from Git tag in CI.

## Process
- QaaS harness pipeline: plan → contract → implement → adversarial evaluation (rubric ≥7/10
  on Correctness / Completeness / Craft / Robustness).
- TDD: write failing tests first; mock file I/O via `System.IO.Abstractions`; coverage ≥70%.
- Conventional commits: `feat:`, `fix:`, `chore(release):`, scoped forms like `fix(ci):`.
- `dotnet format` / csharpier clean before every commit.