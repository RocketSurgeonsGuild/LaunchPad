namespace Rocket.Surgery.LaunchPad.Api.Generators;

internal enum RequestKind
{
    Infer = 0,
    Unary = 1,
    Void = 2,
    Stream = 3,
}

internal enum BindingSource
{
    Route,
    Query,
    Header,
    Body,
    Service,
    Ignored,
}

/// <summary>How the server reaches the mediator.</summary>
internal enum DispatchKind
{
    /// <summary>A sender service with Send/CreateStream methods (MediatR, Mediator, ChannelMediator, Foundatio, ZeroAlloc).</summary>
    Sender,

    /// <summary>The closed handler interface is injected and called directly (Immediate.Handlers).</summary>
    HandlerInterface,
}

internal sealed record RequestKindModel(
    string? RequestInterface,
    RequestKind Kind,
    int ResponseTypeArgumentIndex,
    string? HandlerInterface,
    string HandleMethod
);

internal sealed record ProfileModel(
    string Name,
    DispatchKind Dispatch,
    string? SenderType,
    string SendMethod,
    string? StreamMethod,
    bool SendNeedsResponseTypeArgument,
    string? UnitType,
    EquatableArray<RequestKindModel> Kinds
);

/// <summary>How a server turns a route, query or header string into a member's value.</summary>
internal enum ParseKind
{
    None,
    String,
    Enum,
    Uri,

    /// <summary>The type implements <c>IParsable&lt;T&gt;</c>: <c>T.TryParse(value, CultureInfo.InvariantCulture, out var result)</c>.</summary>
    Parsable,

    /// <summary>A static <c>TryParse(string, IFormatProvider, out T)</c> outside <c>IParsable&lt;T&gt;</c>.</summary>
    TryParseWithProvider,

    /// <summary>A static <c>TryParse(string, out T)</c>.</summary>
    TryParse,
}

internal sealed record MemberModel(
    string Name,
    string WireName,
    string Type,
    string TypeWithoutNullable,
    BindingSource Source,
    bool IsNullable,
    bool IsCollection,
    bool CanAssign,
    bool CanInit,
    int ConstructorIndex,
    ParseKind ParseKind,
    string? ParseType
);

internal sealed record OperationModel(
    string Name,
    string RequestType,
    string RequestDisplayName,
    bool RequestSupportsWith,
    string? InputType,
    RequestKind Kind,
    bool Markerless,
    string? ResponseType,
    string? ResponseTypeNotNull,
    bool ResponseNullable,
    bool ResponseIsNullableValueType,
    bool ResponseIsUnit,
    EquatableArray<string> ResponseMembers,
    HttpVerb Verb,
    int SuccessStatusCode,
    string Route,
    EquatableArray<string> RouteTokens,
    EquatableArray<MemberModel> Members,
    bool CanConstruct,
    string? Summary,
    string? Description,
    string EndpointName,
    EquatableArray<string> Tags,
    EquatableArray<string> Policies,
    EquatableArray<string> Roles,
    EquatableArray<string> AuthenticationSchemes,
    bool AllowAnonymous,
    string? Version,
    string? HandlerInterface,
    string? CreatedLocationRequest,
    string? CreatedLocationRoute,
    EquatableArray<string> CreatedLocationTokens,
    string GraphQLField,
    bool GraphQLIsQuery,
    string? ResponseGraphQLName,
    string WireGraphQLName,
    bool WireNeedsGraphQLName,
    bool RequestIsMessage,
    bool ResponseIsMessage,
    bool RequestIsStruct,
    bool ResponseIsStruct,
    LocationInfo? Location
)
{
    public bool HasBody => Verb is HttpVerb.Post or HttpVerb.Put or HttpVerb.Patch;

    /// <summary>The type the transport carries: the input type when one is declared, otherwise the request.</summary>
    public string WireType => InputType ?? RequestType;
}

internal sealed record HostModel(
    string? Namespace,
    string Name,
    string FullyQualifiedName,
    string Accessibility,
    HostKind Kind,
    string Group,
    string RoutePrefix,
    string? Version,
    bool DeclaresBaseType,
    string? DeclaredRoute,
    bool DeclaresApiController,
    bool DeclaresGraphQLType,
    bool HasApiVersioning,
    bool CanExportProto,
    ProfileModel? Profile,
    EquatableArray<OperationModel> Operations,
    EquatableArray<DiagnosticInfo> Diagnostics,
    LocationInfo? Location
)
{
    /// <summary>
    ///     The route the host's operations live under, e.g. api/rockets. A controller's own <c>[Route]</c> replaces prefix and group; its
    ///     <c>[controller]</c> token is the host's noun, as MVC fills it in.
    /// </summary>
    public string BaseRoute => DeclaredRoute is { } declared
        ? declared.Replace("[controller]", Naming.HostNoun(Name)).Trim('/')
        : RouteText.Join(RoutePrefix, Group);
}

internal static class RouteText
{
    public static string Join(params string?[] parts) =>
        string.Join("/", parts.Where(p => p is { Length: > 0 }).Select(p => p!.Trim('/')).Where(p => p.Length > 0));

    /// <summary>Route tokens in order: {id}, {id:guid}, {id?} and {*rest} all yield the name.</summary>
    public static IEnumerable<string> Tokens(string route)
    {
        var start = -1;
        for (var i = 0; i < route.Length; i++)
        {
            if (route[i] == '{')
            {
                start = i + 1;
            }
            else if (route[i] == '}' && start >= 0)
            {
                var token = route[start..i].TrimStart('*');
                var end = token.IndexOfAny([':', '?', '=']);
                yield return end >= 0 ? token[..end] : token;
                start = -1;
            }
        }
    }
}

