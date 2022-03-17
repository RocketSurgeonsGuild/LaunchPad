using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions;

/// <summary>
///     Extensions to ProblemDetailsOptions
/// </summary>
public static class ProblemDetailsOptionsExtensions
{
    /// <summary>
    ///     Creates a mapping for an exception that implements <see cref="IProblemDetailsData" /> with the given status code
    /// </summary>
    /// <param name="options"></param>
    /// <param name="statusCode"></param>
    /// <typeparam name="TException"></typeparam>
    public static void MapToProblemDetailsDataException<TException>(this ProblemDetailsOptions options, int statusCode)
        where TException : Exception, IProblemDetailsData
    {
        options.Map<TException>((_, ex) => ConstructProblemDetails(ex, statusCode));
    }

    private static ProblemDetails ConstructProblemDetails<TException>(TException ex, int statusCode) where TException : Exception, IProblemDetailsData
    {
        var details = new ProblemDetails
        {
            Detail = ex.Message,
            Title = ex.Title,
            Type = ex.Link,
            Instance = ex.Instance,
            Status = statusCode
        };
        foreach (var item in ex.Properties)
        {
            if (details.Extensions.ContainsKey(item.Key)) continue;
            details.Extensions.Add(item);
        }

        return details;
    }
}
