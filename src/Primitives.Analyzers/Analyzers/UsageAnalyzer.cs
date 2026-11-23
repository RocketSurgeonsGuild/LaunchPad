using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Analyzers;

/// <summary>
///     Checks how code uses domain primitives, of this project or of the assemblies it references.
///     <list type="bullet">
///         <item>LPAD2007: <c>new T()</c>, <c>new()</c>, <c>default(T)</c> or <c>default</c> make a struct domain primitive without its rules.</item>
///         <item>LPAD2010: a string compared with <c>==</c> or <c>!=</c> to a domain primitive of a string.</item>
///         <item>LPAD2011: a domain primitive of a string given where a string is expected (a string variable, field or parameter).</item>
///     </list>
///     LPAD2010 and LPAD2011 are about a domain primitive whose value is a string, so that its <c>Value</c> is what the code meant: a number, a
///     Guid or a value object compared with a string is a different mistake, which its <c>Value</c> wouldn't fix. Nothing is checked in a
///     project that can't see <c>IDomainPrimitive&lt;TSelf&gt;</c>, which has no domain primitive.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UsageAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        AnalyzerDiagnostics.UninitializedStruct,
        AnalyzerDiagnostics.StringComparedWithPrimitive,
        AnalyzerDiagnostics.StringAssignedFromPrimitive,
    ];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(
            start =>
            {
                if (!PrimitiveSymbols.SeesDomainPrimitives(start.Compilation)) return;
                var isPrimitive = PrimitiveSymbols.CreateDomainPrimitiveCache();
                var isStringPrimitive = PrimitiveSymbols.CreateStringPrimitiveCache();
                start.RegisterSyntaxNodeAction(c => AnalyzeObjectCreation(c, isPrimitive), SyntaxKind.ObjectCreationExpression, SyntaxKind.ImplicitObjectCreationExpression);
                start.RegisterSyntaxNodeAction(c => AnalyzeDefault(c, isPrimitive), SyntaxKind.DefaultExpression, SyntaxKind.DefaultLiteralExpression);
                start.RegisterSyntaxNodeAction(c => AnalyzeComparison(c, isStringPrimitive), SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression);
                start.RegisterSyntaxNodeAction(c => AnalyzeAssignment(c, isStringPrimitive), SyntaxKind.SimpleAssignmentExpression);
                start.RegisterSyntaxNodeAction(c => AnalyzeVariable(c, isStringPrimitive), SyntaxKind.VariableDeclarator);
                start.RegisterSyntaxNodeAction(c => AnalyzeArgument(c, isStringPrimitive), SyntaxKind.Argument);
            }
        );
    }

    // ─── LPAD2007 ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    private static void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context, Func<ITypeSymbol?, bool> isPrimitive)
    {
        var creation = (BaseObjectCreationExpressionSyntax)context.Node;
        if (creation.ArgumentList is { Arguments.Count: > 0 }) return;
        var type = context.SemanticModel.GetTypeInfo(creation, context.CancellationToken).Type;
        ReportUninitialized(context, type, isPrimitive);
    }

    private static void AnalyzeDefault(SyntaxNodeAnalysisContext context, Func<ITypeSymbol?, bool> isPrimitive)
    {
        var info = context.SemanticModel.GetTypeInfo(context.Node, context.CancellationToken);
        // `default` takes the type it converts to; `default(T)` names it.
        ReportUninitialized(context, info.ConvertedType ?? info.Type, isPrimitive);
    }

    private static void ReportUninitialized(SyntaxNodeAnalysisContext context, ITypeSymbol? type, Func<ITypeSymbol?, bool> isPrimitive)
    {
        // A record class has no instance without a constructor: null is its missing value.
        if (type is not { TypeKind: TypeKind.Struct } || !isPrimitive(type)) return;
        context.ReportDiagnostic(Diagnostic.Create(AnalyzerDiagnostics.UninitializedStruct, context.Node.GetLocation(), type.Name));
    }

    // ─── LPAD2010 ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    private static void AnalyzeComparison(SyntaxNodeAnalysisContext context, Func<ITypeSymbol?, bool> isPrimitive)
    {
        var comparison = (BinaryExpressionSyntax)context.Node;
        var left = context.SemanticModel.GetTypeInfo(comparison.Left, context.CancellationToken).Type;
        var right = context.SemanticModel.GetTypeInfo(comparison.Right, context.CancellationToken).Type;
        var primitive = IsString(left) && isPrimitive(right) ? right
            : IsString(right) && isPrimitive(left) ? left
            : null;
        if (primitive is null) return;
        context.ReportDiagnostic(Diagnostic.Create(AnalyzerDiagnostics.StringComparedWithPrimitive, comparison.GetLocation(), primitive.Name));
    }

    // ─── LPAD2011 ────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    private static void AnalyzeAssignment(SyntaxNodeAnalysisContext context, Func<ITypeSymbol?, bool> isPrimitive)
    {
        var assignment = (AssignmentExpressionSyntax)context.Node;
        var target = context.SemanticModel.GetTypeInfo(assignment.Left, context.CancellationToken).Type;
        ReportStringFromPrimitive(context, assignment, target, assignment.Right, isPrimitive);
    }

    private static void AnalyzeVariable(SyntaxNodeAnalysisContext context, Func<ITypeSymbol?, bool> isPrimitive)
    {
        var variable = (VariableDeclaratorSyntax)context.Node;
        if (variable is not { Initializer.Value: { } value, Parent: VariableDeclarationSyntax { Type: { IsVar: false } declaredType } }) return;
        var target = context.SemanticModel.GetTypeInfo(declaredType, context.CancellationToken).Type;
        ReportStringFromPrimitive(context, variable, target, value, isPrimitive);
    }

    private static void AnalyzeArgument(SyntaxNodeAnalysisContext context, Func<ITypeSymbol?, bool> isPrimitive)
    {
        var argument = (ArgumentSyntax)context.Node;
        if (argument.Parent is not BaseArgumentListSyntax { Parent: { } invocation } list) return;
        // Most arguments aren't domain primitives: their type is cheaper to read than the call's overloads.
        var source = context.SemanticModel.GetTypeInfo(argument.Expression, context.CancellationToken).Type;
        if (!isPrimitive(source)) return;
        var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken);
        // With no conversion to string the call doesn't bind: its candidates still say what the parameter is.
        var method = symbolInfo.Symbol as IMethodSymbol
         ?? ( symbolInfo.CandidateSymbols.Length == 1 ? symbolInfo.CandidateSymbols[0] as IMethodSymbol : null );
        if (method is null) return;
        var parameter = Parameter(method, list, argument);
        if (parameter is null) return;
        if (!IsString(parameter.Type)) return;
        context.ReportDiagnostic(Diagnostic.Create(AnalyzerDiagnostics.StringAssignedFromPrimitive, argument.GetLocation(), source!.Name));
    }

    private static IParameterSymbol? Parameter(IMethodSymbol method, BaseArgumentListSyntax list, ArgumentSyntax argument)
    {
        if (argument.NameColon is { Name.Identifier.ValueText: var name }) return method.Parameters.FirstOrDefault(p => p.Name == name);
        var index = list.Arguments.IndexOf(argument);
        if (index < method.Parameters.Length)
        {
            var parameter = method.Parameters[index];
            // An argument of `params string[]` is one of the array's elements.
            return parameter.IsParams ? null : parameter;
        }

        return null;
    }

    private static void ReportStringFromPrimitive(
        SyntaxNodeAnalysisContext context,
        SyntaxNode node,
        ITypeSymbol? target,
        ExpressionSyntax value,
        Func<ITypeSymbol?, bool> isPrimitive
    )
    {
        if (!IsString(target)) return;
        var source = context.SemanticModel.GetTypeInfo(value, context.CancellationToken).Type;
        if (!isPrimitive(source)) return;
        context.ReportDiagnostic(Diagnostic.Create(AnalyzerDiagnostics.StringAssignedFromPrimitive, node.GetLocation(), source!.Name));
    }

    private static bool IsString(ITypeSymbol? type) => type?.SpecialType == SpecialType.System_String;
}
