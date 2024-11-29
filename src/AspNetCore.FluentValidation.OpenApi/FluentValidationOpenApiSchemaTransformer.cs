using FluentValidation;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

[Experimental(Constants.ExperimentalId)]
public class FluentValidationOpenApiSchemaTransformer(IEnumerable<IPropertyRuleHandler> ruleDefinitionHandlers) : IOpenApiSchemaTransformer
{
    public async Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(context.JsonTypeInfo.Type);
        if (context.ApplicationServices.GetService(validatorType) is not IValidator validator) return;

        var descriptor = validator.CreateDescriptor();
        foreach (var (validators, propertyKey, property) in descriptor
                                                   .GetMembersWithValidators()
                                                   .Join(
                                                        schema.Properties,
                                                        z => z.Key,
                                                        z => z.Key,
                                                        (z, y) => ( validators: z.AsEnumerable(), property: y.Key, schema: y.Value ),
                                                        StringComparer.OrdinalIgnoreCase
                                                    ))

        {
            foreach (var (propertyValidator, component) in validators)
            {
                foreach (var item in ruleDefinitionHandlers)
                {
                    var ctx = new OpenApiValidationContext(
                        schema,
                        property,
                        context,
                        propertyValidator,
                        propertyKey,
                        component
                    );
                    await item.HandleAsync(ctx, cancellationToken);
                }
            }
        }
    }
}
