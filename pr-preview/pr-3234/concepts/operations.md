# Operations and operation hosts

An **operation** is a mediator request, together with its response, that is exposed over one or more transports. `GetRocket.Request` is
the same operation whether it is reached as `GET /api/rockets/{id}`, as a gRPC method or as a GraphQL field. On the server, each one
reaches its handler through the mediator: `mediator.Send(new GetRocket.Request(id))`.

LaunchPad's API generators write the endpoints, services and resolvers that carry operations. Clients are generated from what those servers
describe: their OpenAPI document, GraphQL schema or `.proto`. The [API generators guide](../../guides/api-generators/) shows how to use
them; this page explains the vocabulary.

## The pieces

| Term                  | Meaning                                                                                                                             |
| --------------------- | ----------------------------------------------------------------------------------------------------------------------------------- |
| **Operation**         | A mediator request and its response, exposed over a transport. Its name comes from the request: `GetRocket.Request` is `GetRocket`. |
| **Contracts project** | The project holding the requests and responses, shared by the servers that expose them.                                             |
| **Transport**         | A way of carrying an operation: Minimal API, MVC controllers, gRPC or GraphQL.                                                      |
| **Operation host**    | A server type that lists, one by one, the operations it exposes over one transport.                                                 |
| **Group**             | The name a host gives its operations: the middle of their routes, the gRPC package and service.                                     |
| **Mediator profile**  | How one mediator library shapes requests and dispatches them, so operations work with any mediator.                                 |

## Operation hosts

An operation is reachable only where a host lists it with `[Operation<TRequest>]`:

```csharp
// A Minimal API host in the web project.
[Operation<GetRocket.Request>]
[Operation<CreateRocket.Request>]
public static partial class RocketsEndpoints;
```

Referencing a contracts project exposes nothing by itself, and each server shows exactly which operations it exposes. LaunchPad chose this
over scanning the contracts for marked requests, which would make every marked request public on every server that references them
([ADR-0003](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/docs/adr/0003-explicit-operation-hosts.md)).

The host's **transport** is inferred from its base type, a transport attribute or its name: `…Endpoints` is Minimal API, `…Controller` is
MVC, `…Service` is gRPC and `…Mutations` is GraphQL. The generator writes a complete member for each listed operation and adds whatever
else the transport needs on the host.

The host also owns the **group**, by default its name without the suffix in kebab-case: `RocketsEndpoints` and `RocketsService` are both
`rockets`, so the operations live under `/api/rockets` and in the gRPC service `rockets.Rockets`.

## Clients

The generators don't write clients. A server describes its operations in the transport's standard format, and a client generator for that
format (NSwag or Kiota for OpenAPI, StrawberryShake for GraphQL, Grpc.Tools for a `.proto`) writes the client. A client then needs neither
the contracts project nor a mediator, and a route or name the server changes reaches it with the next generated client.

## Mediator profiles

Mediator libraries disagree on how a request declares its response and how it is sent. A **mediator profile** describes those shapes, and
the generators write code against the profile rather than against one library. LaunchPad has built-in profiles for MediatR,
martinothamar/Mediator, ZeroAlloc.Mediator, Immediate.Handlers, Foundatio.Mediator and ChannelMediator, and picks the one the project
references. A [custom profile](../../guides/api-generators/#a-custom-profile) covers any other library.

The terms on this page are defined in the repository's
[glossary](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/GLOSSARY.md).