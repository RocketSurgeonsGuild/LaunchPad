using System.Reflection;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using Microsoft.Extensions.DependencyInjection;

using ZeroAlloc.Validation;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Blazor;

/// <summary>
///     Validates the model of the surrounding <see cref="EditForm" /> with the ZeroAlloc validator registered for it, and reports the failures
///     through the form's <see cref="EditContext" /> so the validation summary and messages show them.
/// </summary>
/// <remarks>
///     The validator is resolved as <see cref="ValidatorFor{T}" /> of the model's runtime type, which needs that closed generic type at run time.
///     ZeroAlloc.Validation registers a validator for each <c>[Validate]</c> type, so a trimmed application keeps the types it validates.
///     A model without a registered validator is an error. A view model <c>[Mutable]</c> fills in has one: Foundation's
///     <c>LaunchPadMutableValidatorsJoint</c> registers the <c>Validator</c> the generator nests in it, which validates the value the view
///     model makes, and the failures go to its properties of the same name, so the form binds to the view model and the request's rules
///     apply.
/// </remarks>
[PublicAPI]
public sealed class ZeroAllocValidator : ComponentBase, IDisposable
{
    private static readonly MethodInfo ValidateMethod = typeof(ZeroAllocValidator).GetMethod(nameof(Validate), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static ValidationFailure[] Validate<TModel>(IServiceProvider services, TModel model) =>
        services.GetRequiredService<ValidatorFor<TModel>>().Validate(model).Failures.ToArray();

    private EditContext? _editContext;
    private ValidationMessageStore? _messages;

    [CascadingParameter]
    private EditContext? CurrentEditContext { get; set; }

    [Inject]
    private IServiceProvider Services { get; set; } = null!;

    /// <inheritdoc />
    public void Dispose() => Detach();

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (ReferenceEquals(CurrentEditContext, _editContext)) return;
        Detach();
        _editContext = CurrentEditContext ?? throw new InvalidOperationException($"{nameof(ZeroAllocValidator)} must be placed inside an {nameof(EditForm)}.");
        _messages = new(_editContext);
        _editContext.OnValidationRequested += OnValidationRequested;
        _editContext.OnFieldChanged += OnFieldChanged;
    }

    private void Detach()
    {
        if (_editContext is null) return;
        _editContext.OnValidationRequested -= OnValidationRequested;
        _editContext.OnFieldChanged -= OnFieldChanged;
        _editContext = null;
    }

    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "ValidatorFor<T> is registered for each validated model type, so its instantiation exists.")]
    [UnconditionalSuppressMessage("Trimming", "IL2060", Justification = "Validate<TModel> has no trimming requirements on TModel.")]
    private ValidationFailure[] ValidateModel(object model) => (ValidationFailure[])ValidateMethod.MakeGenericMethod(model.GetType()).Invoke(null, BindingFlags.DoNotWrapExceptions, null, [Services, model], null)!;

    private void OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
    {
        var context = _editContext!;
        _messages!.Clear();
        foreach (var failure in ValidateModel(context.Model))
        {
            _messages.Add(context.Field(failure.PropertyName), failure.ErrorMessage);
        }

        context.NotifyValidationStateChanged();
    }

    private void OnFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        var context = _editContext!;
        _messages!.Clear(e.FieldIdentifier);
        foreach (var failure in ValidateModel(context.Model).Where(failure => failure.PropertyName == e.FieldIdentifier.FieldName))
        {
            _messages.Add(e.FieldIdentifier, failure.ErrorMessage);
        }

        context.NotifyValidationStateChanged();
    }
}
