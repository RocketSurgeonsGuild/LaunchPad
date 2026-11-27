using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.LaunchPad.Analyzers;

/// <summary>
///     Writes a tree the generators built with <see cref="SyntaxFactory" /> as laid-out text in one pass, in place of
///     <c>NormalizeWhitespace().ToFullString()</c>. Every layout rule of LaunchPad's generated code lives in this file.
/// </summary>
/// <remarks>
///     <para>
///         The rules are those of Roslyn's <c>SyntaxNormalizer</c> (MIT, the .NET Foundation), which <c>NormalizeWhitespace</c> runs, so the
///         text is byte for byte what it wrote. What changes is the cost: the normalizer is a rewriter that gives every token new trivia and
///         so rebuilds every token and node of the tree, only for the generator to ask for the text. This walks the tree once and appends the
///         whitespace each token needs, and the token, straight to a <see cref="StringBuilder" />.
///     </para>
///     <para>
///         The decisions below test token kinds with <c>is</c> patterns and <c>if</c> chains, not a <c>switch</c> over
///         <see cref="SyntaxKind" />: the repository's formatter fills such a switch with every member of the enum.
///     </para>
/// </remarks>
internal sealed class SyntaxLayout
{
    /// <summary>The line ending <c>NormalizeWhitespace</c> writes by default.</summary>
    public const string DefaultEndOfLine = "\r\n";

    private const string IndentUnit = "    ";
    private const string DocumentationCommentExterior = "///";

    /// <summary>The laid-out text of <paramref name="root" />, a node without a parent, as <c>NormalizeWhitespace(eol: endOfLine)</c> writes it.</summary>
    public static string ToText(SyntaxNode root, string endOfLine = DefaultEndOfLine) => ToText(root, endOfLine, null);

    /// <summary>
    ///     As <see cref="ToText(SyntaxNode, string)" />, writing <c>/// …</c> lines from <paramref name="documentation" /> in front of the
    ///     token each one is keyed by (a declaration's first token), indented as that token is.
    /// </summary>
    public static string ToText(SyntaxNode root, string endOfLine, IReadOnlyDictionary<SyntaxToken, string>? documentation)
    {
        var layout = new SyntaxLayout(root, endOfLine, documentation);
        layout.Visit(root, 0);
        return layout._text.ToString();
    }

    private readonly StringBuilder _text;
    private readonly string _endOfLine;
    private readonly IReadOnlyDictionary<SyntaxToken, string>? _documentation;
    private readonly Microsoft.CodeAnalysis.Text.TextSpan _consideredSpan;
    private readonly List<string> _indentations = [""];

    private bool _isInStructuredTrivia;
    private bool _afterLineBreak = true;
    private bool _afterIndentation;
    private bool _inSingleLineInterpolation;

    // What the trailing trivia of the last token written ended with, which a structured trivia ending in that token needs.
    private SyntaxToken _lastToken;
    private TriviaRun _lastTrailing;

    private SyntaxLayout(SyntaxNode root, string endOfLine, IReadOnlyDictionary<SyntaxToken, string>? documentation)
    {
        _endOfLine = endOfLine;
        _documentation = documentation;
        _consideredSpan = root.FullSpan;
        // Laid-out code runs about a third longer than its tokens alone.
        _text = new(root.FullSpan.Length + ( root.FullSpan.Length / 3 ) + 256);
    }

    /// <summary>What a rewritten trivia list held: how many pieces, the kind of the last, whether it ended a line and the last one's text.</summary>
    private struct TriviaRun
    {
        public int Count;
        public SyntaxKind LastKind;
        public bool EndsInLineBreak;
        public int LastLength;

        public void Add(SyntaxKind kind, bool endsInLineBreak, int length)
        {
            Count++;
            LastKind = kind;
            EndsInLineBreak = endsInLineBreak;
            LastLength = length;
        }
    }

    private void Visit(SyntaxNode node, int depth)
    {
        var interpolation = node is InterpolatedStringExpressionSyntax interpolated && interpolated.StringStartToken.IsKind(SyntaxKind.InterpolatedStringStartToken);
        var wasInSingleLineInterpolation = _inSingleLineInterpolation;
        if (interpolation) _inSingleLineInterpolation = true;

        foreach (var child in node.ChildNodesAndTokens())
        {
            if (child.AsNode() is { } childNode)
            {
                Visit(childNode, ChildDepth(childNode, depth));
            }
            else
            {
                VisitToken(child.AsToken(), depth);
            }
        }

        if (interpolation) _inSingleLineInterpolation = wasInSingleLineInterpolation;

        // An XML attribute in a documentation comment is followed by a space unless the element ends right after it.
        if (node is XmlTextAttributeSyntax attribute && _lastToken == attribute.EndQuoteToken && _lastTrailing.Count == 0)
        {
            var next = GetNextRelevantToken(attribute.EndQuoteToken);
            if (!next.IsKind(SyntaxKind.GreaterThanToken) && !next.IsKind(SyntaxKind.SlashGreaterThanToken)) _text.Append(' ');
        }
    }

