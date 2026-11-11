using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations;

/// <summary>
///     For hand-written callers (the Blazor and Razor Pages UIs, the proto-first gRPC services) that want an operation's value case and let
///     LaunchPad's exception filters and interceptors answer its problems. Generated operation hosts answer union responses themselves.
/// </summary>
[PublicAPI]
public static class UnionExtensions
{
    /// <summary>The value case of a union response's <c>Value</c>; a problem throws the exception LaunchPad's filters answer the same way.</summary>
    /// <typeparam name="T">The value case.</typeparam>
    /// <param name="value">The union's <c>Value</c>.</param>
    public static T Expect<T>(this object? value) => value switch
    {
        T result => result,
        NotFound problem => throw new NotFoundException(problem.Detail ?? "Not Found"),
        Forbidden or Unauthorized => throw new NotAuthorizedException(( (Problem)value ).Detail ?? "Not Authorized"),
        Problem problem => throw new RequestFailedException(problem.Detail ?? problem.GetType().Name)
        {
            Title = problem.Title,
            Properties = problem.Properties,
        },
        _ => throw new InvalidOperationException($"Expected a {typeof(T).Name}, got {value?.GetType().Name ?? "null"}"),
    };
}
