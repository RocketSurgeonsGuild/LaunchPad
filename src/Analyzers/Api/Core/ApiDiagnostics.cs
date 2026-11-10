using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Every diagnostic the API generators report. One table, shared by every generator package, so IDs never collide.
/// </summary>
internal static class ApiDiagnostics
{
    private const string Category = "LaunchPad.Api";

    public static DiagnosticDescriptor ResponseNotFound { get; } = Error(
        "LPAD1001",
        "Operation response could not be found",
        "{0} does not match any request kind of the {1} mediator profile, and no response type is set with [ApiOperation(Response = ...)]"
    );

    public static DiagnosticDescriptor NoMediatorProfile { get; } = Error(
        "LPAD1002",
        "No mediator profile",
        "Operation host {0} needs a mediator profile: {1}"
    );

    public static DiagnosticDescriptor RouteTokenWithoutMember { get; } = Error(
        "LPAD1003",
        "Route token has no matching member",
        "The route '{0}' of operation {1} has the token '{2}', but {3} has no member with that name"
    );

    public static DiagnosticDescriptor RouteMemberNotInRoute { get; } = Error(
        "LPAD1004",
        "Route member is not in the route",
        "{0}.{1} is marked [ApiRoute] but the route '{2}' has no '{{{3}}}' token"
    );

    public static DiagnosticDescriptor ComplexMemberNotBindable { get; } = Error(
        "LPAD1005",
        "Complex member bound from the route or query string",
        "{0}.{1} of type {2} cannot be bound from the {3}; use a body verb, [ApiBody] or [ApiIgnore]"
    );

    public static DiagnosticDescriptor RequestNotConstructible { get; } = Error(
        "LPAD1006",
        "Request cannot be constructed",
        "{0} cannot be built for {1}: member {2} is neither a constructor parameter nor settable"
    );

    public static DiagnosticDescriptor MemberNotParsable { get; } = Error(
        "LPAD1007",
        "Route or query member is not parsable",
        "{0}.{1} of type {2} is bound from the {3} but the type has no TryParse method and does not implement IParsable<T>"
    );

    public static DiagnosticDescriptor DuplicateRoute { get; } = Error(
        "LPAD1008",
        "Duplicate route",
        "{0} {1} is declared by both {2} and {3}"
    );

    public static DiagnosticDescriptor DuplicateGraphQLField { get; } = Error(
        "LPAD1009",
        "Duplicate GraphQL field",
        "The GraphQL field '{0}' is declared twice on {1}"
    );

    public static DiagnosticDescriptor HostMustBePartial { get; } = Error(
        "LPAD1010",
        "Operation host must be partial",
        "Operation host {0} must be {1}"
    );




    public static DiagnosticDescriptor GrpcSerialization { get; } = Warning(
        "LPAD1014",
        "gRPC type lacks serialization attributes",
        "{0} is sent over gRPC but has no [DataContract] or [ProtoContract] with ordered members"
    );

    public static DiagnosticDescriptor StreamOnGraphQL { get; } = Warning(
        "LPAD1015",
        "Stream operation on a GraphQL host",
        "{0} is a stream operation and is skipped on GraphQL host {1}"
    );


    public static DiagnosticDescriptor FeatureSkipped { get; } = Info(
        "LPAD1016",
        "Feature skipped",
        "{0} is skipped on {1}: {2}"
    );

    public static DiagnosticDescriptor HostTransportUnknown { get; } = Error(
        "LPAD1017",
        "Operation host transport cannot be inferred",
        "The transport of operation host {0} cannot be inferred: {1}. A host is a ControllerBase subclass or [ExtendObjectType] type, or is named *Controller, *Endpoints/*Api (static), *Service or *Queries/*Mutations."
    );


    public static DiagnosticDescriptor WrongGraphQLHostKind { get; } = Error(
        "LPAD1019",
        "Operation is on the wrong GraphQL host kind",
        "{0} is a GraphQL {1} but {2} is a {3} host"
    );

    public static DiagnosticDescriptor InputWithoutCreate { get; } = Error(
        "LPAD1020",
        "Input type has no Create method",
        "Input type {0} needs a public parameterless Create() method returning {1}"
    );

    public static DiagnosticDescriptor HostSettingNotApplicable { get; } = Warning(
        "LPAD1021",
        "Operation host setting does not apply",
        "[OperationHost({0})] has no effect on {1}, which is a {2} host"
    );


    public static DiagnosticDescriptor CreatedLocationInvalid { get; } = Warning(
        "LPAD1023",
        "A created resource has no Location",
        "{0} answers 201 Created without a Location: {1}"
    );

    public static DiagnosticDescriptor VersionNotMapped { get; } = Warning(
        "LPAD1024",
        "API version is not mapped",
        "{0} has API version {1}, but Asp.Versioning isn't referenced, so the server doesn't map it; reference Asp.Versioning.Http for Minimal API hosts or Asp.Versioning.Mvc for controllers"
    );

    public static DiagnosticDescriptor PatchHandlerWithoutSender { get; } = Error(
        "LPAD1025",
        "Patch handler has no mediator sender",
        "{0} handles the patch operation {1}, but has no constructor parameter, field or property of type {2} to send the patched request through"
    );

    public static DiagnosticDescriptor PatchHandlerNotPartial { get; } = Error(
        "LPAD1026",
        "Patch handler is in a type that is not partial",
        "The handle method of {0} is generated, so {1} must be partial"
    );

    public static DiagnosticDescriptor PatchHandlerNotAsync { get; } = Error(
        "LPAD1027",
        "Patch handler interface does not return a task",
        "The handle method of {0} cannot be generated: the handler interface returns {1}, not a Task<T> or ValueTask<T>"
    );

    // LPAD1030 and up: JSON metadata. The ids between are left to the union work (#3147, #3148).
    public static DiagnosticDescriptor JsonMetadataMissing { get; } = Warning(
        "LPAD1030",
        "JSON type has no source generated metadata",
        "{0} is read or written as JSON by {1}.{2}, but no JsonSerializerContext in this project or its references declares it, so the endpoint fails once reflection is off; add [JsonSerializable(typeof({0}))] to a context (a nested type needs a TypeInfoPropertyName) and chain the context with AddJsonTypeInfoResolver or ConfigureHttpJsonOptions"
    );

    public static IReadOnlyList<DiagnosticDescriptor> All { get; } =
    [
        ResponseNotFound, NoMediatorProfile, RouteTokenWithoutMember, RouteMemberNotInRoute, ComplexMemberNotBindable, RequestNotConstructible,
        MemberNotParsable, DuplicateRoute, DuplicateGraphQLField, HostMustBePartial, GrpcSerialization, StreamOnGraphQL, FeatureSkipped, HostTransportUnknown, WrongGraphQLHostKind, InputWithoutCreate, HostSettingNotApplicable, CreatedLocationInvalid, VersionNotMapped,
        PatchHandlerWithoutSender, PatchHandlerNotPartial, PatchHandlerNotAsync,
        JsonMetadataMissing,
    ];

    public static DiagnosticDescriptor Find(string id) => All.First(d => d.Id == id);

    private static DiagnosticDescriptor Error(string id, string title, string message) => new(id, title, message, Category, DiagnosticSeverity.Error, true);

    private static DiagnosticDescriptor Warning(string id, string title, string message) => new(id, title, message, Category, DiagnosticSeverity.Warning, true);

    private static DiagnosticDescriptor Info(string id, string title, string message) => new(id, title, message, Category, DiagnosticSeverity.Info, true);
}
