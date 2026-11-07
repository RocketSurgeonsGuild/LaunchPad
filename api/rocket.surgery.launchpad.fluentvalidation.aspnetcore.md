# Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore

- Package: `Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore`
- Version: `2.0.1-beta.314`
- Target framework: `net10.0`
- NuGet: [Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore](https://www.nuget.org/packages/Rocket.Surgery.LaunchPad.FluentValidation.AspNetCore)

## Namespaces

### Rocket.Surgery.LaunchPad.AspNetCore.FluentValidation.OpenApi

- [BetweenPropertyRule](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/betweenpropertyrule.md) -- `class` `sealed` -- Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.
- [ComparisonPropertyRule](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/comparisonpropertyrule.md) -- `class` `sealed` -- Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.
- [EmailPropertyRule](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/emailpropertyrule.md) -- `class` `sealed` -- Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.
- [FluentValidationOpenApiSchemaTransformer](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationopenapischematransformer.md) -- `class` -- Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.
- [IPropertyRuleHandler](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/ipropertyrulehandler.md) -- `interface`
- [LengthPropertyRule](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/lengthpropertyrule.md) -- `class` `sealed` -- Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.
- [NotEmptyPropertyRule](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/notemptypropertyrule.md) -- `class` `sealed` -- Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.
- [OpenApiValidationContext](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/openapivalidationcontext.md) -- `record` -- Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.
- [RegularExpressionPropertyRule](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/regularexpressionpropertyrule.md) -- `class` `sealed` -- Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.
- [RequiredPropertyRuleHandler](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/requiredpropertyrulehandler.md) -- `class` `sealed` -- Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.
- [ServiceCollectionExtensions](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/servicecollectionextensions.md) -- `class` `static` -- Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

### Rocket.Surgery.LaunchPad.AspNetCore.Joints

- [FluentValidationJoint](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationjoint.md) -- `class` -- Wires FluentValidation into ASP.NET Core: the client side adapters, the validation exception filter, the problem details converter, and the [FluentValidationProblemDetails](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md) as the documented validation response of the restful api controllers. Implements the `Joints.IServiceJoint`
- [FluentValidationOpenApiJoint](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationopenapijoint.md) -- `class` -- Adds the experimental FluentValidation schema transformer to the OpenAPI document, so the validation rules show in the schemas. Implements the `Joints.IServiceJoint`
- [FluentValidationProblemDetailsJoint](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetailsjoint.md) -- `class` -- Maps the FluentValidation `ValidationException` to an unprocessable entity, and registers the problem details writers that describe validation failures with [FluentValidationProblemDetails](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md). Implements the `Joints.IServiceJoint`

### Rocket.Surgery.LaunchPad.AspNetCore.Validation

- [FluentValidationProblemDetails](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md) -- `class` -- Problem Details for Fluent Validation
- [ValidationExceptionFilter](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/validationexceptionfilter.md) -- `class` -- Captures the validation exception
- [ValidationProblemDetailsConverter](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/validationproblemdetailsconverter.md) -- `class` `sealed` -- A RFC 7807 compliant `Serialization.JsonConverter` for [FluentValidationProblemDetails](/LaunchPad/api/rocket.surgery.launchpad.fluentvalidation.aspnetcore/fluentvalidationproblemdetails.md).
