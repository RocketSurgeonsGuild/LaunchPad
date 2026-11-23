using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Model;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Emit;

/// <summary>
///     Writes a value object: a domain primitive that holds several values, validated together. The values are the type's <c>init</c>
///     properties, or for the Money, DateRange and TimeRange kinds the properties the generator declares.
/// </summary>
internal static class ValueObjectEmitter
{
    public static void Emit(SourceWriter writer, PrimitiveModel model)
    {
        var t = model.Name;
        var members = model.Members;
        var nullable = model.IsStruct ? "" : "?";

        PrimitiveEmitter.TypeAttributes(writer, model, typeConverter: false);
        writer.Line($"{PrimitiveEmitter.Declaration(model)} : {Names.Primitives}.IValueObject<{t}>");
        writer.Open();
        KindProperties(writer, model);
        if (model.IsStruct)
        {
            // No field of its own: a partial struct whose fields are declared in more than one part has no defined layout (CS0282).
            writer.Line("/// <summary>Whether this is the default value, which holds none of the values.</summary>");
            writer.Line($"public bool IsDefault => Equals(default({t}));");
            writer.Line();
        }

        writer.Line("/// <inheritdoc />");
        writer.Line($"public static string PrimitiveName => {Names.Literal(t)};");
        writer.Line();
        writer.Line("/// <inheritdoc />");
        writer.Line($"public static {Names.ValueObjectDescription} Description {{ get; }} = new(typeof({t}), [{string.Join(", ", members.Select(m => Names.Literal(m.Name)))}])");
        writer.Line("{");
        writer.Line($"    Members = [{string.Join(", ", members.Select(m => $"new({Names.Literal(m.Name)}, typeof({m.TypeOf}), {( m.IsNullable ? "true" : "false" )})"))}],");
        writer.Line("};");
        writer.Line();
        writer.Line("/// <summary>The codes of the errors this value object reports.</summary>");
        writer.Open("public static class Errors");
        writer.Line("public const string Invariant = \"INVARIANT\";");
        if (model.Kind == "Money") writer.Line("public const string Currency = \"CURRENCY\";");
        if (model.Range is not null || model.Kind is "DateRange" or "TimeRange") writer.Line("public const string Range = \"RANGE\";");
        writer.Close();
        writer.Line();
        writer.Line("/// <summary>");
        writer.Line("///     Checks the values together, after the checks of the kind. Implement it to set <paramref name=\"error\" /> when they don't fit.");
        writer.Line("/// </summary>");
        writer.Line($"static partial void Validate(ref {t} value, ref {Names.Error} error);");
        writer.Line();
        PrimitiveEmitter.Fail(writer, model);

        var parameters = string.Join(", ", members.Select(m => $"{m.Type} {m.Parameter}"));
        var arguments = string.Join(", ", members.Select(m => m.Parameter));
        var result = $"[{Names.NotNullWhen}(true)] out {t}{nullable} result, out {Names.Error} validationError";
        writer.Line("/// <summary>Creates the value object, or throws when its values don't fit.</summary>");
        writer.Line($"public static {t} Create({parameters}) => TryCreate({arguments}, out var result, out var error) ? result : throw Fail(error);");
        writer.Line();
        if (model.Kind == "Money" && model.Currency is { } currency)
        {
            writer.Line($"/// <summary>Creates an amount of {currency}, or throws when it doesn't fit.</summary>");
            writer.Line($"public static {t} Create(decimal amount) => Create(amount, {Names.Literal(currency)});");
            writer.Line();
            writer.Line($"/// <summary>Creates an amount of {currency}, or says why it doesn't fit.</summary>");
            writer.Line($"public static bool TryCreate(decimal amount, {result}) => TryCreate(amount, {Names.Literal(currency)}, out result, out validationError);");
            writer.Line();
        }

        writer.Line("/// <summary>Creates the value object, or says why its values don't fit.</summary>");
        writer.Line($"public static bool TryCreate({parameters}, {result})");
        writer.Open();
        if (model.Kind == "Money") writer.Line("currency = currency?.Trim().ToUpperInvariant() ?? string.Empty;");
        var initializers = members.Select(m => $"{m.Name} = {m.Parameter}").ToList();
        writer.Line($"var instance = new {t} {{ {string.Join(", ", initializers)} }};");
        writer.Line("validationError = Check(instance);");
        writer.Line("if (!validationError.IsError) Validate(ref instance, ref validationError);");
        writer.Line("if (validationError.IsError)");
        writer.Open();
        PrimitiveEmitter.RecordFailure(writer);
        writer.Line("result = default;");
        writer.Line("return false;");
        writer.Close();
        writer.Line();
        PrimitiveEmitter.RecordSuccess(writer);
        writer.Line("result = instance;");
        writer.Line("return true;");
        writer.Close();
        writer.Line();
        Check(writer, model);
        KindMembers(writer, model);
        JsonConverter(writer, model);
        writer.Close();
    }

