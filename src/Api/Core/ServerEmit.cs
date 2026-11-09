using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Rocket.Surgery.LaunchPad.Api.Generators.ApiSyntax;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>Emit helpers every server transport shares: handler parameters, building the request, and the Location of a created resource.</summary>
internal static class ServerEmit
{
    public static string Verb(HttpVerb verb) => verb switch
    {
        HttpVerb.Get => "GET",
        HttpVerb.Post => "POST",
        HttpVerb.Put => "PUT",
        HttpVerb.Patch => "PATCH",
        HttpVerb.Delete => "DELETE",
        _ => throw new ArgumentOutOfRangeException(nameof(verb)),
    };

    public static string KindName(RequestKind kind) => kind switch
    {
        RequestKind.Unary => "unary",
        RequestKind.Void => "void",
        RequestKind.Stream => "stream",
        RequestKind.Infer => "unknown",
        _ => throw new ArgumentOutOfRangeException(nameof(kind)),
    };

    public static string Parameter(MemberModel member) => "@" + Naming.CamelCase(member.Name);

    /// <summary>
    ///     The handler's or action's parameters as syntax: route, query (no body), header and service members, the body, then the sender
    ///     (when <paramref name="service" /> is given) and the cancellation token.
    /// </summary>
    public static IEnumerable<(ParameterSyntax Parameter, bool HasTryParse, bool IsOptional)> Parameters(OperationModel operation, string? service, string cancellationToken)
    {
        foreach (var member in operation.Members)
        {
            var binding = member.Source switch
            {
                BindingSource.Route => "FromRoute",
                BindingSource.Query when !operation.HasBody => "FromQuery",
                BindingSource.Header => "FromHeader",
                BindingSource.Service => "FromServices",
                BindingSource.Query or BindingSource.Body or BindingSource.Ignored => null,
                _ => null,
            };
            if (binding is null) continue;
            var attribute = member.Source == BindingSource.Service
                ? Attribute("global::Microsoft.AspNetCore.Mvc.FromServices")
                : Attribute("global::Microsoft.AspNetCore.Mvc." + binding, Arg("Name", Literal(member.WireName)));
            yield return (ApiSyntax.Parameter(Type(member.Type), Parameter(member), attribute), member.Source != BindingSource.Service && member.ParseKind != ParseKind.String, member.Source != BindingSource.Service && member.IsNullable);
        }

        if (operation.HasBody) yield return (ApiSyntax.Parameter(Type(operation.WireType), "__body", Attribute("global::Microsoft.AspNetCore.Mvc.FromBody")), false, false);
        if (service is { }) yield return (ApiSyntax.Parameter(Type(service), ServerDispatch.SenderParameter, Attribute("global::Microsoft.AspNetCore.Mvc.FromServices")), false, false);
        yield return (ApiSyntax.Parameter(Type(CancellationTokenType), cancellationToken), false, false);
    }

    /// <summary>
    ///     The statements that build <c>__request</c> from the bound parameters: constructor arguments and initializers for GET/DELETE, the body
    ///     with route, header and service values applied on top for POST/PUT/PATCH.
    /// </summary>
    public static IEnumerable<StatementSyntax> BuildRequest(OperationModel operation)
    {
        var bound = operation.Members.Where(m => m.Source is BindingSource.Route or BindingSource.Header or BindingSource.Service || !operation.HasBody && m.Source == BindingSource.Query).ToArray();
        if (operation.HasBody)
        {
            yield return Var("__wire", Id("__body"));
            if (bound.Length > 0 && operation.RequestSupportsWith)
            {
                yield return Statement(Assign(Id("__wire"), With(Id("__wire"), bound.Select(m => (m.Name, (ExpressionSyntax)Id(Parameter(m)))))));
            }
            else
            {
                foreach (var member in bound) yield return Statement(Assign(Member(Id("__wire"), member.Name), Id(Parameter(member))));
            }
        }
        else
        {
            var arguments = operation.Members.Where(m => m.ConstructorIndex >= 0)
                                     .OrderBy(m => m.ConstructorIndex)
                                     .Select(m => bound.Contains(m) ? Id(Parameter(m)) : NullForgiving(Default));
            var initializers = bound.Where(m => m.ConstructorIndex < 0).Select(m => (m.Name, (ExpressionSyntax)Id(Parameter(m))));
            yield return Var("__wire", New(Type(operation.WireType), arguments, initializers));
        }

        yield return Var("__request", operation.InputType is { } ? Invoke(Member(Id("__wire"), "Create")) : Id("__wire"));
    }

