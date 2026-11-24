using System.Text;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Emit;

/// <summary>
///     Writes indented C#.
/// </summary>
internal sealed class SourceWriter
{
    public SourceWriter Line(string text = "")
    {
        if (text.Length == 0)
        {
            _builder.AppendLine();
            return this;
        }

        foreach (var line in text.Split('\n'))
        {
            _builder.Append(' ', _indent * 4).AppendLine(line.TrimEnd('\r'));
        }

        return this;
    }

    public SourceWriter Open(string? header = null)
    {
        if (header is not null) Line(header);
        Line("{");
        _indent++;
        return this;
    }

    public SourceWriter Close(string suffix = "")
    {
        _indent--;
        Line("}" + suffix);
        return this;
    }

    public SourceWriter Indent()
    {
        _indent++;
        return this;
    }

    public SourceWriter Outdent()
    {
        _indent--;
        return this;
    }

    public override string ToString() => _builder.ToString();

    private readonly StringBuilder _builder = new();
    private int _indent;
}
