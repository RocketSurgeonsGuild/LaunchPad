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
        var members = model.EnumMembers;
        var nullable = model.IsStruct ? "" : "?";

        PrimitiveEmitter.TypeAttributes(writer, model);
        writer.Line($"{PrimitiveEmitter.Declaration(model)} :");
        writer.Line($"    {Names.Primitives}.IDomainPrimitive<{t}, {vt}>,");
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
        var allowed = members.Select(Names.CamelCase).OrderBy(z => z, StringComparer.Ordinal).Select(Names.Literal);
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
        writer.Line($"    new global::System.Collections.Generic.KeyValuePair<{vt}, string>[] {{ {string.Join(", ", members.Select(m => $"new({m}._value, {Names.Literal(m)})"))} }});");
        writer.Line();
        writer.Line($"private static global::System.Collections.Frozen.FrozenDictionary<string, {t}> ByName => _byName ??= global::System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(");
        writer.Line($"    new global::System.Collections.Generic.KeyValuePair<string, {t}>[] {{ {string.Join(", ", members.Select(m => $"new({Names.Literal(m)}, {m})"))} }}, global::System.StringComparer.Ordinal);");
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

        Exhaustive(writer, model);

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

    private static void Exhaustive(SourceWriter writer, PrimitiveModel model)
    {
        var t = model.Name;
        var members = model.EnumMembers;
        if (members.Length == 0) return;
        string Parameter(string member) => "when" + member;
        string Is(string member) => $"global::System.Collections.Generic.EqualityComparer<{model.Value!.Display}>.Default.Equals(_value, {member}._value)";
        var unknown = $"throw new global::System.InvalidOperationException($\"{{this}} is not a member of {t}.\")";

        writer.Line("/// <summary>The result of the function for this member.</summary>");
        writer.Line($"public TResult Match<TResult>({string.Join(", ", members.Select(m => $"global::System.Func<TResult> {Parameter(m)}"))})");
        writer.Open();
        foreach (var member in members)
        {
            writer.Line($"if ({Is(member)}) return {Parameter(member)}();");
        }

        writer.Line(unknown + ";");
        writer.Close();
        writer.Line();
        writer.Line("/// <summary>The result of the function for this member, given the member.</summary>");
        writer.Line($"public TResult Map<TResult>({string.Join(", ", members.Select(m => $"global::System.Func<{t}, TResult> {Parameter(m)}"))})");
        writer.Open();
        foreach (var member in members)
        {
            writer.Line($"if ({Is(member)}) return {Parameter(member)}(this);");
        }

        writer.Line(unknown + ";");
        writer.Close();
        writer.Line();
        writer.Line("/// <summary>Runs the action for this member.</summary>");
        writer.Line($"public void Switch({string.Join(", ", members.Select(m => $"global::System.Action {Parameter(m)}"))})");
        writer.Open();
        foreach (var member in members)
        {
            writer.Line($"if ({Is(member)}) {{ {Parameter(member)}(); return; }}");
        }

        writer.Line(unknown + ";");
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
