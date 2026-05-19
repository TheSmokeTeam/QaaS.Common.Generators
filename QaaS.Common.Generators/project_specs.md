# project_specs.md — QaaS.Common.Generators (package project)

Single package project. Generators grouped by source type. Tests live in
`QaaS.Common.Generators.Tests`.

## Folders

- `JsonGenerators/` — `Json`, `JsonSchemaDraft4`.
- `FromDataSources/`, `Stacking/` — pull from existing session sources.
- `FromCSV/`, `FromFileSystem/`, `Lettuce*` — file-driven.
- `FromS3/`, `FromDataLake/` — cloud + Trino.

## Forbidden

- Adding non-generator hook implementations.
- Eager materialisation of `IEnumerable<Data<object>>`.
