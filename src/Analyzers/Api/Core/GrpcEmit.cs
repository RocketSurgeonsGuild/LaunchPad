using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Rocket.Surgery.LaunchPad.Api.Generators.ApiSyntax;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     The code-first gRPC contract a gRPC service host gets: the service name (the group), the operation names and the message shapes, which is
///     all protobuf-net.Grpc puts on the wire.
/// </summary>
/// <remarks>
///     protobuf-net.Grpc only carries classes. A request, response or stream item that isn't one (a struct request, a <c>bool</c> response) travels
///     in the contract's nested <c>Message&lt;T&gt;</c>, wrapped and unwrapped by the generated service. A user struct is the wrapper's
///     protobuf-net surrogate, so the wire (and the <c>.proto</c>) carries the struct's own message, as it would a class; a primitive stays
///     wrapped (<c>message Message_Boolean { bool Value = 1; }</c>).
/// </remarks>
internal static class GrpcEmit
{
    public static string ContractName(HostModel host) => $"I{host.Name}Contract";

    /// <summary>
    ///     The <c>.proto</c> package the service lives in: the group, with what a package can't hold as underscores (launch-records →
    ///     launch_records). Part of the route (<c>/rockets.Rockets/GetRocket</c>), so clients generated from the <c>.proto</c> reach the server
    ///     whatever the server's C# namespace; protobuf-net would otherwise name the package after the namespace but route without it.
    /// </summary>
    public static string PackageName(HostModel host) => string.Join(".", host.Group.Split(['/', '.'], StringSplitOptions.RemoveEmptyEntries).Select(part => part.Replace('-', '_')));

    /// <summary>The gRPC service name: the group in PascalCase, e.g. launch-records → LaunchRecords.</summary>
    public static string ServiceName(HostModel host) =>
        string.Concat(host.Group.Split(['-', '/', '.'], StringSplitOptions.RemoveEmptyEntries).Select(part => char.ToUpperInvariant(part[0]) + part[1..]));

