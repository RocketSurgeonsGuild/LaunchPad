using FluentValidation.Validators;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi.Validation.FluentValidation;

/// <summary>
///     <see cref="FluentValidationRule" /> extensions.
/// </summary>
public static class FluentValidationRuleExtensions
{
    /// <summary>
    ///     Adds match predicate.
    /// </summary>
    /// <param name="rule">Source rule.</param>
    /// <param name="validatorPredicate">Validator selector.</param>
    /// <returns>New rule instance.</returns>
    public static FluentValidationRule WithCondition(this FluentValidationRule rule, Func<IPropertyValidator, bool> validatorPredicate)
    {
        var matches = rule.Conditions.Append(validatorPredicate).ToArray();
        return new FluentValidationRule(rule.Name, matches, rule.Apply);
    }

    /// <summary>
    ///     Sets <see cref="WithApply" /> action.
    /// </summary>
    /// <param name="rule">Source rule.</param>
    /// <param name="applyAction">New apply action.</param>
    /// <returns>New rule instance.</returns>
    public static FluentValidationRule WithApply(this FluentValidationRule rule, Action<RuleContext> applyAction)
    {
        return new FluentValidationRule(rule.Name, rule.Conditions, applyAction);
    }
}
