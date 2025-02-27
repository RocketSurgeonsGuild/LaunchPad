using System.Linq.Expressions;

using DryIoc.ImTools;

using FluentValidation;

using MediatR;

using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Analyzers.Tests;

public class PropertyTrackingGeneratorTests(ITestContextAccessor testContext) : GeneratorTest(testContext)
{
    [Fact]
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
        result.TryGetResult<PropertyTrackingGenerator>(out var output).ShouldBeTrue();
        var diagnostic = output!.Diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("LPAD0001");
        diagnostic.ToString().ShouldContain("Type Sample.Core.Operations.Rockets.PatchRocket must be made partial.");

        await Verify(result);
    }

    [Fact]
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
        result.TryGetResult<PropertyTrackingGenerator>(out var output).ShouldBeTrue();
        var diagnostic = output!.Diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("LPAD0001");
        diagnostic.ToString().ShouldContain("Type Sample.Core.Operations.Rockets.PublicClass must be made partial.");

        await Verify(result);
    }

    [Fact]
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
        result.TryGetResult<PropertyTrackingGenerator>(out var output).ShouldBeTrue();
        output!.Diagnostics.ShouldBeEmpty();

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
        serialNumberChanged.ShouldBeTrue();

        typeProperty.SetValue(instance, new Assigned<int>(12345));
        changes = getChangesMethod.Invoke(instance, []);

        var typeChanged = (bool)typeChangedProperty.GetValue(changes)!;
        typeChanged.ShouldBeTrue();

        idProperty.SetValue(instance, new Assigned<Guid>(Guid.NewGuid()));
        changes = getChangesMethod.Invoke(instance, []);

        var idChanged = (bool)idChangedProperty.GetValue(changes)!;
        idChanged.ShouldBeTrue();

        await Verify(result);
    }

    [Fact]
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
        result.TryGetResult<PropertyTrackingGenerator>(out var output).ShouldBeTrue();
        output!.Diagnostics.ShouldBeEmpty();

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
        serialNumberChanged.ShouldBeTrue();

        typeProperty.SetValue(instance, new Assigned<int>(12345));
        changes = getChangesMethod.Invoke(instance, []);

        var typeChanged = (bool)typeChangedProperty.GetValue(changes)!;
        typeChanged.ShouldBeTrue();

        changesType.GetProperty("Id").ShouldBeNull();
        type.GetProperty("Id").ShouldBeNull();

        await Verify(result);
    }

    [Fact]
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
        result.TryGetResult<PropertyTrackingGenerator>(out var output).ShouldBeTrue();
        var diagnostic = output!.Diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("LPAD0005");
        diagnostic.ToString().ShouldContain("The declaration Sample.Core.Operations.Rockets.PatchRocket must be a record.");

        await Verify(result);
    }

    [Fact]
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
        result.TryGetResult<PropertyTrackingGenerator>(out var output).ShouldBeTrue();
        var diagnostic = output!.Diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("LPAD0005");
        diagnostic.ToString().ShouldContain("The declaration Sample.Core.Operations.Rockets.PatchRocket must be a class.");

        await Verify(result);
    }

    [Fact]
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
        result.TryGetResult<PropertyTrackingGenerator>(out _).ShouldBeTrue();

        await Verify(result);
    }

    [Fact]
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
        result.TryGetResult<PropertyTrackingGenerator>(out _).ShouldBeTrue();

        await Verify(result);
    }

    [Fact]
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

    [Fact]
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

    [Fact]
    public async Task Should_Generate_Class_Primary_Constructor() { }

    [Fact]
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

    [Theory]
    [InlineData("SerialNumber", "12345")]
    [InlineData("Type", 12345)]
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
        result.TryGetResult<PropertyTrackingGenerator>(out var output).ShouldBeTrue();
        output!.Diagnostics.ShouldBeEmpty();

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
        r.ShouldBe(value);

        await Verify(result).UseParameters(property);
    }

    [Theory]
    [InlineData("SerialNumber", "12345")]
    [InlineData("Type", 12345)]
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
        result.TryGetResult<PropertyTrackingGenerator>(out var output).ShouldBeTrue();
        output!.Diagnostics.ShouldBeEmpty();

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
        r.ShouldBe(value);

        await Verify(result).UseParameters(property);
    }

    [Theory]
    [MemberData(nameof(Should_Generate_Class_With_Underlying_FluentValidation_Validator_Methods_Data))]
    public async Task Should_Generate_Class_With_Underlying_FluentValidation_Validator_Methods(string name, string source)
    {
        var result = await Builder
                          .AddReferences(typeof(AbstractValidator<>))
                          .AddReferences(typeof(Expression))
                          .WithGenerator<InheritFromGenerator>()
                          .AddSources(source)
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);
        await Verify(result).UseParameters(name);
    }

    public static IEnumerable<object[]> Should_Generate_Class_With_Underlying_FluentValidation_Validator_Methods_Data()
    {
        yield return
        [
            "RuleFor",
            @"
using FluentValidation;
using Rocket.Surgery.LaunchPad.Foundation;
public class Model
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null!;
    public string Something { get; set; } = null!;

    class Validator : AbstractValidator<Model>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            this.RuleFor(x => x.SerialNumber).NotEmpty();
            RuleFor(x => x.Something).NotEmpty();
        }
    }
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

