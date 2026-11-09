# Error cases are union cases, named on the wire, with status by convention

An operation can report an error by returning a union response instead of throwing: its value case answers as a normal response, and each error case answers in its transport's own error shape. An error case's status comes from its type name (`NotFound` → 404, `Invalid` → 400, `Validation`/`UnprocessableEntity` → 422, `Unauthorized` → 401, `Forbidden` → 403, `Conflict` → 409, matched at the start or end of the name), and `[ApiError]` on the case type overrides it. A case that is neither the value case nor recognisably an error case doesn't compile (LPAD1025). The generators recognise a union by C# 15's `[Union]` attribute or `IUnion` (#3147's contract, ADR-0005), so native `union` declarations and LaunchPad's portable unions behave the same.

Every transport carries the **case name**, so two cases that share a status (`InvalidName` and `ValidationFailed`, both 400) stay distinct for any client:

- HTTP: ProblemDetails with a `case` extension and the case's members as extensions.
- gRPC: the status, a `case` trailer, and the case serialized by protobuf-net in `case-bin`.
- GraphQL: `extensions.code` holds the category (`NOT_FOUND`) and `extensions.case` holds the name.

A stream of unions streams its value cases and ends with the first error case.

## Considered options

- **Exceptions mapped by filters and interceptors** (what LaunchPad had): it hides a handler's error outcomes from its signature and from the specs, so clients can't know them.
- **A status on every case, always declared**: it's explicit, but it's noise for the five cases every API has. The naming convention covers those, and the attribute covers the rest.
- **Status only on the wire, no case name**: it's simpler, but it can't tell apart cases that share a status.
- **GraphQL `extensions.code` = case name**: rejected, so that `code` keeps the category GraphQL clients conventionally switch on; the name moved to `extensions.case`, the same key HTTP and gRPC use.
- **gRPC rich error model (`google.rpc.Status`) or a JSON trailer**: the first needs Google.Protobuf types around protobuf-net messages, and the second needs System.Text.Json metadata. protobuf-net is what the gRPC transport already uses, and `GetProto()` declares the case messages.

## Consequences

The case name and the member names of error cases are part of an API's wire contract. Renaming a case type is a breaking change for clients. Exceptions (`NotFoundException`, …) keep working for handlers that don't return unions, until the samples move to unions.
