namespace Rocket.Surgery.LaunchPad.Analyzers.Api.Core;

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
    /// <summary>A sender service with Send/CreateStream methods (Mediator, ChannelMediator, Foundatio, ZeroAlloc).</summary>
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
    string? ParseType,
    EquatableArray<SchemaRuleModel> Rules,
    string? Summary
);

/// <summary>A property's camel-cased name on the wire and its XML documentation summary, the description of its schema.</summary>
internal sealed record DescriptionModel(string WireName, string Summary);

/// <summary>
///     One ZeroAlloc.Validation rule of a member as the OpenAPI schema states it, read at compile time: the <c>ApiSchemaRule</c> method the
///     generated transformer calls (<c>Required</c>, <c>MinLength</c>, <c>Minimum</c>, …) and its arguments, already formatted.
/// </summary>
internal sealed record SchemaRuleModel(string Method, EquatableArray<string> Arguments);

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
    UnionModel? Union,
    LocationInfo? Location,
    string? WireSummary = null,
    string? ResponseSummary = null,
    EquatableArray<DescriptionModel> ResponseDescriptions = default,
    RpcModel? Rpc = null,
    // Authorization (docs/design/authorization.md), where the server has LaunchPad's edge: the request, whose plan states the scopes of the
    // endpoint; the policies of the request the endpoint names (they are in Policies too, for the transports that read those); whether a
    // group of alternatives leaves the endpoint requiring only a caller; the scopes as alternatives; whether the GraphQL schema states them.
    string? RequestPolicies = null,
    EquatableArray<string> RequestPolicyNames = default,
    bool RequestAuthenticated = false,
    EquatableArray<EquatableArray<string>> ScopeAlternatives = default,
    bool GraphQLScopes = false
)
{
    public bool HasBody => Verb is HttpVerb.Post or HttpVerb.Put or HttpVerb.Patch;

    /// <summary>The operation names a policy, roles or authentication schemes and doesn't allow anonymous callers: it can answer 401 and 403.</summary>
    public bool RequiresAuthorization => !AllowAnonymous && ( !Policies.IsEmpty || !Roles.IsEmpty || !AuthenticationSchemes.IsEmpty || RequestAuthenticated );

    /// <summary>The type the transport carries: the input type when one is declared, otherwise the request.</summary>
    public string WireType => InputType ?? RequestType;

    /// <summary>The type the handler returns: the union for a union response, otherwise the response.</summary>
    public string? DispatchResponseType => Union?.Type ?? ResponseType;

    /// <summary>The item type a stream operation's handler produces: the union for a stream of unions, otherwise the response.</summary>
    public string? StreamItemType => Union is { } union ? union.Nullable ? union.Type + "?" : union.Type : ResponseTypeNotNull;
}

/// <summary>
///     The rpc of a proto-first host's Grpc.Tools service base that an operation implements: its method (overridden by the generated host), its
///     proto messages and whether it streams its responses.
/// </summary>
internal sealed record RpcModel(string Method, string RequestMessage, string ResponseMessage, bool ServerStreaming);

/// <summary>
///     A ZeroAlloc.Mapping mapping the user declared: <c>[Map&lt;Source, Destination&gt;]</c> on <c>Mapper</c>, which gets <c>Mapper.Map(Source)</c>,
///     and with <c>InPlace</c> the update-in-place <c>Mapper.Map(Source, Destination)</c> the mapper declares, which sets settable properties.
/// </summary>
internal sealed record MappingModel(string Mapper, string Source, string Destination, bool DestinationIsProtoMessage, bool InPlace);

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
    bool HasOpenApi,
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

