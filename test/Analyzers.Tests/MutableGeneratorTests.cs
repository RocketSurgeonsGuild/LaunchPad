using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.Foundation;

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
                                       [Mutable]
                                       {{declaration}}
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

        var viewModel = (IMutable)viewModelType.GetMethod("From")!.Invoke(null, [request])!;
        await viewModelType.GetProperty("SerialNumber")!.GetValue(viewModel).Should().BeEqualTo("SN-1");
        viewModelType.GetProperty("Type")!.SetValue(viewModel, 7);

        var made = viewModel.ToImmutable();
        await made.GetType().Should().BeEqualTo(requestType);
        await requestType.GetProperty("Id")!.GetValue(made).Should().BeEqualTo(id);
        await requestType.GetProperty("SerialNumber")!.GetValue(made).Should().BeEqualTo("SN-1");
        await requestType.GetProperty("Type")!.GetValue(made).Should().BeEqualTo(7);

        await Verify(result).UseParameters(kind);
    }

    /// <summary>
    ///     The prefix and suffix name the view model, and each attribute writes one.
    /// </summary>
    [Test]
    public async Task Should_Name_The_View_Model_With_The_Prefix_And_Suffix()
    {
        var result = await Builder
                          .AddSources(
                               """
                               namespace Sample.Core.Operations.Rockets
                               {
                                   [Mutable]
                                   [Mutable("Editable", "Form")]
                                   public readonly record struct Rocket(Guid Id, string SerialNumber);
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<MutableGenerator>(out var output).Should().BeTrue();
        await output!.Diagnostics.Should().BeEmpty();
        await result.Assembly!.GetType("Sample.Core.Operations.Rockets.RocketViewModel").Should().NotBeNull();
        await result.Assembly.GetType("Sample.Core.Operations.Rockets.EditableRocketForm").Should().NotBeNull();

        await Verify(result);
    }

    /// <summary>
    ///     The view model is written next to the type, so the types that contain it must be partial.
    /// </summary>
    [Test]
    public async Task Should_Require_Partial_Containing_Types()
    {
        var result = await Builder
                          .AddSources(
                               """
                               namespace Sample.Core.Operations.Rockets
                               {
                                   public static class CreateRocket
                                   {
                                       [Mutable]
                                       public readonly record struct Request(Guid Id);
                                   }
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<MutableGenerator>(out var output).Should().BeTrue();
        var diagnostic = await output!.Diagnostics.Should().HaveSingleItem();
        await diagnostic.Id.Should().BeEqualTo("LPAD0001");
        await diagnostic.ToString().Should().Contain("Type CreateRocket must be made partial.");
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
