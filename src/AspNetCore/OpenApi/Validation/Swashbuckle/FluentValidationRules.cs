﻿// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi.Validation.Swashbuckle
{
    /// <summary>
    /// Swagger <see cref="ISchemaFilter"/> that uses FluentValidation validators instead System.ComponentModel based attributes.
    /// </summary>
    public class FluentValidationRules : ISchemaFilter
    {
        private readonly IValidatorFactory _validatorFactory;
        private readonly FluentValidationSwaggerGenOptions _options;
        private readonly ILogger _logger;
        private readonly IReadOnlyList<FluentValidationRule> _rules;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentValidationRules"/> class.
        /// </summary>
        /// <param name="validatorFactory">The validator factory.</param>
        /// <param name="rules">External FluentValidation rules. External rule overrides default rule with the same name.</param>
        /// <param name="loggerFactory"><see cref="ILoggerFactory"/> for logging. Can be null.</param>
        /// <param name="options">Schema generation options.</param>
        public FluentValidationRules(
            IValidatorFactory validatorFactory,
            IEnumerable<FluentValidationRule>? rules = null,
            ILoggerFactory? loggerFactory = null,
            IOptions<FluentValidationSwaggerGenOptions>? options = null
        )
        {
            _validatorFactory = validatorFactory;
            _options = options?.Value ?? new FluentValidationSwaggerGenOptions();
            _logger = loggerFactory?.CreateLogger(typeof(FluentValidationRules)) ?? NullLogger.Instance;
            _rules = new DefaultFluentValidationRuleProvider(options).GetRules().ToArray().OverrideRules(rules);
        }

        /// <inheritdoc />
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            IValidator? validator = null;
            try
            {
                validator = _validatorFactory.GetValidator(context.Type);
            }
            catch (Exception e)
            {
                _logger.LogWarning(0, e, "GetValidator for type '{Type}' fails", context.Type);
            }

            if (validator == null)
                return;

            var localContext = new OpenApiValidationFilterContext(context.Type, context.MemberInfo, context.SchemaGenerator, context.SchemaRepository);
            ApplyRulesToSchema(schema, localContext, validator);

            try
            {
                AddRulesFromIncludedValidators(schema, localContext, validator);
            }
            catch (Exception e)
            {
                _logger.LogWarning(0, e, "Applying IncludeRules for type '{Type}' fails", context.Type);
            }
        }

        private void ApplyRulesToSchema(OpenApiSchema schema, OpenApiValidationFilterContext context, IValidator validator)
            => FluentValidationSchemaBuilder.ApplyRulesToSchema(
                schema: schema,
                schemaType: context.Type,
                schemaPropertyNames: null,
                schemaFilterContext: context,
                validator: validator,
                rules: _rules,
                logger: _logger
            );

        private void AddRulesFromIncludedValidators(OpenApiSchema schema, OpenApiValidationFilterContext context, IValidator validator)
            => FluentValidationSchemaBuilder.AddRulesFromIncludedValidators(
                schema: schema,
                schemaFilterContext: context,
                validator: validator,
                rules: _rules,
                logger: _logger
            );
    }
}