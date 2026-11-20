using System.Collections.Immutable;

namespace Rocket.Surgery.LaunchPad.Analyzers.Api.Core;

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
            if (host.Kind is not ( HostKind.MinimalApi or HostKind.Controller ) || !CreatesAnything(host.Operations))
            {
                continue;
            }

            var candidates = Candidates(hosts, host);
            var diagnostics = new List<DiagnosticInfo>(host.Diagnostics);
            var operations = host.Operations.AsImmutableArray().ToBuilder();
            for (var o = 0; o < operations.Count; o++)
            {
                var operation = operations[o];
                if (operation.CreatedLocationRequest is not { } request) continue;
                var (Host, Operation) = FindGet(candidates, request);
                if (Host is null || Operation is null)
                {
                    diagnostics.Add(
                        DiagnosticInfo.Create(ApiDiagnostics.CreatedLocationInvalid, operation.Location, operation.Name, $"no {host.Kind.Describe()} host exposes {request.Replace("global::", "")} as a GET")
                    );
                    continue;
                }

                var tokens = ImmutableArray.CreateBuilder<string>(Operation.RouteTokens.Length);
                string? missing = null;
                foreach (var token in Operation.RouteTokens)
                {
                    if (FindMember(operation.ResponseMembers, token) is not { } member)
                    {
                        missing = token;
                        break;
                    }

                    tokens.Add(member);
                }

                if (missing is { })
                {
                    diagnostics.Add(
                        DiagnosticInfo.Create(ApiDiagnostics.CreatedLocationInvalid, operation.Location, operation.Name, $"its response has no member for {Operation.Name}'s route value '{missing}'")
                    );
                    continue;
                }

                operations[o] = operation with
                {
                    CreatedLocationRoute = RouteText.Join(Host.BaseRoute, Operation.Route),
                    CreatedLocationTokens = new(tokens.ToImmutable()),
                };
            }

            hosts[index] = host with { Operations = new(operations.ToImmutable()), Diagnostics = new([.. diagnostics]) };
        }

        return hosts;
    }

    private static bool CreatesAnything(EquatableArray<OperationModel> operations)
    {
        foreach (var operation in operations)
        {
            if (operation.CreatedLocationRequest is { })
            {
                return true;
            }
        }

        return false;
    }

    // The host itself first, then every other host of the same transport, in order.
    private static List<HostModel> Candidates(HostModel[] hosts, HostModel host)
    {
        var candidates = new List<HostModel> { host };
        foreach (var other in hosts)
        {
            if (other != host && other.Kind == host.Kind)
            {
                candidates.Add(other);
            }
        }

        return candidates;
    }

    private static (HostModel? Host, OperationModel? Operation) FindGet(List<HostModel> candidates, string request)
    {
        foreach (var candidate in candidates)
        {
            foreach (var target in candidate.Operations)
            {
                if (target.RequestType == request && target.Verb == HttpVerb.Get)
                {
                    return (candidate, target);
                }
            }
        }

        return (null, null);
    }

    // The response member a route value is read from: the first whose name matches, ignoring case.
    private static string? FindMember(EquatableArray<string> members, string token)
    {
        foreach (var member in members)
        {
            if (string.Equals(member, token, StringComparison.OrdinalIgnoreCase))
            {
                return member;
            }
        }

        return null;
    }
}
