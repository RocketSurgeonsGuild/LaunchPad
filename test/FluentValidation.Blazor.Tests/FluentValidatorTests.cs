using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Rocket.Surgery.LaunchPad.AspNetCore.Blazor;

namespace FluentValidation.Blazor.Tests;

public class FluentValidatorTests
{
    [Test]
    public async Task Adds_The_Messages_Of_The_Registered_Validator_When_The_Form_Is_Validated()
    {
        var model = new Person { Name = "", Address = new() { Street = "" } };
        var editContext = new EditContext(model);
        await using var renderer = await Render(editContext, services => services.AddSingleton<IValidator<Person>, PersonValidator>());

        editContext.Validate().ShouldBeFalse();

        editContext.GetValidationMessages(editContext.Field(nameof(Person.Name))).ShouldBe(["Name is required"]);
        editContext.GetValidationMessages(new FieldIdentifier(model.Address, nameof(Address.Street))).ShouldBe(["Street is required"]);
    }

    [Test]
    public async Task Validates_Only_The_Field_That_Changed()
    {
        var model = new Person { Name = "", Address = new() { Street = "" } };
        var editContext = new EditContext(model);
        await using var renderer = await Render(editContext, services => services.AddSingleton<IValidator<Person>, PersonValidator>());

        editContext.NotifyFieldChanged(editContext.Field(nameof(Person.Name)));

        editContext.GetValidationMessages().ShouldBe(["Name is required"]);
    }

    [Test]
    public async Task Clears_The_Messages_Once_The_Model_Is_Valid()
    {
        var model = new Person { Name = "", Address = new() { Street = "" } };
        var editContext = new EditContext(model);
        await using var renderer = await Render(editContext, services => services.AddSingleton<IValidator<Person>, PersonValidator>());
        editContext.Validate();

        model.Name = "Ada";
        model.Address.Street = "Main";

        editContext.Validate().ShouldBeTrue();
        editContext.GetValidationMessages().ShouldBeEmpty();
    }

    [Test]
    public async Task Prefers_The_Validator_Given_As_A_Parameter()
    {
        var model = new Person { Name = "", Address = new() { Street = "" } };
        var editContext = new EditContext(model);
        var validator = new InlineValidator<Person>();
        validator.RuleFor(x => x.Name).Must(_ => false).WithMessage("from the parameter");
        await using var renderer = await Render(editContext, services => services.AddSingleton<IValidator<Person>, PersonValidator>(), validator);

        editContext.Validate().ShouldBeFalse();

        editContext.GetValidationMessages().ShouldBe(["from the parameter"]);
    }

    [Test]
    public async Task Leaves_The_Form_Valid_When_No_Validator_Is_Registered()
    {
        var editContext = new EditContext(new Person { Name = "" });
        await using var renderer = await Render(editContext);

        editContext.Validate().ShouldBeTrue();
    }

    private static async Task<HtmlRenderer> Render(EditContext editContext, Action<IServiceCollection>? configure = null, IValidator? validator = null)
    {
        var services = new ServiceCollection().AddLogging();
        configure?.Invoke(services);
        var provider = services.BuildServiceProvider();
        var renderer = new HtmlRenderer(provider, provider.GetRequiredService<ILoggerFactory>());
        await renderer.Dispatcher.InvokeAsync(
            () => renderer.RenderComponentAsync<Form>(
                ParameterView.FromDictionary(
                    new Dictionary<string, object?> { [nameof(Form.EditContext)] = editContext, [nameof(Form.Validator)] = validator, }
                )
            )
        );
        return renderer;
    }

    public class Person
    {
        public string Name { get; set; } = "";
        public Address? Address { get; set; }
    }

    public class Address
    {
        public string Street { get; set; } = "";
    }

    private sealed class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Address!.Street).NotEmpty().WithMessage("Street is required").When(x => x.Address is { });
        }
    }

    // Stands in for an EditForm: cascades the edit context to the validator.
    private sealed class Form : ComponentBase
    {
        [Parameter]
        public EditContext EditContext { get; set; } = null!;

        [Parameter]
        public IValidator? Validator { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddComponentParameter(1, nameof(CascadingValue<EditContext>.Value), EditContext);
            builder.AddComponentParameter(
                2,
                nameof(CascadingValue<EditContext>.ChildContent),
                (RenderFragment)( child =>
                                {
                                    child.OpenComponent<FluentValidator>(3);
                                    if (Validator is { }) child.AddComponentParameter(4, nameof(FluentValidator.Validator), Validator);
                                    child.CloseComponent();
                                } )
            );
            builder.CloseComponent();
        }
    }
}
