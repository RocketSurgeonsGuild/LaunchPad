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

    public static string Message(HostModel host, string type) => $"{ContractName(host)}.Message<{type}>";

    public static string WireRequest(HostModel host, OperationModel operation) => operation.RequestIsMessage ? operation.WireType : Message(host, operation.WireType);

    public static string WireResponse(HostModel host, OperationModel operation) =>
        operation.ResponseIsMessage ? operation.ResponseTypeNotNull! : Message(host, operation.ResponseTypeNotNull!);

    public static (string ReturnType, string Parameters) Signature(HostModel host, OperationModel operation, bool withDefault)
    {
        var parameters = $"{WireRequest(host, operation)} request, global::ProtoBuf.Grpc.CallContext context{( withDefault ? " = default" : "" )}";
        var returnType = operation.Kind switch
        {
            RequestKind.Stream => $"global::System.Collections.Generic.IAsyncEnumerable<{WireResponse(host, operation)}>",
            RequestKind.Void => "global::System.Threading.Tasks.ValueTask",
            RequestKind.Unary or RequestKind.Infer => $"global::System.Threading.Tasks.ValueTask<{WireResponse(host, operation)}>",
            _ => throw new ArgumentOutOfRangeException(nameof(operation)),
        };
        return (returnType, parameters);
    }

    public static void WriteContract(SourceWriter w, HostModel host, IEnumerable<OperationModel> operations, Version? version)
    {
        w.Line($"/// <summary>The gRPC contract of <see cref=\"{host.Name}\" />: service <c>{ServiceName(host)}</c>, one operation per listed request.</summary>");
        w.Line($"[global::ProtoBuf.Grpc.Configuration.Service({SourceWriter.Literal(ServiceName(host))})]");
        w.Line($"[global::System.CodeDom.Compiler.GeneratedCode(\"Rocket.Surgery.LaunchPad.Api\", \"{version}\")]");
        w.Open($"{host.Accessibility} interface {ContractName(host)}");
        foreach (var operation in operations)
        {
            var (returnType, parameters) = Signature(host, operation, withDefault: true);
            w.Line($"[global::ProtoBuf.Grpc.Configuration.Operation({SourceWriter.Literal(operation.Name)})]");
            w.Line($"{returnType} {operation.Name}({parameters});");
            w.Line();
        }

        w.Line("/// <summary>Carries a request, response or stream item that isn't a class, which protobuf-net.Grpc requires.</summary>");
        w.Line("[global::System.Runtime.Serialization.DataContract]");
        w.Open("public sealed class Message<T>");
        w.Line("public Message() { }");
        w.Line();
        w.Line("public Message(T value) => Value = value;");
        w.Line();
        w.Line("[global::System.Runtime.Serialization.DataMember(Order = 1)]");
        w.Line("public T Value { get; set; } = default!;");
        w.Close();
        w.Close();
    }

    /// <summary>
    ///     <c>ConfigureGrpcModel()</c> on the host: positional records have no parameterless constructor, so protobuf-net is told to create the
    ///     carried classes without one. Called by the generated <c>Map{Host}()</c> / <c>Add{Host}()</c>, before protobuf-net builds its
    ///     serializers.
    /// </summary>
    public static void WriteModelConfiguration(SourceWriter w, IEnumerable<OperationModel> operations)
    {
        var types = operations.SelectMany(o => new[] { o.RequestIsMessage ? o.WireType : null, o.ResponseIsMessage ? o.ResponseTypeNotNull : null })
                              .OfType<string>()
                              .Distinct()
                              .ToArray();
        w.Line("private static int __grpcModelConfigured;");
        w.Line();
        w.Line("/// <summary>Lets protobuf-net create the carried messages that have no parameterless constructor (positional records).</summary>");
        w.Open("internal static void ConfigureGrpcModel()");
        w.Line("if (global::System.Threading.Interlocked.Exchange(ref __grpcModelConfigured, 1) == 1) return;");
        foreach (var type in types) w.Line($"__CreateWithoutConstructor(typeof({type}));");
        w.Close();
        w.Line();
        w.Line(
            "private static void __CreateWithoutConstructor([global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(global::System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] global::System.Type type)"
        );
        w.Open("");
        w.Line("if (type.GetConstructor(global::System.Type.EmptyTypes) is null) global::ProtoBuf.Meta.RuntimeTypeModel.Default[type].UseConstructor = false;");
        w.Close();
    }
}
