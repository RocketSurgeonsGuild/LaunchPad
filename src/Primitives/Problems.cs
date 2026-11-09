namespace Rocket.Surgery.LaunchPad.Primitives;

/// <summary>
///     The base of LaunchPad's standard error cases: what a handler returns, as a case of its result union, instead of throwing. Each transport
///     turns a case into its own error shape (HTTP ProblemDetails, a gRPC status, a GraphQL error) from <see cref="IProblemDetailsData" />.
/// </summary>
/// <remarks>
///     A union may list the derived cases it can return (<c>union GetRocketResult(RocketModel, NotFound)</c>), or <see cref="Problem" /> itself to
///     return any of them. See <c>docs/design/union-types.md</c>.
/// </remarks>
[PublicAPI]
public abstract record Problem : IProblemDetailsData
{
    /// <summary>Initializes a problem with an optional detail message.</summary>
    /// <param name="detail">A human-readable explanation specific to this occurrence of the problem.</param>
    protected Problem(string? detail = null) => Detail = detail;

    /// <summary>A human-readable explanation specific to this occurrence of the problem.</summary>
    public string? Detail { get; init; }

    /// <inheritdoc />
    public string? Title { get; init; }

    /// <inheritdoc />
    public string? Link { get; init; }

    /// <inheritdoc />
    public string? Instance { get; init; }

    /// <inheritdoc />
    public IDictionary<string, object?> Properties { get; init; } = new Dictionary<string, object?>(StringComparer.Ordinal);
}

/// <summary>The requested entity doesn't exist. HTTP 404, gRPC <c>NotFound</c>, GraphQL <c>NOT_FOUND</c>.</summary>
/// <param name="Detail">A human-readable explanation specific to this occurrence of the problem.</param>
[PublicAPI]
public sealed record NotFound(string? Detail = null) : Problem(Detail);

/// <summary>The request is malformed or can never succeed as sent. HTTP 400, gRPC <c>InvalidArgument</c>, GraphQL <c>BAD_USER_INPUT</c>.</summary>
/// <param name="Detail">A human-readable explanation specific to this occurrence of the problem.</param>
[PublicAPI]
public sealed record Invalid(string? Detail = null) : Problem(Detail)
{
    /// <summary>The errors, by property name.</summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; init; } = new Dictionary<string, string[]>(StringComparer.Ordinal);
}

/// <summary>
///     The request is well formed but fails validation: what FluentValidation reports. HTTP 422, gRPC <c>InvalidArgument</c>, GraphQL
///     <c>BAD_USER_INPUT</c>.
/// </summary>
/// <param name="Detail">A human-readable explanation specific to this occurrence of the problem.</param>
[PublicAPI]
public sealed record UnprocessableEntity(string? Detail = null) : Problem(Detail)
{
    /// <summary>The errors, by property name.</summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; init; } = new Dictionary<string, string[]>(StringComparer.Ordinal);
}

/// <summary>The caller isn't authenticated. HTTP 401, gRPC <c>Unauthenticated</c>, GraphQL <c>UNAUTHENTICATED</c>.</summary>
/// <param name="Detail">A human-readable explanation specific to this occurrence of the problem.</param>
[PublicAPI]
public sealed record Unauthorized(string? Detail = null) : Problem(Detail);

/// <summary>The caller may not do this. HTTP 403, gRPC <c>PermissionDenied</c>, GraphQL <c>FORBIDDEN</c>.</summary>
/// <param name="Detail">A human-readable explanation specific to this occurrence of the problem.</param>
[PublicAPI]
public sealed record Forbidden(string? Detail = null) : Problem(Detail);

/// <summary>The request conflicts with the current state. HTTP 409, gRPC <c>FailedPrecondition</c>, GraphQL <c>CONFLICT</c>.</summary>
/// <param name="Detail">A human-readable explanation specific to this occurrence of the problem.</param>
[PublicAPI]
public sealed record Conflict(string? Detail = null) : Problem(Detail);