    /// <summary>The Location of a created resource: the target operation's full route, filled from the response's members.</summary>
    public static ExpressionSyntax LocationSyntax(string route, OperationModel operation, string generatedNamespace, ExpressionSyntax response)
    {
        var path = "/" + route.Trim('/');
        var tokens = RouteText.Tokens(route).ToArray();
        var parts = new List<ExpressionSyntax>();
        var position = 0;
        for (var index = 0; index < tokens.Length; index++)
        {
            var start = path.IndexOf('{', position);
            var end = path.IndexOf('}', start);
            if (start > position) parts.Add(Literal(path[position..start]));
            var value = Invoke(Member(generatedNamespace + ".ApiFormat", "Value"), Member(response, operation.CreatedLocationTokens[index]));
            parts.Add(Invoke(Member("global::System.Uri", "EscapeDataString"), value));
            position = end + 1;
        }

        if (position < path.Length) parts.Add(Literal(path[position..]));
        return Concat(parts);
    }

    /// <summary>
    ///     What a server does with a union response in <c>__response</c> (or a stream item in <paramref name="response" />): its value case (<c>__value</c>, null for a void value case), then each
    ///     error case (<c>__error0</c>, …), then a null value (a nullable value case, or a default union) through <paramref name="onNull" />. A
    ///     union holding none of its cases is a bug in the handler, so it throws.
    /// </summary>
    public static IEnumerable<StatementSyntax> UnionResult(
        OperationModel operation,
        Func<ExpressionSyntax?, IEnumerable<StatementSyntax>> onValue,
        Func<UnionCaseModel, ExpressionSyntax, IEnumerable<StatementSyntax>> onError,
        Func<IEnumerable<StatementSyntax>>? onNull,
        string response = "__response"
    )
    {
        var union = operation.Union!;
        // A nullable union reads its case with ?. (on a Nullable<U>, that reaches U's own Value), so null falls through to onNull.
        var target = union.ValueInterface is { } contract ? Parenthesized(Cast(Type(union.Nullable ? contract + "?" : contract), Id(response))) : Id(response);
        var value = union.Nullable ? ConditionalMember(target, "Value") : Member(target, "Value");
        yield return Var("__case", value);
        yield return If(
            IsType(Id("__case"), Type(operation.ResponseTypeNotNull!), "__value"),
            Block(onValue(operation.Kind == RequestKind.Void ? null : Id("__value")))
        );
        // Pattern variables of an if's condition live in the enclosing block, so each case gets its own.
        for (var index = 0; index < union.Errors.Length; index++)
        {
            var error = union.Errors[index];
            var name = $"__error{index}";
            yield return If(IsType(Id("__case"), Type(error.Type), name), Block(onError(error, Id(name))));
        }

        if (onNull is { }) yield return If(IsNull(Id("__case")), Block(onNull()));
        yield return Throw(
            New(
                Type("global::System.InvalidOperationException"),
                Literal($"{operation.RequestDisplayName} returned a {union.Type.Replace("global::", "")} that holds none of its cases.")
            )
        );
    }

    /// <summary>
    ///     An error case's members, as the arguments of the generated problem helpers: <c>"id", __error.Id, …</c> after the status, case name
    ///     and transport code.
    /// </summary>
    public static IEnumerable<ExpressionSyntax> ErrorMembers(UnionCaseModel error, ExpressionSyntax value) =>
        error.Members.SelectMany(m => new[] { Literal(m.WireName), Member(value, m.Name) });

    /// <summary>The version an operation is served under: its own, or its host's.</summary>
    public static string? Version(HostModel host, OperationModel operation) => operation.Version ?? host.Version;

    /// <summary>Every version a host's operations are served under, in order of appearance.</summary>
    public static IReadOnlyList<string> Versions(HostModel host) => host.Operations.Select(o => Version(host, o)).OfType<string>().Distinct().ToArray();

    /// <summary>LPAD1024 for a host whose operations have versions that nothing maps.</summary>
    public static void ReportUnmappedVersions(Microsoft.CodeAnalysis.SourceProductionContext output, HostModel host)
    {
        if (host.HasApiVersioning) return;
        foreach (var operation in host.Operations.Where(o => Version(host, o) is { }))
        {
            output.ReportDiagnostic(DiagnosticInfo.Create(ApiDiagnostics.VersionNotMapped, operation.Location, operation.Name, Version(host, operation)!).ToDiagnostic());
        }
    }

    /// <summary><c>global::Asp.Versioning.ApiVersionParser.Default.Parse("1.0")</c>.</summary>
    public static ExpressionSyntax ApiVersion(string version) => Invoke(Member(Member("global::Asp.Versioning.ApiVersionParser", "Default"), "Parse"), Literal(version));
}
