using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Analyzers.Composition;

/// <summary>
///     This class allows you to define methods default conventions when using the RestfulApiController
///     This allows the response codes to be automatically inferred based on the rules defined.
/// </summary>
internal class RestfulApiMethodBuilder : IRestfulApiMethodMatcher
{
    private static RestfulApiParameterMatcher DefaultMatcher(Index index)
    {
        return new(
            index,
            ApiConventionNameMatchBehavior.Any,
            Array.Empty<string>(),
            ApiConventionTypeMatchBehavior.Any,
            null
        );
    }

    private readonly RestfulApiMethod _method;

    private readonly IDictionary<Index, IRestfulApiParameterMatcher> _parameters = new Dictionary<Index, IRestfulApiParameterMatcher>();
    private int? _parameterCount;
    private ApiConventionNameMatchBehavior _nameMatchBehavior = ApiConventionNameMatchBehavior.Any;
    private string[] _names = Array.Empty<string>();

    /// <summary>
    ///     Create a method build for the given <see cref="RestfulApiMethod" />.
    /// </summary>
    /// <param name="method"></param>
    public RestfulApiMethodBuilder(RestfulApiMethod method)
    {
        _method = method;
    }

    /// <summary>
    ///     Match against one of the given suffixes
    /// </summary>
    /// <param name="value">The first suffix</param>
    /// <param name="values">Any additional suffixes</param>
    /// <returns></returns>
    public RestfulApiMethodBuilder MatchSuffix(string value, params string[] values)
    {
        _names = new[] { value, }.Concat(values).ToArray();
        _nameMatchBehavior = ApiConventionNameMatchBehavior.Suffix;

        return this;
    }

    /// <summary>
    ///     Match against one of the given prefixes
    /// </summary>
    /// <param name="value">The first prefix</param>
    /// <param name="values">Any additional prefixes</param>
    /// <returns></returns>
    public RestfulApiMethodBuilder MatchPrefix(string value, params string[] values)
    {
        _names = new[] { value, }.Concat(values).ToArray();
        _nameMatchBehavior = ApiConventionNameMatchBehavior.Prefix;

        return this;
    }

    /// <summary>
    ///     Match based one of the given method names
    /// </summary>
    /// <param name="value">The first name</param>
    /// <param name="values">Any additional names</param>
    /// <returns></returns>
    public RestfulApiMethodBuilder MatchName(string value, params string[] values)
    {
        _names = new[] { value, }.Concat(values).ToArray();
        _nameMatchBehavior = ApiConventionNameMatchBehavior.Exact;

        return this;
    }

    /// <summary>
    ///     Matched on the names of the parameter at the given index
    /// </summary>
    /// <remarks>
    ///     The index can be either positive or negative to allow for comparing against first or last parameters
    /// </remarks>
    /// <param name="parameter">The parameter, maybe a positive or negative <see cref="Index" /></param>
    /// <param name="value">The first name</param>
    /// <param name="values">Any additional names</param>
    /// <returns></returns>
    public RestfulApiMethodBuilder MatchParameterName(Index parameter, string value, params string[] values)
    {
        if (!_parameters.TryGetValue(parameter, out var item)) item = _parameters[parameter] = DefaultMatcher(parameter);

        _parameters[parameter] = new RestfulApiParameterMatcher(
            parameter,
            ApiConventionNameMatchBehavior.Exact,
            new[] { value, }.Concat(values).ToArray(),
            item.TypeMatch,
            item.Type
        );

        return this;
    }

    /// <summary>
    ///     Match only if a parameter exists at the given index
    /// </summary>
    /// <remarks>
    ///     The index can be either positive or negative to allow for comparing against first or last parameters
    /// </remarks>
    /// <param name="parameter">The parameter, maybe a positive or negative <see cref="Index" /></param>
    /// <returns></returns>
    public RestfulApiMethodBuilder HasParameter(Index parameter)
    {
        if (!_parameters.TryGetValue(parameter, out var item)) item = _parameters[parameter] = DefaultMatcher(parameter);

        _parameters[parameter] =
            new RestfulApiParameterMatcher(
                parameter,
                item.NameMatch,
                item.Names.Length > 0 ? item.Names : Array.Empty<string>(),
                item.TypeMatch,
                item.Type
            );

        return this;
    }

    /// <summary>
    ///     Match only if there are a given number of parameters
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public RestfulApiMethodBuilder MatchParameterCount(int count)
    {
        _parameterCount = count;
        return this;
    }

