using System.Globalization;

using Rocket.Surgery.LaunchPad.Primitives.Analyzers.Model;

namespace Rocket.Surgery.LaunchPad.Primitives.Analyzers.Emit;

/// <summary>
///     Writes a domain primitive that holds one value: a string, number, date or time, or strong ID.
/// </summary>
internal static class SingleValueEmitter
{
    public static void Emit(SourceWriter writer, PrimitiveModel model)
    {
        var t = model.Name;
        var v = model.Value!;
        var vt = v.Display;
        var clock = Clock(model);

        PrimitiveEmitter.TypeAttributes(writer, model);
        writer.Line($"{PrimitiveEmitter.Declaration(model)} :");
        writer.Indent();
        var interfaces = new List<string>
        {
            model.Shape == PrimitiveShape.StrongId ? $"{Names.Primitives}.IStrongId<{t}, {vt}>" : $"{Names.Primitives}.IDomainPrimitive<{t}, {vt}>",
            $"global::System.IParsable<{t}>",
            $"global::System.ISpanParsable<{t}>",
            $"global::System.IUtf8SpanParsable<{t}>",
            "global::System.IFormattable",
            "global::System.ISpanFormattable",
            "global::System.IUtf8SpanFormattable",
            $"global::System.Numerics.IEqualityOperators<{t}, {t}, bool>",
        };
        if (v.IsOrdered)
        {
            interfaces.Add($"global::System.IComparable<{t}>");
            interfaces.Add("global::System.IComparable");
            interfaces.Add($"global::System.Numerics.IComparisonOperators<{t}, {t}, bool>");
        }

        var operations = model.Shape == PrimitiveShape.Numeric ? model.Operations : 0;
        if (( operations & 1 ) != 0) interfaces.Add($"global::System.Numerics.IAdditionOperators<{t}, {t}, {t}>");
        if (( operations & 2 ) != 0) interfaces.Add($"global::System.Numerics.ISubtractionOperators<{t}, {t}, {t}>");
        if (( operations & 4 ) != 0) interfaces.Add($"global::System.Numerics.IMultiplyOperators<{t}, {vt}, {t}>");
        if (( operations & 8 ) != 0) interfaces.Add($"global::System.Numerics.IDivisionOperators<{t}, {vt}, {t}>");
        if (( operations & 16 ) != 0) interfaces.Add($"global::System.Numerics.IUnaryNegationOperators<{t}, {t}>");
        for (var i = 0; i < interfaces.Count; i++)
        {
            writer.Line(interfaces[i] + ( i < interfaces.Count - 1 ? "," : "" ));
        }

        writer.Outdent();
        writer.Open();

        Storage(writer, model);
        Description(writer, model);
        Errors(writer, model);
        Regexes(writer, model);
        PrimitiveEmitter.Fail(writer, model);
        Factories(writer, model, clock);
        Normalize(writer, model);
        Validate(writer, model, clock);
        if (model.Shape == PrimitiveShape.StrongId) StrongId(writer, model);
        if (model.Shape == PrimitiveShape.Date) DateHelpers(writer, model, clock);
        Parsing(writer, model);
        Formatting(writer, model);
        if (v.IsOrdered) Comparison(writer, model);
        Conversions(writer, model);
        if (operations != 0) Operators(writer, model, operations);
        TypeConverter(writer, model);
        JsonConverter(writer, model);
        writer.Close();
    }

    /// <summary>
    ///     What "now" is read from, for a date with PastOnly, FutureOnly or an age: a TimeProvider, or for NodaTime an IClock.
    /// </summary>
    private sealed record ClockInfo(string Type, string Name, string Default);

    private static ClockInfo? Clock(PrimitiveModel model) =>
        model.Dates is not { } dates || !( dates.PastOnly || dates.FutureOnly || dates.MaxAge is not null ) || Now(model.Value!, "clock") is null
            ? null
            : model.Value!.IsNodaTime
                ? new("global::NodaTime.IClock", "clock", "global::NodaTime.SystemClock.Instance")
                : new("global::System.TimeProvider", "timeProvider", "global::System.TimeProvider.System");

    /// <summary>
    ///     "Now" as a value of the primitive's type (ordered the way <see cref="ValueTypeInfo.Ordered" /> orders it), or null when the type has
    ///     no "now" (a Duration, Period, Offset or AnnualDate).
    /// </summary>
    private static string? Now(ValueTypeInfo value, string clock) => value.MetadataName switch
    {
        "DateOnly" => $"global::System.DateOnly.FromDateTime({clock}.GetUtcNow().UtcDateTime)",
        "TimeOnly" => $"global::System.TimeOnly.FromDateTime({clock}.GetUtcNow().UtcDateTime)",
        "DateTimeOffset" => $"{clock}.GetUtcNow()",
        "Instant" or "OffsetDateTime" or "ZonedDateTime" => $"{clock}.GetCurrentInstant()",
        "LocalDate" => $"{clock}.GetCurrentInstant().InUtc().Date",
        "LocalTime" => $"{clock}.GetCurrentInstant().InUtc().TimeOfDay",
        "LocalDateTime" => $"{clock}.GetCurrentInstant().InUtc().LocalDateTime",
        "YearMonth" => $"{clock}.GetCurrentInstant().InUtc().Date.ToYearMonth()",
        _ => null,
    };

