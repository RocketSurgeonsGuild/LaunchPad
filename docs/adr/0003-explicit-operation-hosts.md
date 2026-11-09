# Operations are exposed and called only through explicit operation hosts

An operation (a mediator request with its response) is reachable over a transport only where an operation host lists it with `[Operation<TRequest>]`: a controller, Minimal API class, gRPC service, or GraphQL query or mutation type. The generator writes the complete method for each entry, adds whatever the transport needs on the host that the user did not declare, and works with any mediator through a mediator profile. The transport is inferred from the host's base type, transport attribute or name, and the host owns the group the operations live under.

## Considered Options

- **Scan the contracts project and expose every marked request** (ChannelMediator's approach). Rejected: anything marked becomes public on every server that references the contracts, and nothing in the server shows which operations it exposes.
- **User-written partial method signatures** (LaunchPad's earlier `ControllerActionBodyGenerator` and `GraphqlMutationActionBodyGenerator`). Rejected: C# requires a partial method's return type in its declaration, so every signature had to be hand-written, kept in step with the response type and coupled to MediatR.

## Consequences

The two earlier generators are removed, one transport per phase, together with `RestfulApiController`'s MediatR plumbing. `GraphqlOptionalPropertyTrackingGenerator` stays, because transport-specific inputs (`[Operation<T>(Input = …)]`) are built with its `Create()`. Clients are generated from what each server describes (its OpenAPI document, GraphQL schema or `.proto`), so a route a server overrides reaches its clients with the next generated client.

Amended 2026-11-08: client operation hosts (`*Client`, `*GrpcClient`, `*GraphQLClient`), which generated typed clients plus one mediator handler per request, were removed before release. Mapping a client's mediator requests onto a remote server's tied the client to the server's contracts and mediator, and needed per-mediator placement rules; clients generated from the servers' specs need neither.
