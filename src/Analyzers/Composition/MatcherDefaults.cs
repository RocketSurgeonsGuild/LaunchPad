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

    private static string[] PostMethodPrefixes { get; } = { "Post", "Create", "Add", "Insert", "New", "Post", "Make", "Construct" };
    private static string[] DeleteMethodPrefixes { get; } = { "Delete", "Remove", "Destroy", "Erase", "Kill", "Cancel" };
    private static string[] SearchMethodPrefixes { get; } = { "Search", "Find", "Query", "Lookup", "Fetch", "Retrieve" };
    private static string[] GetMethodPrefixes { get; } = { "Get", "Fetch", "Retrieve", "Read", "Find" };
    private static string[] UpdateMethodPrefixes { get; } = { "Put", "Update", "Edit", "Save", "Change", "Modify" };

    private static IEnumerable<RestfulApiMethodBuilder> DefaultMatchers(Compilation compilation)
    {
        // ReSharper disable NullableWarningSuppressionIsUsed
        var IBaseRequest = compilation.GetTypeByMetadataName("MediatR.IBaseRequest")!;
        var IStreamRequest = compilation.GetTypeByMetadataName("MediatR.IStreamRequest`1")!;
        // ReSharper enable NullableWarningSuppressionIsUsed
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.List)
                    .MatchPrefix("List", SearchMethodPrefixes)
                    .MatchParameterType(^1, IBaseRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.List)
                    .MatchPrefix("List", SearchMethodPrefixes)
                    .MatchParameterType(^1, IStreamRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Read)
                    .MatchPrefix("Get", GetMethodPrefixes)
                    .MatchParameterType(^1, IBaseRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Read)
                    .MatchPrefix("Get", GetMethodPrefixes)
                    .MatchParameterType(^1, IStreamRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Create)
                    .MatchPrefix("Post", PostMethodPrefixes)
                    .MatchParameterType(^1, IBaseRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Create)
                    .MatchPrefix("Post", PostMethodPrefixes)
                    .MatchParameterType(^1, IStreamRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Update)
                    .MatchPrefix("Put", UpdateMethodPrefixes)
                    .MatchParameterSuffix(^2, "id")
                    .MatchParameterType(^1, IBaseRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Update)
                    .MatchPrefix("Put", UpdateMethodPrefixes)
                    .MatchParameterSuffix(^2, "id")
                    .MatchParameterType(^1, IStreamRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Update)
                    .MatchPrefix("Put", UpdateMethodPrefixes)
                    .MatchParameterSuffix(^2, "id")
                    .MatchParameterSuffix(^1, "model", "request");
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Delete)
                    .MatchPrefix("Delete", DeleteMethodPrefixes)
                    .MatchParameterType(^1, IBaseRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Delete)
                    .MatchPrefix("Delete", DeleteMethodPrefixes)
                    .MatchParameterType(^1, IStreamRequest);
        yield return new RestfulApiMethodBuilder(RestfulApiMethod.Delete)
                    .MatchPrefix("Delete", DeleteMethodPrefixes)
                    .MatchParameterSuffix(^1, "id");
    }
}
