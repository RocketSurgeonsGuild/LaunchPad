# Operations and operation hosts

An **operation** is a mediator request, together with its response, that is exposed over one or more transports. `GetRocket.Request` is
the same operation whether a client reaches it as `GET /api/rockets/{id}`, as a gRPC method or as a GraphQL field. On both sides of the
wire the code is the same: `mediator.Send(new GetRocket.Request(id))`. On the server the request reaches its handler; on the client a
generated handler sends it over the transport.

LaunchPad's API generators write the endpoints, services, resolvers and clients that carry operations. The
[API generators guide](../../guides/api-generators/) shows how to use them; this page explains the vocabulary.

## The pieces

| Term                  | Meaning                                                                                                                             |
| --------------------- | ----------------------------------------------------------------------------------------------------------------------------------- |
| **Operation**         | A mediator request and its response, exposed over a transport. Its name comes from the request: `GetRocket.Request` is `GetRocket`. |
| **Contracts project** | The project holding the requests and responses, shared by the servers that expose them and the clients that call them.              |
| **Transport**         | A way of carrying an operation: Minimal API, MVC controllers, gRPC or GraphQL.                                                      |
| **Operation host**    | A type that lists, one by one, the operations it exposes (on a server) or calls (on a client) over one transport.                   |
| **Group**             | The name a host gives its operations. A server and a client must share it to agree on where an operation lives.                     |
| **Mediator profile**  | How one mediator library shapes requests, dispatches them and declares handlers, so operations work with any mediator.              |

## Operation hosts

An operation is reachable only where a host lists it with `[Operation<TRequest>]`:

```csharp
// Server: a Minimal API host in the web project.
[Operation<GetRocket.Request>]
[Operation<CreateRocket.Request>]
public static partial class RocketsEndpoints;

// Client: an HTTP client host in the client project.
[Operation<GetRocket.Request>]
[Operation<CreateRocket.Request>]
public partial class RocketsClient;
```

Referencing a contracts project exposes nothing by itself, and each server shows exactly which operations it exposes. LaunchPad chose this
over scanning the contracts for marked requests, which would make every marked request public on every server that references them
([ADR-0003](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/docs/adr/0003-explicit-operation-hosts.md)).

The host's **transport** is inferred from its base type, a transport attribute or its name: `…Endpoints` is Minimal API, `…Controller` is
MVC, `…Service` is gRPC, `…Mutations` is GraphQL, and `…Client`, `…GrpcClient` and `…GraphQLClient` are clients. The generator writes a
complete member for each listed operation and adds whatever else the transport needs on the host.

The host also owns the **group**, by default its name without the suffix in kebab-case: `RocketsEndpoints` and `RocketsClient` are both
`rockets`, so their routes, gRPC service names and GraphQL fields line up. A client never sees the server, so a route or name changed on
the server has to be repeated on the client.

## Mediator profiles

Mediator libraries disagree on how a request declares its response, how it is sent and how a handler is written. A **mediator profile**
describes those shapes, and the generators write code against the profile rather than against one library. LaunchPad has built-in
profiles for MediatR, martinothamar/Mediator, ZeroAlloc.Mediator, Immediate.Handlers, Foundatio.Mediator and ChannelMediator, and picks
the one the project references. A [custom profile](../../guides/api-generators/#a-custom-profile) covers any other library.

The terms on this page are defined in the repository's
[glossary](https://github.com/RocketSurgeonsGuild/LaunchPad/blob/main/GLOSSARY.md).