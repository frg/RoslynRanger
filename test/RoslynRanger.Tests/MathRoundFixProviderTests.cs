using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<RoslynRanger.MathRoundSemanticAnalyzer,
        RoslynRanger.MathRoundFixProvider>;

namespace RoslynRanger.Tests;

public class MathRoundFixProviderTests
{
    [Fact]
    public async Task MathRound_WithoutExplicitRoundingStrategy_CodeFixWithMidpointRounding()
    {
        const string text = @"
using System;

public class Example
{
    public static void MathRoundMidpointExample()
    {
        Math.Round(1.2345); // Should trigger analyzer
        Math.Round(1.2345, 2); // Should trigger analyzer
    }
}
";

        const string newText = @"
using System;

public class Example
{
    public static void MathRoundMidpointExample()
    {
        Math.Round(1.2345, MidpointRounding.AwayFromZero); // Should trigger analyzer
        Math.Round(1.2345, 2, MidpointRounding.AwayFromZero); // Should trigger analyzer
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

        await Verifier
              .VerifyCodeFixAsync(text, new[] { expected1, expected2 }, newText)
              .ConfigureAwait(false);
    }
}
