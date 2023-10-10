using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxFactory = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace RoslynRanger;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MathRoundFixProvider)), Shared]
public class MathRoundFixProvider : CodeFixProvider
{
    private const string RecommendedMidpointRounding = "MidpointRounding.AwayFromZero";

    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(MathRoundSemanticAnalyzer.DiagnosticId);

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null)
        {
            return;
        }

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // Find the invocation expression identified by the diagnostic.
        var syntaxNode = root.FindToken(diagnosticSpan.Start).Parent;
        if (syntaxNode == null)
        {
            return;
        }

        var invocation = syntaxNode.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First();

        context.RegisterCodeFix(
            CodeAction.Create(
                title: string.Format(Resources.FR60001CodeFixTitle, RecommendedMidpointRounding),
                createChangedDocument: c => AddMidpointRoundingArgument(context.Document, invocation, c),
                equivalenceKey: nameof(Resources.FR60001CodeFixTitle)),
            diagnostic);
    }

    private static async Task<Document> AddMidpointRoundingArgument(
        Document document,
        InvocationExpressionSyntax invocation,
        CancellationToken cancellationToken)
    {
        var oldArgumentList = invocation.ArgumentList;
        var newArgumentList = SyntaxFactory.ArgumentList(
            SyntaxFactory.SeparatedList(
                oldArgumentList.Arguments.Add(
                    SyntaxFactory.Argument(SyntaxFactory.ParseExpression(RecommendedMidpointRounding))
                )
            )
        );

        var newInvocationExpr = invocation.WithArgumentList(newArgumentList);
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        if (root == null)
        {
            return document;
        }

        var newRoot = root.ReplaceNode(invocation, newInvocationExpr);
        return document.WithSyntaxRoot(newRoot);
    }
}
