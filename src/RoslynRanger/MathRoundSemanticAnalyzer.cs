using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RoslynRanger;

/// <summary>
/// An analyzer that reports when Math.Round is used without specifying the MidpointRounding convention.
/// The default convention can cause rounding issues if the convention is not explicitly intended to be used.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MathRoundSemanticAnalyzer : DiagnosticAnalyzer
{
    private const string TypeName = "System.Math";
    private const string MethodName = "Round";
    private const string ExpectedArgumentTypeName = "System.MidpointRounding";

    public const string DiagnosticId = "FR60001";

    private static readonly LocalizableString _title = new LocalizableResourceString(
        nameof(Resources.FR60001Title),
        Resources.ResourceManager,
        typeof(Resources));

    private static readonly LocalizableString _message =
        new LocalizableResourceString(
            nameof(Resources.FR60001Message),
            Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString _description =
        new LocalizableResourceString(
            nameof(Resources.FR60001Description),
            Resources.ResourceManager,
            typeof(Resources));

    // The category of the diagnostic (Design, Naming etc.).
    private const string Category = "Usage";

    private static readonly DiagnosticDescriptor _rule = new(
        DiagnosticId,
        _title,
        _message,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: _description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(_rule);

    public override void Initialize(AnalysisContext context)
    {
        // Avoid analyzing generated code.
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    /// <summary>
    /// Analyzes invocation expressions to determine if they are calls to <see cref="System.Math.Round(double)"/>
    /// or its overloads without specifying a <see cref="System.MidpointRounding"/> argument.
    /// </summary>
    /// <param name="context">Provides information about the syntax node being analyzed.</param>
    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        var semanticModel = context.SemanticModel;

        var methodSymbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        if (methodSymbol is null)
        {
            return;
        }

        if (methodSymbol.MethodKind is not MethodKind.Ordinary ||
            methodSymbol.ContainingType?.ToString().Equals(TypeName, StringComparison.Ordinal) is false ||
            methodSymbol.Name.Equals(MethodName, StringComparison.Ordinal) is false)
        {
            return;
        }

        var hasMidpointRoundingArg = false;
        foreach (var param in methodSymbol.Parameters)
        {
            if (param.Type.ToString().Equals(ExpectedArgumentTypeName, StringComparison.Ordinal))
            {
                hasMidpointRoundingArg = true;
                break;
            }
        }

        if (hasMidpointRoundingArg)
        {
            return;
        }

        var diagnostic = Diagnostic.Create(
            _rule,
            invocation.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }
}
