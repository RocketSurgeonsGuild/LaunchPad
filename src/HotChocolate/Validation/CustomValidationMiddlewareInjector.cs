using FairyBread;
using HotChocolate.Configuration;
using HotChocolate.Internal;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using HotChocolate.Types.Descriptors.Definitions;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Validation;

internal class CustomValidationMiddlewareInjector : TypeInterceptor
{
    private static List<ValidatorDescriptor> DetermineValidatorsForArg(
        ICustomValidatorRegistry validatorRegistry,
        ArgumentDefinition argDef
    )
    {
        // If validation is explicitly disabled, return none so validation middleware won't be added
        if (argDef.ContextData.ContainsKey(WellKnownContextData.DontValidate))
        {
            return new List<ValidatorDescriptor>(0);
        }

        var validators = new List<ValidatorDescriptor>();

        // Include implicit validator/s first (if allowed)
        if (!argDef.ContextData.ContainsKey(WellKnownContextData.DontValidateImplicitly))
        {
            // And if we can figure out the arg's runtime type
            var argRuntimeType = TryGetArgRuntimeType(argDef);
            if (argRuntimeType is not null)
            {
                if (validatorRegistry.TryGetValidator(argRuntimeType, out var validatorDescriptor))
                {
                    validators.Add(validatorDescriptor);
                }
            }
        }

        // Include explicit validator/s (that aren't already added implicitly)
        if (argDef.ContextData.TryGetValue(WellKnownContextData.ExplicitValidatorTypes, out var explicitValidatorTypesRaw) &&
            explicitValidatorTypesRaw is IEnumerable<Type> explicitValidatorTypes)
        {
            // TODO: Potentially check and throw if there's a validator being explicitly applied for the wrong runtime type

            foreach (var validatorType in explicitValidatorTypes)
            {
                if (validators.Any(v => v.ValidatorType == validatorType))
                {
                    continue;
                }

                validators.Add(new ValidatorDescriptor(validatorType));
            }
        }

        return validators;
    }

    private static Type? TryGetArgRuntimeType(
        ArgumentDefinition argDef
    )
    {
        if (argDef.Parameter?.ParameterType is { } argRuntimeType)
        {
            return argRuntimeType;
        }

        if (argDef.Type is ExtendedTypeReference extTypeRef)
        {
            return TryGetRuntimeType(extTypeRef.Type);
        }

        return null;
    }

    private static Type? TryGetRuntimeType(IExtendedType extType)
    {
        // It's already a runtime type, .Type(typeof(int))
        if (extType.Kind == ExtendedTypeKind.Runtime)
        {
            return extType.Source;
        }

        // Array (though not sure what produces this scenario as seems to always be list)
        if (extType.IsArray)
        {
            if (extType.ElementType is null)
            {
                return null;
            }

            var elementRuntimeType = TryGetRuntimeType(extType.ElementType);
            if (elementRuntimeType is null)
            {
                return null;
            }

            return Array.CreateInstance(elementRuntimeType, 0).GetType();
        }

        // List
        if (extType.IsList)
        {
            if (extType.ElementType is null)
            {
                return null;
            }

            var elementRuntimeType = TryGetRuntimeType(extType.ElementType);
            if (elementRuntimeType is null)
            {
                return null;
            }

            return typeof(List<>).MakeGenericType(elementRuntimeType);
        }

        // Input object
        if (typeof(InputObjectType).IsAssignableFrom(extType))
        {
            var currBaseType = extType.Type.BaseType;
            while (currBaseType is not null &&
                   ( !currBaseType.IsGenericType ||
                     currBaseType.GetGenericTypeDefinition() != typeof(InputObjectType<>) ))
            {
                currBaseType = currBaseType.BaseType;
            }

            if (currBaseType is null)
            {
                return null;
            }

            return currBaseType.GenericTypeArguments[0];
        }

        // Singular scalar
        if (typeof(ScalarType).IsAssignableFrom(extType))
        {
            var currBaseType = extType.Type.BaseType;
            while (currBaseType is not null &&
                   ( !currBaseType.IsGenericType ||
                     currBaseType.GetGenericTypeDefinition() != typeof(ScalarType<>) ))
            {
                currBaseType = currBaseType.BaseType;
            }

            if (currBaseType is null)
            {
                return null;
            }

            var argRuntimeType = currBaseType.GenericTypeArguments[0];
            if (argRuntimeType.IsValueType && extType.IsNullable)
            {
                return typeof(Nullable<>).MakeGenericType(argRuntimeType);
            }

            return argRuntimeType;
        }

        return null;
    }

    private FieldMiddlewareDefinition? _validationFieldMiddlewareDef;

    public override void OnBeforeCompleteType(
        ITypeCompletionContext completionContext,
        DefinitionBase definition
    )
    {
        if (definition is not ObjectTypeDefinition objTypeDef)
        {
            return;
        }

        var validatorRegistry = completionContext.Services.GetRequiredService<ICustomValidatorRegistry>();

        foreach (var fieldDef in objTypeDef.Fields)
        {
            // Don't add validation middleware unless:
            // 1. we have args
            var needsValidationMiddleware = false;

            foreach (var argDef in fieldDef.Arguments)
            {
                // 3. there's validators for it
                List<ValidatorDescriptor> validatorDescs;
                try
                {
                    validatorDescs = DetermineValidatorsForArg(validatorRegistry, argDef);
                    if (validatorDescs.Count < 1)
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
#pragma warning disable CA2201
                    throw new Exception(
                        $"Problem getting runtime type for argument '{argDef.Name}' " +
                        $"in field '{fieldDef.Name}' on object type '{objTypeDef.Name}'.",
                        ex
                    );
#pragma warning restore CA2201
                }

                // Cleanup context now we're done with these
                foreach (var key in argDef.ContextData.Keys)
                {
                    if (key.StartsWith(WellKnownContextData.Prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        argDef.ContextData.Remove(key);
                    }
                }

                validatorDescs.TrimExcess();
                needsValidationMiddleware = true;
                argDef.ContextData[WellKnownContextData.ValidatorDescriptors] = validatorDescs.AsReadOnly();
            }

            if (needsValidationMiddleware)
            {
                if (_validationFieldMiddlewareDef is null)
                {
                    _validationFieldMiddlewareDef = new FieldMiddlewareDefinition(
                        FieldClassMiddlewareFactory.Create<ValidationMiddleware>()
                    );
                }

                fieldDef.MiddlewareDefinitions.Insert(0, _validationFieldMiddlewareDef);
            }
        }
    }
}

internal static class WellKnownContextData
{
    public const string Prefix = "FairyBread";

    public const string DontValidate =
        Prefix + ".DontValidate";

    public const string DontValidateImplicitly =
        Prefix + ".DontValidateImplicitly";

    public const string ExplicitValidatorTypes =
        Prefix + ".ExplicitValidatorTypes";

    public const string ValidatorDescriptors =
        Prefix + ".Validators";
}