    public static TypeSyntax Message(HostModel host, string type) => SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName(ContractName(host)), SyntaxFactory.GenericName(SyntaxFactory.Identifier("Message")).AddTypeArgumentListArguments(Type(type)));

    public static TypeSyntax WireRequest(HostModel host, OperationModel operation) => operation.RequestIsMessage ? Type(operation.WireType) : Message(host, operation.WireType);

    public static TypeSyntax WireResponse(HostModel host, OperationModel operation) =>
        operation.ResponseIsMessage ? Type(operation.ResponseTypeNotNull!) : Message(host, operation.ResponseTypeNotNull!);

    /// <summary>A contract method's return type and parameters (<c>request</c>, <c>context</c>).</summary>
    public static (TypeSyntax ReturnType, ParameterListSyntax Parameters) Signature(HostModel host, OperationModel operation, bool withDefault)
    {
        var parameters = Parameters(
            Parameter(WireRequest(host, operation), "request"),
            Parameter(Type("global::ProtoBuf.Grpc.CallContext"), "context", defaultValue: withDefault ? Default : null)
        );
        var returnType = operation.Kind switch
        {
            RequestKind.Stream => AsyncEnumerable(WireResponse(host, operation)),
            RequestKind.Void => ValueTask(),
            RequestKind.Unary or RequestKind.Infer => ValueTask(WireResponse(host, operation)),
            _ => throw new ArgumentOutOfRangeException(nameof(operation)),
        };
        return (returnType, parameters);
    }

    public static InterfaceDeclarationSyntax Contract(HostModel host, IEnumerable<OperationModel> operations, Version? version)
    {
        var members = operations.Select(
                                     operation =>
                                     {
                                         var (returnType, parameters) = Signature(host, operation, withDefault: true);
                                         return (MemberDeclarationSyntax)ApiSyntax.Signature(returnType, operation.Name, parameters)
                                                                                .AddAttributeLists(Attribute("global::ProtoBuf.Grpc.Configuration.Operation", Arg(Literal(operation.Name))));
                                     }
                                 )
                                .Append(MessageClass());
        return Interface(ContractName(host), Modifiers(Accessibility(host.Accessibility)), members)
              .AddAttributeLists(Attribute("global::ProtoBuf.Grpc.Configuration.Service", Arg(Literal(PackageName(host) + "." + ServiceName(host)))), GeneratedCode("Rocket.Surgery.LaunchPad.Api", version))
              .Summary($"The gRPC contract of <see cref=\"{host.Name}\" />: service <c>{PackageName(host)}.{ServiceName(host)}</c>, one operation per listed request.");
    }

    private static ClassDeclarationSyntax MessageClass() =>
        Class(
                "Message",
                Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword),
                [
                    Constructor("Message", Parameters(), []),
                    Constructor("Message", Parameters(Parameter(SyntaxFactory.IdentifierName("T"), "value")), [Statement(Assign(Id("Value"), Id("value")))]),
                    AutoProperty(Modifiers(SyntaxKind.PublicKeyword), SyntaxFactory.IdentifierName("T"), "Value", NullForgiving(Default))
                       .AddAttributeLists(Attribute("global::System.Runtime.Serialization.DataMember", Arg("Order", Literal(1)))),
                    // What protobuf-net converts through when the carried struct is the surrogate; it converts the (null) existing value too.
                    Conversion(MessageOfT, SyntaxFactory.IdentifierName("T"), New(MessageOfT, Id("value"))),
                    Conversion(SyntaxFactory.IdentifierName("T"), MessageOfT, SyntaxFactory.ConditionalExpression(IsNull(Id("value")), NullForgiving(Default), Member(Id("value"), "Value"))),
                ]
            )
           .WithTypeParameterList(SyntaxFactory.TypeParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.TypeParameter("T"))))
           .AddAttributeLists(Attribute("global::System.Runtime.Serialization.DataContract"))
           .Summary("Carries a request, response or stream item that isn't a class, which protobuf-net.Grpc requires.");

    private static readonly TypeSyntax MessageOfT = SyntaxFactory.GenericName(SyntaxFactory.Identifier("Message")).AddTypeArgumentListArguments(SyntaxFactory.IdentifierName("T"));

    private static ConversionOperatorDeclarationSyntax Conversion(TypeSyntax to, TypeSyntax from, ExpressionSyntax body) =>
        SyntaxFactory.ConversionOperatorDeclaration(SyntaxFactory.Token(SyntaxKind.ImplicitKeyword), to)
                     .WithModifiers(Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword))
                     .WithParameterList(Parameters(Parameter(from, "value")))
                     .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(body))
                     .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

    /// <summary>
    ///     <c>ConfigureGrpcModel()</c> on the host, called by the generated <c>Map{Host}()</c> and <c>GetProto()</c> before protobuf-net builds its
    ///     serializers. Each carried class is added to protobuf-net's model once: at compatibility level 300, so Guids, decimals, DateTimes and
    ///     TimeSpans travel as the standard types a <c>.proto</c> declares rather than protobuf-net's own bcl.proto messages; created without a
    ///     constructor when it has no parameterless one (positional records); and, when nested, named with its container
    ///     (<c>CreateRocketRequest</c>), so the many <c>X.Request</c> types don't all become <c>message Request</c>. A carried struct is then made
    ///     the surrogate of its <c>Message&lt;T&gt;</c>.
    /// </summary>
    public static IEnumerable<MemberDeclarationSyntax> ModelConfiguration(HostModel host, IReadOnlyList<OperationModel> operations)
    {
        var types = operations.SelectMany(
                                   o => new (string? Type, string? Name)[]
                                   {
                                       (o.RequestIsMessage || o.RequestIsStruct ? o.WireType : null, o.WireNeedsGraphQLName ? o.WireGraphQLName : null),
                                       (o.ResponseIsMessage || o.ResponseIsStruct ? o.ResponseTypeNotNull : null, o.ResponseGraphQLName),
                                   }
                               )
                              .Where(t => t.Type is { })
                              .GroupBy(t => t.Type!, StringComparer.Ordinal)
                              .Select(g => (Type: g.Key, g.First().Name))
                              .ToArray();
        var structs = operations.SelectMany(o => new[] { o.RequestIsStruct ? o.WireType : null, o.ResponseIsStruct ? o.ResponseTypeNotNull : null })
                                .OfType<string>()
                                .Distinct(StringComparer.Ordinal)
                                .ToArray();
        yield return Field(Modifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword), SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), "__grpcModelConfigured");
        yield return Method(
                Modifiers(SyntaxKind.InternalKeyword, SyntaxKind.StaticKeyword),
                VoidType,
                "ConfigureGrpcModel",
                Parameters(),
                new[]
                    {
                        If(
                            Equal(Invoke(Member("global::System.Threading.Interlocked", "Exchange"), [RefArgument("__grpcModelConfigured"), SyntaxFactory.Argument(Literal(1))]), Literal(1)),
                            Return()
                        ),
                    }
                   .Concat(types.Select(t => Statement(Invoke(Id("__ConfigureGrpcType"), TypeOf(Type(t.Type)), t.Name is { } name ? Literal(name) : Null))))
                   .Concat(structs.Select(t => Statement(Invoke(Id("__ConfigureGrpcSurrogate"), TypeOf(Message(host, t)), TypeOf(Type(t))))))
            )
           .Summary("Adds the carried messages to protobuf-net's model: standard types, no constructor for positional records, nested types named with their container, structs on the wire as themselves.");
        var dynamicallyAccessed = Attribute(
            "global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers",
            Arg(Member("global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes", "PublicParameterlessConstructor"))
        );
        var model = Member("global::ProtoBuf.Meta.RuntimeTypeModel", "Default");
        yield return Method(
            Modifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword),
            VoidType,
            "__ConfigureGrpcType",
            Parameters(Parameter(Type("global::System.Type"), "type", dynamicallyAccessed), Parameter(Type("string?"), "name")),
            [
                // Another host that carries the same type has configured it already. (IsDefined would add it, at the default level.)
                SyntaxFactory.ForEachStatement(
                    Type("global::ProtoBuf.Meta.MetaType"),
                    SyntaxFactory.Identifier("existing"),
                    Invoke(Member(model, "GetTypes")),
                    If(Equal(Member(Id("existing"), "Type"), Id("type")), Return())
                ),
                Var("meta", Invoke(Member(model, "Add"), Id("type"), True, Member("global::ProtoBuf.CompatibilityLevel", "Level300"))),
                If(IsNull(Invoke(Member(Id("type"), "GetConstructor"), Member("global::System.Type", "EmptyTypes"))), Statement(Assign(Member(Id("meta"), "UseConstructor"), False))),
                If(NotEqual(Id("name"), Null), Statement(Assign(Member(Id("meta"), "Name"), NullForgiving(Id("name"))))),
            ]
        );
        if (structs.Length == 0) yield break;
        yield return Method(
            Modifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword),
            VoidType,
            "__ConfigureGrpcSurrogate",
            Parameters(Parameter(Type("global::System.Type"), "wrapper"), Parameter(Type("global::System.Type"), "type")),
            [
                SyntaxFactory.ForEachStatement(
                    Type("global::ProtoBuf.Meta.MetaType"),
                    SyntaxFactory.Identifier("existing"),
                    Invoke(Member(model, "GetTypes")),
                    If(Equal(Member(Id("existing"), "Type"), Id("wrapper")), Return())
                ),
                Statement(Invoke(Member(Invoke(Member(model, "Add"), Id("wrapper"), False), "SetSurrogate"), Id("type"))),
            ]
        );
    }
}
