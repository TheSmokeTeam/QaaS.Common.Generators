# CLAUDE.md — QaaS.Common.Generators

> Operating manual; see `project_specs.md` for the spec.
> Live docs: <https://docs.qaas.online/generators/>.

## Mission

Pre-built `IGenerator` implementations consumed by `QaaS.Runner`.

## Build / Test

```bash
dotnet build QaaS.Common.Generators.sln --nologo -clp:ErrorsOnly
dotnet test  QaaS.Common.Generators.sln --nologo --no-build
csharpier format <changed-files>
```

## Solution layout

| Project | Purpose |
|---|---|
| `QaaS.Common.Generators` | Generator implementations. |
| `QaaS.Common.Generators.Tests` | NUnit tests, mocked filesystem and S3. |

## Shipped generators

**JSON:** `Json` (from prototype document), `JsonSchemaDraft4` (from
schema).

**DataSource:** `FromDataSources`, `FromLettuceDataSources`,
`FromSessionDataDataSources`, `Stacking`.

**Files / templates:** `FromCSV` (RFC-4180), `FromFileSystem`,
`LettuceFromFileSystem`.

**Cloud / data lake:** `FromS3` (AWSSDK.S3), `FromDataLake` (Trino).

## Conventions

- Every generator inherits `BaseGenerator<TConfig>`.
- File / S3 access goes through `System.IO.Abstractions` + AWSSDK so
  tests can mock cleanly.
- `IEnumerable<Data<object>>` is **lazy** — do not materialise
  internally.

## Forbidden

1. Mutate `IImmutableList<DataSource>`.
2. Yield infinite sequences without an explicit policy-driven cap.
3. Open file / socket handles without `using`.
4. Cache parsed JSON / schema across instances.
5. Hard-code connection defaults.
6. Yield the same instance twice (downstream may mutate).
7. Parse CSV without quoted-field support.

## Must-verify

1. `dotnet build` / `dotnet test` green.
2. Framework SDK 1.4.2 (`csproj:24`).
3. CSV quoted-field support intact.
4. S3 / FS paths absolute or resolved (no surprise CWD reads).
5. CI green.

## Recent

- PR #17 (`feature/docs-claude`) — CLAUDE.md drop.
- Latest: 3.2.1-alpha.2; framework 1.4.2 alignment.
