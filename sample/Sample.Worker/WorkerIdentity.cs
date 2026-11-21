using Sample.Core.Authorization;

using ZeroAlloc.Authorization;

namespace Sample.Worker;

/// <summary>The worker as the policies of the operations see it: a service, which may read rockets.</summary>
internal sealed class WorkerIdentity : ISecurityContext
{
    public static WorkerIdentity Instance { get; } = new();

    public string Id => "sample-worker";
    public IReadOnlySet<string> Roles { get; } = new HashSet<string>(StringComparer.Ordinal);
    public IReadOnlyDictionary<string, string> Claims { get; } = new Dictionary<string, string>(StringComparer.Ordinal) { ["scope"] = Scopes.RocketsRead };
}
