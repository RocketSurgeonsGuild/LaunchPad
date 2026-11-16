using DryIoc.ImTools;
using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.Foundation;
using ZeroAlloc.Mediator;

namespace Analyzers.Tests;

public class PropertyTrackingGeneratorTests() : GeneratorTest()
{
    [Test]
    public async Task Should_Require_Partial_Type_Declaration()
    {
        var result = await Builder
                          .AddSources(
                               """

                               namespace Sample.Core.Operations.Rockets
                               {
                                   /// <summary>
                                   /// Request
                                   /// </summary>
                                   /// <param name="Id">The rocket id</param>
                                   public class Request : IRequest<RocketModel>
                                   {
                                       public Guid Id { get; init; }
                                       public string SerialNumber { get; set; } = null!;
                                       public int Type { get; set; }
                                   }
                                   public class PatchRocket : IPropertyTracking<Request>, IRequest<RocketModel>
                                   {
                                       public Guid Id { get; init; }
                                   }
                               }

                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<PropertyTrackingGenerator>(out var output).Should().BeTrue();
        var diagnostic = await output!.Diagnostics.Should().HaveSingleItem();
        await diagnostic.Id.Should().BeEqualTo("LPAD0001");
        await diagnostic.ToString().Should().Contain("Type Sample.Core.Operations.Rockets.PatchRocket must be made partial.");

        await Verify(result);
    }

    [Test]
    public async Task Should_Require_Partial_Parent_Type_Declaration()
    {
        var result = await Builder
                          .AddSources(
                               """

                               namespace Sample.Core.Operations.Rockets
                               {
                                   /// <summary>
                                   /// Request
                                   /// </summary>
                                   /// <param name="Id">The rocket id</param>
                                   public record Request : IRequest<RocketModel>
                                   {
                                       public Guid Id { get; init; }
                                       public string SerialNumber { get; set; } = null!;
                                       public int Type { get; set; }
                                   }
                                   public static class PublicClass
                                   {
                                       public partial record PatchRocket : IPropertyTracking<Request>, IRequest<RocketModel>
                                       {
                                           public Guid Id { get; init; }
                                       }
                                   }
                               }

                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<PropertyTrackingGenerator>(out var output).Should().BeTrue();
        var diagnostic = await output!.Diagnostics.Should().HaveSingleItem();
        await diagnostic.Id.Should().BeEqualTo("LPAD0001");
        await diagnostic.ToString().Should().Contain("Type Sample.Core.Operations.Rockets.PublicClass must be made partial.");

        await Verify(result);
    }

    [Test]
    public async Task Should_Generate_Record_With_Underlying_Properties_And_Track_Changes()
    {
        var result = await Builder
                          .AddSources(
                               """

                               /// <summary>
                               /// Request
                               /// </summary>
                               /// <param name="Id">The rocket id</param>
                               public record Request : IRequest<RocketModel>
                               {
                                   public Guid Id { get; init; }
                                   public string SerialNumber { get; set; } = null!;
                                   public int Type { get; set; }
                               }
                               public partial record PatchRocket : IPropertyTracking<Request>, IRequest<RocketModel>
                               {
                               }

                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<PropertyTrackingGenerator>(out var output).Should().BeTrue();
        await output!.Diagnostics.Should().BeEmpty();

        var type = result.Assembly!.DefinedTypes.FindFirst(z => z.Name == "PatchRocket");
        var serialNumberProperty = type.GetProperty("SerialNumber")!;
        var typeProperty = type.GetProperty("Type")!;
        var idProperty = type.GetProperty("Id")!;
        var getChangesMethod = type.GetMethod("GetChangedState")!;
        var changesType = getChangesMethod.ReturnType;
        var serialNumberChangedProperty = changesType.GetProperty("SerialNumber")!;
        var typeChangedProperty = changesType.GetProperty("Type")!;
        var idChangedProperty = changesType.GetProperty("Id")!;
        var instance = Activator.CreateInstance(type);

        serialNumberProperty.SetValue(instance, new Assigned<string>("12345"));
        var changes = getChangesMethod.Invoke(instance, []);

        var serialNumberChanged = (bool)serialNumberChangedProperty.GetValue(changes)!;
        await serialNumberChanged.Should().BeTrue();

        typeProperty.SetValue(instance, new Assigned<int>(12345));
        changes = getChangesMethod.Invoke(instance, []);

        var typeChanged = (bool)typeChangedProperty.GetValue(changes)!;
        await typeChanged.Should().BeTrue();

        idProperty.SetValue(instance, new Assigned<Guid>(Guid.NewGuid()));
        changes = getChangesMethod.Invoke(instance, []);

        var idChanged = (bool)idChangedProperty.GetValue(changes)!;
        await idChanged.Should().BeTrue();

        await Verify(result);
    }

    [Test]
    public async Task Should_Generate_Class_With_Underlying_Properties_And_Track_Changes()
    {
        var result = await Builder
                          .AddSources(
                               """

                               /// <summary>
                               /// Request
                               /// </summary>
                               /// <param name="Id">The rocket id</param>
                               public class Request : IRequest<RocketModel>
                               {
                                   public Guid Id { get; init; }
                                   public string SerialNumber { get; set; } = null!;
                                   public int Type { get; set; }
                               }
                               public partial class PatchRocket : IPropertyTracking<Request>, IRequest<RocketModel>
                               {
                               }

                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<PropertyTrackingGenerator>(out var output).Should().BeTrue();
        await output!.Diagnostics.Should().BeEmpty();

        var type = result.Assembly!.DefinedTypes.FindFirst(z => z.Name == "PatchRocket");
        var serialNumberProperty = type.GetProperty("SerialNumber")!;
        var typeProperty = type.GetProperty("Type")!;
        var getChangesMethod = type.GetMethod("GetChangedState")!;
        var changesType = getChangesMethod.ReturnType;
        var serialNumberChangedProperty = changesType.GetProperty("SerialNumber")!;
        var typeChangedProperty = changesType.GetProperty("Type")!;
        var instance = Activator.CreateInstance(type);

        serialNumberProperty.SetValue(instance, new Assigned<string>("12345"));
        var changes = getChangesMethod.Invoke(instance, []);

        var serialNumberChanged = (bool)serialNumberChangedProperty.GetValue(changes)!;
        await serialNumberChanged.Should().BeTrue();

        typeProperty.SetValue(instance, new Assigned<int>(12345));
        changes = getChangesMethod.Invoke(instance, []);

        var typeChanged = (bool)typeChangedProperty.GetValue(changes)!;
        await typeChanged.Should().BeTrue();

        await changesType.GetProperty("Id").Should().BeNull();
        await type.GetProperty("Id").Should().BeNull();

        await Verify(result);
    }

    [Test]
    public async Task Should_Require_Same_Type_As_Record()
    {
        var result = await Builder
                          .AddSources(
                               """

                               /// <summary>
                               /// Request
                               /// </summary>
                               /// <param name="Id">The rocket id</param>
                               namespace Sample.Core.Operations.Rockets
                               {
                                   public record Request : IRequest<RocketModel>
                                   {
                                       public Guid Id { get; init; }
                                       public string SerialNumber { get; set; } = null!;
                                       public int Type { get; set; }
                                   }
                                   public partial class PatchRocket(Guid Id) : IPropertyTracking<Request>, IRequest<RocketModel>;
                               }

                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<PropertyTrackingGenerator>(out var output).Should().BeTrue();
        var diagnostic = await output!.Diagnostics.Should().HaveSingleItem();
        await diagnostic.Id.Should().BeEqualTo("LPAD0005");
        await diagnostic.ToString().Should().Contain("The declaration Sample.Core.Operations.Rockets.PatchRocket must be a record.");

        await Verify(result);
    }

    [Test]
    public async Task Should_Require_Same_Type_As_Class()
    {
        var result = await Builder
                          .AddSources(
                               """

                               namespace Sample.Core.Operations.Rockets
                               {
                                   /// <summary>
                                   /// Request
                                   /// </summary>
                                   /// <param name="Id">The rocket id</param>
                                   public class Request : IRequest<RocketModel>
                                   {
                                       public Guid Id { get; init; }
                                       public string SerialNumber { get; set; } = null!;
                                       public int Type { get; set; }
                                   }
                                   public partial record PatchRocket : IPropertyTracking<Request>, IRequest<RocketModel>
                                   {
                                       public Guid Id { get; init; }
                                   }
                               }

                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<PropertyTrackingGenerator>(out var output).Should().BeTrue();
        var diagnostic = await output!.Diagnostics.Should().HaveSingleItem();
        await diagnostic.Id.Should().BeEqualTo("LPAD0005");
        await diagnostic.ToString().Should().Contain("The declaration Sample.Core.Operations.Rockets.PatchRocket must be a class.");

        await Verify(result);
    }

    [Test]
    public async Task Should_Support_Nullable_Class_Property()
    {
        var result = await Builder
                          .AddSources(
                               """

                               namespace Sample.Core.Operations.Rockets
                               {
                                   /// <summary>
                                   /// Request
                                   /// </summary>
                                   /// <param name="Id">The rocket id</param>
                                   public record Request : IRequest<RocketModel>
                                   {
                                       public Guid Id { get; init; }
                                       public string? SerialNumber { get; set; } = null!;
                                       public int Type { get; set; }
                                   }
                                   public partial record PatchRocket(Guid Id) : IPropertyTracking<Request>, IRequest<RocketModel>;
                               }

                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<PropertyTrackingGenerator>(out _).Should().BeTrue();

        await Verify(result);
    }

    [Test]
    public async Task Should_Support_Nullable_Struct_Property()
    {
        var result = await Builder
                          .AddSources(
                               """

                               namespace Sample.Core.Operations.Rockets
                               {
                                   /// <summary>
                                   /// Request
                                   /// </summary>
                                   /// <param name="Id">The rocket id</param>
                                   public record Request : IRequest<RocketModel>
                                   {
                                       public Guid Id { get; init; }
                                       public string SerialNumber { get; set; } = null!;
                                       public int? Type { get; set; }
                                   }
                                   public partial record PatchRocket : IPropertyTracking<Request>, IRequest<RocketModel>
                                   {
                                       public Guid Id { get; init; }
                                   }
                               }

                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<PropertyTrackingGenerator>(out _).Should().BeTrue();

        await Verify(result);
    }

    [Test]
    public async Task Should_Generate_Class_With_Underlying_IPropertyTracking_Properties_When_Using_InheritsFromGenerator()
    {
        var result = await Builder
                          .WithGenerator<InheritFromGenerator>()
                          .AddSources(
                               @"
public class Model
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null!;
}

[InheritFrom(typeof(Model))]
public partial class Request : IRequest<RocketModel>
{
    public int Type { get; set; }
}
public partial class PatchRequest : IPropertyTracking<Request>, IRequest<RocketModel>
{
    public Guid Id { get; init; }
}
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await Verify(result);
    }

    [Test]
    public async Task Should_Generate_Class_With_Underlying_IPropertyTracking_Properties_When_Using_InheritsFromGenerator_Exclude()
    {
        var result = await Builder
                          .WithGenerator<InheritFromGenerator>()
                          .AddSources(
                               @"
public class Model
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null!;
    public string Something { get; set; } = null!;
}

[InheritFrom(typeof(Model), Exclude = new[] { nameof(Model.SerialNumber) })]
public partial class Request : IRequest<RocketModel>
{
    public int Type { get; set; }
}
public partial class PatchRequest : IPropertyTracking<Request>, IRequest<RocketModel>
{
    public Guid Id { get; init; }
}
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await Verify(result);
    }

    [Test]
    public async Task Should_Generate_Class_Primary_Constructor() { }

    [Test]
    public async Task Should_Generate_And_Exclude_Properties()
    {
        var result = await Builder
                          .WithGenerator<InheritFromGenerator>()
                          .AddSources(
                               @"
using Rocket.Surgery.LaunchPad.Foundation;
public partial class Request : IRequest<RocketModel>
{
    [GenerationIgnore]
    public int Type { get; set; }
    public string Name { get; set; }
    [ExcludeFromGeneration]
    public string Other { get; set; }
}
public partial class PatchRequest(Guid Id) : IPropertyTracking<Request>, IRequest<RocketModel>
{
    public Guid Id { get; } = Id;
}
"
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await Verify(result);
    }

    [Test]
    [Arguments("SerialNumber", "12345")]
    [Arguments("Type", 12345)]
    public async Task Should_Generate_Record_With_Underlying_Properties_And_Apply_Changes(string property, object value)
    {
        var result = await Builder
                          .AddSources(
                               """

                               /// <summary>
                               /// Request
                               /// </summary>
                               /// <param name="Id">The rocket id</param>
                               public record Request : IRequest<RocketModel>
                               {
                                   public Guid Id { get; init; }
                                   public string SerialNumber { get; set; } = null!;
                                   public int Type { get; set; }
                               }
                               public partial record PatchRocket : IPropertyTracking<Request>, IRequest<RocketModel>
                               {
                               }

                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<PropertyTrackingGenerator>(out var output).Should().BeTrue();
        await output!.Diagnostics.Should().BeEmpty();

        var type = result.Assembly!.DefinedTypes.FindFirst(z => z.Name == "PatchRocket");
        var applyChangesMethod = type.GetMethod("ApplyChanges")!;
        var propertyUnderTest = type.GetProperty(property)!;
        var requestType = result.Assembly.DefinedTypes.FindFirst(z => z.Name == "Request");
        var requestPropertyUnderTest = requestType.GetProperty(property)!;
        var request = Activator.CreateInstance(requestType);
        var instance = Activator.CreateInstance(type);

        var assignedType = typeof(Assigned<>).MakeGenericType(value.GetType());
        propertyUnderTest.SetValue(instance, Activator.CreateInstance(assignedType, value));
        request = applyChangesMethod.Invoke(instance, [request]);
        var r = requestPropertyUnderTest.GetValue(request);
        await r.Should().BeEqualTo(value);

        await Verify(result).UseParameters(property);
    }

    [Test]
    [Arguments("SerialNumber", "12345")]
    [Arguments("Type", 12345)]
    public async Task Should_Generate_Class_With_Underlying_Properties_And_Apply_Changes(string property, object value)
    {
        var result = await Builder
                          .AddSources(
                               """

                               /// <summary>
                               /// Request
                               /// </summary>
                               /// <param name="Id">The rocket id</param>
                               public class Request : IRequest<RocketModel>
                               {
                                   public Guid Id { get; init; }
                                   public string SerialNumber { get; set; } = null!;
                                   public int Type { get; set; }
                               }
                               public partial class PatchRocket : IPropertyTracking<Request>, IRequest<RocketModel>
                               {
                                   public Guid Id { get; init; }
                               }

                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<PropertyTrackingGenerator>(out var output).Should().BeTrue();
        await output!.Diagnostics.Should().BeEmpty();

        var type = result.Assembly!.DefinedTypes.FindFirst(z => z.Name == "PatchRocket");
        var applyChangesMethod = type.GetMethod("ApplyChanges")!;
        var propertyUnderTest = type.GetProperty(property)!;
        var requestType = result.Assembly.DefinedTypes.FindFirst(z => z.Name == "Request");
        var requestPropertyUnderTest = requestType.GetProperty(property)!;
        var request = Activator.CreateInstance(requestType);
        var instance = Activator.CreateInstance(type);

        var assignedType = typeof(Assigned<>).MakeGenericType(value.GetType());
        propertyUnderTest.SetValue(instance, Activator.CreateInstance(assignedType, value));
        applyChangesMethod.Invoke(instance, [request]);
        var r = requestPropertyUnderTest.GetValue(request);
        await r.Should().BeEqualTo(value);

        await Verify(result).UseParameters(property);
    }

    [Test]
    public async Task Should_Stamp_The_Generator_Version_From_Its_Typed_Assembly_Information()
    {
        var result = await Builder
                          .AddSources(
                               """
                               public record Request : IRequest<RocketModel>
                               {
                                   public Guid Id { get; init; }
                               }
                               public partial record PatchRocket : IPropertyTracking<Request>, IRequest<RocketModel>
                               {
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await result.TryGetResult<PropertyTrackingGenerator>(out var output).Should().BeTrue();

        // The generator's identity and version, read from AssemblyInfo.Dependencies rather than the generator assembly.
        const string generatorVersion = AssemblyInfo.Dependencies.Rocket_Surgery_LaunchPad_Analyzers.Attributes.FileVersion;
        await string.IsNullOrWhiteSpace(generatorVersion).Should().BeFalse();
        await output!
           .SyntaxTrees.Should().Contain(tree => tree.ToString().Contains(
                    $"""GeneratedCode("{typeof(PropertyTrackingGenerator).Namespace}", "{generatorVersion}")""",
                    StringComparison.Ordinal
                ));
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        Builder = Builder
                 .WithGenerator<PropertyTrackingGenerator>()
                 .AddReferences(
                      typeof(IPropertyTracking<>),
                      typeof(IRequest<>),
                      // the generated tracking properties carry System.Text.Json's JsonConverter attribute
                      typeof(System.Text.Json.JsonSerializer)
                  )
                 .AddSources(
                      @"
global using System;
global using ZeroAlloc.Mediator;
global using Rocket.Surgery.LaunchPad.Foundation;
global using Sample.Core.Operations.Rockets;
namespace Sample.Core.Operations.Rockets
{
    public class RocketModel
    {
        public Guid Id { get; init; }
        public string SerialNumber { get; set; } = null!;
    }
}
"
                  );
    }
}
