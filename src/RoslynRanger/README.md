# Roslyn Ranger

RoslynRanger is a collection of Roslyn analyzers designed to reduce pull request back and forth and help developers avoid common pitfalls in C# and .NET

## Math.Round Analyzer

### Description

The `Math.Round` analyzer warns you when you use `Math.Round` without specifying a `MidpointRounding` argument. The absence of this argument could lead to rounding behavior that might not be intended.

### Pitfall

By default `Math.Round` uses rounding strategy `MidpointRounding.ToEven`, this is also called Banker's Rounding.

| Number | MidpointRounding.ToEven | MidpointRounding.AwayFromZero |
|--------|------------------------|-------------------------------|
| 1.5    | 2.0                    | 2.0                           |
| 2.5    | 2.0                    | 3.0                           |
| -1.5   | -2.0                   | -2.0                          |
| -2.5   | -2.0                   | -3.0                          |

This can cause major issues within any system, especially systems that deal with monetary values.

### Example

When `Math.Round` is used without explicitly specifying the rounding strategy the analyzer will report a diagnostic.

```csharp
var result1 = Math.Round(3.14159);
var result2 = Math.Round(3.14159, 2);
```

The following examples will not report a diagnostic.

```csharp
var result1 = Math.Round(3.14159, MidpointRounding.AwayFromZero);
var result2 = Math.Round(3.14159, MidpointRounding.ToEven);
var result3 = Math.Round(3.14159, MidpointRounding.ToZero);
var result4 = Math.Round(3.14159, 2, MidpointRounding.ToNegativeInfinity);
var result5 = Math.Round(3.14159, 2, MidpointRounding.ToPositiveInfinity);
```

### Code Fix

This analyzer provides an automatic code fix with defaults to `MidpointRounding.AwayFromZero` as the rounding strategy is a commonly expected.

From
```csharp
var result1 = Math.Round(3.14159);
var result2 = Math.Round(3.14159, 2);
```

To
```csharp
var result1 = Math.Round(3.14159, MidpointRounding.AwayFromZero);
var result2 = Math.Round(3.14159, 2, MidpointRounding.AwayFromZero);
```
