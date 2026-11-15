using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations;

/// <summary>
///     For hand-written callers (the Blazor and Razor Pages UIs, the proto-first gRPC services) that read a union response themselves. Generated
///     operation hosts answer union responses on their own.
/// </summary>
[PublicAPI]
public static class UnionExtensions
{
    /// <summary>Splits a union response's <c>Value</c> into its value case or its problem, so the caller answers the problem in its own way.</summary>
    /// <typeparam name="T">The value case.</typeparam>
    /// <param name="value">The union's <c>Value</c>.</param>
    /// <param name="result">The value case, when the response is one.</param>
    /// <param name="problem">The error case, when the response is one.</param>
    /// <returns><see langword="true" /> for the value case, <see langword="false" /> for an error case.</returns>
    /// <exception cref="InvalidOperationException">The value is neither <typeparamref name="T" /> nor a <see cref="Problem" />: the caller asked for the wrong case.</exception>
    public static bool TryGetValue<T>(this object? value, [MaybeNullWhen(false)] out T result, [NotNullWhen(false)] out Problem? problem)
    {
        switch (value)
        {
            case T found:
                result = found;
                problem = null;
                return true;
            case Problem error:
                result = default;
                problem = error;
                return false;
            default:
                throw new InvalidOperationException($"Expected a {typeof(T).Name} or a problem, got {value?.GetType().Name ?? "null"}");
        }
    }

    /// <summary>The message to show for a problem: its detail, or else its title, or else the name of its case.</summary>
    /// <param name="problem">The problem.</param>
    public static string Message(this Problem problem) => problem.Detail ?? problem.Title ?? problem.GetType().Name;
}
