# project_specs.md — QaaS.Common.Generators

Distributes 10 pre-built `IGenerator` implementations grouped by source
type: JSON shapes, existing DataSources, file/template, cloud / data
lake. Discovered by `QaaS.Framework.Providers`.

## Categories

- **JSON generation (2):** `Json`, `JsonSchemaDraft4`.
- **DataSource-based (4):** `FromDataSources`, `FromLettuceDataSources`,
  `FromSessionDataDataSources`, `Stacking`.
- **File / template (3):** `FromCSV`, `FromFileSystem`,
  `LettuceFromFileSystem`.
- **Cloud / lake (2):** `FromS3` (AWSSDK.S3 4.x), `FromDataLake` (Trino).

## Public surface

- `BaseGenerator<TConfig>` is the inheritance root (from `Framework.SDK`).
- `BaseExternalSourceBasedGenerator<TConfig, TItem>` for cloud sources.
- Configurations are records with DataAnnotations.

## Build, packaging, CI

- Target: `.NET 10.0`. NuGet identity: `QaaS.Common.Generators`.
- CI: standard solution pipeline.
- Test FS / S3 are mocked via `System.IO.Abstractions` + AWS mocks.

## Compatibility

Tracks `QaaS.Framework.SDK` major version (currently 1.4.2).

## References

- Live docs: <https://docs.qaas.online/generators/>
- Sibling repos: `QaaS.Common.Assertions`, `…Probes`, `…Processors`.
