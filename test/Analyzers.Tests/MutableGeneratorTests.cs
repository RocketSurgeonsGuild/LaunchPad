using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.Foundation;

using ZeroAlloc.Validation;

namespace Analyzers.Tests;

public class MutableGeneratorTests() : GeneratorTest()
{
    /// <summary>
    ///     Each kind of type gets a view model with a settable property for each of its properties: a value copied in with From comes back out
    ///     of ToImmutable, through the type's constructor (a positional record's) and its init or set accessors.
    /// </summary>
    /// <param name="kind">The kind of the source type.</param>
    /// <param name="declaration">The source type.</param>
    [Test]
    [Arguments("ReadonlyRecordStruct", "public readonly record struct Request { public Guid Id { get; init; } public string SerialNumber { get; init; } public int Type { get; init; } }")]
    [Arguments("RecordStruct", "public record struct Request(Guid Id) { public string SerialNumber { get; set; } public int Type { get; init; } }")]
    [Arguments("Record", "public record Request(Guid Id, string SerialNumber) { public int Type { get; init; } }")]
    [Arguments("Class", "public class Request { public Guid Id { get; set; } public string SerialNumber { get; set; } = null!; public int Type { get; set; } }")]
    public async Task Should_Generate_A_View_Model_That_Round_Trips(string kind, string declaration)
    {
        var result = await Builder
                          .AddSources(
                               $$"""
                               namespace Sample.Core.Operations.Rockets
                               {
                                   public static partial class CreateRocket
                                   {
                                       {{declaration}}

                                       [Mutable(typeof(Request))]
                                       public partial class RequestViewModel;
                                   }
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<MutableGenerator>(out var output).Should().BeTrue();
        await output!.Diagnostics.Should().BeEmpty();

        var requestType = result.Assembly!.GetType("Sample.Core.Operations.Rockets.CreateRocket+Request")!;
        var viewModelType = result.Assembly.GetType("Sample.Core.Operations.Rockets.CreateRocket+RequestViewModel")!;
        var id = Guid.NewGuid();
        var constructors = requestType.GetConstructors();
        var request = Activator.CreateInstance(requestType, ( constructors.Length == 0 ? 0 : constructors[0].GetParameters().Length ) switch
        {
            1 => [id],
            2 => [id, "SN-1"],
            _ => [],
        })!;
        requestType.GetProperty("Id")!.GetSetMethod(true)?.Invoke(request, [id]);
        requestType.GetProperty("SerialNumber")!.GetSetMethod(true)?.Invoke(request, ["SN-1"]);
        requestType.GetProperty("Type")!.GetSetMethod(true)!.Invoke(request, [3]);

        var viewModel = viewModelType.GetMethod("From")!.Invoke(null, [request])!;
        await viewModelType.GetProperty("SerialNumber")!.GetValue(viewModel).Should().BeEqualTo("SN-1");
        viewModelType.GetProperty("Type")!.SetValue(viewModel, 7);

        var made = viewModelType.GetMethod("ToImmutable")!.Invoke(viewModel, [])!;
        await made.GetType().Should().BeEqualTo(requestType);
        await requestType.GetProperty("Id")!.GetValue(made).Should().BeEqualTo(id);
        await requestType.GetProperty("SerialNumber")!.GetValue(made).Should().BeEqualTo("SN-1");
        await requestType.GetProperty("Type")!.GetValue(made).Should().BeEqualTo(7);

        // The implicit conversions do the same as From and ToImmutable.
        var conversions = viewModelType.GetMethods().Where(method => method.Name == "op_Implicit").ToArray();
        var toViewModel = conversions.Single(method => method.ReturnType == viewModelType);
        var toRequest = conversions.Single(method => method.ReturnType == requestType);
        var converted = toViewModel.Invoke(null, [made])!;
        await viewModelType.GetProperty("Type")!.GetValue(converted).Should().BeEqualTo(7);
        var convertedBack = toRequest.Invoke(null, [converted])!;
        await requestType.GetProperty("Id")!.GetValue(convertedBack).Should().BeEqualTo(id);
        await requestType.GetProperty("SerialNumber")!.GetValue(convertedBack).Should().BeEqualTo("SN-1");
        await requestType.GetProperty("Type")!.GetValue(convertedBack).Should().BeEqualTo(7);

        await Verify(result).UseParameters(kind);
    }

    /// <summary>
    ///     The generic attribute does the same as the one that takes a type, and the view model can live anywhere: its name, namespace and
    ///     accessibility are its own.
    /// </summary>
    [Test]
    public async Task Should_Generate_A_View_Model_From_The_Generic_Attribute()
    {
        var result = await Builder
                          .AddSources(
                               """
                               namespace Sample.Core.Operations.Rockets
                               {
                                   public readonly record struct Rocket(Guid Id, string SerialNumber);
                               }

                               namespace Sample.BlazorWasm.Forms
                               {
                                   [Mutable<Sample.Core.Operations.Rockets.Rocket>]
                                   internal sealed partial class RocketForm;
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<MutableGenerator>(out var output).Should().BeTrue();
        await output!.Diagnostics.Should().BeEmpty();
        var viewModelType = result.Assembly!.GetType("Sample.BlazorWasm.Forms.RocketForm")!;
        await viewModelType.Should().NotBeNull();
        await viewModelType.IsPublic.Should().BeFalse();
        await viewModelType.GetProperty("SerialNumber").Should().NotBeNull();

        await Verify(result);
    }

    /// <summary>
    ///     A property the view model declares itself isn't written again, and From and ToImmutable use it.
    /// </summary>
    [Test]
    public async Task Should_Use_A_Property_The_View_Model_Declares()
    {
        var result = await Builder
                          .AddSources(
                               """
                               namespace Sample.Core.Operations.Rockets
                               {
                                   public record Rocket(Guid Id, string SerialNumber);

                                   [Mutable<Rocket>]
                                   public partial class RocketViewModel
                                   {
                                       public string SerialNumber { get; set; } = "unset";
                                   }
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<MutableGenerator>(out var output).Should().BeTrue();
        await output!.Diagnostics.Should().BeEmpty();

        var rocketType = result.Assembly!.GetType("Sample.Core.Operations.Rockets.Rocket")!;
        var viewModelType = result.Assembly.GetType("Sample.Core.Operations.Rockets.RocketViewModel")!;
        var rocket = Activator.CreateInstance(rocketType, [Guid.NewGuid(), "SN-1"])!;
        var viewModel = viewModelType.GetMethod("From")!.Invoke(null, [rocket])!;
        await viewModelType.GetProperty("SerialNumber")!.GetValue(viewModel).Should().BeEqualTo("SN-1");
        var made = viewModelType.GetMethod("ToImmutable")!.Invoke(viewModel, [])!;
        await rocketType.GetProperty("SerialNumber")!.GetValue(made).Should().BeEqualTo("SN-1");

        await Verify(result);
    }

    /// <summary>
    ///     Where ZeroAlloc.Validation is referenced, the view model gets a validator that validates the value it makes with that type's
    ///     validator, which it requires.
    /// </summary>
    [Test]
    public async Task Should_Generate_A_Validator_Where_ZeroAlloc_Validation_Is_Referenced()
    {
        var result = await Builder
                          .AddReferences(typeof(ValidatorFor<>))
                          .AddSources(
                               """
                               using ZeroAlloc.Validation;

                               namespace Sample.Core.Operations.Rockets
                               {
                                   public readonly record struct Rocket(string SerialNumber);

                                   public sealed class RocketValidator : ValidatorFor<Rocket>
                                   {
                                       public override ValidationResult Validate(Rocket instance) => instance.SerialNumber.Length == 0
                                           ? new([new ValidationFailure { PropertyName = nameof(Rocket.SerialNumber), ErrorMessage = "required" }])
                                           : new([]);
                                   }

                                   [Mutable<Rocket>]
                                   public partial class RocketViewModel;
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<MutableGenerator>(out var output).Should().BeTrue();
        await output!.Diagnostics.Should().BeEmpty();

        var viewModelType = result.Assembly!.GetType("Sample.Core.Operations.Rockets.RocketViewModel")!;
        var validatorType = viewModelType.GetNestedType("Validator")!;
        var constructor = await validatorType.GetConstructors().Should().HaveSingleItem();
        var parameter = await constructor.GetParameters().Should().HaveSingleItem();
        await parameter.HasDefaultValue.Should().BeFalse();

        var viewModel = Activator.CreateInstance(viewModelType)!;
        viewModelType.GetProperty("SerialNumber")!.SetValue(viewModel, "");
        var requestValidator = Activator.CreateInstance(result.Assembly.GetType("Sample.Core.Operations.Rockets.RocketValidator")!)!;
        var validator = (IModelValidator)Activator.CreateInstance(validatorType, [requestValidator])!;
        var failures = ( await validator.ValidateAsync(viewModel, TestContext.CancellationToken) ).Failures.ToArray();
        await failures.Select(failure => failure.PropertyName).Should().BeEquivalentTo(["SerialNumber"], CollectionOrdering.Matching);

        await Verify(result);
    }

    /// <summary>
    ///     Where Foundation's LaunchPadMutableValidatorsJoint template is in the project, AddLaunchPadMutableValidators registers the validator
    ///     of each view model it can name, and none of a generic or private one; without the template there is no registration.
    /// </summary>
    [Test]
    public async Task Should_Register_The_Validators_Where_The_Joint_Template_Is()
    {
        var result = await Builder
                          .AddReferences(typeof(ValidatorFor<>), typeof(IServiceCollection), typeof(ServiceCollection), typeof(ServiceCollectionDescriptorExtensions))
                          .AddSources(
                               """
                               using Microsoft.Extensions.DependencyInjection;
                               using ZeroAlloc.Validation;

                               namespace Microsoft.CodeAnalysis
                               {
                                   internal sealed class EmbeddedAttribute : Attribute;
                               }

                               // stands in for the joint Foundation's template writes
                               namespace Rocket.Surgery.LaunchPad.Foundation.Generated
                               {
                                   public static class LaunchPadMutableValidatorsJoint
                                   {
                                       public static IServiceCollection Register(IServiceCollection services) => services.AddLaunchPadMutableValidators();
                                   }
                               }

                               namespace Sample.Core.Operations.Rockets
                               {
                                   public readonly record struct Rocket(string SerialNumber);

                                   public static partial class CreateRocket
                                   {
                                       public readonly record struct Request(string SerialNumber);

                                       [Mutable<Request>]
                                       public partial class RequestViewModel;

                                       [Mutable<Request>]
                                       private partial class HiddenViewModel;
                                   }

                                   [Mutable<Rocket>]
                                   internal partial class RocketViewModel;

                                   [Mutable<Rocket>]
                                   public partial class GenericViewModel<T>;
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<MutableGenerator>(out var output).Should().BeTrue();
        await output!.Diagnostics.Should().BeEmpty();
        await result.FinalDiagnostics.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).Select(diagnostic => diagnostic.ToString()).Should().BeEmpty();

        var joint = result.Assembly!.GetType("Rocket.Surgery.LaunchPad.Foundation.Generated.LaunchPadMutableValidatorsJoint")!;
        var services = (IServiceCollection)joint.GetMethod("Register")!.Invoke(null, [new ServiceCollection()])!;
        await services.Select(descriptor => descriptor.ImplementationType!.FullName).Should().BeEquivalentTo(
            [
                "Sample.Core.Operations.Rockets.CreateRocket+RequestViewModel+Validator",
                "Sample.Core.Operations.Rockets.RocketViewModel+Validator",
            ],
            CollectionOrdering.Any
        );

        await Verify(result);
    }

    /// <summary>
    ///     The generator fills in the view model, so it must be partial.
    /// </summary>
    [Test]
    public async Task Should_Require_A_Partial_View_Model()
    {
        var result = await Builder
                          .AddSources(
                               """
                               namespace Sample.Core.Operations.Rockets
                               {
                                   public readonly record struct Request(Guid Id);

                                   [Mutable<Request>]
                                   public class RequestViewModel;
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<MutableGenerator>(out var output).Should().BeTrue();
        var diagnostic = await output!.Diagnostics.Should().HaveSingleItem();
        await diagnostic.Id.Should().BeEqualTo("LPAD0001");
        await diagnostic.ToString().Should().Contain("Type RequestViewModel must be made partial.");
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        Builder = Builder
                 .WithGenerator<MutableGenerator>()
                 .AddReferences(typeof(MutableAttribute))
                 .AddSources(
                      """
                      global using System;
                      global using Rocket.Surgery.LaunchPad.Foundation;
                      """
                  );
    }
}
