using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Matches the operations of a proto-first gRPC host to the rpcs of the service base Grpc.Tools generated from the <c>.proto</c>: by name
///     (the operation's name, or its <c>[Operation&lt;T&gt;(Name)]</c>), then by shape.
/// </summary>
internal static class ProtoRpcs
{
    private const string AsyncStreamReader = "Grpc.Core.IAsyncStreamReader`1";
    private const string ServerStreamWriter = "Grpc.Core.IServerStreamWriter`1";

    /// <summary>The operation with its rpc, or null (with LPAD1040 or LPAD1042) when it has no rpc it fits.</summary>
    public static OperationModel? Match(INamedTypeSymbol host, INamedTypeSymbol serviceBase, OperationModel operation, List<DiagnosticInfo> diagnostics)
    {
        var name = operation.EndpointName;
        var method = FindRpc(serviceBase, name);
        if (method is null)
        {
            diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.RpcNotFound, operation.Location, host.Name, name, serviceBase.ToDisplayString()));
            return null;
        }

        var (shape, request, response) = Shape(method);
        var fits = shape switch
        {
            "unary" => operation.Kind is RequestKind.Unary or RequestKind.Void,
            "server streaming" => operation.Kind == RequestKind.Stream,
            _ => false,
        };
        if (!fits || request is null || response is null)
        {
            diagnostics.Add(
                DiagnosticInfo.Create(ApiDiagnostics.RpcKindMismatch, operation.Location, operation.RequestDisplayName, ServerEmit.KindName(operation.Kind), name, serviceBase.ToDisplayString(), shape)
            );
            return null;
        }

        return operation with { Rpc = new RpcModel(method.Name, request.FullyQualified(), response.FullyQualified(), shape == "server streaming") };
    }

    private static IMethodSymbol? FindRpc(INamedTypeSymbol serviceBase, string name)
    {
        foreach (var member in serviceBase.GetMembers(name))
        {
            if (member is IMethodSymbol { IsVirtual: true, IsStatic: false, Parameters.Length: 2 or 3 } method && method.Parameters[^1].Type.Is(KnownNames.ServerCallContext))
            {
                return method;
            }
        }

        return null;
    }

    // Grpc.Tools' four shapes: unary (TRequest, context) → Task<TResponse>; server streaming (TRequest, IServerStreamWriter<TResponse>, context)
    // → Task; client and bidirectional streaming read an IAsyncStreamReader<TRequest>, which an operation can't take.
    private static (string Shape, ITypeSymbol? Request, ITypeSymbol? Response) Shape(IMethodSymbol method)
    {
        var first = method.Parameters[0].Type;
        if (first.Is(AsyncStreamReader)) return (method.Parameters.Length == 2 ? "client streaming" : "bidirectional streaming", null, null);
        if (method.Parameters.Length == 3)
        {
            return method.Parameters[1].Type is INamedTypeSymbol writer && writer.Is(ServerStreamWriter)
                ? ("server streaming", first, writer.TypeArguments[0])
                : ("not an rpc", null, null);
        }

        return method.ReturnType is INamedTypeSymbol { IsGenericType: true, TypeArguments.Length: 1 } task
            ? ("unary", first, task.TypeArguments[0])
            : ("not an rpc", null, null);
    }
}