    private static void KindProperties(SourceWriter writer, PrimitiveModel model)
    {
        if (model.Kind is not ( "Money" or "DateRange" or "TimeRange" )) return;
        var initializer = model.IsStruct ? "" : " = default!;";
        foreach (var member in model.Members)
        {
            var summary = member.Name switch
            {
                "Amount" => "The amount.",
                "Currency" => "The ISO 4217 code of the currency.",
                "Start" => "The first " + ( model.Kind == "DateRange" ? "day" : "time" ) + " of the range.",
                _ => "The last " + ( model.Kind == "DateRange" ? "day" : "time" ) + " of the range.",
            };
            writer.Line($"/// <summary>{summary}</summary>");
            writer.Line($"public {member.Type} {member.Name} {{ get; private init; }}{initializer}");
            writer.Line();
        }
    }

    private static void Check(SourceWriter writer, PrimitiveModel model)
    {
        var t = model.Name;
        writer.Line($"private static {Names.Error} Check({t} value)");
        writer.Open();
        switch (model.Kind)
        {
            case "Money":
                writer.Line($"if (!{Names.Primitives}.Registries.Iso4217Currencies.Codes.Contains(value.Currency))");
                writer.Line($"    return {Names.NewError(model.KindError is { Code.Length: > 0 } k ? k : new Check("CURRENCY", model.KindError?.Message), $"$\"{t} must be in an ISO 4217 currency. Got '{{value.Currency}}'.\"")};");
                if (model.Currency is { } pinned)
                {
                    writer.Line($"if (!string.Equals(value.Currency, {Names.Literal(pinned)}, global::System.StringComparison.Ordinal))");
                    writer.Line($"    return {Names.NewError("CURRENCY", $"$\"{t} must be in {pinned}. Got '{{value.Currency}}'.\"")};");
                }

