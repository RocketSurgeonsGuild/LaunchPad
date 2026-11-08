using System.Reflection;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using ZeroAlloc.Validation;

namespace Sample.Shared;

/// <summary>
///     Validates the model of the surrounding <see cref="EditForm" /> with the ZeroAlloc validator registered for it, and reports the failures
///     through the form's <see cref="EditContext" /> so the validation summary and messages show them.
/// </summary>
public sealed class ZeroAllocValidator : ComponentBase, IDisposable
{
    private static readonly MethodInfo ValidateMethod = typeof(ZeroAllocValidator).GetMethod(nameof(Validate), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static ValidationFailure[] Validate<TModel>(IServiceProvider services, TModel model) =>
        services.GetService<ValidatorFor<TModel>>()?.Validate(model).Failures.ToArray() ?? [];

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

    private ValidationFailure[] ValidateModel(object model) =>
        (ValidationFailure[])ValidateMethod.MakeGenericMethod(model.GetType()).Invoke(null, [Services, model])!;

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
