# QaaS.Common.Generators

Composable generator package for QaaS test workflows.

[![CI](https://img.shields.io/badge/CI-GitHub_Actions-2088FF)](./.github/workflows/ci.yml)
[![Docs](https://img.shields.io/badge/docs-qaas--docs-blue)](https://thesmoketeam.github.io/qaas-docs/)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)

## Contents
- [Overview](#overview)
- [Packages](#packages)
- [Architecture](#architecture)
- [Install and Upgrade](#install-and-upgrade)
- [Documentation](#documentation)

## Overview
This repository contains one solution: [`QaaS.Common.Generators.sln`](./QaaS.Common.Generators.sln).

The solution provides one NuGet package focused on reusable generators for QaaS pipelines.

## Packages
| Package | Latest Version | Total Downloads |
|---|---|---|
| [QaaS.Common.Generators](https://www.nuget.org/packages/QaaS.Common.Generators/) | [![NuGet](https://img.shields.io/nuget/v/QaaS.Common.Generators?logo=nuget)](https://www.nuget.org/packages/QaaS.Common.Generators/) | [![Downloads](https://img.shields.io/nuget/dt/QaaS.Common.Generators?logo=nuget)](https://www.nuget.org/packages/QaaS.Common.Generators/) |

## Architecture
| Project | Type | What It Includes |
|---|---|---|
| [`QaaS.Common.Generators`](./QaaS.Common.Generators/) | NuGet package | Generator implementations and configuration objects for JSON generation (`Json`, `JsonSchemaDraft4`), data source generation (`FromDataSources`, `FromLettuceDataSources`, `FromSessionDataDataSources`, `Stacking`), external source loading (`FromFileSystem`, `LettuceFromFileSystem`, `FromS3`), and data-lake generation (`FromDataLake`). |
| [`QaaS.Common.Generators.Tests`](./QaaS.Common.Generators.Tests/) | Test project | NUnit tests validating generator behavior, configuration handling, parsers, value generators, and source integrations. Not published as a NuGet package. |

## Install and Upgrade
Install:

```bash
dotnet add package QaaS.Common.Generators
```

Upgrade:

```bash
dotnet add package QaaS.Common.Generators --version <TARGET_VERSION>
dotnet restore
```

## Documentation
- Official docs: [thesmoketeam.github.io/qaas-docs](https://thesmoketeam.github.io/qaas-docs/)
- CI workflow: [`.github/workflows/ci.yml`](./.github/workflows/ci.yml)
- NuGet package: [QaaS.Common.Generators](https://www.nuget.org/packages/QaaS.Common.Generators/)
