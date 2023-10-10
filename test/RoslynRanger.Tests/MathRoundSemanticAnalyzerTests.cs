using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        RoslynRanger.MathRoundSemanticAnalyzer>;

namespace RoslynRanger.Tests;

public class MathRoundSemanticAnalyzerTests
{
    [Fact]
    public async Task MathRound_WithoutExplicitRoundingStrategy_AlertDiagnostic()
    {
        const string text = @"
using System;

public class Example
{
    public static void MathRoundMidpointExample()
    {
        Math.Round(1.2345); // Should trigger analyzer
        Math.Round(1.2345, 4); // Should trigger analyzer
        Math.Round(1.2345, MidpointRounding.AwayFromZero);
        Math.Round(1.2345, MidpointRounding.ToEven);
        Math.Round(1.2345, MidpointRounding.ToZero);
        Math.Round(1.2345, MidpointRounding.ToNegativeInfinity);
        Math.Round(1.2345, MidpointRounding.ToPositiveInfinity);
    }
}
";

        var expected1 = Verifier
                       .Diagnostic()
                       .WithSeverity(DiagnosticSeverity.Warning)
                       .WithLocation(8, 9);

        var expected2 = Verifier
                       .Diagnostic()
                       .WithSeverity(DiagnosticSeverity.Warning)
                       .WithLocation(9, 9);

        await Verifier.VerifyAnalyzerAsync(text, expected1, expected2);
    }
}
