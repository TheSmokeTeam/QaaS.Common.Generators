# CLAUDE.md — QaaS.Common.Generators (library)

> Project-level operating manual. See repo root `CLAUDE.md` and
> `project_specs.md`.

## Purpose

Pre-built `IGenerator` implementations consumed by `QaaS.Runner`.
Generators yield `IEnumerable<Data<object>>` lazily for downstream
processors / assertions.

## Source folders

- `JsonGenerators/` — `Json.cs` (prototype-document driven),
  `JsonSchemaDraft4.cs`, plus `JsonNodeGenerators/`,
  `JsonValueGenerators/`, `JsonParsers/`, `JsonExtensions/`,
  `JsonNodeFieldInjector.cs`.
- `FromDataSourcesGenerators/` — `FromDataSources`,
  `FromLettuceDataSources`, `FromSessionDataDataSources`, `Stacking`.
- `CsvGenerators/` — RFC-4180 `FromCSV` + `LettuceFromFileSystem`,
  `FromFileSystem`.
- `FromExternalSourceGenerators/`, `FromDataLakeGenerator/` —
  AWSSDK.S3 / Trino sourced.
- `ConfigurationObjects/`, `Constants/`.

## Conventions

- Every generator inherits `BaseGenerator<TConfig>`.
- File / S3 access via `System.IO.Abstractions` + AWSSDK so tests can
  mock cleanly.
- Lazy `IEnumerable<Data<object>>` — never materialise upstream into
  `List<>` inside the generator.
- Each yielded `Data<object>` is a fresh instance (downstream may
  mutate).

## Forbidden

1. Mutating `IImmutableList<DataSource>`.
2. Yielding infinite sequences without an explicit policy-driven cap.
3. Opening file / socket handles without `using` / `await using`.
4. Caching parsed JSON or schema across generator instances.
5. Hard-coding connection defaults (host, region, bucket).
6. Yielding the same instance twice.
7. Parsing CSV without quoted-field support (`FromCSV` must stay
   RFC-4180).

## Build

```bash
dotnet build ../QaaS.Common.Generators.sln --nologo -clp:ErrorsOnly
csharpier format <changed-files>
```

Framework SDK 1.4.2 alignment is enforced via `Directory.Build.props`.
