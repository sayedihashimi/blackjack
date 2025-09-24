# .NET 9.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 9.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 9.0 upgrade.
3. Upgrade SayedHa.Blackjack.Shared.csproj
4. Upgrade SayedHa.Blackjack.Console.csproj
5. Upgrade SayedHa.Blackjack.Api.csproj
6. Upgrade SayedHa.Roulette.Cli.csproj
7. Upgrade SayedHa.Roulette.Console.csproj
8. Upgrade SayedHa.Blackjack.Cli.csproj
9. Upgrade SayedHa.Blackjack.Tests.csproj
10. Run unit tests to validate upgrade in the projects listed below:
  SayedHa.Blackjack.Tests.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

| Project name                                   | Description                 |
|:-----------------------------------------------|:---------------------------:|

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                        | Current Version | New Version | Description                                   |
|:------------------------------------|:---------------:|:-----------:|:----------------------------------------------|
| Microsoft.AspNetCore.OpenApi         |   8.0.8         |  9.0.9      | Recommended for .NET 9.0                      |
| Microsoft.Extensions.DependencyInjection |   8.0.0     |  9.0.9      | Recommended for .NET 9.0                      |
| Microsoft.Extensions.Hosting        |   8.0.0         |  9.0.9      | Recommended for .NET 9.0                      |
| Newtonsoft.Json                     |   13.0.3        |  13.0.4     | Recommended for .NET 9.0                      |

### Project upgrade details

#### SayedHa.Blackjack.Shared.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net9.0`

NuGet packages changes:
  - Newtonsoft.Json should be updated from `13.0.3` to `13.0.4` (recommended for .NET 9.0)

#### SayedHa.Blackjack.Console.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net9.0`

#### SayedHa.Blackjack.Api.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net9.0`

NuGet packages changes:
  - Microsoft.AspNetCore.OpenApi should be updated from `8.0.8` to `9.0.9` (recommended for .NET 9.0)

#### SayedHa.Roulette.Cli.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net9.0`

NuGet packages changes:
  - Microsoft.Extensions.DependencyInjection should be updated from `8.0.0` to `9.0.9` (recommended for .NET 9.0)

#### SayedHa.Roulette.Console.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net9.0`

#### SayedHa.Blackjack.Cli.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net9.0`

NuGet packages changes:
  - Microsoft.Extensions.DependencyInjection should be updated from `8.0.0` to `9.0.9` (recommended for .NET 9.0)
  - Microsoft.Extensions.Hosting should be updated from `8.0.0` to `9.0.9` (recommended for .NET 9.0)

#### SayedHa.Blackjack.Tests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net9.0`

NuGet packages changes:
  - Newtonsoft.Json should be updated from `13.0.3` to `13.0.4` (recommended for .NET 9.0)
