using System.Reflection;
using FluentValidation.AspNetCore;
using FluentValidation.Validators;
using MicroElements.OpenApi.FluentValidation;
using MicroElements.Swashbuckle.FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Extensions;
using Rocket.Surgery.LaunchPad.AspNetCore.Validation;
using Rocket.Surgery.LaunchPad.Foundation.Validation;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions;

/// <summary>
///     ValidationConvention.
///     Implements the <see cref="IServiceConvention" />
/// </summary>
/// <seealso cref="IServiceConvention" />
/// <seealso cref="IServiceConvention" />
[PublicAPI]
[ExportConvention]
[AfterConvention(typeof(AspNetCoreConvention))]
public partial class FluentValidationConvention : IServiceConvention
{
    private static void AddFluentValidationRules(IServiceCollection services)
    {
        services.AddSingleton(
            new FluentValidationRule("NotEmpty")
               .WithCondition(propertyValidator => propertyValidator is INotEmptyValidator)
               .WithApply(
                    context =>
                    {
                        var ruleContext = ((ValidationRuleContext)context
                                                                 .GetType()
                                                                 .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
                                                                 .First(z => z.PropertyType == typeof(ValidationRuleContext))
                                                                 .GetValue(context)!)
                           .GetReflectionContext();
                        var propertyType = ruleContext?.PropertyInfo?.DeclaringType;
                        if (propertyType == typeof(string))
                        {
                            context.Schema.Properties[context.PropertyKey].MinLength = 1;
                        }
                    }
                )
        );

        services.AddSingleton(
            new FluentValidationRule("ValueTypeOrEnum")
               .WithApply(
                    context =>
                    {
                        var ruleContext = ((ValidationRuleContext)context
                                                                .GetType()
                                                                .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
                                                                .First(z => z.PropertyType == typeof(ValidationRuleContext))
                                                                .GetValue(context)!)
                           .GetReflectionContext();
                        var propertyType = ruleContext?.PropertyInfo?.DeclaringType;
                        if (propertyType != null &&
                            ( ( propertyType.IsValueType && Nullable.GetUnderlyingType(propertyType) == null ) ||
                              propertyType.IsEnum ))
                        {
                            context.Schema.Required.Add(context.PropertyKey);
                            context.Schema.Properties[context.PropertyKey].Nullable = false;
                        }
                    }
                )
        );

        services.AddSingleton(
            new FluentValidationRule("Nullable")
               .WithApply(
                    context =>
                    {
                        var ruleContext = ((ValidationRuleContext)context
                                                                 .GetType()
                                                                 .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
                                                                 .First(z => z.PropertyType == typeof(ValidationRuleContext))
                                                                 .GetValue(context)!)
                           .GetReflectionContext();
                        context.Schema.Properties[context.PropertyKey].Nullable =
                            context.PropertyValidator is not (INotNullValidator or INotEmptyValidator)
                         || ( ruleContext.PropertyInfo is FieldInfo fi && getNullableValue(fi.GetNullability(), fi.FieldType) )
                            ;

                        static bool getNullableValue(Nullability nullability, Type propertyType)
                        {
                            return nullability switch
                            {
                                Nullability.Nullable    => true,
                                Nullability.NonNullable => false,
                                Nullability.NotDefined  => !propertyType.IsValueType || Nullable.GetUnderlyingType(propertyType) is not null,
                                _                       => false
                            };
                        }
                    }
                )
        );

        services.AddSingleton(
            new FluentValidationRule("IsOneOf")
               .WithCondition(propertyValidator => propertyValidator is IStringInValidator)
               .WithApply(
                    context =>
                    {
                        var validator = context.PropertyValidator as IStringInValidator;
                        context.Schema.Properties[context.PropertyKey].Enum =
                            // ReSharper disable once NullableWarningSuppressionIsUsed
                            validator!.Values.Select(x => new OpenApiString(x)).Cast<IOpenApiAny>().ToList();
                    }
                )
        );
    }

    /// <summary>
    ///     Registers the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configuration"></param>
    /// <param name="services"></param>
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        services.AddFluentValidationClientsideAdapters();
        services
           .Configure<MvcOptions>(mvcOptions => mvcOptions.Filters.Insert(0, new ValidationExceptionFilter()))
           .Configure<JsonOptions>(options => options.JsonSerializerOptions.Converters.Add(new ValidationProblemDetailsConverter()));

        AddFluentValidationRules(services);
    }
}
