using System.Globalization;
using FluentValidation;
using FluentValidation.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Blazor;

/// <summary>
///     Blazor FluentValidator component
/// </summary>
public class FluentValidator : ComponentBase
{
    private static readonly char[] separators = { '.', '[' };

    private static FieldIdentifier ToFieldIdentifier(EditContext editContext, string propertyPath)
    {
        // This code is taken from an article by Steve Sanderson (https://blog.stevensanderson.com/2019/09/04/blazor-fluentvalidation/)
        // all credit goes to him for this code.

        // This method parses property paths like 'SomeProp.MyCollection[123].ChildProp'
        // and returns a FieldIdentifier which is an (instance, propName) pair. For example,
        // it would return the pair (SomeProp.MyCollection[123], "ChildProp"). It traverses
        // as far into the propertyPath as it can go until it finds any null instance.

        var obj = editContext.Model;

        while (true)
        {
            var nextTokenEnd = propertyPath.IndexOfAny(separators);
            if (nextTokenEnd < 0)
            {
                return new FieldIdentifier(obj, propertyPath);
            }

            var nextToken = propertyPath.Substring(0, nextTokenEnd);
            propertyPath = propertyPath.Substring(nextTokenEnd + 1);

            object? newObj;
            if (nextToken.EndsWith("]", StringComparison.OrdinalIgnoreCase))
            {
                // It's an indexer
                // This code assumes C# conventions (one indexer named Item with one param)
                nextToken = nextToken.Substring(0, nextToken.Length - 1);
                var prop = obj.GetType().GetProperty("Item");
                var indexerType = prop!.GetIndexParameters()[0].ParameterType;
                var indexerValue = Convert.ChangeType(nextToken, indexerType, CultureInfo.InvariantCulture);
                newObj = prop.GetValue(obj, new[] { indexerValue });
            }
            else
            {
                // It's a regular property
                var prop = obj.GetType().GetProperty(nextToken);
                if (prop == null)
                {
                    throw new InvalidOperationException($"Could not find property named {nextToken} on object of type {obj.GetType().FullName}.");
                }

                newObj = prop.GetValue(obj);
            }

            if (newObj == null)
            {
                // This is as far as we can go
                return new FieldIdentifier(obj, nextToken);
            }

            obj = newObj;
        }
    }

    /// <summary>
    ///     The validator to validate against
    /// </summary>
    [Parameter]
    public IValidator Validator { get; set; } = null!;

    [Inject] private IValidatorFactory ValidatorFactory { get; set; } = null!;

    [CascadingParameter] private EditContext CurrentEditContext { get; set; } = null!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (CurrentEditContext == null)
        {
            throw new InvalidOperationException(
                $"{nameof(FluentValidator)} requires a cascading " +
                $"parameter of type {nameof(EditContext)}. For example, you can use {nameof(FluentValidator)} " +
                $"inside an {nameof(EditForm)}."
            );
        }

        AddFluentValidation(Validator);
    }

    private void AddFluentValidation(IValidator validator)
    {
        var messages = new ValidationMessageStore(CurrentEditContext);

        CurrentEditContext.OnValidationRequested +=
            (_, _) => ValidateModel(messages, validator);

        CurrentEditContext.OnFieldChanged +=
            (_, eventArgs) => ValidateField(messages, eventArgs.FieldIdentifier, validator);
    }

    private async void ValidateModel(ValidationMessageStore messages, IValidator? validator = null)
    {
        validator ??= GetValidatorForModel(CurrentEditContext.Model);

        if (validator != null)
        {
            var context = new ValidationContext<object>(CurrentEditContext.Model);

            var validationResults = await validator.ValidateAsync(context);

            messages.Clear();
            foreach (var validationResult in validationResults.Errors)
            {
                var fieldIdentifier = ToFieldIdentifier(CurrentEditContext, validationResult.PropertyName);
                messages.Add(fieldIdentifier, validationResult.ErrorMessage);
            }

            CurrentEditContext.NotifyValidationStateChanged();
        }
    }

    private async void ValidateField(
        ValidationMessageStore messages,
        FieldIdentifier fieldIdentifier,
        IValidator? validator = null
    )
    {
        var properties = new[] { fieldIdentifier.FieldName };
        var context = new ValidationContext<object>(fieldIdentifier.Model, new PropertyChain(), new MemberNameValidatorSelector(properties));

        validator ??= GetValidatorForModel(fieldIdentifier.Model);

        if (validator != null)
        {
            var validationResults = await validator.ValidateAsync(context);

            messages.Clear(fieldIdentifier);
            messages.Add(fieldIdentifier, validationResults.Errors.Select(error => error.ErrorMessage));

            CurrentEditContext.NotifyValidationStateChanged();
        }
    }

    private IValidator? GetValidatorForModel(object? model)
    {
        return model == null ? null : ValidatorFactory.GetValidator(model.GetType());
    }
}