    private static void Storage(SourceWriter writer, PrimitiveModel model)
    {
        var t = model.Name;
        var vt = model.Value!.Display;
        var tracksInitialization = model.IsStruct && !model.Value.IsString;
        writer.Line($"private readonly {vt} _value;");
        if (tracksInitialization) writer.Line("private readonly bool _initialized;");
        writer.Line();
        writer.Line($"private {t}({vt} value)");
        writer.Open();
        writer.Line("_value = value;");
        if (tracksInitialization) writer.Line("_initialized = true;");
        writer.Close();
        writer.Line();
        if (model.IsStruct)
        {
            writer.Line("/// <summary>Whether this is the default value, which holds no value: it was not created by <c>Create</c>.</summary>");
            writer.Line($"public bool IsDefault => {( tracksInitialization ? "!_initialized" : "_value is null" )};");
            writer.Line();
            writer.Line("/// <summary>The value. Reading it from the default value throws <see cref=\"global::System.InvalidOperationException\" />.</summary>");
            writer.Line($"public {vt} Value => IsDefault");
            writer.Indent();
            writer.Line($"? throw new global::System.InvalidOperationException(\"Value accessed on a default instance of {t}. Check IsDefault before accessing Value.\")");
            writer.Line(": _value;");
            writer.Outdent();
        }
        else
        {
            writer.Line("/// <summary>The value.</summary>");
            writer.Line($"public {vt} Value => _value;");
        }

        writer.Line();
        writer.Line("/// <inheritdoc />");
        writer.Line($"public static string PrimitiveName => {Names.Literal(t)};");
        writer.Line();
    }

    private static void Description(SourceWriter writer, PrimitiveModel model)
    {
        var properties = new List<string>();
        if (model.Shape == PrimitiveShape.StrongId) properties.Add("IsStrongId = true");
        if (model.Format is { } format) properties.Add($"Format = {Names.Literal(format)}");
        if (model.Strings is { } strings)
        {
            var min = Math.Max(strings.MinLength ?? 0, strings.NotEmpty is null ? 0 : 1);
            if (min > 0) properties.Add($"MinLength = {min}");
            if (strings.MaxLength is { } max) properties.Add($"MaxLength = {max}");
            if (strings.Regexes.Length > 0) properties.Add($"Patterns = [{string.Join(", ", strings.Regexes.Select(z => Names.Literal(z.Pattern)))}]");
        }

        if (model.Range is { } range && ( model.Value!.IsNumber || model.Shape == PrimitiveShape.StrongId ))
        {
            if (range.MinText is { } minimum)
            {
                properties.Add($"Minimum = {Names.Literal(minimum)}");
                if (range.MinExclusive) properties.Add("MinimumIsExclusive = true");
            }

            if (range.MaxText is { } maximum)
            {
                properties.Add($"Maximum = {Names.Literal(maximum)}");
                if (range.MaxExclusive) properties.Add("MaximumIsExclusive = true");
            }
        }

        if (model.Scale is { } scale) properties.Add($"Scale = {scale}");
        writer.Line("/// <inheritdoc />");
        writer.Line(
            $"public static {Names.Description}<{model.Name}, {model.Value!.Display}> Description {{ get; }} = new() {{ {string.Join(", ", properties)}{( properties.Count > 0 ? " " : "" )}}};"
        );
        writer.Line();
    }

    private static void Errors(SourceWriter writer, PrimitiveModel model)
    {
        var codes = new SortedSet<string>(StringComparer.Ordinal);
        if (model.Value!.IsString) codes.UnionWith(["NULL_INPUT", "INVALID_UNICODE"]);
        if (model.Strings is { } strings)
        {
            codes.Add(strings.LengthError.Code);
            if (strings.NotEmpty is { } notEmpty) codes.Add(notEmpty.Code);
            foreach (var regex in strings.Regexes)
            {
                codes.Add(regex.Error.Code);
            }

            if (strings.AllowedSchemes.Length > 0 || strings.Registry is not null) codes.Add(model.KindError is { Code.Length: > 0 } k ? k.Code : "FORMAT");
        }

        if (model.Range is { } range) codes.Add(range.Error.Code);
        if (model.Dates is { } dates) codes.Add(dates.Error.Code);
        if (model.Value.IsFloatingPoint) codes.Add("RANGE");
        if (model.Scale is not null) codes.Add("SCALE");
        if (model.Shape == PrimitiveShape.StrongId && model.RejectEmpty) codes.Add("EMPTY");

        writer.Line("/// <summary>The codes of the errors this domain primitive reports.</summary>");
        writer.Open("public static class Errors");
        foreach (var code in codes)
        {
            writer.Line($"public const string {ConstantName(code)} = {Names.Literal(code)};");
        }

        writer.Close();
        writer.Line();
    }

    private static string ConstantName(string code)
    {
        var parts = code.Split(['_', '-', ' ', '.'], StringSplitOptions.RemoveEmptyEntries);
        var name = string.Concat(parts.Select(z => char.ToUpperInvariant(z[0]) + z[1..].ToLowerInvariant()));
        return name.Length == 0 || !char.IsLetter(name[0]) ? "Code" + name : name;
    }

