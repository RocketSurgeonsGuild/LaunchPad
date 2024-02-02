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
    private static readonly char[] _separators = { '.', '[', };

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
            var nextTokenEnd = propertyPath.IndexOfAny(_separators);
            if (nextTokenEnd < 0)
            {
                return new(obj, propertyPath);
            }

            var nextToken = propertyPath.Substring(0, nextTokenEnd);
            propertyPath = propertyPath.Substring(nextTokenEnd + 1);

            object? newObj;
            if (nextToken.EndsWith(']'))
            {
                // It's an indexer
                // This code assumes C# conventions (one indexer named Item with one param)
                nextToken = nextToken.Substring(0, nextToken.Length - 1);
                // ReSharper disable once NullableWarningSuppressionIsUsed
                var prop = obj.GetType().GetProperty("Item")!;
                var indexerType = prop.GetIndexParameters()[0].ParameterType;
                var indexerValue = Convert.ChangeType(nextToken, indexerType, CultureInfo.InvariantCulture);
                newObj = prop.GetValue(obj, new[] { indexerValue, });
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
                return new(obj, nextToken);
            }

            obj = newObj;
        }
    }

    private static void AddFluentValidation(IValidator? validator, EditContext editContext, IServiceProvider services)
    {
        var messages = new ValidationMessageStore(editContext);

        editContext.OnValidationRequested +=
            (_, _) => ValidateModel(messages, editContext, validator ?? services.GetValidator(editContext.Model.GetType()));

        editContext.OnFieldChanged +=
            (_, eventArgs) => ValidateField(messages, editContext, eventArgs.FieldIdentifier, validator ?? services.GetValidator(editContext.Model.GetType()));
    }

    private static async void ValidateModel(
        ValidationMessageStore messages,
        EditContext editContext,
        IValidator? validator = null
    )
    {
        if (validator != null)
        {
            var context = new ValidationContext<object>(editContext.Model);

            var validationResults = await validator.ValidateAsync(context);

            messages.Clear();
            foreach (var validationResult in validationResults.Errors)
            {
                var fieldIdentifier = ToFieldIdentifier(editContext, validationResult.PropertyName);
                messages.Add(fieldIdentifier, validationResult.ErrorMessage);
            }

            editContext.NotifyValidationStateChanged();
        }
    }

    private static async void ValidateField(
        ValidationMessageStore messages,
        EditContext editContext,
        FieldIdentifier fieldIdentifier,
        IValidator? validator = null
    )
    {
        if (validator != null)
        {
            var properties = new[] { fieldIdentifier.FieldName, };
            var context = new ValidationContext<object>(fieldIdentifier.Model, new(), new MemberNameValidatorSelector(properties));
            var validationResults = await validator.ValidateAsync(context);

            messages.Clear(fieldIdentifier);
            messages.Add(fieldIdentifier, validationResults.Errors.Select(error => error.ErrorMessage));

            editContext.NotifyValidationStateChanged();
        }
    }

    /// <summary>
    ///     The validator to validate against
    /// </summary>
    [Parameter]
    [DisallowNull]
    // ReSharper disable once NullableWarningSuppressionIsUsed
    public IValidator? Validator { get; set; } = null!;

    // ReSharper disable once NullableWarningSuppressionIsUsed
    [Inject]
    private IServiceProvider Services { get; set; } = null!;

    // ReSharper disable once NullableWarningSuppressionIsUsed
    [CascadingParameter]
    private EditContext CurrentEditContext { get; set; } = null!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (CurrentEditContext == null)
        {
            throw new InvalidOperationException(
                $"{nameof(FluentValidator)} requires a cascading "
              + $"parameter of type {nameof(EditContext)}. For example, you can use {nameof(FluentValidator)} "
              + $"inside an {nameof(EditForm)}."
            );
        }

        AddFluentValidation(Validator, CurrentEditContext, Services);
    }
}