                if (model.Range is { } range)
                {
                    if (range.Min is { } min)
                        writer.Line($"if (value.Amount < {min}) return {Names.NewError(range.Error, $"$\"{t} must be at least {range.MinText}. Got {{value.Amount}}.\"")};");
                    if (range.Max is { } max)
                        writer.Line($"if (value.Amount > {max}) return {Names.NewError(range.Error, $"$\"{t} must be at most {range.MaxText}. Got {{value.Amount}}.\"")};");
                }

                break;
            case "DateRange":
            case "TimeRange":
                writer.Line("if (value.Start.CompareTo(value.End) > 0)");
                writer.Line($"    return {Names.NewError(model.KindError is { Code.Length: > 0 } r ? r : new Check("RANGE", model.KindError?.Message), $"$\"{t} must not start after it ends.\"")};");
                break;
            default:
                break;
        }

        writer.Line($"return {Names.Error}.None;");
        writer.Close();
        writer.Line();
    }

    private static void KindMembers(SourceWriter writer, PrimitiveModel model)
    {
        var t = model.Name;
        var empty = model.IsStruct ? "IsDefault ? string.Empty : " : "";
        switch (model.Kind)
        {
            case "Money":
                writer.Line("/// <inheritdoc />");
                writer.Line($"public override string ToString() => {empty}$\"{{Amount.ToString({Names.Invariant})}} {{Currency}}\";");
                writer.Line();
                writer.Line($"private static string SameCurrency({t} left, {t} right) => string.Equals(left.Currency, right.Currency, global::System.StringComparison.Ordinal)");
                writer.Line("    ? left.Currency");
                writer.Line("    : throw new global::System.InvalidOperationException($\"Amounts in {left.Currency} and {right.Currency} can't be combined or compared.\");");
                writer.Line();
                writer.Line("// Every result is checked again by the rules.");
                writer.Line($"public static {t} operator +({t} left, {t} right) => Create(left.Amount + right.Amount, SameCurrency(left, right));");
                writer.Line($"public static {t} operator -({t} left, {t} right) => Create(left.Amount - right.Amount, SameCurrency(left, right));");
                writer.Line($"public static {t} operator *({t} left, decimal right) => Create(left.Amount * right, left.Currency);");
                writer.Line($"public static {t} operator /({t} left, decimal right) => Create(left.Amount / right, left.Currency);");
                foreach (var op in new[] { "<", "<=", ">", ">=" })
                {
                    writer.Line($"public static bool operator {op}({t} left, {t} right) => SameCurrency(left, right) is not null && left.Amount {op} right.Amount;");
                }

                writer.Line();
                break;
            case "DateRange":
            case "TimeRange":
                var bound = model.Members[0];
                var v = bound.Value!;
                string Text(string value) => v.NodaPattern is { } pattern ? $"({pattern}.Format({value}))" : $"({value}.ToString(\"O\", {Names.Invariant}))";
                writer.Line("/// <inheritdoc />");
                writer.Line($"public override string ToString() => {empty}$\"{{{Text("Start")}}}/{{{Text("End")}}}\";");
                writer.Line();
                writer.Line("/// <summary>Whether the range includes a value (both ends included).</summary>");
                writer.Line($"public bool Contains({bound.Type} value) => value.CompareTo(Start) >= 0 && value.CompareTo(End) <= 0;");
                writer.Line();
                writer.Line("/// <summary>Whether the range shares at least one value with another.</summary>");
                writer.Line($"public bool Overlaps({t} other) => Start.CompareTo(other.End) <= 0 && other.Start.CompareTo(End) <= 0;");
                writer.Line();
                if (model.Kind == "DateRange")
                {
                    writer.Line("/// <summary>The number of days in the range, both ends included.</summary>");
                    writer.Line(
                        v.IsNodaTime
                            ? "public int Days => global::NodaTime.Period.DaysBetween(Start, End) + 1;"
                            : "public int Days => End.DayNumber - Start.DayNumber + 1;"
                    );
                }
                else
                {
                    writer.Line("/// <summary>The time from the start of the range to its end.</summary>");
                    writer.Line(
                        v.IsNodaTime
                            ? "public global::NodaTime.Duration Duration => global::NodaTime.Period.Between(Start, End).ToDuration();"
                            : "public global::System.TimeSpan Duration => End - Start;"
                    );
                }

                writer.Line();
                break;
            default:
                break;
        }
    }

    private static string Read(MemberInfo member)
    {
        return  member.PrimitiveValue is { } primitive 
            ?  $"{member.Underlying}.Create({PrimitiveEmitter.ReadJson(primitive)})" 
            :  member.Value is { } value
            ? PrimitiveEmitter.ReadJson(value)
            : $"{Names.Json}.JsonSerializer.Deserialize(ref reader, ({Names.JsonMetadata}.JsonTypeInfo<{member.TypeOf}>)options.GetTypeInfo(typeof({member.TypeOf})))!";
    }

    private static string Write(MemberInfo member, string expression)
    {
        var value = member.IsNullableValueType ? expression + ".Value" : expression;
        return  member.PrimitiveValue is { } primitive 
            ?  PrimitiveEmitter.WriteJson(primitive, value + ".Value") 
            :  member.Value is { } info
            ? PrimitiveEmitter.WriteJson(info, value)
            : $"{Names.Json}.JsonSerializer.Serialize(writer, {expression}, ({Names.JsonMetadata}.JsonTypeInfo<{member.TypeOf}>)options.GetTypeInfo(typeof({member.TypeOf})));";
    }

    private static void JsonConverter(SourceWriter writer, PrimitiveModel model)
    {
        var t = model.Name;
        var members = model.Members;
        writer.Line("// Written as an object of its values, named by the options' naming policy (camel case when there is none).");
        // Public, so the JSON source generator of a project that serializes the domain primitive can create it (it reads the
        // converter from the attribute and can only instantiate one it can see).
        writer.Line("/// <summary>The JSON converter of the domain primitive, which the JsonConverter attribute names.</summary>");
        writer.Line("[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]");
        writer.Line($"public sealed class {t}JsonConverter : {Names.Json}.Serialization.JsonConverter<{t}>");
        writer.Open();
        writer.Line($"private static string Name({Names.Json}.JsonSerializerOptions options, string name, string camelCase) => options.PropertyNamingPolicy?.ConvertName(name) ?? camelCase;");
        writer.Line();
        writer.Line($"public override {t} Read(ref {Names.Json}.Utf8JsonReader reader, global::System.Type typeToConvert, {Names.Json}.JsonSerializerOptions options)");
        writer.Open();
        if (model.IsStruct) writer.Line($"if (reader.TokenType == {Names.Json}.JsonTokenType.Null) return default;");
        writer.Line($"if (reader.TokenType != {Names.Json}.JsonTokenType.StartObject) throw new {Names.Json}.JsonException(\"{t} is written as an object.\");");
        foreach (var member in members)
        {
            writer.Line($"{member.Type} {member.Parameter} = default!;");
        }

        writer.Line($"while (reader.Read() && reader.TokenType != {Names.Json}.JsonTokenType.EndObject)");
        writer.Open();
        writer.Line("var property = reader.GetString();");
        writer.Line("reader.Read();");
        var first = true;
        foreach (var member in members)
        {
            var camel = Names.Literal(Names.CamelCase(member.Name));
            writer.Line(
                $"{( first ? "if" : "else if" )} (string.Equals(property, {camel}, global::System.StringComparison.OrdinalIgnoreCase) || string.Equals(property, Name(options, {Names.Literal(member.Name)}, {camel}), global::System.StringComparison.Ordinal))"
            );
            writer.Line(
                member.IsNullable
                    ? $"    {member.Parameter} = reader.TokenType == {Names.Json}.JsonTokenType.Null ? default : {Read(member)};"
                    : $"    {member.Parameter} = {Read(member)};"
            );
            first = false;
        }

        writer.Line(first ? "reader.Skip();" : "else reader.Skip();");
        writer.Close();
        writer.Line();
        var arguments = string.Join(", ", members.Select(m => m.Parameter));
        writer.Line($"return TryCreate({arguments}{( members.Length > 0 ? ", " : "" )}out var result, out var error) ? result : throw new {Names.Json}.JsonException($\"[{{error.Code}}] {{error.Message}}\");");
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

        writer.Line("writer.WriteStartObject();");
        foreach (var member in members)
        {
            writer.Line($"writer.WritePropertyName(Name(options, {Names.Literal(member.Name)}, {Names.Literal(Names.CamelCase(member.Name))}));");
            if (member.IsNullable)
            {
                writer.Line($"if (value.{member.Name} is null) writer.WriteNullValue();");
                writer.Line($"else {Write(member, "value." + member.Name)}");
            }
            else
            {
                writer.Line(Write(member, "value." + member.Name));
            }
        }

        writer.Line("writer.WriteEndObject();");
        writer.Close();
        writer.Close();
    }
}
