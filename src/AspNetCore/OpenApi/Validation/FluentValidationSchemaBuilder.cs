// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Rocket.Surgery.LaunchPad.AspNetCore.OpenApi.Validation.Core;
using Rocket.Surgery.LaunchPad.AspNetCore.OpenApi.Validation.FluentValidation;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi.Validation
{
    /// <summary>
    /// The context used to investigate type information about a given schema node.
    /// </summary>
    /// <param name="Type"></param>
    /// <param name="MemberInfo"></param>
    /// <param name="SchemaGenerator"></param>
    /// <param name="SchemaRepository"></param>
    public record OpenApiValidationFilterContext(Type Type, MemberInfo MemberInfo, ISchemaGenerator SchemaGenerator, SchemaRepository SchemaRepository);

    /// <summary>
    /// OpenApi schema builder.
    /// </summary>
    public static class FluentValidationSchemaBuilder
    {
        /// <summary>
        /// Applies rules from validator.
        /// </summary>
        internal static void ApplyRulesToSchema(
            OpenApiSchema schema,
            Type schemaType,
            IEnumerable<string>? schemaPropertyNames,
            OpenApiValidationFilterContext schemaFilterContext,
            IValidator validator,
            IReadOnlyCollection<FluentValidationRule> rules,
            ILogger logger
        )
        {
            var schemaTypeName = schemaType.Name;

            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("Applying FluentValidation rules to swagger schema '{SchemaTypeName}'", schemaTypeName);

            schemaPropertyNames ??= schema.Properties?.Keys ?? Array.Empty<string>();
            foreach (var schemaPropertyName in schemaPropertyNames)
            {
                var validationRules = validator.GetValidationRulesForMemberIgnoreCase(schemaPropertyName).ToArrayDebug();
                foreach (var ruleContext in validationRules)
                {
                    var propertyValidators = ruleContext.PropertyRule.GetValidators();
                    foreach (var propertyValidator in propertyValidators)
                    {
                        foreach (var rule in rules)
                        {
                            if (rule.IsMatches(propertyValidator))
                            {
                                try
                                {
                                    var ruleHistoryItem = new RuleHistoryCache.RuleHistoryItem(
                                        schemaTypeName,
                                        schemaPropertyName,
                                        propertyValidator,
                                        rule.Name
                                    );
                                    if (!schema.ContainsRuleHistoryItem(ruleHistoryItem))
                                    {
                                        rule.Apply(
                                            new RuleContext(
                                                schema,
                                                schemaPropertyName,
                                                propertyValidator,
                                                schemaFilterContext.Type,
                                                schemaFilterContext.MemberInfo,
                                                isCollectionValidator: ruleContext.IsCollectionRule
                                            )
                                        );

                                        logger.LogDebug(
                                            "Rule '{rule.Name}' applied for property '{SchemaTypeName}.{SchemaPropertyName}'",
                                            schemaTypeName,
                                            schemaPropertyName
                                        );
                                        schema.AddRuleHistoryItem(ruleHistoryItem);
                                    }
                                    else
                                    {
                                        logger.LogDebug(
                                            "Rule '{rule.Name}' already applied for property '{SchemaTypeName}.{SchemaPropertyName}'",
                                            schemaTypeName,
                                            schemaPropertyName
                                        );
                                    }
                                }
                                catch (Exception e)
                                {
                                    logger.LogWarning(
                                        0,
                                        e,
                                        "Error on apply rule '{rule.Name}' for property '{SchemaTypeName}.{SchemaPropertyName}'",
                                        schemaTypeName,
                                        schemaPropertyName
                                    );
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static void AddRulesFromIncludedValidators(
            OpenApiSchema schema,
            OpenApiValidationFilterContext schemaFilterContext,
            IValidator validator,
            IReadOnlyCollection<FluentValidationRule> rules,
            ILogger logger
        )
        {            // Note: IValidatorDescriptor doesn't return IncludeRules so we need to get validators manually.
            var validationRules = validator
               .GetValidationRules()
               .ToArrayDebug();

            var propertiesWithChildAdapters = validationRules
               .Select(context => (context.PropertyRule, context.PropertyRule.GetValidators().OfType<IChildValidatorAdaptor>().ToArray()))
               .ToArrayDebug();

            foreach (( IValidationRule propertyRule, IChildValidatorAdaptor[] childAdapters ) in propertiesWithChildAdapters)
            {
                foreach (var childAdapter in childAdapters)
                {
                    IValidator? childValidator = childAdapter.GetValidatorFromChildValidatorAdapter();
                    if (childValidator != null)
                    {
                        var canValidateInstancesOfType = childValidator.CanValidateInstancesOfType(schemaFilterContext.Type);

                        if (canValidateInstancesOfType)
                        {
                            // It's a validator for current type (Include for example) so apply changes to current schema.
                            ApplyRulesToSchema(
                                schema: schema,
                                schemaType: schemaFilterContext.Type,
                                schemaPropertyNames: null,
                                schemaFilterContext: schemaFilterContext,
                                validator: childValidator,
                                rules: rules,
                                logger: logger
                            );

                            AddRulesFromIncludedValidators(
                                schema: schema,
                                schemaFilterContext: schemaFilterContext,
                                validator: childValidator,
                                rules: rules,
                                logger: logger
                            );
                        }
                        else
                        {
                            // It's a validator for sub schema so get schema and apply changes to it.
                            var schemaForChildValidator = GetSchemaForType(
                                schemaRepository: schemaFilterContext.SchemaRepository,
                                schemaGenerator: schemaFilterContext.SchemaGenerator,
                                schemaIdSelector: type => type.Name,
                                parameterType: propertyRule.TypeToValidate
                            );

                            ApplyRulesToSchema(
                                schema: schemaForChildValidator,
                                schemaType: propertyRule.TypeToValidate,
                                schemaPropertyNames: null,
                                schemaFilterContext: schemaFilterContext,
                                validator: childValidator,
                                rules: rules,
                                logger: logger
                            );

                            AddRulesFromIncludedValidators(
                                schema: schemaForChildValidator,
                                schemaFilterContext: schemaFilterContext,
                                validator: childValidator,
                                rules: rules,
                                logger: logger
                            );
                        }
                    }
                }
            }
        }

        internal static IValidator? GetValidatorFromChildValidatorAdapter(this IChildValidatorAdaptor childValidatorAdapter)
        {
            // Try to validator with reflection.
            var childValidatorAdapterType = childValidatorAdapter.GetType();
            var genericTypeArguments = childValidatorAdapterType.GenericTypeArguments;
            if (genericTypeArguments.Length != 2)
                return null;

            var getValidatorGeneric = typeof(FluentValidationSchemaBuilder)
               .GetMethod(nameof(GetValidatorGeneric), BindingFlags.Static | BindingFlags.NonPublic)
              ?.MakeGenericMethod(genericTypeArguments[0]);

            if (getValidatorGeneric != null)
            {
                var validator = (IValidator)getValidatorGeneric.Invoke(null, new []{ childValidatorAdapter });
                return validator;
            }

            return null;
        }

        internal static IValidator? GetValidatorGeneric<T>(this IChildValidatorAdaptor childValidatorAdapter)
        {
            // public class ChildValidatorAdaptor<T,TProperty>
            // public virtual IValidator GetValidator(ValidationContext<T> context, TProperty value) {
            var getValidatorMethodName = nameof(ChildValidatorAdaptor<object, object>.GetValidator);
            var getValidatorMethod = childValidatorAdapter.GetType().GetMethod(getValidatorMethodName);
            if (getValidatorMethod != null)
            {
                // Fake context. We have not got real context because no validation yet.
                var fakeContext = new ValidationContext<T>(default);
                object? value = null;

                var validator = (IValidator)getValidatorMethod.Invoke(childValidatorAdapter, new[] { fakeContext, value })!;
                return validator;
            }

            return null;
        }

        /// <summary>
        /// Get the schema for the given type
        /// </summary>
        /// <param name="schemaRepository"></param>
        /// <param name="schemaGenerator"></param>
        /// <param name="schemaIdSelector"></param>
        /// <param name="parameterType"></param>
        /// <returns></returns>
        public static OpenApiSchema GetSchemaForType(
            SchemaRepository schemaRepository,
            ISchemaGenerator schemaGenerator,
            Func<Type, string> schemaIdSelector,
            Type parameterType
        )
        {
            var schemaId = schemaIdSelector(parameterType);

            if (!schemaRepository.Schemas.TryGetValue(schemaId, out var schema))
            {
                schema = schemaGenerator.GenerateSchema(parameterType, schemaRepository);
            }

            if (( schema.Properties == null || schema.Properties.Count == 0 ) &&
                schemaRepository.Schemas.ContainsKey(schemaId))
            {
                schema = schemaRepository.Schemas[schemaId];
            }

            return schema;
        }
    }
}