    private static void Regexes(SourceWriter writer, PrimitiveModel model)
    {
        if (model.Strings is not { } strings) return;
        const string options = "global::System.Text.RegularExpressions.RegexOptions.CultureInvariant | global::System.Text.RegularExpressions.RegexOptions.Compiled";
        for (var i = 0; i < strings.Regexes.Length; i++)
        {
            writer.Line(
                $"private static readonly global::System.Text.RegularExpressions.Regex Pattern{i} = new({Names.Literal(strings.Regexes[i].Pattern)}, {options}, global::System.TimeSpan.FromMilliseconds(100));"
            );
        }

        if (strings.NormalizeWhitespace)
            writer.Line($"private static readonly global::System.Text.RegularExpressions.Regex WhiteSpace = new(@\"\\s+\", {options}, global::System.TimeSpan.FromMilliseconds(100));");
        if (strings.Regexes.Length > 0 || strings.NormalizeWhitespace) writer.Line();
    }

    private static void Factories(SourceWriter writer, PrimitiveModel model, ClockInfo? clock)
    {
        var t = model.Name;
        var vt = model.Value!.Display;
        var outResult = PrimitiveEmitter.OutResult(model);
        var result = $"[{Names.NotNullWhen}(true)] out {outResult} result, out {Names.Error} validationError";

        writer.Line("/// <summary>Creates the domain primitive from a value, or throws when its rules reject the value.</summary>");
        if (clock is null)
        {
            writer.Line($"public static {t} Create({vt} value) => TryCreate(value, out var result, out var error) ? result : throw Fail(error);");
        }
        else
        {
            writer.Line($"public static {t} Create({vt} value) => Create(value, {clock.Default});");
            writer.Line();
            writer.Line($"/// <summary>Creates the domain primitive from a value, reading \"now\" from <paramref name=\"{clock.Name}\" />, or throws when its rules reject the value.</summary>");
            writer.Line($"public static {t} Create({vt} value, {clock.Type} {clock.Name}) =>");
            writer.Line($"    TryCreate(value, {clock.Name}, out var result, out var error) ? result : throw Fail(error);");
            writer.Line();
            writer.Line("/// <summary>Creates the domain primitive from a value, or says why its rules reject the value.</summary>");
            writer.Line($"public static bool TryCreate({vt} value, {result}) => TryCreate(value, {clock.Default}, out result, out validationError);");
        }

        writer.Line();
        if (clock is null)
        {
            writer.Line("/// <summary>Creates the domain primitive from a value, or says why its rules reject the value.</summary>");
            writer.Line($"public static bool TryCreate({vt} value, {result})");
        }
        else
        {
            writer.Line($"/// <summary>Creates the domain primitive from a value, reading \"now\" from <paramref name=\"{clock.Name}\" />, or says why its rules reject the value.</summary>");
            writer.Line($"public static bool TryCreate({vt} value, {clock.Type} {clock.Name}, {result})");
        }

        writer.Open();
        if (model.Value.IsString)
        {
            writer.Line("if (value is null)");
            writer.Open();
            writer.Line($"validationError = {Names.NewError("NULL_INPUT", $"\"{t} must not be null.\"")};");
            PrimitiveEmitter.RecordFailure(writer);
            writer.Line("result = default;");
            writer.Line("return false;");
            writer.Close();
            writer.Line();
        }

