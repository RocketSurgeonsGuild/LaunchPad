// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi.Validation;

/// <summary>
///     Provides rules for schema generation.
/// </summary>
public interface IFluentValidationRuleProvider
{
    /// <summary>
    ///     Gets rules for schema generation.
    /// </summary>
    /// <returns>Enumeration of <see cref="FluentValidationRule" />.</returns>
    IEnumerable<FluentValidationRule> GetRules();
}
