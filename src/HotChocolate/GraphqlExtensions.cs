using HotChocolate;
using HotChocolate.Data.Filters;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.Foundation;

namespace Rocket.Surgery.LaunchPad.HotChocolate;

/// <summary>
///     GraphQl extensions
/// </summary>
public static class GraphqlExtensions
{
    /// <summary>
    ///     Add problem details to the error
    /// </summary>
    /// <param name="error"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static ErrorBuilder WithProblemDetails(this ErrorBuilder error, IProblemDetailsData data)
    {
        error.SetExtension("type", "ProblemDetails");
        if (data.Title is { })
            error.SetExtension("title", data.Title);
        if (data.Link is { })
            error.SetExtension("link", data.Link);
        if (data.Instance is { })
            error.SetExtension("instance", data.Instance);
        foreach (var property in data.Properties)
        {
            if (property.Value is { }) error.SetExtension(property.Key, property.Value);
        }

        return error;
    }

    /// <summary>
    ///     Configures a strongly typed id with the GraphQL scalar type of its underlying value: converts between the id and its value, binds the
    ///     id to the scalar type, and filters it as that scalar.
    /// </summary>
    /// <remarks>
    ///     The conversions are given rather than found by reflection, so this is trim and native AOT safe:
    ///     <c>builder.ConfigureStronglyTypedId&lt;RocketId, Guid, UuidType&gt;(id =&gt; id.Value, value =&gt; new(value))</c>.
    /// </remarks>
    /// <param name="builder">The request executor builder.</param>
    /// <param name="toValue">Gets the underlying value of an id.</param>
    /// <param name="fromValue">Creates an id from its underlying value.</param>
    /// <typeparam name="TStrongType">The strongly typed id.</typeparam>
    /// <typeparam name="TValue">The underlying value of the id.</typeparam>
    /// <typeparam name="TSchemaType">The GraphQL scalar type of the underlying value.</typeparam>
    /// <returns>The builder.</returns>
    public static IRequestExecutorBuilder ConfigureStronglyTypedId<TStrongType, TValue, TSchemaType>(
        this IRequestExecutorBuilder builder,
        Func<TStrongType, TValue> toValue,
        Func<TValue, TStrongType> fromValue
    )
        where TSchemaType : ITypeDefinition
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(toValue);
        ArgumentNullException.ThrowIfNull(fromValue);
        builder.AddTypeConverter<TStrongType, TValue>(value => toValue(value));
        builder.AddTypeConverter<TValue, TStrongType>(value => fromValue(value));
        builder.BindRuntimeType<TStrongType, TSchemaType>();
        builder.AddConvention<IFilterConvention, StronglyTypedIdFilterConventionExtension<TStrongType, TSchemaType>>();
        return builder;
    }
}
