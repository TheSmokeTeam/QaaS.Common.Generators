# QaaS.Common.Generators

Composable .NET package for generating QaaS test data workflows.

[![CI](https://img.shields.io/badge/CI-GitHub_Actions-2088FF)](./.github/workflows/ci.yml)
[![Docs](https://img.shields.io/badge/docs-qaas--docs-blue)](https://thesmoketeam.github.io/qaas-docs/)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)

## Contents
- [Overview](#overview)
- [Packages](#packages)
- [Functionalities](#functionalities)
- [Quick Start](#quick-start)
- [Build and Test](#build-and-test)
- [Documentation](#documentation)

## Overview
This repository contains one solution: [`QaaS.Common.Generators.sln`](./QaaS.Common.Generators.sln).

The solution is focused on one NuGet package that provides reusable generator implementations for QaaS data pipelines.

## Packages
| Package | Latest Version | Total Downloads |
|---|---|---|
| [QaaS.Common.Generators](https://www.nuget.org/packages/QaaS.Common.Generators/) | [![NuGet](https://img.shields.io/nuget/v/QaaS.Common.Generators?logo=nuget)](https://www.nuget.org/packages/QaaS.Common.Generators/) | [![Downloads](https://img.shields.io/nuget/dt/QaaS.Common.Generators?logo=nuget)](https://www.nuget.org/packages/QaaS.Common.Generators/) |

## Functionalities
### [QaaS.Common.Generators](./QaaS.Common.Generators/)
- JSON generation from prototypes and Draft-4 schemas (`Json`, `JsonSchemaDraft4`).
- Data-source based generation (`FromDataSources`, `FromLettuceDataSources`, `FromSessionDataDataSources`, `Stacking`).
- External source loading from file system and S3 (`FromFileSystem`, `LettuceFromFileSystem`, `FromS3`).
- Data lake querying through Trino (`FromDataLake`).
- Typed configuration model set for each generator family under `ConfigurationObjects`.

### [QaaS.Common.Generators.Tests](./QaaS.Common.Generators.Tests/)
- NUnit coverage for generators, parsers, value generators, and configuration behavior.
- Source and integration behavior validation for file system, S3, and session-data based flows.

## Quick Start
Install package:

```bash
dotnet add package QaaS.Common.Generators
```

Update package:

```bash
dotnet add package QaaS.Common.Generators --version 1.1.0
dotnet restore
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
