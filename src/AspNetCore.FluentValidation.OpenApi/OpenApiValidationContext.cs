using FluentValidation.Internal;
using FluentValidation.Validators;

using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi;

[Experimental(Constants.ExperimentalId)]
public record OpenApiValidationContext
(
    OpenApiSchema TypeSchema,
    OpenApiSchema PropertySchema,
    OpenApiSchemaTransformerContext TransformerContext,
    IPropertyValidator PropertyValidator,
    string PropertyKey,
    IRuleComponent RuleComponent);
