# RoslynRanger

RoslynRanger is a collection of Roslyn analyzers designed to reduce pull request back and forth and help developers avoid common pitfalls in C# and .NET

## Analyzers

| Name                                      | Diagnostic Id | Description                           |
|-------------------------------------------|---------------|---------------------------------------|
| [MathRound](./src/RoslynRanger/README.md) | FR60001       | Recommends explicit rounding strategy |

## How to?

### Change the severity within .editorconfig

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
