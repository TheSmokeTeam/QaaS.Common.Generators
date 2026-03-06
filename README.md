# QaaS.Common.Generators

Composable generator package for QaaS test workflows.

[![CI](https://img.shields.io/badge/CI-GitHub_Actions-2088FF)](./.github/workflows/ci.yml)
[![Docs](https://img.shields.io/badge/docs-qaas--docs-blue)](https://thesmoketeam.github.io/qaas-docs/)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![NuGet Version](https://img.shields.io/nuget/v/QaaS.Common.Generators?logo=nuget)](https://www.nuget.org/packages/QaaS.Common.Generators/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/QaaS.Common.Generators?logo=nuget)](https://www.nuget.org/packages/QaaS.Common.Generators/)

## Contents
- [Overview](#overview)
- [Packages](#packages)
- [Functionalities](#functionalities)
- [Project Structure](#project-structure)
- [Quick Start](#quick-start)
- [Build and Test](#build-and-test)
- [Documentation](#documentation)

## Overview
This repository contains one solution: [`QaaS.Common.Generators.sln`](./QaaS.Common.Generators.sln).

It provides reusable generators that convert input sources into QaaS `Data<object>` outputs, including JSON-template generation, schema-based generation, external source loading, data-source reshaping, and data-lake querying.

## Packages
| Package | Latest Version | Total Downloads |
|---|---|---|
| [QaaS.Common.Generators](https://www.nuget.org/packages/QaaS.Common.Generators/) | [![NuGet](https://img.shields.io/nuget/v/QaaS.Common.Generators?logo=nuget)](https://www.nuget.org/packages/QaaS.Common.Generators/) | [![Downloads](https://img.shields.io/nuget/dt/QaaS.Common.Generators?logo=nuget)](https://www.nuget.org/packages/QaaS.Common.Generators/) |

## Functionalities
### JSON Generators
- `Json`: Generates from a prototype JSON data source.
- `JsonSchemaDraft4`: Generates JSON values from Draft-4 schema definitions.
- JSON field replacement pipeline via JSONPath-based injection rules.
- Output conversion through parser strategies (`Json`, `Binary`, `Xml`, `ProtobufMessage`).

### Data Source Generators
- `FromDataSources`: Pass-through generation from configured data sources.
- `FromLettuceDataSources`: Converts lettuce-formatted JSON payloads into generated data.
- `FromSessionDataDataSources`: Loads serialized session data and emits selected communication items.
- `Stacking`: Round-robin generation across multiple sources with configurable items-per-source strategy.

### External Source Generators
- `FromFileSystem`: Loads files from a configured path with ordering, count limits, and regex filtering.
- `LettuceFromFileSystem`: Reads lettuce files from the file system and maps routing metadata.
- `FromS3`: Loads from S3-compatible storage with ordering and metadata options.

### Data Lake Generator
- `FromDataLake`: Executes Trino queries and emits row-based JSON output.

## Project Structure
| Project | Role |
|---|---|
| [`QaaS.Common.Generators`](./QaaS.Common.Generators/) | Packable library that contains all generator and configuration implementations. |
| [`QaaS.Common.Generators.Tests`](./QaaS.Common.Generators.Tests/) | NUnit-based test suite for generator behavior and configuration validation. |

## Quick Start
Install package:

```bash
dotnet add package QaaS.Common.Generators
```

Minimal usage:

```csharp
using QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;
using QaaS.Common.Generators.JsonGenerators;

var generator = new Json
{
    Context = /* generator context */,
    Configuration = new JsonConfiguration
    {
        Count = 10,
        JsonDataSourceName = "seed-json"
    }
};
```

## Build and Test
```bash
dotnet restore QaaS.Common.Generators.sln
dotnet build QaaS.Common.Generators.sln -c Release --no-restore
dotnet test QaaS.Common.Generators.sln -c Release --no-build
```

## Documentation
- Official docs: [thesmoketeam.github.io/qaas-docs](https://thesmoketeam.github.io/qaas-docs/)
- CI workflow: [`.github/workflows/ci.yml`](./.github/workflows/ci.yml)
- NuGet package: [QaaS.Common.Generators](https://www.nuget.org/packages/QaaS.Common.Generators/)