    private void VisitToken(SyntaxToken token, int depth)
    {
        if (token.IsKind(SyntaxKind.None) || ( token.IsMissing && token.FullSpan.Length == 0 )) return;

        var leading = RewriteTrivia(
            token.LeadingTrivia,
            depth,
            isTrailing: false,
            indentAfterLineBreak: !token.IsKind(SyntaxKind.EndOfFileToken),
            mustHaveSeparator: false,
            lineBreaksAfter: LineBreaksAfterLeading(token)
        );
        if (_documentation is { } documentation && documentation.TryGetValue(token, out var xml)) WriteDocumentation(xml, leading);

        _text.Append(token.Text);

        var next = GetNextRelevantToken(token);
        _afterLineBreak = token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken);
        _afterIndentation = false;
        _lastTrailing = RewriteTrivia(
            token.TrailingTrivia,
            depth,
            isTrailing: true,
            indentAfterLineBreak: false,
            mustHaveSeparator: NeedsSeparator(token, next),
            lineBreaksAfter: LineBreaksAfter(token, next)
        );
        _lastToken = token;
    }

    // The documentation goes in front of the declaration's indentation, at the same indentation; its lines end with "\n" whatever the
    // line ending, as the generators always wrote them.
    private void WriteDocumentation(string xml, TriviaRun leading)
    {
        var indent = "";
        if (leading is { Count: > 0, LastKind: SyntaxKind.WhitespaceTrivia })
        {
            indent = _text.ToString(_text.Length - leading.LastLength, leading.LastLength);
            _text.Length -= leading.LastLength;
        }

        foreach (var line in xml.Split('\n'))
        {
            _text.Append(indent).Append("/// ").Append(line.TrimEnd()).Append('\n');
        }

        _text.Append(indent);
    }

    private SyntaxToken GetNextRelevantToken(SyntaxToken token)
    {
        // The next token with width, or the end of a directive, stepping into skipped tokens but not into directives or documentation.
        var next = token.GetNextToken(includeZeroWidth: true, includeSkipped: true);
        while (!next.IsKind(SyntaxKind.None) && next.Span.Length == 0 && !next.IsKind(SyntaxKind.EndOfDirectiveToken))
        {
            next = next.GetNextToken(includeZeroWidth: true, includeSkipped: true);
        }

        return _consideredSpan.Contains(next.FullSpan) ? next : default;
    }

    private string GetIndentation(int count)
    {
        for (var i = _indentations.Count; i <= count; i++) _indentations.Add(_indentations[i - 1] + IndentUnit);
        return _indentations[count];
    }

    private void AddEndOfLine(ref TriviaRun run)
    {
        _text.Append(_endOfLine);
        run.Add(SyntaxKind.EndOfLineTrivia, true, _endOfLine.Length);
        _afterLineBreak = true;
        _afterIndentation = false;
    }

    private void AddWhitespace(ref TriviaRun run, string whitespace)
    {
        _text.Append(whitespace);
        run.Add(SyntaxKind.WhitespaceTrivia, false, whitespace.Length);
    }

    private TriviaRun RewriteTrivia(SyntaxTriviaList triviaList, int depth, bool isTrailing, bool indentAfterLineBreak, bool mustHaveSeparator, int lineBreaksAfter)
    {
        var run = default(TriviaRun);
        foreach (var trivia in triviaList)
        {
            var kind = trivia.Kind();
            if (kind is SyntaxKind.WhitespaceTrivia or SyntaxKind.EndOfLineTrivia || trivia.FullSpan.Length == 0) continue;

            var needsSeparator = ( run.Count > 0 && NeedsSeparatorBetween(run.LastKind) ) || ( run.Count == 0 && isTrailing );
            var needsLineBreak = NeedsLineBreakBefore(kind, isTrailing)
             || ( run.Count > 0 && ( NeedsLineBreakAfter(run.LastKind, isTrailing) || NeedsLineBreakBefore(kind, isTrailing) ) );

            if (needsLineBreak && !_afterLineBreak) AddEndOfLine(ref run);

            if (_afterLineBreak)
            {
                if (!_afterIndentation && NeedsIndentAfterLineBreak(kind))
                {
                    AddWhitespace(ref run, GetIndentation(SyntaxFacts.IsPreprocessorDirective(kind) ? 0 : depth));
                    _afterIndentation = true;
                }
            }
            else if (needsSeparator)
            {
                AddWhitespace(ref run, " ");
                _afterLineBreak = false;
                _afterIndentation = false;
            }

            if (trivia.HasStructure)
            {
                var endsInLineBreak = VisitStructuredTrivia(trivia, depth);
                run.Add(kind, endsInLineBreak, 0);
            }
            else if (kind == SyntaxKind.DocumentationCommentExteriorTrivia)
            {
                // The exterior is written again without the whitespace in front of it.
                _text.Append(DocumentationCommentExterior);
                run.Add(kind, false, DocumentationCommentExterior.Length);
            }
            else
            {
                var text = trivia.ToFullString();
                _text.Append(text);
                run.Add(kind, EndsInLineBreak(kind, text), text.Length);
            }

            if (NeedsLineBreakAfter(kind, isTrailing) && !run.EndsInLineBreak) AddEndOfLine(ref run);
        }

        if (lineBreaksAfter > 0)
        {
            if (run.Count > 0 && run.EndsInLineBreak) lineBreaksAfter--;
            for (var i = 0; i < lineBreaksAfter; i++) AddEndOfLine(ref run);
        }
        else if (indentAfterLineBreak && _afterLineBreak && !_afterIndentation)
        {
            AddWhitespace(ref run, GetIndentation(depth));
            _afterIndentation = true;
        }
        else if (mustHaveSeparator)
        {
            AddWhitespace(ref run, " ");
            _afterLineBreak = false;
            _afterIndentation = false;
        }

        return run;
    }

    // Writes a directive or a documentation comment, and says whether it ended its line.
    private bool VisitStructuredTrivia(SyntaxTrivia trivia, int depth)
    {
        var structure = trivia.GetStructure()!;
        var wasInStructuredTrivia = _isInStructuredTrivia;
        _isInStructuredTrivia = true;
        Visit(structure, SyntaxFacts.IsPreprocessorDirective(trivia.Kind()) ? 0 : depth);
        _isInStructuredTrivia = wasInStructuredTrivia;

        var last = structure.GetLastToken(includeZeroWidth: true);
        if (last == _lastToken)
        {
            if (_lastTrailing.Count > 0) return _lastTrailing.EndsInLineBreak;
        }
        else if (last.TrailingTrivia is { Count: > 0 } trailing)
        {
            var lastTrivia = trailing[^1];
            return EndsInLineBreak(lastTrivia.Kind(), lastTrivia.ToFullString());
        }

        return !structure.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia) && structure.GetLastToken().IsKind(SyntaxKind.XmlTextLiteralNewLineToken);
    }

    private static bool EndsInLineBreak(SyntaxKind kind, string text) =>
        kind == SyntaxKind.EndOfLineTrivia
     || ( kind is SyntaxKind.PreprocessingMessageTrivia or SyntaxKind.DisabledTextTrivia && text.Length > 0 && SyntaxFacts.IsNewLine(text[^1]) );

    private static int LineBreaksAfterLeading(SyntaxToken token)
    {
        var leading = token.LeadingTrivia;
        return leading.Count >= 2
         && leading[^2].IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia)
         && leading[^1].IsKind(SyntaxKind.EndOfLineTrivia)
                ? 1
                : 0;
    }

    private static bool NeedsSeparatorBetween(SyntaxKind kind) =>
        kind is not ( SyntaxKind.None or SyntaxKind.WhitespaceTrivia or SyntaxKind.DocumentationCommentExteriorTrivia ) && !SyntaxFacts.IsPreprocessorDirective(kind);

    private static bool NeedsLineBreakBefore(SyntaxKind kind, bool isTrailing) =>
        kind == SyntaxKind.DocumentationCommentExteriorTrivia ? !isTrailing : SyntaxFacts.IsPreprocessorDirective(kind);

    private static bool NeedsLineBreakAfter(SyntaxKind kind, bool isTrailing) =>
        kind == SyntaxKind.SingleLineCommentTrivia
     || ( kind == SyntaxKind.MultiLineCommentTrivia ? !isTrailing : SyntaxFacts.IsPreprocessorDirective(kind) );

    private static bool NeedsIndentAfterLineBreak(SyntaxKind kind) =>
        kind is SyntaxKind.SingleLineCommentTrivia
             or SyntaxKind.MultiLineCommentTrivia
             or SyntaxKind.DocumentationCommentExteriorTrivia
             or SyntaxKind.SingleLineDocumentationCommentTrivia
             or SyntaxKind.MultiLineDocumentationCommentTrivia;

    private int LineBreaksAfter(SyntaxToken currentToken, SyntaxToken nextToken)
    {
        if (_inSingleLineInterpolation) return 0;
        if (currentToken.IsKind(SyntaxKind.EndOfDirectiveToken)) return 1;
        if (nextToken.IsKind(SyntaxKind.None)) return 0;
        if (_isInStructuredTrivia) return 0;

        if (nextToken.IsKind(SyntaxKind.CloseBraceToken))
        {
            if (IsAccessorListWithoutAccessorsWithBlockBody(currentToken.Parent?.Parent)) return 0;
            if (nextToken.Parent is InitializerExpressionSyntax or AnonymousObjectCreationExpressionSyntax && !IsSingleLineInitializerContext(nextToken.Parent)) return 1;
        }

        var current = currentToken.Kind();
        if (current == SyntaxKind.OpenBraceToken) return LineBreaksAfterOpenBrace(currentToken);
        if (current == SyntaxKind.FinallyKeyword) return 1;
        if (current == SyntaxKind.CloseBraceToken) return LineBreaksAfterCloseBrace(currentToken, nextToken);
        if (current == SyntaxKind.CloseParenToken)
        {
            // Not inside a positional pattern, nor before the brace of an initializer in a single-line context.
            return currentToken.Parent is PositionalPatternClauseSyntax
                ? 0
                : nextToken.IsKind(SyntaxKind.OpenBraceToken) && IsInitializerInSingleLineContext(nextToken.Parent)
                ? 0
                : ( currentToken.Parent is StatementSyntax && nextToken.Parent != currentToken.Parent )
             || nextToken.IsKind(SyntaxKind.OpenBraceToken)
             || nextToken.IsKind(SyntaxKind.WhereKeyword)
                ? 1
                : 0;
        }

        if (current == SyntaxKind.CloseBracketToken)
        {
            if (currentToken.Parent is AttributeListSyntax && currentToken.Parent.Parent is not ParameterSyntax) return 1;
        }
        else if (current == SyntaxKind.SemicolonToken)
        {
            return LineBreaksAfterSemicolon(currentToken, nextToken);
        }
        else if (current == SyntaxKind.CommaToken)
        {
            return currentToken.Parent is InitializerExpressionSyntax or AnonymousObjectCreationExpressionSyntax && !IsSingleLineInitializerContext(nextToken.Parent)
                ? 1
                : currentToken.Parent is EnumDeclarationSyntax or SwitchExpressionSyntax ? 1 : 0;
        }
        else if (current == SyntaxKind.ElseKeyword)
        {
            return nextToken.IsKind(SyntaxKind.IfKeyword) ? 0 : 1;
        }
        else if (current == SyntaxKind.ColonToken)
        {
            if (currentToken.Parent is LabeledStatementSyntax or SwitchLabelSyntax) return 1;
        }
        else if (current == SyntaxKind.SwitchKeyword && currentToken.Parent is SwitchExpressionSyntax)
        {
            return 1;
        }

        if (StartsQueryClause(nextToken)) return 1;

        var next = nextToken.Kind();
        return next == SyntaxKind.OpenBraceToken
            ? LineBreaksBeforeOpenBrace(nextToken)
            : next == SyntaxKind.CloseBraceToken
            ? LineBreaksBeforeCloseBrace(nextToken)
            : next is SyntaxKind.ElseKeyword or SyntaxKind.FinallyKeyword
            ? 1
            : next == SyntaxKind.OpenBracketToken
            ? nextToken.Parent is AttributeListSyntax && nextToken.Parent.Parent is not ParameterSyntax ? 1 : 0
            : next == SyntaxKind.WhereKeyword ? currentToken.Parent is TypeParameterListSyntax ? 1 : 0 : 0;
    }

    private static bool StartsQueryClause(SyntaxToken token) =>
        ( token.IsKind(SyntaxKind.FromKeyword) && token.Parent.IsKind(SyntaxKind.FromClause) )
     || ( token.IsKind(SyntaxKind.LetKeyword) && token.Parent.IsKind(SyntaxKind.LetClause) )
     || ( token.IsKind(SyntaxKind.WhereKeyword) && token.Parent.IsKind(SyntaxKind.WhereClause) )
     || ( token.IsKind(SyntaxKind.JoinKeyword) && token.Parent.IsKind(SyntaxKind.JoinClause) )
     || ( token.IsKind(SyntaxKind.JoinKeyword) && token.Parent.IsKind(SyntaxKind.JoinIntoClause) )
     || ( token.IsKind(SyntaxKind.OrderByKeyword) && token.Parent.IsKind(SyntaxKind.OrderByClause) )
     || ( token.IsKind(SyntaxKind.SelectKeyword) && token.Parent.IsKind(SyntaxKind.SelectClause) )
     || ( token.IsKind(SyntaxKind.GroupKeyword) && token.Parent.IsKind(SyntaxKind.GroupClause) );

    private static bool IsAccessorListWithoutAccessorsWithBlockBody(SyntaxNode? node)
    {
        if (node is not AccessorListSyntax accessorList) return false;
        foreach (var accessor in accessorList.Accessors)
        {
            if (accessor.Body is { }) return false;
        }

        return true;
    }

    private static bool IsAccessorListFollowedByInitializer(SyntaxNode? node) => node is AccessorListSyntax { Parent: PropertyDeclarationSyntax { Initializer: { } } };

    private static int LineBreaksBeforeOpenBrace(SyntaxToken openBraceToken)
    {
        var parent = openBraceToken.Parent;
        return parent.IsKind(SyntaxKind.Interpolation)
         || parent is PropertyPatternClauseSyntax
         || IsAccessorListWithoutAccessorsWithBlockBody(parent)
         || IsInitializerInSingleLineContext(parent)
                ? 0
                : 1;
    }

    private static int LineBreaksBeforeCloseBrace(SyntaxToken closeBraceToken)
    {
        var parent = closeBraceToken.Parent;
        return parent.IsKind(SyntaxKind.Interpolation) || parent is PropertyPatternClauseSyntax || IsInitializerInSingleLineContext(parent) ? 0 : 1;
    }

    private static int LineBreaksAfterOpenBrace(SyntaxToken openBraceToken)
    {
        var parent = openBraceToken.Parent;
        return parent is PropertyPatternClauseSyntax
         || parent.IsKind(SyntaxKind.Interpolation)
         || IsAccessorListWithoutAccessorsWithBlockBody(parent)
         || IsInitializerInSingleLineContext(parent)
                ? 0
                : 1;
    }

    private static int LineBreaksAfterCloseBrace(SyntaxToken currentToken, SyntaxToken nextToken)
    {
        var parent = currentToken.Parent;
        if (parent is SwitchExpressionSyntax or PropertyPatternClauseSyntax
         || parent.IsKind(SyntaxKind.Interpolation)
         || parent?.Parent is AnonymousFunctionExpressionSyntax
         || IsAccessorListFollowedByInitializer(parent)
         || nextToken.Kind() is SyntaxKind.CommaToken or SyntaxKind.SemicolonToken
         || nextToken.Parent is MemberAccessExpressionSyntax or BracketedArgumentListSyntax
         || IsInitializerInSingleLineContext(parent))
        {
            return 0;
        }

        // Single-line properties sit together.
        if (parent?.Parent is PropertyDeclarationSyntax property
         && IsSingleLineProperty(property)
         && nextToken.Parent is PropertyDeclarationSyntax nextProperty
         && IsSingleLineProperty(nextProperty))
        {
            return 1;
        }

        var next = nextToken.Kind();
        return next is SyntaxKind.EndOfFileToken or SyntaxKind.CloseBraceToken or SyntaxKind.CatchKeyword or SyntaxKind.FinallyKeyword or SyntaxKind.ElseKeyword
            ? 1
            : next == SyntaxKind.WhileKeyword && nextToken.Parent.IsKind(SyntaxKind.DoStatement) ? 1 : 2;
    }

    private static int LineBreaksAfterSemicolon(SyntaxToken currentToken, SyntaxToken nextToken)
    {
        var parent = currentToken.Parent;
        return parent.IsKind(SyntaxKind.ForStatement)
            ? 0
            : nextToken.IsKind(SyntaxKind.CloseBraceToken)
            ? 1
            : parent.IsKind(SyntaxKind.UsingDirective)
            ? nextToken.Parent.IsKind(SyntaxKind.UsingDirective) ? 1 : 2
            : parent.IsKind(SyntaxKind.ExternAliasDirective)
            ? nextToken.Parent.IsKind(SyntaxKind.ExternAliasDirective) ? 1 : 2
            : parent is AccessorDeclarationSyntax && IsAccessorListWithoutAccessorsWithBlockBody(parent.Parent)
            ? 0
            : parent is PropertyDeclarationSyntax property
            ? IsSingleLineProperty(property) && nextToken.Parent is PropertyDeclarationSyntax nextProperty && IsSingleLineProperty(nextProperty) ? 1 : 2
            : 1;
    }

    private static bool NeedsSeparatorForPropertyPattern(SyntaxToken token, SyntaxToken next)
    {
        var propertyPattern = token.Parent as PropertyPatternClauseSyntax ?? next.Parent as PropertyPatternClauseSyntax;
        return propertyPattern is not null && ( token.IsKind(SyntaxKind.OpenBraceToken) || next.IsKind(SyntaxKind.CloseBraceToken) || propertyPattern.Parent is RecursivePatternSyntax recursive && ( next.IsKind(SyntaxKind.OpenBraceToken)
            ? recursive.Type is { } || recursive.PositionalPatternClause is { }
            : token.IsKind(SyntaxKind.CloseBraceToken) && recursive.Designation is { } ) );
    }

    private static bool NeedsSeparatorForPositionalPattern(SyntaxToken token, SyntaxToken next)
    {
        var positionalPattern = token.Parent as PositionalPatternClauseSyntax ?? next.Parent as PositionalPatternClauseSyntax;
        return positionalPattern is not null && !token.IsKind(SyntaxKind.OpenParenToken) && !next.IsKind(SyntaxKind.CloseParenToken) && positionalPattern.Parent is RecursivePatternSyntax recursive && ( next.IsKind(SyntaxKind.OpenParenToken)
            ? recursive.Type is { }
            : token.IsKind(SyntaxKind.CloseParenToken) && recursive.PropertyPatternClause is null && recursive.Designation is { } );
    }

    private static bool NeedsSeparatorForListPattern(SyntaxToken token, SyntaxToken next)
    {
        var listPattern = token.Parent as ListPatternSyntax ?? next.Parent as ListPatternSyntax;
        return listPattern is not null && ( next.IsKind(SyntaxKind.OpenBracketToken) || token.IsKind(SyntaxKind.OpenBracketToken) && listPattern.Designation is { } );
    }

    private static bool NeedsSeparator(SyntaxToken token, SyntaxToken next)
    {
        if (token.Parent is null || next.Parent is null) return false;

        // Accessors written inline take a space before each, but none before a semicolon: { get; set; }.
        if (IsAccessorListWithoutAccessorsWithBlockBody(next.Parent) || IsAccessorListWithoutAccessorsWithBlockBody(next.Parent.Parent)) return !next.IsKind(SyntaxKind.SemicolonToken);

        var tokenKind = token.Kind();
        var nextKind = next.Kind();
        if (tokenKind is SyntaxKind.XmlTextLiteralNewLineToken or SyntaxKind.XmlTextLiteralToken || nextKind is SyntaxKind.XmlTextLiteralNewLineToken or SyntaxKind.XmlTextLiteralToken) return false;

        // In a directive there is often nothing between the keyword and the end of the directive.
        if (nextKind == SyntaxKind.EndOfDirectiveToken) return IsKeyword(tokenKind) && next.Span.Start > next.FullSpan.Start;

        if (( token.Parent is AssignmentExpressionSyntax && AssignmentTokenNeedsSeparator(tokenKind) )
         || ( next.Parent is AssignmentExpressionSyntax && AssignmentTokenNeedsSeparator(nextKind) )
         || ( token.Parent is BinaryExpressionSyntax && BinaryTokenNeedsSeparator(tokenKind) )
         || ( next.Parent is BinaryExpressionSyntax && BinaryTokenNeedsSeparator(nextKind) ))
        {
            return true;
        }

        if (tokenKind == SyntaxKind.GreaterThanToken && token.Parent.IsKind(SyntaxKind.TypeArgumentList) && !SyntaxFacts.IsPunctuation(nextKind)) return true;
        if (tokenKind == SyntaxKind.GreaterThanToken && token.Parent.IsKind(SyntaxKind.FunctionPointerParameterList) && token.Parent.Parent?.Parent is not UsingDirectiveSyntax) return true;
        if (tokenKind == SyntaxKind.CommaToken && nextKind != SyntaxKind.CommaToken && !token.Parent.IsKind(SyntaxKind.EnumDeclaration)) return true;
        if (tokenKind == SyntaxKind.SemicolonToken && nextKind is not ( SyntaxKind.SemicolonToken or SyntaxKind.CloseParenToken )) return true;
        if (nextKind == SyntaxKind.SwitchKeyword && next.Parent is SwitchExpressionSyntax) return true;

        if (tokenKind == SyntaxKind.QuestionToken
         && ( token.Parent.IsKind(SyntaxKind.ConditionalExpression) || token.Parent is TypeSyntax )
         && token.Parent.Parent?.Kind() is not ( SyntaxKind.TypeArgumentList or SyntaxKind.UsingDirective ))
        {
            return true;
        }

        if (tokenKind == SyntaxKind.ColonToken)
        {
            return !token.Parent.IsKind(SyntaxKind.InterpolationFormatClause) && !token.Parent.IsKind(SyntaxKind.XmlPrefix) && !token.Parent.IsKind(SyntaxKind.IgnoredDirectiveTrivia);
        }

        if (nextKind == SyntaxKind.ColonToken && ( next.Parent.IsKind(SyntaxKind.BaseList) || next.Parent.IsKind(SyntaxKind.TypeParameterConstraintClause) || next.Parent is ConstructorInitializerSyntax )) return true;
        if (tokenKind == SyntaxKind.CloseBracketToken && IsWord(nextKind)) return true;
        // A space after a tuple type, not after a cast.
        if (tokenKind == SyntaxKind.CloseParenToken && IsWord(nextKind) && token.Parent.IsKind(SyntaxKind.TupleType)) return true;
        if (nextKind is SyntaxKind.QuestionToken or SyntaxKind.ColonToken && next.Parent.IsKind(SyntaxKind.ConditionalExpression)) return true;
        if (tokenKind == SyntaxKind.EqualsToken) return !token.Parent.IsKind(SyntaxKind.XmlTextAttribute);
        if (nextKind == SyntaxKind.EqualsToken) return !next.Parent.IsKind(SyntaxKind.XmlTextAttribute);

        if (FunctionPointerSeparator(token, next) is { } functionPointer) return functionPointer;

        if (tokenKind == SyntaxKind.EqualsGreaterThanToken || nextKind == SyntaxKind.EqualsGreaterThanToken) return true;
        if (IsLiteral(tokenKind) && IsLiteral(nextKind)) return true;
        if (nextKind == SyntaxKind.AsteriskToken && next.Parent is PointerTypeSyntax) return false;
        if (tokenKind == SyntaxKind.AsteriskToken && token.Parent is PointerTypeSyntax && ( nextKind == SyntaxKind.IdentifierToken || next.Parent.IsKind(SyntaxKind.IndexerDeclaration) )) return true;

        // An initializer in a single-line context (an argument, an attribute argument, an interpolation hole) is written on one line: { A = 1 }.
        if (IsSingleLineInitializerContext(token.Parent))
        {
            if (next.Parent is InitializerExpressionSyntax or AnonymousObjectCreationExpressionSyntax && nextKind is SyntaxKind.OpenBraceToken or SyntaxKind.CloseBraceToken) return true;
            if (token.Parent is InitializerExpressionSyntax or AnonymousObjectCreationExpressionSyntax && tokenKind == SyntaxKind.OpenBraceToken) return true;
        }

        // A lambda's return type and its parameters.
        if (nextKind == SyntaxKind.OpenParenToken && next.Parent?.Parent is ParenthesizedLambdaExpressionSyntax lambda && lambda.ReturnType?.GetLastToken() == token) return true;

        if (token.Parent.Parent.IsKind(SyntaxKind.SuppressNullableWarningExpression)) return false;

        if (IsKeyword(tokenKind)
         && tokenKind != SyntaxKind.ExtensionKeyword
         && nextKind is not ( SyntaxKind.ColonToken
                          or SyntaxKind.DotToken
                          or SyntaxKind.QuestionToken
                          or SyntaxKind.SemicolonToken
                          or SyntaxKind.OpenBracketToken
                          or SyntaxKind.CloseParenToken
                          or SyntaxKind.CloseBraceToken
                          or SyntaxKind.ColonColonToken
                          or SyntaxKind.GreaterThanToken
                          or SyntaxKind.CommaToken )
         && ( nextKind != SyntaxKind.OpenParenToken || KeywordNeedsSeparatorBeforeOpenParen(tokenKind) || next.Parent.IsKind(SyntaxKind.TupleType) ))
        {
            return true;
        }

        if (IsWordOrLiteral(tokenKind) && IsWordOrLiteral(nextKind)) return true;
        if (token.Span.Length > 1 && next.Span.Length > 1)
        {
            var last = token.Text[^1];
            if (last == next.Text[0] && last is '+' or '-' or '<' or ':' or '?' or '=' or '"') return true;
        }

        return token.Parent is RelationalPatternSyntax || nextKind is SyntaxKind.AndKeyword or SyntaxKind.OrKeyword || tokenKind is SyntaxKind.AndKeyword or SyntaxKind.OrKeyword or SyntaxKind.NotKeyword || NeedsSeparatorForPropertyPattern(token, next) || NeedsSeparatorForPositionalPattern(token, next) || NeedsSeparatorForListPattern(token, next) || ( token.Parent.IsKind(SyntaxKind.LineSpanDirectiveTrivia) && next.Parent.IsKind(SyntaxKind.LineDirectivePosition) )
         || ( token.Parent.IsKind(SyntaxKind.LineDirectivePosition) && next.Parent.IsKind(SyntaxKind.LineSpanDirectiveTrivia) );
    }

    // The spacing of a function pointer type (delegate* unmanaged[Cdecl]<int, void>), or null when neither token is part of one.
    private static bool? FunctionPointerSeparator(SyntaxToken token, SyntaxToken next)
    {
        var tokenKind = token.Kind();
        var nextKind = next.Kind();
        if (token.Parent.IsKind(SyntaxKind.FunctionPointerType))
        {
            if (nextKind == SyntaxKind.AsteriskToken && tokenKind == SyntaxKind.DelegateKeyword) return false;
            if (tokenKind == SyntaxKind.AsteriskToken
             && next.Parent.IsKind(SyntaxKind.FunctionPointerCallingConvention)
             && nextKind is SyntaxKind.IdentifierToken or SyntaxKind.ManagedKeyword or SyntaxKind.UnmanagedKeyword)
            {
                return true;
            }
        }

        if (next.Parent.IsKind(SyntaxKind.FunctionPointerParameterList)
         && nextKind == SyntaxKind.LessThanToken
         && ( tokenKind is SyntaxKind.AsteriskToken or SyntaxKind.ManagedKeyword or SyntaxKind.UnmanagedKeyword
             || ( tokenKind == SyntaxKind.CloseBracketToken && token.Parent.IsKind(SyntaxKind.FunctionPointerUnmanagedCallingConventionList) ) ))
        {
            return false;
        }

        if (token.Parent.IsKind(SyntaxKind.FunctionPointerCallingConvention)
         && next.Parent.IsKind(SyntaxKind.FunctionPointerUnmanagedCallingConventionList)
         && nextKind == SyntaxKind.OpenBracketToken)
        {
            return false;
        }

        if (next.Parent.IsKind(SyntaxKind.FunctionPointerUnmanagedCallingConventionList) && token.Parent.IsKind(SyntaxKind.FunctionPointerUnmanagedCallingConventionList))
        {
            if (nextKind == SyntaxKind.IdentifierToken)
            {
                if (tokenKind == SyntaxKind.OpenBracketToken) return false;
                if (tokenKind == SyntaxKind.CommaToken) return true;
            }

            if (nextKind is SyntaxKind.CommaToken or SyntaxKind.CloseBracketToken) return false;
        }

        return tokenKind == SyntaxKind.LessThanToken && token.Parent.IsKind(SyntaxKind.FunctionPointerParameterList)
            ? false
            : nextKind == SyntaxKind.GreaterThanToken && next.Parent.IsKind(SyntaxKind.FunctionPointerParameterList) ? false : null;
    }

    private static bool KeywordNeedsSeparatorBeforeOpenParen(SyntaxKind kind) =>
        kind is not ( SyntaxKind.TypeOfKeyword
                  or SyntaxKind.DefaultKeyword
                  or SyntaxKind.NewKeyword
                  or SyntaxKind.BaseKeyword
                  or SyntaxKind.ThisKeyword
                  or SyntaxKind.CheckedKeyword
                  or SyntaxKind.UncheckedKeyword
                  or SyntaxKind.SizeOfKeyword
                  or SyntaxKind.ArgListKeyword );

    private static bool BinaryTokenNeedsSeparator(SyntaxKind kind) =>
        kind is not ( SyntaxKind.DotToken or SyntaxKind.MinusGreaterThanToken ) && SyntaxFacts.GetBinaryExpression(kind) != SyntaxKind.None;

    private static bool AssignmentTokenNeedsSeparator(SyntaxKind kind) => SyntaxFacts.GetAssignmentExpression(kind) != SyntaxKind.None;

    // Roslyn's internal SyntaxFacts.IsLiteral, identifiers included.
    private static bool IsLiteral(SyntaxKind kind) =>
        kind is SyntaxKind.IdentifierToken
             or SyntaxKind.StringLiteralToken
             or SyntaxKind.Utf8StringLiteralToken
             or SyntaxKind.SingleLineRawStringLiteralToken
             or SyntaxKind.Utf8SingleLineRawStringLiteralToken
             or SyntaxKind.MultiLineRawStringLiteralToken
             or SyntaxKind.Utf8MultiLineRawStringLiteralToken
             or SyntaxKind.CharacterLiteralToken
             or SyntaxKind.NumericLiteralToken
             or SyntaxKind.XmlTextLiteralToken
             or SyntaxKind.XmlTextLiteralNewLineToken
             or SyntaxKind.XmlEntityLiteralToken;

    private static bool IsWord(SyntaxKind kind) => kind == SyntaxKind.IdentifierToken || IsKeyword(kind);

    private static bool IsWordOrLiteral(SyntaxKind kind) =>
        IsLiteral(kind) || IsKeyword(kind) || kind is SyntaxKind.InterpolatedStringEndToken or SyntaxKind.InterpolatedRawStringEndToken;

    private static bool IsKeyword(SyntaxKind kind) => SyntaxFacts.IsKeywordKind(kind) || SyntaxFacts.IsPreprocessorKeyword(kind);

    // The indentation depth of a node from its parent's: one more for a statement, a member, an accessor, a switch section or arm, a using
    // directive, a query, and the entries of a multi-line initializer.
    private static int ChildDepth(SyntaxNode node, int parentDepth)
    {
        var parent = node.Parent!;
        if (parent.IsKind(SyntaxKind.CompilationUnit)) return 0;
        if (parent.Kind() is SyntaxKind.GlobalStatement or SyntaxKind.FileScopedNamespaceDeclaration) return parentDepth;
        if (node.IsKind(SyntaxKind.IfStatement) && parent.IsKind(SyntaxKind.ElseClause)) return parentDepth;
        if (parent is BlockSyntax) return parentDepth + 1;
        if (( parent is InitializerExpressionSyntax or AnonymousObjectMemberDeclaratorSyntax || ( node is AssignmentExpressionSyntax && parent is InitializerExpressionSyntax ) )
         && !IsSingleLineInitializerContext(parent))
        {
            return parentDepth + 1;
        }

        if (node is StatementSyntax and not BlockSyntax)
        {
            // using (…) using (…) and fixed (…) fixed (…) chains stay at one depth.
            return node is UsingStatementSyntax { Parent: UsingStatementSyntax } or FixedStatementSyntax { Parent: FixedStatementSyntax } ? parentDepth : parentDepth + 1;
        }

        return node is MemberDeclarationSyntax
                    or AccessorDeclarationSyntax
                    or TypeParameterConstraintClauseSyntax
                    or SwitchSectionSyntax
                    or SwitchExpressionArmSyntax
                    or UsingDirectiveSyntax
                    or ExternAliasDirectiveSyntax
                    or QueryExpressionSyntax
                    or QueryContinuationSyntax
            ? parentDepth + 1
            : parentDepth;
    }

    // Initializers in an interpolation hole, an attribute argument or an argument are written on one line.
    private static bool IsSingleLineInitializerContext(SyntaxNode? node)
    {
        for (var parent = node?.Parent; parent is { }; parent = parent.Parent)
        {
            if (parent is InterpolationSyntax or AttributeArgumentSyntax or ArgumentSyntax) return true;
            if (parent is StatementSyntax or MemberDeclarationSyntax) return false;
        }

        return false;
    }

    private static bool IsInitializerInSingleLineContext(SyntaxNode? node) =>
        node is InitializerExpressionSyntax or AnonymousObjectCreationExpressionSyntax && IsSingleLineInitializerContext(node);

    // Expression-bodied properties and auto-properties are written on one line.
    private static bool IsSingleLineProperty(PropertyDeclarationSyntax property) =>
        property.AccessorList is null || IsAccessorListWithoutAccessorsWithBlockBody(property.AccessorList);
}