partial class Validator : AbstractValidator<PatchRequest>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty();
        InheritFromModel();
    }
}
",
        ];
        yield return
        [
            "Constructor_Parameters",
            @"
using FluentValidation;
using Rocket.Surgery.LaunchPad.Foundation;

public record AddressModel
{
    public string? AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? City { get; init; }
    public string? Country { get; init; }
    public string? State { get; init; }
    public string? Zip { get; init; }

    public class Validator : AbstractValidator<AddressModel>
    {
        public Validator()
        {
            RuleFor(x => x.AddressLine1).NotEmpty();
            RuleFor(x => x.City).NotEmpty();
            RuleFor(x => x.Country).NotEmpty();
            RuleFor(x => x.State).NotEmpty();
            RuleFor(x => x.Zip).NotEmpty();
        }
    }
}

public class Model
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null!;
    public AddressModel Address { get; set; } = null!;

    class Validator : AbstractValidator<Model>
    {
        public Validator(IValidator<AddressModel> addressModelValidator)
        {
            RuleFor(x => x.Id).NotEmpty();
            this.RuleFor(x => x.SerialNumber).NotEmpty();
            RuleFor(x => x.Address).NotNull().SetValidator(addressModelValidator);
        }
    }
}

[InheritFrom(typeof(Model))]
public partial class Request : IRequest<RocketModel>
{
    public int Type { get; set; }
}

public partial class PatchRequest(int Id) : IPropertyTracking<Request>, IRequest<RocketModel>
{
    public int Id { get; } = Id;
}

partial class Validator : AbstractValidator<PatchRequest>
{
    public Validator(IValidator<AddressModel> addressModelValidator)
    {
        RuleFor(x => x.Id).NotEmpty();
        InheritFromModel(addressModelValidator);
    }
}
",
        ];
        yield return
        [
            "Constructor_Parameters_Missing_Parameter",
            @"
using FluentValidation;
using Rocket.Surgery.LaunchPad.Foundation;

public record AddressModel
{
    public string? AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? City { get; init; }
    public string? Country { get; init; }
    public string? State { get; init; }
    public string? Zip { get; init; }

    public class Validator : AbstractValidator<AddressModel>
    {
        public Validator()
        {
            RuleFor(x => x.AddressLine1).NotEmpty();
            RuleFor(x => x.City).NotEmpty();
            RuleFor(x => x.Country).NotEmpty();
            RuleFor(x => x.State).NotEmpty();
            RuleFor(x => x.Zip).NotEmpty();
        }
    }
}

public class Model
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null!;
    public AddressModel Address { get; set; } = null!;

    class Validator : AbstractValidator<Model>
    {
        public Validator(IValidator<AddressModel> addressModelValidator)
        {
            RuleFor(x => x.Id).NotEmpty();
            this.RuleFor(x => x.SerialNumber).NotEmpty();
            RuleFor(x => x.Address).NotNull().SetValidator(addressModelValidator);
        }
    }
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

partial class Validator : AbstractValidator<PatchRequest>
{
    public Validator(IValidator<AddressModel> addressModelValidator)
    {
        RuleFor(x => x.Id).NotEmpty();
        InheritFromModel(addressModelValidator);
    }
}
",
        ];
        yield return
        [
            "When_Otherwise",
            @"
using FluentValidation;
using Rocket.Surgery.LaunchPad.Foundation;

public class Model
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null!;
    public string Something { get; set; } = null!;

    class Validator : AbstractValidator<Model>
    {
        public Validator()
        {
            When(z => z.Id != Guid.Empty, () => RuleFor(z => z.SerialNumber).NotEmpty())
               .Otherwise(
                    () => { RuleFor(z => z.SerialNumber).Empty(); }
                );
        }
    }
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

partial class Validator : AbstractValidator<PatchRequest>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty();
        InheritFromModel();
    }
}
",
        ];
        yield return
        [
            "When_Otherwise_Exclude",
            @"
using FluentValidation;
using Rocket.Surgery.LaunchPad.Foundation;

public class Model
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null!;
    public string Something { get; set; } = null!;

    class Validator : AbstractValidator<Model>
    {
        public Validator()
        {
            When(z => z.Id != Guid.Empty, () => RuleFor(z => z.SerialNumber).NotEmpty())
               .Otherwise(
                    () => { RuleFor(z => z.SerialNumber).Empty(); }
                );
        }
    }
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

partial class Validator : AbstractValidator<PatchRequest>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
",
        ];
        yield return
        [
            "Otherwise_Exclude",
            @"
using FluentValidation;
using Rocket.Surgery.LaunchPad.Foundation;

public class Model
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null!;
    public string Something { get; set; } = null!;

    class Validator : AbstractValidator<Model>
    {
        public Validator()
        {
            When(z => z.Id != Guid.Empty, () => RuleFor(z => z.SerialNumber).NotEmpty())
               .Otherwise(
                    () => { RuleFor(z => z.SerialNumber).Empty();
RuleFor(z => z.Something).Empty(); }
                );
        }
    }
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

partial class Validator : AbstractValidator<PatchRequest>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty();
        InheritFromModel();
    }
}
",
        ];
        yield return
        [
            "When_Otherwise_Exclude_Id",
            @"
using FluentValidation;
using Rocket.Surgery.LaunchPad.Foundation;

public class Model
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null!;
    public string Something { get; set; } = null!;

    class Validator : AbstractValidator<Model>
    {
        public Validator()
        {
            When(z => z.Id != Guid.Empty, () => RuleFor(z => z.SerialNumber).NotEmpty())
               .Otherwise(
                    () => { RuleFor(z => z.SerialNumber).Empty(); }
                );
        }
    }
}

