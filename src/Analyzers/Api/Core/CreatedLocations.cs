namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Points each operation whose response carries <c>[Creates]</c> at the operation that loads what it created, so the server can answer 201
///     with a Location. The target is looked for on the same host first, then on every other host of the same transport.
/// </summary>
internal static class CreatedLocations
{
    public static HostModel[] ResolveCreatedLocations(this HostModel[] hosts)
    {
        for (var index = 0; index < hosts.Length; index++)
        {
            var host = hosts[index];
            if (host.Kind is not ( HostKind.MinimalApi or HostKind.Controller ) || host.Operations.All(o => o.CreatedLocationRequest is null)) continue;

            var candidates = new[] { host }.Concat(hosts.Where(h => h != host && h.Kind == host.Kind)).ToArray();
            var diagnostics = host.Diagnostics.ToList();
            var operations = host.Operations.ToArray();
            for (var o = 0; o < operations.Length; o++)
            {
                var operation = operations[o];
                if (operation.CreatedLocationRequest is not { } request) continue;
                var (Host, Operation) = candidates.SelectMany(h => h.Operations.Where(t => t.RequestType == request && t.Verb == HttpVerb.Get).Select(t => (Host: h, Operation: t))).FirstOrDefault();
                if (Operation is null)
                {
                    diagnostics.Add(
                        DiagnosticInfo.Create(ApiDiagnostics.CreatedLocationInvalid, operation.Location, operation.Name, $"no {host.Kind.Describe()} host exposes {request.Replace("global::", "")} as a GET")
                    );
                    continue;
                }

                var missing = Operation.RouteTokens.FirstOrDefault(t => operation.ResponseMembers.All(m => !string.Equals(m, t, StringComparison.OrdinalIgnoreCase)));
                if (missing is { })
                {
                    diagnostics.Add(
                        DiagnosticInfo.Create(ApiDiagnostics.CreatedLocationInvalid, operation.Location, operation.Name, $"its response has no member for {Operation.Name}'s route value '{missing}'")
                    );
                    continue;
                }

                var tokens = Operation.RouteTokens.Select(t => operation.ResponseMembers.First(m => string.Equals(m, t, StringComparison.OrdinalIgnoreCase)));
                operations[o] = operation with
                {
                    CreatedLocationRoute = RouteText.Join(Host.BaseRoute, Operation.Route),
                    CreatedLocationTokens = tokens.ToEquatableArray(),
                };
            }

            hosts[index] = host with { Operations = operations.ToEquatableArray(), Diagnostics = diagnostics.ToEquatableArray() };
        }

        return hosts;
    }
}
