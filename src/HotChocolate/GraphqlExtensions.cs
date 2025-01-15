using System.Linq.Expressions;
using System.Reflection;
using HotChocolate;
using HotChocolate.Data.Filters;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using HotChocolate.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.LaunchPad.Primitives;

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
    public static IErrorBuilder WithProblemDetails(this IErrorBuilder error, IProblemDetailsData data)
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
    ///     Configures a generated strongly typed id type with the given graphql schema type
    /// </summary>
    /// <remarks>
    ///     Adds converters, binding and filters.
    /// </remarks>
    /// <param name="builder"></param>
    /// <typeparam name="TStrongType"></typeparam>
    /// <typeparam name="TSchemaType"></typeparam>
    /// <returns></returns>
    public static IRequestExecutorBuilder ConfigureStronglyTypedId<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicConstructors)]
        TStrongType,
        TSchemaType>(this IRequestExecutorBuilder builder)
        where TSchemaType : INamedType
    {
        AddTypeConversion<TStrongType>(builder);
        builder.BindRuntimeType<TStrongType, TSchemaType>();
        builder.AddConvention<IFilterConvention, StronglyTypedIdFilterConventionExtension<TStrongType, TSchemaType>>();
        return builder;
    }


    // TOOD: Make a source generator for this.
    private static readonly MethodInfo AddTypeConverterMethod = typeof(RequestExecutorBuilderExtensions)
                                                               .GetMethods()
                                                               .Single(
                                                                    z => z.Name == "AddTypeConverter"
                                                                     && z.ReturnType == typeof(IRequestExecutorBuilder)
                                                                     && z.IsGenericMethod
                                                                     && z.GetGenericMethodDefinition().GetGenericArguments().Length == 2
                                                                     && z.GetParameters().Length == 2
                                                                );

    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "only working with public properties and constructors"
    )]
    private static void AddTypeConversion<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicConstructors)]
        TStrongType>(
        IRequestExecutorBuilder builder
    )
    {
        // ReSharper disable once NullableWarningSuppressionIsUsed
        var underlyingType = typeof(TStrongType).GetProperty("Value")!.PropertyType;

        {
            var value = Expression.Parameter(typeof(TStrongType), "value");
            var delegateType = typeof(ChangeType<,>).MakeGenericType(typeof(TStrongType), underlyingType);

            AddTypeConverterMethod
               .MakeGenericMethod(typeof(TStrongType), underlyingType)
               .Invoke(
                    null,
                    new object[] { builder, Expression.Lambda(delegateType, Expression.Property(value, "Value"), false, value).Compile(), }
                );
        }

        {
            var value = Expression.Parameter(underlyingType, "value");
            var delegateType = typeof(ChangeType<,>).MakeGenericType(underlyingType, typeof(TStrongType));

            // ReSharper disable once NullableWarningSuppressionIsUsed
            var constructor = typeof(TStrongType).GetConstructor(new[] { underlyingType, })!;
            AddTypeConverterMethod
               .MakeGenericMethod(underlyingType, typeof(TStrongType))
               .Invoke(
                    null,
                    new object[] { builder, Expression.Lambda(delegateType, Expression.New(constructor, value), false, value).Compile(), }
                );
        }
    }
}