[InheritFrom(typeof(Model), Exclude = new[] { nameof(Model.Id) })]
public partial class Request : IRequest<RocketModel>
{
    public int Type { get; set; }
}

public partial class PatchRequest : IPropertyTracking<Request>, IRequest<RocketModel>
{
    public Guid Id { get; init; }
}

partial class Validator : AbstractValidator<PatchRequest>
{
    public Validator()
    {
    }
}
",
        ];

        yield return
        [
            "RuleFor_Exclude",
            @"
using FluentValidation;
using Rocket.Surgery.LaunchPad.Foundation;
public class Model
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null!;
    public string Something { get; set; } = null!;

    class Validator : AbstractValidator<Model>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.SerialNumber).NotEmpty();
            this.RuleFor(x => x.Something).NotEmpty();
        }
    }
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

partial class Validator : AbstractValidator<PatchRequest>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty();
        InheritFromModel();
    }
}
",
        ];
        yield return
        [
            "RuleSet",
            """

            using FluentValidation;
            using Rocket.Surgery.LaunchPad.Foundation;
            public class Model
            {
                public Guid Id { get; init; }
                public string SerialNumber { get; set; } = null!;
                public string Something { get; set; } = null!;

                class Validator : AbstractValidator<Model>
                {
                    public Validator()
                    {
                        RuleSet("Create",
                            () =>
                            {
                                RuleFor(x => x.SerialNumber).NotNull();
                                RuleFor(x => x.Id).NotNull();
                                RuleFor(x => x.Something).NotNull();
                            });
                        this.RuleSet("OnlySerialNumber",
                            () =>
                            {
                                RuleFor(x => x.SerialNumber).NotNull();
                            });
                    }
                }
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

            partial class Validator : AbstractValidator<PatchRequest>
            {
                public Validator()
                {
                    RuleFor(x => x.Id).NotEmpty();
                    InheritFromModel();
                }
            }

            """,
        ];

        yield return
        [
            "RuleSet_Exclude",
            """

            using FluentValidation;
            using Rocket.Surgery.LaunchPad.Foundation;
            public class Model
            {
                public Guid Id { get; init; }
                public string SerialNumber { get; set; } = null!;
                public string Something { get; set; } = null!;

                class Validator : AbstractValidator<Model>
                {
                    public Validator()
                    {
                        RuleSet("Create",
                            () =>
                            {
                                RuleFor(x => x.SerialNumber).NotNull();
                                RuleFor(x => x.Id).NotNull();
                                RuleFor(x => x.Something).NotNull();
                            });
                        this.RuleSet("OnlySerialNumber",
                            () =>
                            {
                                RuleFor(x => x.SerialNumber).NotNull();
                            });
                    }
                }
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

            partial class Validator : AbstractValidator<PatchRequest>
            {
                public Validator()
                {
                    RuleFor(x => x.Id).NotEmpty();
                    InheritFromModel();
                }
            }

            """,
        ];
    }

    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();
        Builder = Builder
                 .WithGenerator<PropertyTrackingGenerator>()
                 .AddReferences(
                      typeof(IPropertyTracking<>),
                      typeof(IMediator),
                      typeof(IBaseRequest)
                  )
                 .AddSources(
                      @"
global using System;
global using MediatR;
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
