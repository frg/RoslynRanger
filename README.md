# RoslynRanger

![GitHub](https://img.shields.io/github/license/frg/RoslynRanger)
![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/frg/RoslynRanger/ci.yml)
![GitHub issues](https://img.shields.io/github/issues/frg/RoslynRanger)
![Nuget](https://img.shields.io/nuget/dt/frg.RoslynRanger)
[![Maintainability](https://api.codeclimate.com/v1/badges/bbb3bf1f9a502cd12faa/maintainability)](https://codeclimate.com/github/frg/RoslynRanger/maintainability)

RoslynRanger is a collection of Roslyn analyzers designed to reduce pull request back and forth and help developers avoid common pitfalls in C# and .NET

## Analyzers

| Name                                                          | Diagnostic Id | Description                           |
|---------------------------------------------------------------|---------------|---------------------------------------|
| [Math.Round](./src/RoslynRanger/README.md#mathround-analyzer) | FR60001       | Recommends explicit rounding strategy |

## How to?

### Change severity level within .editorconfig

| Key Format                                   | Value                                                                                                                                    |
|----------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------|
| `dotnet_diagnostic.{diagnostic_id}.severity` | [Documentation](https://learn.microsoft.com/en-us/visualstudio/code-quality/use-roslyn-analyzers?view=vs-2022#configure-severity-levels) |

#### Examples

```editorconfig
# Changes the Math.Round severity to error, preventing compilation
dotnet_diagnostic.FR60001.severity = error
```

```editorconfig
# Disables the Math.Round analyzer
dotnet_diagnostic.FR60001.severity = none
```
