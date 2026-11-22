using System.Text.Json;

using Refit;

namespace Sample.BlazorWasm.Client;

/// <summary>
///     Turns a failed call to <see cref="IRocketsClient" /> into the problem the pages show, as the server sample's pages show the problem case of
///     an operation's union response.
/// </summary>
internal static class ProblemDetailsExtensions
{
    /// <summary>Whether <paramref name="exception" /> is a call that failed: an error response, or Sample.Minimal out of reach.</summary>
    public static bool IsFailedCall(this Exception exception) =>
        exception is ApiException or HttpRequestException or InvalidOperationException;

    /// <summary>
    ///     The problem details of an error response, or a problem that describes the failure when there are none (Sample.Minimal can't be
    ///     reached, say, when the application isn't served by Sample.AppHost).
    /// </summary>
    public static ProblemDetails ToProblem(this Exception exception)
    {
        if (exception is ApiException { Content: { Length: > 0 } content } api)
        {
            try
            {
                if (JsonSerializer.Deserialize(content, RocketsClientJson.Default.ProblemDetails) is { } problem) return problem;
            }
            catch (JsonException)
            {
                // not problem details: described below
            }

            return new() { Status = (int)api.StatusCode, Title = api.ReasonPhrase ?? api.StatusCode.ToString(), Detail = api.Message };
        }

        return new() { Title = "Sample.Minimal could not be reached", Detail = exception.Message };
    }

    /// <summary>The message to show for a problem: its detail, or else its title.</summary>
    public static string Message(this ProblemDetails problem) => problem.Detail ?? problem.Title ?? "The request failed";
}