    /// <summary>
    ///     Match against the prefixes for the given parameter at the given index
    /// </summary>
    /// <remarks>
    ///     The index can be either positive or negative to allow for comparing against first or last parameters
    /// </remarks>
    /// <param name="parameter">The parameter, maybe a positive or negative <see cref="Index" /></param>
    /// <param name="value">The first prefix</param>
    /// <param name="values">Any additional prefixes</param>
    /// <returns></returns>
    public RestfulApiMethodBuilder MatchParameterPrefix(Index parameter, string value, params string[] values)
    {
        if (!_parameters.TryGetValue(parameter, out var item)) item = _parameters[parameter] = DefaultMatcher(parameter);

        _parameters[parameter] = new RestfulApiParameterMatcher(
            parameter,
            ApiConventionNameMatchBehavior.Prefix,
            new[] { value, }.Concat(values).ToArray(),
            item.TypeMatch,
            item.Type
        );

        return this;
    }

    /// <summary>
    ///     Match against the suffixes for the given parameter at the given index
    /// </summary>
    /// <remarks>
    ///     The index can be either positive or negative to allow for comparing against first or last parameters
    /// </remarks>
    /// <param name="parameter">The parameter, maybe a positive or negative <see cref="Index" /></param>
    /// <param name="value">The first suffix</param>
    /// <param name="values">Any additional suffixes</param>
    /// <returns></returns>
    public RestfulApiMethodBuilder MatchParameterSuffix(Index parameter, string value, params string[] values)
    {
        if (!_parameters.TryGetValue(parameter, out var item)) item = _parameters[parameter] = DefaultMatcher(parameter);

        _parameters[parameter] = new RestfulApiParameterMatcher(
            parameter,
            ApiConventionNameMatchBehavior.Suffix,
            new[] { value, }.Concat(values).ToArray(),
            item.TypeMatch,
            item.Type
        );

        return this;
    }

    /// <summary>
    ///     Match the type parameter at the given index.
    /// </summary>
    /// <remarks>
    ///     The index can be either positive or negative to allow for comparing against first or last parameters
    /// </remarks>
    /// <param name="parameter">The parameter, maybe a positive or negative <see cref="Index" /></param>
    /// <param name="type">The type of the parameter</param>
    /// <returns></returns>
    public RestfulApiMethodBuilder MatchParameterType(Index parameter, INamedTypeSymbol type)
    {
        if (!_parameters.TryGetValue(parameter, out var item)) item = _parameters[parameter] = DefaultMatcher(parameter);

        _parameters[parameter] = new RestfulApiParameterMatcher(
            parameter,
            item.NameMatch,
            item.Names,
            ApiConventionTypeMatchBehavior.AssignableFrom,
            type
        );

        return this;
    }

    internal bool IsValid()
    {
        return _names.Length > 0;
    }

    internal bool IsMatch(ActionModel actionModel)
    {
        var nameMatch = _nameMatchBehavior switch
                        {
                            ApiConventionNameMatchBehavior.Exact => _names.Any(name => actionModel.ActionName.Equals(name, StringComparison.OrdinalIgnoreCase)),
                            ApiConventionNameMatchBehavior.Prefix => _names.Any(
                                name => actionModel.ActionName.StartsWith(name, StringComparison.OrdinalIgnoreCase)
                            ),
                            ApiConventionNameMatchBehavior.Suffix => _names.Any(
                                name => actionModel.ActionName.EndsWith(name, StringComparison.OrdinalIgnoreCase)
                            ),
                            _ => true,
                        };

        if (!nameMatch)
            return false;

        var parameters = actionModel.ActionMethod.Parameters;
        if (_parameterCount.HasValue && parameters.Length != _parameterCount.Value) return false;

        if (parameters.Length >= _parameters.Count) return _parameters.Values.All(z => z.IsMatch(actionModel));

        return false;
    }

    RestfulApiMethod IRestfulApiMethodMatcher.Method => _method;
    ApiConventionNameMatchBehavior IRestfulApiMethodMatcher.NameMatch => _nameMatchBehavior;
    string[] IRestfulApiMethodMatcher.Names => _names;
    IDictionary<Index, IRestfulApiParameterMatcher> IRestfulApiMethodMatcher.Parameters => _parameters;

    bool IRestfulApiMethodMatcher.IsMatch(ActionModel actionModel)
    {
        return IsMatch(actionModel);
    }
}

internal record ActionModel(Compilation Compilation, string ActionName, IMethodSymbol ActionMethod);