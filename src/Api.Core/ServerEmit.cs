namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>Emit helpers every server transport shares: handler parameters, building the request, and the Location of a created resource.</summary>
internal static class ServerEmit
{
    /// <summary>
    ///     The bound parameters of a handler or action, in order: route, query (no body), header and service members, the body, then the sender
    ///     (when <paramref name="service" /> is given) and the cancellation token.
    /// </summary>
    public static IEnumerable<(string Attribute, string Type, string Name, bool HasTryParse, bool IsOptional)> HandlerParameters(
        OperationModel operation,
        string? service,
        string cancellationToken
    )
    {
        foreach (var member in operation.Members)
        {
            switch (member.Source)
            {
                case BindingSource.Route:
                    yield return ($"[global::Microsoft.AspNetCore.Mvc.FromRoute(Name = {SourceWriter.Literal(member.WireName)})]", member.Type, Parameter(member), member.ParseKind != ParseKind.String, member.IsNullable);
                    break;
                case BindingSource.Query:
                    if (!operation.HasBody) yield return ($"[global::Microsoft.AspNetCore.Mvc.FromQuery(Name = {SourceWriter.Literal(member.WireName)})]", member.Type, Parameter(member), member.ParseKind != ParseKind.String, member.IsNullable);
                    break;
                case BindingSource.Header:
                    yield return ($"[global::Microsoft.AspNetCore.Mvc.FromHeader(Name = {SourceWriter.Literal(member.WireName)})]", member.Type, Parameter(member), member.ParseKind != ParseKind.String, member.IsNullable);
                    break;
                case BindingSource.Service:
                    yield return ("[global::Microsoft.AspNetCore.Mvc.FromServices]", member.Type, Parameter(member), false, false);
                    break;
                case BindingSource.Body:
                case BindingSource.Ignored:
                    break;
                default:
                    break;
            }
        }

        if (operation.HasBody) yield return ("[global::Microsoft.AspNetCore.Mvc.FromBody]", operation.WireType, "__body", false, false);
        if (service is { }) yield return ("[global::Microsoft.AspNetCore.Mvc.FromServices]", service, ServerDispatch.SenderParameter, false, false);
        yield return ("", "global::System.Threading.CancellationToken", cancellationToken, false, false);
    }

    /// <summary>
    ///     Builds <c>__request</c> from the bound parameters: constructor arguments and initializers for GET/DELETE, the body with route, header and
    ///     service values applied on top for POST/PUT/PATCH.
    /// </summary>
    public static void WriteRequest(SourceWriter w, OperationModel operation)
    {
        var bound = operation.Members.Where(m => m.Source is BindingSource.Route or BindingSource.Header or BindingSource.Service || !operation.HasBody && m.Source == BindingSource.Query).ToArray();
        if (operation.HasBody)
        {
            w.Line("var __wire = __body;");
            if (bound.Length > 0)
            {
                if (operation.RequestSupportsWith)
                {
                    w.Line($"__wire = __wire with {{ {string.Join(", ", bound.Select(m => $"{m.Name} = {Parameter(m)}"))} }};");
                }
                else
                {
                    foreach (var member in bound) w.Line($"__wire.{member.Name} = {Parameter(member)};");
                }
            }
        }
        else
        {
            var arguments = operation.Members.Where(m => m.ConstructorIndex >= 0).OrderBy(m => m.ConstructorIndex).Select(m => bound.Contains(m) ? Parameter(m) : "default!");
            var initializers = bound.Where(m => m.ConstructorIndex < 0).Select(m => $"{m.Name} = {Parameter(m)}").ToArray();
            w.Line($"var __wire = new {operation.WireType}({string.Join(", ", arguments)}){( initializers.Length > 0 ? " { " + string.Join(", ", initializers) + " }" : "" )};");
        }

        w.Line(operation.InputType is { } ? "var __request = __wire.Create();" : "var __request = __wire;");
    }

    /// <summary>The Location of a created resource: <paramref name="baseRoute" /> plus the target route, filled from <c>__response</c>'s members.</summary>
    public static string Location(string baseRoute, string route, OperationModel operation, string generatedNamespace)
    {
        var path = "/" + RouteText.Join(baseRoute, route);
        var tokens = RouteText.Tokens(route).ToArray();
        var parts = new List<string>();
        var position = 0;
        for (var index = 0; index < tokens.Length; index++)
        {
            var start = path.IndexOf('{', position);
            var end = path.IndexOf('}', start);
            parts.Add(SourceWriter.Literal(path[position..start]));
            parts.Add($"global::System.Uri.EscapeDataString({generatedNamespace}.ApiFormat.Value(__response.{operation.CreatedLocationTokens[index]}))");
            position = end + 1;
        }

        parts.Add(SourceWriter.Literal(path[position..]));
        return string.Join(" + ", parts.Where(p => p != "\"\""));
    }

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
}
