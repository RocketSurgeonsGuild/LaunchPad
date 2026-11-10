using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Rocket.Surgery.LaunchPad.AspNetCore.Blazor;

using ZeroAlloc.Validation;

namespace ZeroAlloc.Blazor.Tests;

public class ZeroAllocValidatorTests
{
    [Test]
    public async Task Adds_The_Messages_Of_The_Registered_Validator_When_The_Form_Is_Validated()
    {
        var model = new Person { Name = "", Nickname = "" };
        var editContext = new EditContext(model);
        await using var renderer = await Render(editContext, services => services.AddSingleton<ValidatorFor<Person>, PersonValidator>());

        editContext.Validate().ShouldBeFalse();

        editContext.GetValidationMessages(editContext.Field(nameof(Person.Name))).ShouldBe(["Name is required"]);
        editContext.GetValidationMessages(editContext.Field(nameof(Person.Nickname))).ShouldBe(["Nickname is required"]);
    }

    [Test]
    public async Task Validates_Only_The_Field_That_Changed()
    {
        var model = new Person { Name = "", Nickname = "" };
        var editContext = new EditContext(model);
        await using var renderer = await Render(editContext, services => services.AddSingleton<ValidatorFor<Person>, PersonValidator>());

        editContext.NotifyFieldChanged(editContext.Field(nameof(Person.Name)));

        editContext.GetValidationMessages().ShouldBe(["Name is required"]);
    }

    [Test]
    public async Task Clears_The_Messages_Once_The_Model_Is_Valid()
    {
        var model = new Person { Name = "", Nickname = "" };
        var editContext = new EditContext(model);
        await using var renderer = await Render(editContext, services => services.AddSingleton<ValidatorFor<Person>, PersonValidator>());
        editContext.Validate();

        model.Name = "Ada";
        model.Nickname = "Countess";

        editContext.Validate().ShouldBeTrue();
        editContext.GetValidationMessages().ShouldBeEmpty();
    }

    [Test]
    public async Task Leaves_The_Form_Valid_When_No_Validator_Is_Registered()
    {
        var editContext = new EditContext(new Person { Name = "" });
        await using var renderer = await Render(editContext);

        editContext.Validate().ShouldBeTrue();
    }

    private static async Task<HtmlRenderer> Render(EditContext editContext, Action<IServiceCollection>? configure = null)
    {
        var services = new ServiceCollection().AddLogging();
        configure?.Invoke(services);
        var provider = services.BuildServiceProvider();
        var renderer = new HtmlRenderer(provider, provider.GetRequiredService<ILoggerFactory>());
        await renderer.Dispatcher.InvokeAsync(
            () => renderer.RenderComponentAsync<Form>(ParameterView.FromDictionary(new Dictionary<string, object?> { [nameof(Form.EditContext)] = editContext }))
        );
        return renderer;
    }

    public class Person
    {
        public string Name { get; set; } = "";
        public string Nickname { get; set; } = "";
    }

    // Stands in for the validator ZeroAlloc.Validation generates for a [Validate] type.
    private sealed class PersonValidator : ValidatorFor<Person>
    {
        public override Validation.ValidationResult Validate(Person instance)
        {
            var failures = new List<ValidationFailure>();
            if (instance.Name.Length == 0) failures.Add(new() { PropertyName = nameof(Person.Name), ErrorMessage = "Name is required" });
            if (instance.Nickname.Length == 0) failures.Add(new() { PropertyName = nameof(Person.Nickname), ErrorMessage = "Nickname is required" });
            return new([.. failures]);
        }
    }

    // Stands in for an EditForm: cascades the edit context to the validator.
    private sealed class Form : ComponentBase
    {
        [Parameter]
        public EditContext EditContext { get; set; } = null!;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<CascadingValue<EditContext>>(0);
            builder.AddComponentParameter(1, nameof(CascadingValue<EditContext>.Value), EditContext);
            builder.AddComponentParameter(
                2,
                nameof(CascadingValue<EditContext>.ChildContent),
                (RenderFragment)( child =>
                                {
                                    child.OpenComponent<ZeroAllocValidator>(3);
                                    child.CloseComponent();
                                } )
            );
            builder.CloseComponent();
        }
    }
}
