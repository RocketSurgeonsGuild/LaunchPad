using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Model;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Emit;

/// <summary>
///     Writes a smart enum: a fixed set of named values, the type's <c>public static readonly</c> fields, each created with <c>new(value)</c>.
/// </summary>
internal static class SmartEnumEmitter
{
    public static void Emit(SourceWriter writer, PrimitiveModel model)
    {
        var t = model.Name;
        var v = model.Value!;
        var vt = v.Display;
        var names = model.EnumMembers;
        // The members as C# code: a member may be named with a keyword (@class).
        var members = names.Select(Names.Identifier).ToArray();
        var nullable = model.IsStruct ? "" : "?";

        PrimitiveEmitter.TypeAttributes(writer, model);
        if (model.IsUnion) writer.Line("[global::System.Runtime.CompilerServices.Union]");
        writer.Line($"{PrimitiveEmitter.Declaration(model)} :");
        writer.Line($"    {Names.Primitives}.IDomainPrimitive<{t}, {vt}>,");
        if (model.IsUnion) writer.Line($"    {t}.{UnionMembers},");
        writer.Line($"    global::System.IParsable<{t}>,");
        writer.Line("    global::System.IFormattable");
        writer.Open();
        writer.Line($"private readonly {vt} _value;");
        if (model.IsStruct) writer.Line("private readonly bool _initialized;");
        writer.Line();
        writer.Line($"private {t}({vt} value)");
        writer.Open();
        writer.Line("_value = value;");
        if (model.IsStruct) writer.Line("_initialized = true;");
        writer.Close();
        writer.Line();
        if (model.IsStruct)
        {
            writer.Line("/// <summary>Whether this is the default value, which is none of the members.</summary>");
            writer.Line("public bool IsDefault => !_initialized;");
            writer.Line();
            writer.Line("/// <summary>The value. Reading it from the default value throws <see cref=\"global::System.InvalidOperationException\" />.</summary>");
            writer.Line($"public {vt} Value => IsDefault ? throw new global::System.InvalidOperationException(\"Value accessed on a default instance of {t}.\") : _value;");
        }
        else
        {
            writer.Line("/// <summary>The value.</summary>");
            writer.Line($"public {vt} Value => _value;");
        }

        writer.Line();
        writer.Line("/// <summary>The name of the member: the name of its field.</summary>");
        writer.Line($"public string Name => {( model.IsStruct ? "IsDefault ? string.Empty : " : "" )}Names.TryGetValue(_value, out var name) ? name : _value.ToString() ?? string.Empty;");
        writer.Line();
        writer.Line("/// <inheritdoc />");
        writer.Line($"public static string PrimitiveName => {Names.Literal(t)};");
        writer.Line();
        var allowed = names.Select(Names.CamelCase).OrderBy(z => z, StringComparer.Ordinal).Select(Names.Literal);
        writer.Line("/// <inheritdoc />");
        writer.Line($"public static {Names.Description}<{t}, {vt}> Description {{ get; }} = new() {{ IsSmartEnum = true, AllowedValues = [{string.Join(", ", allowed)}] }};");
        writer.Line();
        writer.Line("/// <summary>The codes of the errors this smart enum reports.</summary>");
        writer.Open("public static class Errors");
        writer.Line("/// <summary>The code <c>UNKNOWN_MEMBER</c>.</summary>");
        writer.Line("public const string UnknownMember = \"UNKNOWN_MEMBER\";");
        writer.Close();
        writer.Line();

        // The members are the type's own static fields, which may be initialized after this part's: everything is built when first used.
        writer.Line($"private static global::System.Collections.Generic.IReadOnlyList<{t}>? _all;");
        writer.Line($"private static global::System.Collections.Frozen.FrozenDictionary<{vt}, string>? _names;");
        writer.Line($"private static global::System.Collections.Frozen.FrozenDictionary<string, {t}>? _byName;");
        writer.Line($"private static global::System.Collections.Frozen.FrozenDictionary<string, {t}>? _byNameIgnoringCase;");
        writer.Line();
        writer.Line("/// <summary>Every member, in declaration order.</summary>");
        writer.Line($"public static global::System.Collections.Generic.IReadOnlyList<{t}> All => _all ??= [{string.Join(", ", members)}];");
        writer.Line();
        writer.Line($"private static global::System.Collections.Frozen.FrozenDictionary<{vt}, string> Names => _names ??= global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(");
        writer.Line($"    new global::System.Collections.Generic.KeyValuePair<{vt}, string>[] {{ {string.Join(", ", names.Select(m => $"new({Names.Identifier(m)}._value, {Names.Literal(m)})"))} }});");
        writer.Line();
        writer.Line($"private static global::System.Collections.Frozen.FrozenDictionary<string, {t}> ByName => _byName ??= global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(");
        writer.Line($"    new global::System.Collections.Generic.KeyValuePair<string, {t}>[] {{ {string.Join(", ", names.Select(m => $"new({Names.Literal(m)}, {Names.Identifier(m)})"))} }}, global::System.StringComparer.Ordinal);");
        writer.Line();
        writer.Line($"private static global::System.Collections.Frozen.FrozenDictionary<string, {t}> ByNameIgnoringCase => _byNameIgnoringCase ??=");
        writer.Line($"    global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(ByName, global::System.StringComparer.OrdinalIgnoreCase);");
        writer.Line();
        PrimitiveEmitter.Fail(writer, model);

        writer.Line("/// <summary>The member with a value, or throws when there is none.</summary>");
        writer.Line($"public static {t} Create({vt} value) => TryCreate(value, out var result, out var error) ? result : throw Fail(error);");
        writer.Line();
        writer.Line("/// <summary>The member with a value, or why there is none.</summary>");
        writer.Line($"public static bool TryCreate({vt} value, [{Names.NotNullWhen}(true)] out {t}{nullable} result, out {Names.Error} validationError)");
        writer.Open();
        writer.Line(v.IsString ? "if (value is not null && Names.ContainsKey(value))" : "if (Names.ContainsKey(value))");
        writer.Open();
        writer.Line($"result = new {t}(value);");
        writer.Line($"validationError = {Names.Error}.None;");
        PrimitiveEmitter.RecordSuccess(writer);
        writer.Line("return true;");
        writer.Close();
        writer.Line();
        writer.Line($"validationError = {Names.NewError("UNKNOWN_MEMBER", $"$\"{t} has no member with the value '{{value}}'.\"")};");
        PrimitiveEmitter.RecordFailure(writer);
        writer.Line("result = default;");
        writer.Line("return false;");
        writer.Close();
        writer.Line();
        writer.Line("/// <summary>The member with a value, or throws when there is none.</summary>");
        writer.Line($"public static {t} FromValue({vt} value) => Create(value);");
        writer.Line();
        writer.Line("/// <summary>The member with a value.</summary>");
        writer.Line($"public static bool TryFromValue({vt} value, [{Names.NotNullWhen}(true)] out {t}{nullable} result) => TryCreate(value, out result, out _);");
        writer.Line();
        writer.Line("/// <summary>The member with a name, or throws when there is none.</summary>");
        writer.Line($"public static {t} FromName(string name, bool ignoreCase = false) =>");
        writer.Line($"    TryFromName(name, ignoreCase, out var result) ? result : throw Fail({Names.NewError("UNKNOWN_MEMBER", $"$\"{t} has no member named '{{name}}'.\"")});");
        writer.Line();
        writer.Line("/// <summary>The member with a name.</summary>");
        writer.Line($"public static bool TryFromName(string? name, bool ignoreCase, [{Names.NotNullWhen}(true)] out {t}{nullable} result)");
        writer.Open();
        writer.Line("if (name is not null && (ignoreCase ? ByNameIgnoringCase : ByName).TryGetValue(name, out var member))");
        writer.Open();
        writer.Line("result = member;");
        writer.Line("return true;");
        writer.Close();
        writer.Line();
        writer.Line("result = default;");
        writer.Line("return false;");
        writer.Close();
        writer.Line();

        if (model.IsUnion) Union(writer, model);

        writer.Line("/// <inheritdoc />");
        writer.Line($"public static {t} Parse(string s, global::System.IFormatProvider? provider) =>");
        writer.Line($"    TryParse(s, provider, out var result) ? result : throw new global::System.FormatException($\"'{{s}}' is not a {t}.\");");
        writer.Line();
        writer.Line("/// <inheritdoc />");
        writer.Line($"public static bool TryParse([{Names.NotNullWhen}(true)] string? s, global::System.IFormatProvider? provider, [{Names.MaybeNullWhen}(false)] out {t} result)");
        writer.Open();
        writer.Line("if (TryFromName(s, true, out var named))");
        writer.Open();
        writer.Line("result = named;");
        writer.Line("return true;");
        writer.Close();
        writer.Line();
        if (v.IsString || v.IsParsable)
        {
            writer.Line(
                v.IsString
                    ? "if (s is not null && TryFromValue(s, out var byValue))"
                    : $"if ({vt}.TryParse(s, provider ?? {Names.Invariant}, out var value) && TryFromValue(value, out var byValue))"
            );
            writer.Open();
            writer.Line("result = byValue;");
            writer.Line("return true;");
            writer.Close();
            writer.Line();
        }

        writer.Line("result = default;");
        writer.Line("return false;");
        writer.Close();
        writer.Line();
        writer.Line("/// <inheritdoc />");
        writer.Line("public override string ToString() => Name;");
        writer.Line();
        writer.Line("/// <inheritdoc />");
        writer.Line("public string ToString(string? format, global::System.IFormatProvider? formatProvider) => Name;");
        writer.Line();
        writer.Line($"/// <summary>The value of a {t}.</summary>");
        writer.Line($"public static explicit operator {vt}({t} value) => value.Value;");
        writer.Line();

        TypeConverter(writer, model);
        JsonConverter(writer, model);
        writer.Close();
    }

    // The type a union member provider must have, by name: C# 15 reads the union members from it rather than the smart enum, so the smart enum
    // keeps its own Value. Its members and the case types are named here, and a member of the smart enum with one of these names is LPAD2104.
    public const string UnionMembers = "IUnionMembers";
    public const string CasesClass = "Cases";

    // A C# 15 custom union over one case type per member, through a union member provider: the smart enum keeps its typed Value, and a switch
    // over it with the case types is exhaustive. The smart enum keeps one instance of each case type, so matching allocates nothing.
    private static void Union(SourceWriter writer, PrimitiveModel model)
    {
        var t = model.Name;
        var members = model.EnumMembers.Select(Names.Identifier).ToArray();
        string Is(string member) => $"global::System.Collections.Generic.EqualityComparer<{model.Value!.Display}>.Default.Equals(_value, {member}._value)";

        writer.Line($"/// <summary>The case type of each member, for C# 15's patterns: <c>value switch {{ {t}.{CasesClass}.{members[0]} => … }}</c> must handle every member.</summary>");
        // CS8981: a member in lower case names its case type.
        writer.Line("#pragma warning disable CS8981");
        writer.Open($"public static class {CasesClass}");
        var first = true;
        foreach (var member in members)
        {
            if (!first) writer.Line();
            first = false;
            writer.Line($"/// <summary>The case of <see cref=\"{t}.{member}\" />.</summary>");
            // No members: a member's name is the name of its case type, and a member can't share it (CS0542). Internal, so only this
            // assembly can make one; the smart enum keeps one of each.
            writer.Open($"public sealed class {member}");
            writer.Line($"internal {member}() {{ }}");
            writer.Close();
        }

        writer.Close();
        writer.Line("#pragma warning restore CS8981");
        writer.Line();
        // Built when first used, like All: a static initializer of the declaration may match a member before this part's run.
        writer.Line("private static object[]? _cases;");
        writer.Line();
        writer.Line("/// <summary>The members C# 15 reads this smart enum through as a union of <see cref=\"" + CasesClass + "\" />'s case types.</summary>");
        writer.Line("[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]");
        writer.Open($"public interface {UnionMembers}");
        foreach (var member in members)
        {
            writer.Line($"/// <summary>The member <see cref=\"{t}.{member}\" />.</summary>");
            writer.Line($"public static {t} Create({CasesClass}.{member} value) => {t}.{member};");
            writer.Line();
        }

        writer.Line("/// <summary>The case type of the member, or null for none.</summary>");
        writer.Line("public object? Value { get; }");
        writer.Close();
        writer.Line();
        writer.Line($"object? {UnionMembers}.Value");
        writer.Open();
        writer.Open("get");
        if (model.IsStruct) writer.Line("if (IsDefault) return null;");
        writer.Line($"var cases = _cases ??= [{string.Join(", ", members.Select(m => $"new {CasesClass}.{m}()"))}];");
        for (var i = 0; i < members.Length; i++)
        {
            writer.Line($"if ({Is(members[i])}) return cases[{i}];");
        }

        writer.Line("return null;");
        writer.Close();
        writer.Close();
        writer.Line();
    }

    private static void TypeConverter(SourceWriter writer, PrimitiveModel model)
    {
        var t = model.Name;
        var vt = model.Value!.Display;
        writer.Line($"private sealed class {t}TypeConverter : global::System.ComponentModel.TypeConverter");
        writer.Open();
        writer.Line("public override bool CanConvertFrom(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Type sourceType) =>");
        writer.Line($"    sourceType == typeof(string) || sourceType == typeof({vt}) || base.CanConvertFrom(context, sourceType);");
        writer.Line();
        writer.Line(
            "public override object? ConvertFrom(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Globalization.CultureInfo? culture, object value) => value switch"
        );
        writer.Open();
        writer.Line($"string text => {t}.Parse(text, culture),");
        if (!model.Value.IsString) writer.Line($"{vt} typed => {t}.FromValue(typed),");
        writer.Line("_ => base.ConvertFrom(context, culture, value),");
        writer.Close(";");
        writer.Line();
        writer.Line("public override bool CanConvertTo(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Type? destinationType) =>");
        writer.Line($"    destinationType == typeof(string) || destinationType == typeof({vt}) || base.CanConvertTo(context, destinationType);");
        writer.Line();
        writer.Line(
            "public override object? ConvertTo(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Globalization.CultureInfo? culture, object? value, global::System.Type destinationType)"
        );
        writer.Open();
        writer.Line($"if (value is {t} member)");
        writer.Open();
        writer.Line("if (destinationType == typeof(string)) return member.Name;");
        writer.Line($"if (destinationType == typeof({vt})) return member.Value;");
        writer.Close();
        writer.Line();
        writer.Line("return base.ConvertTo(context, culture, value, destinationType);");
        writer.Close();
        writer.Close();
        writer.Line();
    }

    private static void JsonConverter(SourceWriter writer, PrimitiveModel model)
    {
        var t = model.Name;
        var v = model.Value!;
        writer.Line("// A member is written as its name in camel case, and read by its name (in any case) or its value.");
        // Public, so the JSON context of an application (System.Text.Json's source generator) can create it from another assembly.
        writer.Line("/// <summary>The System.Text.Json converter of the domain primitive.</summary>");
        writer.Line("[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]");
        writer.Line($"public sealed class {t}JsonConverter : {Names.Json}.Serialization.JsonConverter<{t}>");
        writer.Open();
        writer.Line($"public override {t} Read(ref {Names.Json}.Utf8JsonReader reader, global::System.Type typeToConvert, {Names.Json}.JsonSerializerOptions options)");
        writer.Open();
        if (model.IsStruct) writer.Line($"if (reader.TokenType == {Names.Json}.JsonTokenType.Null) return default;");
        writer.Line($"if (reader.TokenType == {Names.Json}.JsonTokenType.String && TryFromName(reader.GetString(), true, out var named)) return named;");
        writer.Line($"var value = {PrimitiveEmitter.ReadJson(v)};");
        writer.Line($"return TryFromValue(value, out var result) ? result : throw new {Names.Json}.JsonException($\"'{{value}}' is not a member of {t}.\");");
        writer.Close();
        writer.Line();
        writer.Line($"public override void Write({Names.Json}.Utf8JsonWriter writer, {t} value, {Names.Json}.JsonSerializerOptions options)");
        writer.Open();
        if (model.IsStruct)
        {
            writer.Line("if (value.IsDefault)");
            writer.Open();
            writer.Line("writer.WriteNullValue();");
            writer.Line("return;");
            writer.Close();
            writer.Line();
        }

        writer.Line($"writer.WriteStringValue({Names.Json}.JsonNamingPolicy.CamelCase.ConvertName(value.Name));");
        writer.Close();
        writer.Line();
        writer.Line(
            $"public override {t} ReadAsPropertyName(ref {Names.Json}.Utf8JsonReader reader, global::System.Type typeToConvert, {Names.Json}.JsonSerializerOptions options) =>"
        );
        writer.Line($"    TryFromName(reader.GetString(), true, out var result) ? result : throw new {Names.Json}.JsonException(\"The property name is not a member of {t}.\");");
        writer.Line();
        writer.Line($"public override void WriteAsPropertyName({Names.Json}.Utf8JsonWriter writer, {t} value, {Names.Json}.JsonSerializerOptions options) =>");
        writer.Line($"    writer.WritePropertyName({Names.Json}.JsonNamingPolicy.CamelCase.ConvertName(value.Name));");
        writer.Close();
    }
}
