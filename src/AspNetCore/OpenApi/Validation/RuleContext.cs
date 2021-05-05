// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation.Validators;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Reflection;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi.Validation
{
    /// <summary>
    /// RuleContext.
    /// </summary>
    public class RuleContext
    {
        /// <summary>
        /// Swagger schema.
        /// </summary>
        public OpenApiSchema Schema { get; }

        /// <summary>
        /// Property name.
        /// </summary>
        public string PropertyKey { get; }

        /// <summary>
        /// Property validator.
        /// </summary>
        public IPropertyValidator PropertyValidator { get; }

        /// <summary>
        /// Gets value indicating that <see cref="PropertyValidator"/> should be applied to collection item instead of property.
        /// </summary>
        public bool IsCollectionValidator { get; }

        /// <summary>
        /// Gets target property schema.
        /// </summary>
        public OpenApiSchema Property => !IsCollectionValidator ? Schema.Properties[PropertyKey] : Schema.Properties[PropertyKey].Items;

        /// <summary>
        /// The runtime type of the schema
        /// </summary>
        public Type SchemaType { get; }

        /// <summary>
        /// The associated member info
        /// </summary>
        public MemberInfo MemberInfo { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleContext"/> class.
        /// </summary>
        /// <param name="schema">Swagger schema.</param>
        /// <param name="propertyKey">Property name.</param>
        /// <param name="propertyValidator">Property validator.</param>
        /// <param name="schemaType">The schema type.</param>
        /// <param name="schemaMemberInfo">The schema member info</param>
        /// <param name="isCollectionValidator">Should be applied to collection items.</param>
        public RuleContext(
            OpenApiSchema schema,
            string propertyKey,
            IPropertyValidator propertyValidator,
            Type schemaType,
            MemberInfo schemaMemberInfo,
            bool isCollectionValidator = false)
        {
            Schema = schema;
            PropertyKey = propertyKey;
            PropertyValidator = propertyValidator;
            IsCollectionValidator = isCollectionValidator;
            SchemaType = schemaType;
            MemberInfo = schemaMemberInfo;
        }
    }
}