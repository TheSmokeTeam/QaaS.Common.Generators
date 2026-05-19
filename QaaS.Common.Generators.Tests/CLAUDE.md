# CLAUDE.md — QaaS.Common.Generators.Tests

> Test operating manual. See repo root `CLAUDE.md`.

## Purpose

Behavioural + laziness coverage for every generator. Filesystem and S3
are mocked; data lake / external sources stay behind interfaces.

## Layout

- `JsonGeneratorsTests/` — `JsonTests.cs`, `JsonSchemaDraft4Tests.cs`,
  `JsonNodeFieldInjectorTests.cs`, plus subfolders mirroring source
  (`JsonExtensionsTests/`, `JsonNodeGeneratorsTests/`,
  `JsonParsersTests/`, `JsonValueGeneratorsTests/`).
- `FromDataSourcesGeneratorsTests/`,
  `FromExternalSourceGenerators/`.
- `ConfigurationObjects/`, `ConfigurationObjectsTests/` — config
  validation and laziness contract checks.
- `TestData/` — JSON / schema / CSV fixtures.
- `Globals.cs` — shared Serilog→MEL `Logger`, a `Context` with
  `RootConfiguration = new ConfigurationBuilder().Build()`, and
  `rootPath = "$"` for JSONPath-rooted tests (see `Globals.cs:18-30`).

## Conventions

- **NUnit** + **Moq** (Moq is already a dep — see csproj).
- Filesystem use `System.IO.Abstractions.TestingHelpers`
  (`MockFileSystem`); never touch the real disk outside `TestData/`.
- S3 / data lake clients are mocked via their interface.
- Laziness tests assert that materialising 1 element does not pull N
  (see `ConfigurationObjects/Laziness/...`).
- Test data is loaded relative to the test assembly — keep
  `CopyToOutputDirectory` set on `TestData/` items.

## Forbidden

1. `[Test(Ignore=...)]` / `[Explicit]` to mask a flake — diagnose.
2. Hitting real S3 / Trino / network in unit tests.
3. `.ToList()` on a generator output before asserting laziness.
4. Sharing mutable state via `Globals` beyond `Logger` / `Context`.
5. Adding a generator without paired tests + a laziness test.

## Run

```bash
dotnet test ../QaaS.Common.Generators.sln --nologo --no-build
```
