# QaaS.Common.Generators

[![CI](https://img.shields.io/github/actions/workflow/status/TheSmokeTeam/QaaS.Common.Generators/ci.yml?branch=master&label=CI&logo=githubactions)](https://github.com/TheSmokeTeam/QaaS.Common.Generators/actions/workflows/ci.yml)
[![NuGet Version](https://img.shields.io/nuget/v/QaaS.Common.Generators?logo=nuget)](https://www.nuget.org/packages/QaaS.Common.Generators)
[![NuGet Downloads](https://img.shields.io/nuget/dt/QaaS.Common.Generators?logo=nuget&label=downloads)](https://www.nuget.org/packages/QaaS.Common.Generators)
[![Documentation](https://img.shields.io/badge/docs-QaaS-blue?logo=readthedocs)](https://thesmoketeam.github.io/qaas-docs/)

Common generators used by the QaaS framework for producing session-ready data from JSON templates, data sources, external storages, and data lake queries.

## Table of Contents

- [Solution Overview](#solution-overview)
- [Projects and Badges](#projects-and-badges)
- [Generator Catalog](#generator-catalog)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Test Coverage](#test-coverage)
- [Documentation](#documentation)

## Solution Overview

- Solution: `QaaS.Common.Generators.sln`
- Main package project: `QaaS.Common.Generators`
- Test project: `QaaS.Common.Generators.Tests`
- Target framework: `net10.0`

## Projects and Badges

| Project | Purpose | NuGet | Coverage |
|---|---|---|---|
| `QaaS.Common.Generators` | Reusable QaaS generator implementations and configuration models | [![NuGet Version](https://img.shields.io/nuget/v/QaaS.Common.Generators?logo=nuget)](https://www.nuget.org/packages/QaaS.Common.Generators)<br>[![NuGet Downloads](https://img.shields.io/nuget/dt/QaaS.Common.Generators?logo=nuget&label=downloads)](https://www.nuget.org/packages/QaaS.Common.Generators) | ![QaaS.Common.Generators coverage](https://img.shields.io/badge/QaaS.Common.Generators%20coverage-85.91%25-green) |
| `QaaS.Common.Generators.Tests` | NUnit test suite for all generators and configuration behaviors | Not published | ![QaaS.Common.Generators.Tests coverage](https://img.shields.io/badge/QaaS.Common.Generators.Tests%20coverage-97.35%25-brightgreen) |

## Generator Catalog

| Generator | Configuration Type | Description |
|---|---|---|
| `Json` | `JsonConfiguration` | Generates from a JSON prototype data source and supports field injection using JSONPath replacements. |
| `JsonSchemaDraft4` | `JsonSchemaConfiguration` | Generates JSON from Draft-4 schema definitions with randomized value generation (seed-supported). |
| `FromDataSources` | `FromDataSourceBasedConfiguration` | Pass-through generation from provided QaaS data sources. |
| `FromLettuceDataSources` | `FromLettuceDataSourcesConfiguration` | Converts lettuce-formatted JSON payloads (base64 body + routing key metadata) to generated output. |
| `FromSessionDataDataSources` | `List<FromSessionDataDataSourcesConfiguration>` | Reads serialized `SessionData` payloads and emits selected input/output communication items. |
| `Stacking` | `StackingConfiguration` | Round-robin/stacking generator across multiple data sources with configurable items-per-source sequencing. |
| `FromFileSystem` | `FromFileSystemConfig` | Loads raw files from disk with filtering, ordering, count limiting, and storage metadata mapping. |
| `LettuceFromFileSystem` | `LettuceFromFileSystemConfig` | Reads lettuce files from disk, extracts body payload, and enriches metadata with routing key. |
| `FromS3` | `FromS3Config` | Loads objects from S3-compatible storage with configurable ordering, prefix/delimiter filtering, and key mapping. |
| `FromDataLake` | `FromDataLakeConfiguration` | Executes Trino queries (LDAP auth) and yields row-based JSON objects as generated data. |

## Installation

```bash
dotnet add package QaaS.Common.Generators
```

## Quick Start

```csharp
using System.Collections.Immutable;
using System.Text.Json.Nodes;
using QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;
using QaaS.Common.Generators.JsonGenerators;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.SessionDataObjects;

var jsonSource = new DataSource { Name = "base-json" }
    .SetGeneratedData(new List<Data<object>>
    {
        new() { Body = JsonNode.Parse("""{"name":"Alice","age":21}""")! }
    });

var generator = new Json
{
    Context = /* QaaS generator context */,
    Configuration = new JsonConfiguration
    {
        Count = 3,
        JsonDataSourceName = "base-json"
    }
};

var output = generator.Generate(
    ImmutableList<SessionData>.Empty,
    new[] { jsonSource }.ToImmutableList()
).ToList();
```

## Test Coverage

Coverage snapshot below was generated on **March 6, 2026** from the current branch test run.

- `QaaS.Common.Generators`: **85.91%** line coverage
- `QaaS.Common.Generators.Tests`: **97.35%** line coverage

Commands used:

```bash
dotnet test QaaS.Common.Generators.Tests/QaaS.Common.Generators.Tests.csproj --configuration Release --collect "Code Coverage"
dotnet-coverage merge QaaS.Common.Generators.Tests/TestResults/**/**/*.coverage -f cobertura -o QaaS.Common.Generators.Tests/TestResults/coverage.cobertura.xml
```

## Documentation

- QaaS docs portal: [https://thesmoketeam.github.io/qaas-docs/](https://thesmoketeam.github.io/qaas-docs/)
- Repository: [https://github.com/TheSmokeTeam/QaaS.Common.Generators](https://github.com/TheSmokeTeam/QaaS.Common.Generators)
- NuGet package: [https://www.nuget.org/packages/QaaS.Common.Generators](https://www.nuget.org/packages/QaaS.Common.Generators)
