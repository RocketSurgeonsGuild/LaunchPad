using Microsoft.CodeAnalysis.CSharp;

namespace Rocket.Surgery.LaunchPad.Analyzers.Api.Code;

/// <summary>
///     Reads the type names the models hold (<c>global::A.B&lt;global::C&gt;?</c>, <c>int[]</c>, <c>(int A, string B)</c>) into names, in the
///     shape Roslyn's <c>ParseTypeName</c> gives them: qualified names nest to the left, so the shortening sees the same names it did.
/// </summary>
internal ref struct TypeNameReader(string text)
{
    private readonly string _text = text;
    private int _position;

    public static CodeType Type(string text)
    {
        var reader = new TypeNameReader(text);
        var type = reader.ReadType();
        reader.SkipWhiteSpace();
        return reader._position == text.Length ? type : throw new ArgumentException($"'{text}' is not a type name.", nameof(text));
    }

    public static CodeName Name(string text)
    {
        var reader = new TypeNameReader(text);
        var name = reader.ReadName();
        reader.SkipWhiteSpace();
        return reader._position == text.Length ? name : throw new ArgumentException($"'{text}' is not a name.", nameof(text));
    }

    private CodeType ReadType()
    {
        SkipWhiteSpace();
        var type = Peek() == '(' ? ReadTuple() : ReadNamedType();
        while (true)
        {
            SkipWhiteSpace();
            if (Peek() == '?')
            {
                _position++;
                type = new CodeNullableType(type);
            }
            else if (Peek() == '[')
            {
                var ranks = new List<int>();
                while (Peek() == '[')
                {
                    _position++;
                    var commas = 0;
                    while (Peek() is var c && c != ']')
                    {
                        if (c == ',') commas++;
                        _position++;
                    }

                    _position++;
                    ranks.Add(commas);
                    SkipWhiteSpace();
                }

                type = new CodeArrayType(type, ranks);
            }
            else
            {
                return type;
            }
        }
    }

    private CodeType ReadTuple()
    {
        _position++;
        var elements = new List<(CodeType, string?)>();
        while (true)
        {
            var type = ReadType();
            SkipWhiteSpace();
            string? name = null;
            if (IsIdentifierCharacter(Peek())) name = ReadIdentifier();
            elements.Add((type, name));
            SkipWhiteSpace();
            if (Peek() == ',')
            {
                _position++;
                continue;
            }

            _position++; // )
            return new CodeTupleType(elements);
        }
    }

    private CodeType ReadNamedType()
    {
        var start = _position;
        var identifier = ReadIdentifier();
        SkipWhiteSpace();
        if (Peek() is not ( '.' or ':' or '<' ) && SyntaxFacts.IsPredefinedType(SyntaxFacts.GetKeywordKind(identifier))) return new CodePredefinedType(identifier);
        _position = start;
        return ReadName();
    }

    private CodeName ReadName()
    {
        SkipWhiteSpace();
        CodeName name;
        var first = ReadIdentifier();
        if (Peek() == ':' && _position + 1 < _text.Length && _text[_position + 1] == ':')
        {
            _position += 2;
            name = new CodeAliasQualifiedName(first, new CodeIdentifier(ReadIdentifier()));
        }
        else
        {
            name = ReadTypeArguments(first);
        }

        while (true)
        {
            SkipWhiteSpace();
            if (Peek() != '.') return name;
            _position++;
            SkipWhiteSpace();
            name = new CodeQualifiedName(name, ReadTypeArguments(ReadIdentifier()));
        }
    }

    // The simple name just read, generic when type arguments follow.
    private CodeName ReadTypeArguments(string identifier)
    {
        SkipWhiteSpace();
        if (Peek() != '<') return new CodeIdentifier(identifier);
        _position++;
        var arguments = new List<CodeType>();
        while (true)
        {
            arguments.Add(ReadType());
            SkipWhiteSpace();
            var next = Peek();
            _position++;
            if (next == '>') return new CodeGenericName(identifier, arguments);
        }
    }

    private string ReadIdentifier()
    {
        SkipWhiteSpace();
        var start = _position;
        while (IsIdentifierCharacter(Peek())) _position++;
        return start == _position ? throw new ArgumentException($"'{_text}' is not a type name.") : _text[start.._position];
    }

    private static bool IsIdentifierCharacter(char c) => char.IsLetterOrDigit(c) || c is '_' or '@';

    private readonly char Peek() => _position < _text.Length ? _text[_position] : '\0';

    private void SkipWhiteSpace()
    {
        while (_position < _text.Length && char.IsWhiteSpace(_text[_position])) _position++;
    }
}