        writer.Line("if (!TryNormalize(ref value, out validationError) || (validationError = Validate(value" + ( clock is null ? "" : ", " + clock.Name ) + ")).IsError)");
        writer.Open();
        PrimitiveEmitter.RecordFailure(writer);
        writer.Line("result = default;");
        writer.Line("return false;");
        writer.Close();
        writer.Line();
        PrimitiveEmitter.RecordSuccess(writer);
        writer.Line($"result = new {t}(value);");
        writer.Line("return true;");
        writer.Close();
        writer.Line();
    }

    private static void Normalize(SourceWriter writer, PrimitiveModel model)
    {
        var t = model.Name;
        var vt = model.Value!.Display;
        writer.Line($"private static bool TryNormalize(ref {vt} value, out {Names.Error} validationError)");
        writer.Open();
        writer.Line($"validationError = {Names.Error}.None;");
        if (model.Strings is { } strings)
        {
            // Normalizing a huge value is expensive: one far longer than any value the rules accept is rejected first.
            if (strings.MaxLength is { } max)
            {
                var guard = Math.Max(max * 2L, 4096);
                writer.Line($"if (value.Length > {guard.ToString(CultureInfo.InvariantCulture)})");
                writer.Open();
                writer.Line($"validationError = {Names.NewError(strings.LengthError, $"$\"{t} must be at most {max} character(s). Got {{value.Length}}.\"")};");
                writer.Line("return false;");
                writer.Close();
            }

            if (strings.Trim) writer.Line("value = value.Trim();");
            if (strings.TrimStart) writer.Line("value = value.TrimStart();");
            if (strings.TrimEnd) writer.Line("value = value.TrimEnd();");
            if (strings.NormalizeWhitespace) writer.Line("value = WhiteSpace.Replace(value, \" \");");
            if (strings.LowerCase) writer.Line("value = value.ToLowerInvariant();");
            if (strings.UpperCase) writer.Line("value = value.ToUpperInvariant();");
            writer.Line("try");
            writer.Open();
            writer.Line("value = value.Normalize(global::System.Text.NormalizationForm.FormC);");
            writer.Close();
            writer.Line("catch (global::System.ArgumentException)");
            writer.Open();
            writer.Line($"validationError = {Names.NewError("INVALID_UNICODE", $"\"{t} contains invalid or unpaired Unicode code points.\"")};");
            writer.Line("return false;");
            writer.Close();
        }

        foreach (var normalizer in model.Normalizers)
        {
            writer.Line($"value = {normalizer}.Normalize(value);");
        }

        writer.Line("return true;");
        writer.Close();
        writer.Line();
    }

    private static void Validate(SourceWriter writer, PrimitiveModel model, ClockInfo? clock)
    {
        var t = model.Name;
        var v = model.Value!;
        writer.Line($"private static {Names.Error} Validate({v.Display} value{( clock is null ? "" : $", {clock.Type} {clock.Name}" )})");
        writer.Open();
        if (model.Shape == PrimitiveShape.StrongId && model.RejectEmpty)
        {
            var empty = v.Category == ValueCategory.Guid ? "value == global::System.Guid.Empty" : v.IsString ? "string.IsNullOrWhiteSpace(value)" : "value == 0";
            writer.Line($"if ({empty}) return {Names.NewError("EMPTY", $"\"{t} must not be empty.\"")};");
        }

        if (model.Strings is { } strings) StringChecks(writer, model, strings);
        if (v.IsFloatingPoint)
        {
            writer.Line($"if (!{v.Display}.IsFinite(value)) return {Names.NewError(model.Range?.Error ?? new Check("RANGE", null), $"\"{t} must be a finite number.\"")};");
        }

        if (model.Range is { } range && v.IsOrdered) RangeChecks(writer, model, range);
        if (model.Scale is { } scale)
            writer.Line($"if (decimal.Round(value, {scale}) != value) return {Names.NewError("SCALE", $"\"{t} must have at most {scale} decimal place(s).\"")};");
        if (model.Dates is { } dates) DateChecks(writer, model, dates, clock);
        foreach (var validator in model.Validators)
        {
            writer.Line($"if ({validator}.Validate(value) is {{ IsError: true }} custom) return custom;");
        }

        writer.Line($"return {Names.Error}.None;");
        writer.Close();
        writer.Line();
        if (model.Strings?.Registry == "Iban") IbanChecksum(writer);
    }

    private static void StringChecks(SourceWriter writer, PrimitiveModel model, StringRules strings)
    {
        var t = model.Name;
        var formatError = model.KindError is { } kindError ? new Check(kindError.Code.Length > 0 ? kindError.Code : "FORMAT", kindError.Message) : new Check("FORMAT", null);
        if (strings.NotEmpty is { } notEmpty)
            writer.Line($"if (string.IsNullOrWhiteSpace(value)) return {Names.NewError(notEmpty, $"\"{t} must not be empty.\"")};");
        if (strings.ExactLength && strings.MinLength is { } exact)
        {
            writer.Line(
                $"if (value.Length != {exact}) return {Names.NewError(strings.LengthError, $"$\"{t} must be exactly {exact} character(s). Got {{value.Length}}.\"")};"
            );
        }
        else
        {
            if (strings.MinLength is { } min)
            {
                writer.Line(
                    $"if (value.Length < {min}) return {Names.NewError(strings.LengthError, $"$\"{t} must be at least {min} character(s). Got {{value.Length}}.\"")};"
                );
            }

            if (strings.MaxLength is { } max)
            {
                writer.Line(
                    $"if (value.Length > {max}) return {Names.NewError(strings.LengthError, $"$\"{t} must be at most {max} character(s). Got {{value.Length}}.\"")};"
                );
            }
        }

        if (strings.AllowedSchemes.Length > 0)
        {
            var schemes = string.Join(" || ", strings.AllowedSchemes.Select(z => $"string.Equals(uri.Scheme, {Names.Literal(z)}, global::System.StringComparison.OrdinalIgnoreCase)"));
            var names = string.Join("/", strings.AllowedSchemes.Select(z => z.ToUpperInvariant()));
            writer.Line($"if (!global::System.Uri.TryCreate(value, global::System.UriKind.Absolute, out var uri) || !({schemes}))");
            writer.Line($"    return {Names.NewError(formatError, $"\"{t} must be an absolute {names} URL.\"")};");
        }

        switch (strings.Registry)
        {
            case "Country":
                writer.Line($"if (!{Names.Primitives}.Registries.Iso3166Countries.Codes.Contains(value))");
                writer.Line($"    return {Names.NewError(formatError, $"$\"{t} must be an ISO 3166-1 alpha-2 country code. Got '{{value}}'.\"")};");
                break;
            case "Currency":
                writer.Line($"if (!{Names.Primitives}.Registries.Iso4217Currencies.Codes.Contains(value))");
                writer.Line($"    return {Names.NewError(formatError, $"$\"{t} must be an ISO 4217 currency code. Got '{{value}}'.\"")};");
                break;
            default:
                break;
        }

        for (var i = 0; i < strings.Regexes.Length; i++)
        {
            var regex = strings.Regexes[i];
            writer.Line("try");
            writer.Open();
            writer.Line($"if (!Pattern{i}.IsMatch(value)) return {Names.NewError(regex.Error, $"\"{t} has an invalid format.\"")};");
            writer.Close();
            writer.Line("catch (global::System.Text.RegularExpressions.RegexMatchTimeoutException)");
            writer.Open();
            writer.Line($"return {Names.NewError(regex.Error, $"\"{t} has an invalid format.\"")};");
            writer.Close();
        }

        if (strings.Registry == "Iban")
            writer.Line($"if (!IbanChecksumIsValid(value)) return {Names.NewError(formatError, $"\"{t} must be an IBAN with a valid check digits.\"")};");
    }

    private static void IbanChecksum(SourceWriter writer)
    {
        writer.Line("// ISO 13616: move the first four characters to the end, turn letters into numbers (A = 10), and the number mod 97 is 1.");
        writer.Line("private static bool IbanChecksumIsValid(string value)");
        writer.Open();
        writer.Line("var remainder = 0;");
        writer.Line("for (var i = 0; i < value.Length; i++)");
        writer.Open();
        writer.Line("var c = value[(i + 4) % value.Length];");
        writer.Line("var digits = c is >= '0' and <= '9' ? c - '0' : c is >= 'A' and <= 'Z' ? c - 'A' + 10 : -1;");
        writer.Line("if (digits < 0) return false;");
        writer.Line("remainder = digits >= 10 ? (remainder * 100 + digits) % 97 : (remainder * 10 + digits) % 97;");
        writer.Close();
        writer.Line("return remainder == 1;");
        writer.Close();
        writer.Line();
    }

    private static void RangeChecks(SourceWriter writer, PrimitiveModel model, RangeRule range)
    {
        var t = model.Name;
        var v = model.Value!;
        var value = v.Ordered("value");
        if (range.Min is { } min)
        {
            writer.Line(
                $"if ({value}.CompareTo({v.Ordered("(" + min + ")")}) {( range.MinExclusive ? "<=" : "<" )} 0) return {Names.NewError(range.Error, $"$\"{t} must be {( range.MinExclusive ? "greater than" : "at least" )} {range.MinText}. Got {{value}}.\"")};"
            );
        }

        if (range.Max is { } max)
        {
            writer.Line(
                $"if ({value}.CompareTo({v.Ordered("(" + max + ")")}) {( range.MaxExclusive ? ">=" : ">" )} 0) return {Names.NewError(range.Error, $"$\"{t} must be {( range.MaxExclusive ? "less than" : "at most" )} {range.MaxText}. Got {{value}}.\"")};"
            );
        }
    }

    private static void DateChecks(SourceWriter writer, PrimitiveModel model, DateRules dates, ClockInfo? clock)
    {
        var t = model.Name;
        var v = model.Value!;
        if (dates.WeekdaysOnly)
        {
            var (saturday, sunday) = v.IsNodaTime
                ? ("global::NodaTime.IsoDayOfWeek.Saturday", "global::NodaTime.IsoDayOfWeek.Sunday")
                : ("global::System.DayOfWeek.Saturday", "global::System.DayOfWeek.Sunday");
            writer.Line($"if (value.DayOfWeek == {saturday} || value.DayOfWeek == {sunday}) return {Names.NewError(dates.Error, $"\"{t} must be a weekday.\"")};");
        }

        if (clock is null) return;
        writer.Line($"var now = {Now(v, clock.Name)};");
        var value = v.Ordered("value");
        if (dates.PastOnly) writer.Line($"if ({value}.CompareTo(now) > 0) return {Names.NewError(dates.Error, $"\"{t} must be in the past.\"")};");
        if (dates.FutureOnly) writer.Line($"if ({value}.CompareTo(now) <= 0) return {Names.NewError(dates.Error, $"\"{t} must be in the future.\"")};");
        if (dates.MaxAge is { } age)
        {
            var oldest = v.IsNodaTime ? $"now.PlusYears(-{age})" : $"now.AddYears(-{age})";
            writer.Line($"if (value.CompareTo({oldest}) < 0) return {Names.NewError(dates.Error, $"\"{t} must be at most {age} years ago.\"")};");
        }
    }

    private static void StrongId(SourceWriter writer, PrimitiveModel model)
    {
        var t = model.Name;
        var v = model.Value!;
        writer.Line("/// <summary>Creates a new ID." + ( v.Category == ValueCategory.Guid ? $" It is a version {model.GuidVersion} Guid." : " Only an ID of a Guid can be created." ) + "</summary>");
        if (v.Category == ValueCategory.Guid)
        {
            writer.Line($"public static {t} Create() => new({( model.GuidVersion == 4 ? "global::System.Guid.NewGuid()" : "global::System.Guid.CreateVersion7()" )});");
        }
        else
        {
            writer.Line(
                $"public static {t} Create() => throw new global::System.NotSupportedException(\"{t} holds a {v.Display}: only a strong ID of a Guid can be created.\");"
            );
        }

        writer.Line();
        writer.Line("/// <summary>The empty ID.</summary>");
        writer.Line(
            model.IsStruct
                ? $"public static {t} Empty => default;"
                : $"public static {t} Empty {{ get; }} = new({( v.IsString ? "string.Empty" : v.Category == ValueCategory.Guid ? "global::System.Guid.Empty" : "0" )});"
        );
        writer.Line();
    }

    private static void DateHelpers(SourceWriter writer, PrimitiveModel model, ClockInfo? clock)
    {
        var v = model.Value!;
        var (clockType, clockName, clockDefault) = v.IsNodaTime
            ? ("global::NodaTime.IClock", "clock", "global::NodaTime.SystemClock.Instance")
            : ("global::System.TimeProvider", "timeProvider", "global::System.TimeProvider.System");
        if (Now(v, "c") is null) return;
        var today = Now(v, $"({clockName} ?? {clockDefault})");
        switch (model.Kind)
        {
            case "BirthDate":
                writer.Line("/// <summary>The age in whole years, today.</summary>");
                writer.Line($"public int GetAge({clockType}? {clockName} = null)");
                writer.Open();
                writer.Line($"var today = {today};");
                if (v.IsNodaTime)
                {
                    writer.Line("return global::NodaTime.Period.Between(Value, today, global::NodaTime.PeriodUnits.Years).Years;");
                }
                else
                {
                    writer.Line("var age = today.Year - Value.Year;");
                    writer.Line("return Value > today.AddYears(-age) ? age - 1 : age;");
                }

                writer.Close();
                writer.Line();
                break;
            case "ExpirationDate":
                writer.Line("/// <summary>Whether the date has passed.</summary>");
                writer.Line($"public bool IsExpired({clockType}? {clockName} = null) => Value.CompareTo({today}) <= 0;");
                writer.Line();
                writer.Line("/// <summary>The number of days until the date; negative when it has passed.</summary>");
                writer.Line(
                    v.IsNodaTime
                        ? $"public int DaysUntilExpiration({clockType}? {clockName} = null) => global::NodaTime.Period.DaysBetween({today}, Value);"
                        : $"public int DaysUntilExpiration({clockType}? {clockName} = null) => Value.DayNumber - {today}.DayNumber;"
                );
                writer.Line();
                break;
            default:
                break;
        }

        _ = clock;
    }

    private static void Parsing(SourceWriter writer, PrimitiveModel model)
    {
        var t = model.Name;
        var v = model.Value!;
        var vt = v.Display;
        var result = $"[{Names.MaybeNullWhen}(false)] out {t} result";
        writer.Line("/// <inheritdoc />");
        writer.Line($"public static {t} Parse(string s, global::System.IFormatProvider? provider) =>");
        writer.Line($"    TryParse(s, provider, out var result) ? result : throw new global::System.FormatException($\"'{{s}}' is not a valid {t}.\");");
        writer.Line();
        writer.Line("/// <inheritdoc />");
        writer.Line($"public static bool TryParse([{Names.NotNullWhen}(true)] string? s, global::System.IFormatProvider? provider, {result})");
        writer.Open();
        writer.Line("if (s is not null)");
        writer.Open();
        if (v.IsString)
        {
            writer.Line("if (TryCreate(s, out var created, out _))");
        }
        else if (v.NodaPattern is { } pattern)
        {
            writer.Line($"if ({pattern}.Parse(s) is {{ Success: true }} parsed && TryCreate(parsed.Value, out var created, out _))");
        }
        else
        {
            writer.Line($"if ({vt}.TryParse(s, provider, out var value) && TryCreate(value, out var created, out _))");
        }

        writer.Open();
        writer.Line("result = created;");
        writer.Line("return true;");
        writer.Close();
        writer.Close();
        writer.Line();
        writer.Line("result = default;");
        writer.Line("return false;");
        writer.Close();
        writer.Line();

        writer.Line("/// <inheritdoc />");
        writer.Line($"public static {t} Parse(global::System.ReadOnlySpan<char> s, global::System.IFormatProvider? provider) =>");
        writer.Line($"    TryParse(s, provider, out var result) ? result : throw new global::System.FormatException($\"'{{s.ToString()}}' is not a valid {t}.\");");
        writer.Line();
        writer.Line("/// <inheritdoc />");
        writer.Line($"public static bool TryParse(global::System.ReadOnlySpan<char> s, global::System.IFormatProvider? provider, {result})");
        if (v.IsSpanParsable && !v.IsString)
        {
            writer.Open();
            writer.Line($"if ({vt}.TryParse(s, provider, out var value) && TryCreate(value, out var created, out _))");
            writer.Open();
            writer.Line("result = created;");
            writer.Line("return true;");
            writer.Close();
            writer.Line();
            writer.Line("result = default;");
            writer.Line("return false;");
            writer.Close();
        }
        else
        {
            writer.Line("    => TryParse(s.ToString(), provider, out result);");
        }

        writer.Line();
        writer.Line("/// <inheritdoc />");
        writer.Line($"public static {t} Parse(global::System.ReadOnlySpan<byte> utf8Text, global::System.IFormatProvider? provider) =>");
        writer.Line($"    TryParse(utf8Text, provider, out var result) ? result : throw new global::System.FormatException(\"The text is not a valid {t}.\");");
        writer.Line();
        writer.Line("/// <inheritdoc />");
        writer.Line($"public static bool TryParse(global::System.ReadOnlySpan<byte> utf8Text, global::System.IFormatProvider? provider, {result})");
        if (v.IsUtf8SpanParsable && !v.IsString)
        {
            writer.Open();
            writer.Line($"if ({vt}.TryParse(utf8Text, provider, out var value) && TryCreate(value, out var created, out _))");
            writer.Open();
            writer.Line("result = created;");
            writer.Line("return true;");
            writer.Close();
            writer.Line();
            writer.Line("result = default;");
            writer.Line("return false;");
            writer.Close();
        }
        else
        {
            writer.Line("    => TryParse(global::System.Text.Encoding.UTF8.GetString(utf8Text), provider, out result);");
        }

        writer.Line();
    }

    /// <summary>
    ///     An expression that writes the value as text, the way it is parsed back.
    /// </summary>
    private static string Text(ValueTypeInfo v, string value) => v switch
    {
        { IsString: true } => value,
        { NodaPattern: { } pattern } => $"{pattern}.Format({value})",
        { IsFormattable: true } => $"{value}.ToString(null, {Names.Invariant})",
        _ => $"{value}.ToString()",
    };

    private static void Formatting(SourceWriter writer, PrimitiveModel model)
    {
        var v = model.Value!;
        var empty = model.IsStruct ? "IsDefault ? string.Empty : " : "";
        writer.Line("/// <inheritdoc />");
        writer.Line($"public override string ToString() => {empty}{Text(v, "_value")};");
        writer.Line();
        writer.Line("/// <inheritdoc />");
        writer.Line(
            v.IsFormattable && !v.IsString
                ? $"public string ToString(string? format, global::System.IFormatProvider? formatProvider) => {empty}_value.ToString(format, formatProvider);"
                : "public string ToString(string? format, global::System.IFormatProvider? formatProvider) => ToString();"
        );
        writer.Line();
        writer.Line("/// <inheritdoc />");
        writer.Line(
            "public bool TryFormat(global::System.Span<char> destination, out int charsWritten, global::System.ReadOnlySpan<char> format, global::System.IFormatProvider? provider)"
        );
        writer.Open();
        if (model.IsStruct)
        {
            writer.Line("if (IsDefault)");
            writer.Open();
            writer.Line("charsWritten = 0;");
            writer.Line("return true;");
            writer.Close();
        }

        if (v.IsSpanFormattable && !v.IsString)
        {
            writer.Line("return FormatValue(_value, destination, out charsWritten, format, provider);");
        }
        else
        {
            writer.Line("var text = ToString(format.Length == 0 ? null : format.ToString(), provider);");
            writer.Line("if (global::System.MemoryExtensions.AsSpan(text).TryCopyTo(destination))");
            writer.Open();
            writer.Line("charsWritten = text.Length;");
            writer.Line("return true;");
            writer.Close();
            writer.Line("charsWritten = 0;");
            writer.Line("return false;");
        }

        writer.Close();
        writer.Line();
        writer.Line("/// <inheritdoc />");
        writer.Line(
            "public bool TryFormat(global::System.Span<byte> utf8Destination, out int bytesWritten, global::System.ReadOnlySpan<char> format, global::System.IFormatProvider? provider)"
        );
        writer.Open();
        if (model.IsStruct)
        {
            writer.Line("if (IsDefault)");
            writer.Open();
            writer.Line("bytesWritten = 0;");
            writer.Line("return true;");
            writer.Close();
        }

        if (v.IsUtf8SpanFormattable && !v.IsString)
        {
            writer.Line("return FormatUtf8Value(_value, utf8Destination, out bytesWritten, format, provider);");
        }
        else
        {
            writer.Line("var text = ToString(format.Length == 0 ? null : format.ToString(), provider);");
            writer.Line("if (global::System.Text.Encoding.UTF8.GetMaxByteCount(text.Length) <= utf8Destination.Length || global::System.Text.Encoding.UTF8.GetByteCount(text) <= utf8Destination.Length)");
            writer.Open();
            writer.Line("bytesWritten = global::System.Text.Encoding.UTF8.GetBytes(text, utf8Destination);");
            writer.Line("return true;");
            writer.Close();
            writer.Line("bytesWritten = 0;");
            writer.Line("return false;");
        }

        writer.Close();
        writer.Line();
        // Through the interfaces, which some values implement explicitly (Guid has no TryFormat that takes a provider).
        if (v.IsSpanFormattable && !v.IsString)
        {
            writer.Line(
                "private static bool FormatValue<TValue>(TValue value, global::System.Span<char> destination, out int charsWritten, global::System.ReadOnlySpan<char> format, global::System.IFormatProvider? provider)"
            );
            writer.Line("    where TValue : global::System.ISpanFormattable => value.TryFormat(destination, out charsWritten, format, provider);");
            writer.Line();
        }

        if (v.IsUtf8SpanFormattable && !v.IsString)
        {
            writer.Line(
                "private static bool FormatUtf8Value<TValue>(TValue value, global::System.Span<byte> destination, out int bytesWritten, global::System.ReadOnlySpan<char> format, global::System.IFormatProvider? provider)"
            );
            writer.Line("    where TValue : global::System.IUtf8SpanFormattable => value.TryFormat(destination, out bytesWritten, format, provider);");
            writer.Line();
        }
    }

    private static void Comparison(SourceWriter writer, PrimitiveModel model)
    {
        var t = model.Name;
        var v = model.Value!;
        var compareValues = v.IsString
            ? "string.CompareOrdinal(left._value, right._value)"
            : $"{v.Ordered("left._value")}.CompareTo({v.Ordered("right._value")})";
        writer.Line("/// <summary>Orders two values; " + ( model.IsStruct ? "the default value" : "null" ) + " comes first.</summary>");
        writer.Line($"private static int Compare({t}{( model.IsStruct ? "" : "?" )} left, {t}{( model.IsStruct ? "" : "?" )} right)");
        writer.Open();
        if (model.IsStruct)
        {
            writer.Line("if (left.IsDefault) return right.IsDefault ? 0 : -1;");
            writer.Line("if (right.IsDefault) return 1;");
        }
        else
        {
            writer.Line("if (left is null) return right is null ? 0 : -1;");
            writer.Line("if (right is null) return 1;");
        }

        writer.Line($"return {compareValues};");
        writer.Close();
        writer.Line();
        writer.Line("/// <inheritdoc />");
        writer.Line($"public int CompareTo({t}{( model.IsStruct ? "" : "?" )} other) => Compare(this, other);");
        writer.Line();
        writer.Line("/// <inheritdoc />");
        writer.Line("public int CompareTo(object? obj) => obj switch");
        writer.Open();
        writer.Line("null => 1,");
        writer.Line($"{t} other => CompareTo(other),");
        writer.Line($"_ => throw new global::System.ArgumentException(\"The object is not a {t}.\", nameof(obj)),");
        writer.Close(";");
        writer.Line();
        var nullable = model.IsStruct ? "" : "?";
        foreach (var op in new[] { "<", "<=", ">", ">=" })
        {
            writer.Line($"/// <inheritdoc />");
            writer.Line($"public static bool operator {op}({t}{nullable} left, {t}{nullable} right) => Compare(left, right) {op} 0;");
        }

        writer.Line();
    }

    private static void Conversions(SourceWriter writer, PrimitiveModel model)
    {
        var t = model.Name;
        var vt = model.Value!.Display;
        writer.Line($"/// <summary>The value of a {t}.</summary>");
        writer.Line($"public static explicit operator {vt}({t} value) => value.Value;");
        writer.Line();
        writer.Line($"/// <summary>Creates a {t}, or throws when its rules reject the value.</summary>");
        writer.Line($"public static explicit operator {t}({vt} value) => Create(value);");
        writer.Line();
    }

    private static void Operators(SourceWriter writer, PrimitiveModel model, int operations)
    {
        var t = model.Name;
        var vt = model.Value!.Display;
        writer.Line("// Every result is checked again by the rules.");
        if (( operations & 1 ) != 0) writer.Line($"public static {t} operator +({t} left, {t} right) => Create(checked(({vt})(left.Value + right.Value)));");
        if (( operations & 2 ) != 0) writer.Line($"public static {t} operator -({t} left, {t} right) => Create(checked(({vt})(left.Value - right.Value)));");
        if (( operations & 4 ) != 0) writer.Line($"public static {t} operator *({t} left, {vt} right) => Create(checked(({vt})(left.Value * right)));");
        if (( operations & 8 ) != 0) writer.Line($"public static {t} operator /({t} left, {vt} right) => Create(checked(({vt})(left.Value / right)));");
        if (( operations & 16 ) != 0) writer.Line($"public static {t} operator -({t} value) => Create(checked(({vt})(-value.Value)));");
        writer.Line();
    }

    private static void TypeConverter(SourceWriter writer, PrimitiveModel model)
    {
        var t = model.Name;
        var v = model.Value!;
        var vt = v.Display;
        writer.Line($"private sealed class {t}TypeConverter : global::System.ComponentModel.TypeConverter");
        writer.Open();
        writer.Line("public override bool CanConvertFrom(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Type sourceType) =>");
        writer.Line($"    sourceType == typeof(string) || sourceType == typeof({vt}) || base.CanConvertFrom(context, sourceType);");
        writer.Line();
        writer.Line(
            "public override object? ConvertFrom(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Globalization.CultureInfo? culture, object value) => value switch"
        );
        writer.Open();
        if (!v.IsString) writer.Line($"{vt} typed => {t}.Create(typed),");
        writer.Line($"string text => {t}.Parse(text, culture),");
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
        writer.Line($"if (value is {t} primitive)");
        writer.Open();
        writer.Line("if (destinationType == typeof(string)) return primitive.ToString();");
        writer.Line($"if (destinationType == typeof({vt})) return primitive.Value;");
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
        writer.Line($"private sealed class {t}JsonConverter : {Names.Json}.Serialization.JsonConverter<{t}>");
        writer.Open();
        writer.Line($"public override {t} Read(ref {Names.Json}.Utf8JsonReader reader, global::System.Type typeToConvert, {Names.Json}.JsonSerializerOptions options)");
        writer.Open();
        if (model.IsStruct) writer.Line($"if (reader.TokenType == {Names.Json}.JsonTokenType.Null) return default;");
        writer.Line($"var value = {PrimitiveEmitter.ReadJson(v)};");
        writer.Line($"return TryCreate(value, out var result, out var error) ? result : throw new {Names.Json}.JsonException($\"[{{error.Code}}] {{error.Message}}\");");
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

        writer.Line(PrimitiveEmitter.WriteJson(v, "value._value"));
        writer.Close();
        writer.Line();
        writer.Line(
            $"public override {t} ReadAsPropertyName(ref {Names.Json}.Utf8JsonReader reader, global::System.Type typeToConvert, {Names.Json}.JsonSerializerOptions options) =>"
        );
        writer.Line($"    TryParse(reader.GetString(), {Names.Invariant}, out var result) ? result : throw new {Names.Json}.JsonException(\"The property name is not a valid {t}.\");");
        writer.Line();
        writer.Line($"public override void WriteAsPropertyName({Names.Json}.Utf8JsonWriter writer, {t} value, {Names.Json}.JsonSerializerOptions options) =>");
        writer.Line("    writer.WritePropertyName(value.ToString());");
        writer.Close();
    }
}
