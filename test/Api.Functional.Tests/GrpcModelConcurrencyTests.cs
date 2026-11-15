using System.Reflection;
using System.Runtime.Loader;

namespace Api.Functional.Tests;

/// <summary>
///     Every generated gRPC host adds the types it carries to protobuf-net's process-wide <c>RuntimeTypeModel.Default</c>, once. Hosts mapped at
///     the same time (several servers started by parallel tests, or two services carrying the same contracts) must not race on it: no
///     <c>Duplicate type</c>, and no host building its serializers before the model holds its configuration.
/// </summary>
public class GrpcModelConcurrencyTests
{
    private const int Iterations = 50;
    private const int CallsPerHost = 8;

    private static readonly string[] Hosts = ["Api.Functional.ZeroAlloc.Server.RocketsService", "Api.Functional.ZeroAlloc.Server.LaunchesService"];

    // The model is configured once per process, so each iteration loads the server and protobuf-net into a fresh load context: a fresh
    // RuntimeTypeModel.Default, configured from scratch by every host at once.
    [Test]
    public async Task Should_Configure_The_Model_From_Many_Hosts_At_Once()
    {
        var expected = Sequentially();
        var failures = new List<string>();

        for (var iteration = 0; iteration < Iterations; iteration++)
        {
            var context = new IsolatedServerContext();
            try
            {
                var calls = new List<(string Host, MethodInfo GetProto)>();
                foreach (var host in Hosts)
                {
                    var getProto = context.LoadServer().GetType(host, throwOnError: true)!.GetMethod("GetProto", BindingFlags.Public | BindingFlags.Static)!;
                    for (var call = 0; call < CallsPerHost; call++) calls.Add((host, getProto));
                }

                using var start = new Barrier(calls.Count);
                var tasks = new Task<string>[calls.Count];
                for (var i = 0; i < calls.Count; i++)
                {
                    var getProto = calls[i].GetProto;
                    tasks[i] = Task.Factory.StartNew(
                        () =>
                        {
                            start.SignalAndWait();
                            return (string)getProto.Invoke(null, null)!;
                        },
                        CancellationToken.None,
                        TaskCreationOptions.LongRunning,
                        TaskScheduler.Default
                    );
                }

                for (var i = 0; i < tasks.Length; i++)
                {
                    try
                    {
                        var proto = await tasks[i];
                        if (!string.Equals(proto, expected[calls[i].Host], StringComparison.Ordinal))
                            failures.Add($"iteration {iteration}: {calls[i].Host}.GetProto() described the model before it was configured");
                    }
                    catch (TargetInvocationException exception) when (exception.InnerException is { } inner)
                    {
                        failures.Add($"iteration {iteration}: {calls[i].Host}.GetProto() threw {inner.GetType().Name}: {inner.Message}");
                    }
                }
            }
            finally
            {
                context.Unload();
            }
        }

        await failures.Should().BeEmpty();
    }

    // What each host describes when nothing races it: in a fresh load context too, so both sides see the same model.
    private static Dictionary<string, string> Sequentially()
    {
        var context = new IsolatedServerContext();
        try
        {
            var server = context.LoadServer();
            var protos = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var host in Hosts)
                protos[host] = (string)server.GetType(host, throwOnError: true)!.GetMethod("GetProto", BindingFlags.Public | BindingFlags.Static)!.Invoke(null, null)!;
            return protos;
        }
        finally
        {
            context.Unload();
        }
    }

    // protobuf-net and the server (with its contracts) load afresh; everything else is shared with the test.
    private sealed class IsolatedServerContext() : AssemblyLoadContext(nameof(IsolatedServerContext), isCollectible: true)
    {
        public Assembly LoadServer() => LoadFromAssemblyName(new("Api.Functional.ZeroAlloc.Server"));

        protected override Assembly? Load(AssemblyName assemblyName) =>
            assemblyName.Name is { } name && ( name.StartsWith("protobuf-net", StringComparison.Ordinal) || name.StartsWith("Api.Functional.ZeroAlloc.", StringComparison.Ordinal) )
                ? LoadFromAssemblyPath(Path.Combine(AppContext.BaseDirectory, name + ".dll"))
                : null;
    }
}
