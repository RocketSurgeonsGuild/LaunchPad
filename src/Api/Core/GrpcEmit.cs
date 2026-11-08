using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Rocket.Surgery.LaunchPad.Api.Generators.ApiSyntax;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     The code-first gRPC contract both gRPC generators write. Server and client hosts of the same group produce the same service name,
///     operation names and message shapes, which is all protobuf-net.Grpc puts on the wire.
/// </summary>
/// <remarks>
///     protobuf-net.Grpc only carries classes. A request, response or stream item that isn't one (a struct request, a <c>bool</c> response) travels
///     in the contract's nested <c>Message&lt;T&gt;</c>, wrapped and unwrapped by the generated code on both sides.
/// </remarks>
internal static class GrpcEmit
{
    public static string ContractName(HostModel host) => $"I{host.Name}Contract";

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
              .AddAttributeLists(Attribute("global::ProtoBuf.Grpc.Configuration.Service", Arg(Literal(ServiceName(host)))), GeneratedCode("Rocket.Surgery.LaunchPad.Api", version))
              .Summary($"The gRPC contract of <see cref=\"{host.Name}\" />: service <c>{ServiceName(host)}</c>, one operation per listed request.");
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
                ]
            )
           .WithTypeParameterList(SyntaxFactory.TypeParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.TypeParameter("T"))))
           .AddAttributeLists(Attribute("global::System.Runtime.Serialization.DataContract"))
           .Summary("Carries a request, response or stream item that isn't a class, which protobuf-net.Grpc requires.");

    /// <summary>
    ///     <c>ConfigureGrpcModel()</c> on the host: positional records have no parameterless constructor, so protobuf-net is told to create the
    ///     carried classes without one. Called by the generated <c>Map{Host}()</c> / <c>Add{Host}()</c>, before protobuf-net builds its
    ///     serializers.
    /// </summary>
    public static IEnumerable<MemberDeclarationSyntax> ModelConfiguration(IEnumerable<OperationModel> operations)
    {
        var types = operations.SelectMany(o => new[] { o.RequestIsMessage ? o.WireType : null, o.ResponseIsMessage ? o.ResponseTypeNotNull : null })
                              .OfType<string>()
                              .Distinct()
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
                   .Concat(types.Select(type => Statement(Invoke(Id("__CreateWithoutConstructor"), TypeOf(Type(type))))))
            )
           .Summary("Lets protobuf-net create the carried messages that have no parameterless constructor (positional records).");
        var dynamicallyAccessed = Attribute(
            "global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers",
            Arg(Member("global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes", "PublicParameterlessConstructor"))
        );
        yield return Method(
            Modifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword),
            VoidType,
            "__CreateWithoutConstructor",
            Parameters(Parameter(Type("global::System.Type"), "type", dynamicallyAccessed)),
            [
                If(
                    IsNull(Invoke(Member(Id("type"), "GetConstructor"), Member("global::System.Type", "EmptyTypes"))),
                    Statement(
                        Assign(
                            Member(SyntaxFactory.ElementAccessExpression(Member("global::ProtoBuf.Meta.RuntimeTypeModel", "Default")).AddArgumentListArguments(SyntaxFactory.Argument(Id("type"))), "UseConstructor"),
                            False
                        )
                    )
                ),
            ]
        );
    }
}
