// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation.Validators;
using System;
using System.Collections.Generic;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi.Validation
{
    /// <summary>
    /// FluentValidationRule.
    /// </summary>
    public class FluentValidationRule
    {
        /// <summary>
        /// Gets rule name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets predicates that checks validator is matches rule.
        /// </summary>
        public IReadOnlyCollection<Func<IPropertyValidator, bool>> Conditions { get; }

        /// <summary>
        /// Gets action that modifies swagger schema.
        /// </summary>
        public Action<RuleContext> Apply { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentValidationRule"/> class.
        /// </summary>
        /// <param name="name">Rule name.</param>
        /// <param name="matches">Validator predicates.</param>
        /// <param name="apply">Apply rule to schema action.</param>
        public FluentValidationRule(
            string name,
            IReadOnlyCollection<Func<IPropertyValidator, bool>>? matches = null,
            Action<RuleContext>? apply = null)
        {
            Name = name;
            Conditions = matches ?? Array.Empty<Func<IPropertyValidator, bool>>();
            Apply = apply ?? (context => { });
        }

        /// <summary>
        /// Checks that validator is matches rule.
        /// </summary>
        /// <param name="validator">Validator.</param>
        /// <returns>True if validator matches rule.</returns>
        public bool IsMatches(IPropertyValidator validator)
        {
            foreach (var match in Conditions)
            {
                if (!match(validator))
                    return false;
            }

            return true;
        }
    }
}