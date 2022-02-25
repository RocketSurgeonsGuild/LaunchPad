using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Analyzers.Composition;

internal static class MatcherDefaults
{
    /// <summary>
    ///     The cache of default status codes for a given method type.
    /// </summary>
    public static Dictionary<RestfulApiMethod, int> MethodStatusCodeMap { get; } = new Dictionary<RestfulApiMethod, int>
    {
        [RestfulApiMethod.List] = StatusCodes.Status200OK,
        [RestfulApiMethod.Read] = StatusCodes.Status200OK,
        [RestfulApiMethod.Create] = StatusCodes.Status201Created,
        [RestfulApiMethod.Update] = StatusCodes.Status200OK,
        [RestfulApiMethod.Delete] = StatusCodes.Status204NoContent,
    };

    public static ImmutableArray<IRestfulApiMethodMatcher> GetMatchers(Compilation compilation)
    {
        return DefaultMatchers(compilation).Where(z => z.IsValid()).OfType<IRestfulApiMethodMatcher>().ToImmutableArray();
    }

    private static IEnumerable<RestfulApiMethodBuilder> DefaultMatchers(Compilation compilation)
    {
        var IBaseRequest = compilation.GetTypeByMetadataName("MediatR.IBaseRequest");
        var IStreamRequest = compilation.GetTypeByMetadataName("MediatR.IStreamRequest`1");
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.List)
                    .MatchPrefix("List", "Search")
                    .MatchParameterType(^1, IBaseRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.List)
                    .MatchPrefix("List", "Search")
                    .MatchParameterType(^1, IStreamRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Read)
                    .MatchPrefix("Get", "Find", "Fetch", "Read")
                    .MatchParameterType(^1, IBaseRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Read)
                    .MatchPrefix("Get", "Find", "Fetch", "Read")
                    .MatchParameterType(^1, IStreamRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Create)
                    .MatchPrefix("Post", "Create", "Add")
                    .MatchParameterType(^1, IBaseRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Create)
                    .MatchPrefix("Post", "Create", "Add")
                    .MatchParameterType(^1, IStreamRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Update)
                    .MatchPrefix("Put", "Edit", "Update")
                    .MatchParameterSuffix(^2, "id")
                    .MatchParameterType(^1, IBaseRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Update)
                    .MatchPrefix("Put", "Edit", "Update")
                    .MatchParameterSuffix(^2, "id")
                    .MatchParameterType(^1, IStreamRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Update)
                    .MatchPrefix("Put", "Edit", "Update")
                    .MatchParameterSuffix(^2, "id")
                    .MatchParameterSuffix(^1, "model", "request");
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Delete)
                    .MatchPrefix("Delete", "Remove")
                    .MatchParameterType(^1, IBaseRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Delete)
                    .MatchPrefix("Delete", "Remove")
                    .MatchParameterType(^1, IStreamRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Delete)
                    .MatchPrefix("Delete", "Remove")
                    .MatchParameterSuffix(^1, "id");
    }
}
