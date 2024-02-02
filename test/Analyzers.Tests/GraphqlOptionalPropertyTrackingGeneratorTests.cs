using Analyzers.Tests.Helpers;
using DryIoc.ImTools;
using HotChocolate;
using MediatR;
using Microsoft.Extensions.Logging;
using NodaTime;
using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.HotChocolate;

namespace Analyzers.Tests;

public class GraphqlOptionalPropertyTrackingGeneratorTests : GeneratorTest
{
    [Fact]
    public async Task Should_Require_Partial_Type_Declaration()
    {
        var source = @"
namespace Sample.Core.Operations.Rockets
{
    public class Request : IRequest<RocketModel>
    {
        public Guid Id { get; init; }
        public string SerialNumber { get; set; } = null!;
        public int Type { get; set; }
        public Instant PlannedDate { get; set; }
    }
    public class PatchGraphRocket : IOptionalTracking<Request>, IRequest<RocketModel>
    {
        public Guid Id { get; init; }
    }
}
";
        var result = await GenerateAsync(source);
        result.TryGetResult<GraphqlOptionalPropertyTrackingGenerator>(out var output).Should().BeTrue();
        var diagnostic = output!.Diagnostics.Should().HaveCount(1).And.Subject.First();
        diagnostic.Id.Should().Be("LPAD0001");
        diagnostic.ToString().Should().Contain("Type Sample.Core.Operations.Rockets.PatchGraphRocket must be made partial.");

        await Verify(result);
    }

    [Fact]
    public async Task Should_Require_Partial_Parent_Type_Declaration()
    {
        var source = @"
namespace Sample.Core.Operations.Rockets
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; init; }
        public string SerialNumber { get; set; } = null!;
        public int Type { get; set; }
        public Instant PlannedDate { get; set; }
    }
    public static class PublicClass
    {
        public partial record PatchGraphRocket : IOptionalTracking<Request>, IRequest<RocketModel>
        {
            public Guid Id { get; init; }
        }
    }
}
";
        var result = await GenerateAsync(source);
        result.TryGetResult<GraphqlOptionalPropertyTrackingGenerator>(out var output).Should().BeTrue();
        var diagnostic = output!.Diagnostics.Should().HaveCount(1).And.Subject.First();
        diagnostic.Id.Should().Be("LPAD0001");
        diagnostic.ToString().Should().Contain("Type Sample.Core.Operations.Rockets.PublicClass must be made partial.");

        await Verify(result);
    }

    [Fact]
    public async Task Should_Require_Same_Type_As_Record()
    {
        var source = @"
namespace Sample.Core.Operations.Rockets
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; init; }
        public string SerialNumber { get; set; } = null!;
        public int Type { get; set; }
        public Instant PlannedDate { get; set; }
    }
    public partial class PatchGraphRocket : IOptionalTracking<Request>, IRequest<RocketModel>
    {
        public Guid Id { get; init; }
    }
}
";
        var result = await GenerateAsync(source);
        result.TryGetResult<GraphqlOptionalPropertyTrackingGenerator>(out var output).Should().BeTrue();
        var diagnostic = output!.Diagnostics.Should().HaveCount(1).And.Subject.First();
        diagnostic.Id.Should().Be("LPAD0005");
        diagnostic.ToString().Should().Contain("The declaration Sample.Core.Operations.Rockets.PatchGraphRocket must be a record.");

        await Verify(result);
    }

    [Fact]
    public async Task Should_Require_Same_Type_As_Class()
    {
        var source = @"
namespace Sample.Core.Operations.Rockets
{
    public class Request : IRequest<RocketModel>
    {
        public Guid Id { get; init; }
        public string SerialNumber { get; set; } = null!;
        public int Type { get; set; }
        public Instant PlannedDate { get; set; }
    }
    public partial record PatchGraphRocket : IOptionalTracking<Request>, IRequest<RocketModel>
    {
        public Guid Id { get; init; }
    }
}
";
        var result = await GenerateAsync(source);
        result.TryGetResult<GraphqlOptionalPropertyTrackingGenerator>(out var output).Should().BeTrue();
        var diagnostic = output!.Diagnostics.Should().HaveCount(1).And.Subject.First();
        diagnostic.Id.Should().Be("LPAD0005");
        diagnostic.ToString().Should().Contain("The declaration Sample.Core.Operations.Rockets.PatchGraphRocket must be a class.");

        await Verify(result);
    }

    [Fact]
    public async Task Should_Support_Nullable_Class_Property()
    {
        var source = @"
namespace Sample.Core.Operations.Rockets
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; init; }
        public string? SerialNumber { get; set; } = null!;
        public int Type { get; set; }
        public Instant PlannedDate { get; set; }
    }
    public partial record PatchRocket : IOptionalTracking<Request>, IRequest<RocketModel>
    {
        public Guid Id { get; init; }
    }
}
";
        var result = await GenerateAsync(source);
        result.TryGetResult<GraphqlOptionalPropertyTrackingGenerator>(out _).Should().BeTrue();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Support_Nullable_Builtin_Struct_Property()
    {
        var source = @"
namespace Sample.Core.Operations.Rockets
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; init; }
        public string SerialNumber { get; set; } = null!;
        public int? Type { get; set; }
        public Instant PlannedDate { get; set; }
    }
    public partial record PatchRocket : IOptionalTracking<Request>, IRequest<RocketModel>
    {
        public Guid Id { get; init; }
    }
}
";
        var result = await GenerateAsync(source);
        result.TryGetResult<GraphqlOptionalPropertyTrackingGenerator>(out _).Should().BeTrue();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Support_Nullable_Struct_Property()
    {
        var source = @"
namespace Sample.Core.Operations.Rockets
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; init; }
        public string SerialNumber { get; set; } = null!;
        public int Type { get; set; }
        public Instant? PlannedDate { get; set; }
    }
    public partial record PatchRocket : IOptionalTracking<Request>, IRequest<RocketModel>
    {
        public Guid Id { get; init; }
    }
}
";
        var result = await GenerateAsync(source);
        result.TryGetResult<GraphqlOptionalPropertyTrackingGenerator>(out _).Should().BeTrue();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Support_Nullable_Enum_Property()
    {
        var source = @"
namespace Sample.Core.Operations.Rockets
{
    public enum RocketType { Falcon9, FalconHeavy, AtlasV }

    public record Request : IRequest<RocketModel>
    {
        public RocketId Id { get; init; }
        public string SerialNumber { get; set; } = null!;
        public RocketType Type { get; set; }
        public Instant? PlannedDate { get; set; }
    }
    public partial record PatchRocketUnderTest : IOptionalTracking<Request>
    {
    }
}
";
        var result = await GenerateAsync(source);
        result.TryGetResult<GraphqlOptionalPropertyTrackingGenerator>(out _).Should().BeTrue();

        await Verify(result);
    }

    public GraphqlOptionalPropertyTrackingGeneratorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper, LogLevel.Trace)
    {
        WithGenerator<GraphqlOptionalPropertyTrackingGenerator>();
        AddReferences(
            typeof(IOptionalTracking<>),
            typeof(Optional<>),
            typeof(Instant),
            typeof(IPropertyTracking<>),
            typeof(IMediator),
            typeof(IBaseRequest)
        );
        AddSources(
            @"
global using System;
global using MediatR;
global using NodaTime;
global using Rocket.Surgery.LaunchPad.Foundation;
global using Rocket.Surgery.LaunchPad.HotChocolate;
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

    [Theory]
    [InlineData("SerialNumber", "12345")]
    [InlineData("Type", 12345)]
    public async Task Should_Generate_Record_With_Underlying_Properties_And_Create(string property, object value)
    {
        var valueType = value.GetType();
        if (value.GetType().IsValueType)
        {
            valueType = typeof(Nullable<>).MakeGenericType(value.GetType());
        }

        var source = @"
public record Request : IRequest<RocketModel>
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null!;
    public int Type { get; set; }
    public Instant PlannedDate { get; set; }
}
public partial record PatchGraphRocket : IOptionalTracking<Request>, IRequest<RocketModel>
{
}
";
        var result = await GenerateAsync(source);
        result.TryGetResult<GraphqlOptionalPropertyTrackingGenerator>(out var output).Should().BeTrue();
        output!.Diagnostics.Should().HaveCount(0);

        var assembly = EmitAssembly(result).Should().NotBeNull().And.Subject;
        var type = assembly.DefinedTypes.FindFirst(z => z.Name == "PatchGraphRocket");
        var applyChangesMethod = type.GetMethod("Create")!;
        var propertyUnderTest = type.GetProperty(property)!;
        var requestType = assembly.DefinedTypes.FindFirst(z => z.Name == "Request");
        var requestPropertyUnderTest = requestType.GetProperty(property)!;
        var instance = Activator.CreateInstance(type);

        var assignedType = typeof(Optional<>).MakeGenericType(valueType);
        propertyUnderTest.SetValue(instance, Activator.CreateInstance(assignedType, value));
        var request = applyChangesMethod.Invoke(instance, Array.Empty<object>());
        var r = requestPropertyUnderTest.GetValue(request);
        r.Should().Be(value);

        await Verify(result).UseParameters(property, value);
    }

    [Theory]
    [InlineData("SerialNumber", "12345")]
    [InlineData("Type", 12345)]
    public async Task Should_Generate_Class_With_Underlying_Properties_And_Create(string property, object value)
    {
        var valueType = value.GetType();
        if (value.GetType().IsValueType)
        {
            valueType = typeof(Nullable<>).MakeGenericType(value.GetType());
        }

        var source = @"
public class Request : IRequest<RocketModel>
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null!;
    public int Type { get; set; }
    public Instant PlannedDate { get; set; }
}
public partial class PatchGraphRocket : IOptionalTracking<Request>, IRequest<RocketModel>
{
    public Guid Id { get; init; }
}
";
        var result = await GenerateAsync(source);
        result.TryGetResult<GraphqlOptionalPropertyTrackingGenerator>(out var output).Should().BeTrue();
        output!.Diagnostics.Should().HaveCount(0);

        var assembly = EmitAssembly(result).Should().NotBeNull().And.Subject;
        var type = assembly.DefinedTypes.FindFirst(z => z.Name == "PatchGraphRocket");
        var applyChangesMethod = type.GetMethod("Create")!;
        var propertyUnderTest = type.GetProperty(property)!;
        var requestType = assembly.DefinedTypes.FindFirst(z => z.Name == "Request");
        var requestPropertyUnderTest = requestType.GetProperty(property)!;
        var instance = Activator.CreateInstance(type);

        var assignedType = typeof(Optional<>).MakeGenericType(valueType);
        propertyUnderTest.SetValue(instance, Activator.CreateInstance(assignedType, value));
        var request = applyChangesMethod.Invoke(instance, Array.Empty<object>());
        var r = requestPropertyUnderTest.GetValue(request);
        r.Should().Be(value);

        await Verify(result).UseParameters(property, value);
    }


    [Theory]
    [InlineData("SerialNumber", "12345")]
    [InlineData("Type", 12345)]
    public async Task Should_Generate_Record_With_Underlying_IPropertyTracking_Properties_And_Create(string property, object value)
    {
        var valueType = value.GetType();
        if (value.GetType().IsValueType)
        {
            valueType = typeof(Nullable<>).MakeGenericType(value.GetType());
        }

        AddPatchRocketModel(RocketModelType.Record);
        var source = @"
public record Request : IRequest<RocketModel>
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null!;
    public int Type { get; set; }
    public Instant PlannedDate { get; set; }
}
public partial record PatchGraphRocket : IOptionalTracking<PatchRocket>
{
    public Guid Id { get; init; }
}
";
        var result = await GenerateAsync(source);
        result.TryGetResult<GraphqlOptionalPropertyTrackingGenerator>(out var output).Should().BeTrue();
        output!.Diagnostics.Should().HaveCount(0);

        var assembly = EmitAssembly(result).Should().NotBeNull().And.Subject;
        var type = assembly.DefinedTypes.FindFirst(z => z.Name == "PatchGraphRocket");
        var applyChangesMethod = type.GetMethod("Create")!;
        var propertyUnderTest = type.GetProperty(property)!;
        var requestType = assembly.DefinedTypes.FindFirst(z => z.Name == "PatchRocket");
        var requestPropertyUnderTest = requestType.GetProperty(property)!;
        var instance = Activator.CreateInstance(type);

        var optionalType = typeof(Optional<>).MakeGenericType(valueType);
        var assignedType = typeof(Assigned<>).MakeGenericType(value.GetType());
        var assignedPropertyUnderTest = assignedType.GetProperty("Value")!;
        propertyUnderTest.SetValue(instance, Activator.CreateInstance(optionalType, value));
        var request = applyChangesMethod.Invoke(instance, Array.Empty<object>());
        var r = requestPropertyUnderTest.GetValue(request);
        assignedPropertyUnderTest.GetValue(r).Should().Be(value);

        await Verify(result).UseParameters(property, value);
    }

    [Theory]
    [InlineData("SerialNumber", "12345")]
    [InlineData("Type", 12345)]
    public async Task Should_Generate_Class_With_Underlying_IPropertyTracking_Properties_And_Create(string property, object value)
    {
        var valueType = value.GetType();
        if (value.GetType().IsValueType)
        {
            valueType = typeof(Nullable<>).MakeGenericType(value.GetType());
        }

        AddPatchRocketModel(RocketModelType.Class);
        var source = @"
public class Request : IRequest<RocketModel>
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null!;
    public int Type { get; set; }
    public Instant PlannedDate { get; set; }
}
public partial class PatchGraphRocket : IOptionalTracking<PatchRocket>
{
    public Guid Id { get; init; }
}
";
        var result = await GenerateAsync(source);
        result.TryGetResult<GraphqlOptionalPropertyTrackingGenerator>(out var output).Should().BeTrue();
        output!.Diagnostics.Should().HaveCount(0);

        var assembly = EmitAssembly(result).Should().NotBeNull().And.Subject;
        var type = assembly.DefinedTypes.FindFirst(z => z.Name == "PatchGraphRocket");
        var applyChangesMethod = type.GetMethod("Create")!;
        var propertyUnderTest = type.GetProperty(property)!;
        var requestType = assembly.DefinedTypes.FindFirst(z => z.Name == "PatchRocket");
        var requestPropertyUnderTest = requestType.GetProperty(property)!;
        var instance = Activator.CreateInstance(type);

        var optionalType = typeof(Optional<>).MakeGenericType(valueType);
        var assignedType = typeof(Assigned<>).MakeGenericType(value.GetType());
        var assignedPropertyUnderTest = assignedType.GetProperty("Value")!;
        propertyUnderTest.SetValue(instance, Activator.CreateInstance(optionalType, value));
        var request = applyChangesMethod.Invoke(instance, Array.Empty<object>());
        var r = requestPropertyUnderTest.GetValue(request);
        assignedPropertyUnderTest.GetValue(r).Should().Be(value);

        await Verify(result).UseParameters(property, value);
    }

    private enum RocketModelType { Record, Class, }

    private void AddPatchRocketModel(RocketModelType type)
    {
        if (type == RocketModelType.Record)
        {
            AddSources(
                @"

namespace Sample.Core.Operations.Rockets
{
    public partial record PatchRocket : IPropertyTracking<Request>, IRequest<RocketModel>
    {
        public Guid Id { get; init; }
    }
    public partial record PatchRocket
    {
        public Rocket.Surgery.LaunchPad.Foundation.Assigned<string> SerialNumber { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(default);
        public Rocket.Surgery.LaunchPad.Foundation.Assigned<int> Type { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(default);
        public Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant> PlannedDate { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant>.Empty(default);

        public record Changes
        {
            public bool SerialNumber { get; init; }

            public bool Type { get; init; }

            public bool PlannedDate { get; init; }
        }

        public Changes GetChangedState()
        {
            return new Changes()
            {SerialNumber = SerialNumber.HasBeenSet(), Type = Type.HasBeenSet(), PlannedDate = PlannedDate.HasBeenSet()};
        }

        public Request ApplyChanges(Request value)
        {
            if (SerialNumber.HasBeenSet())
            {
                value = value with {SerialNumber = SerialNumber};
            }

            if (Type.HasBeenSet())
            {
                value = value with {Type = Type};
            }

            if (PlannedDate.HasBeenSet())
            {
                value.PlannedDate = PlannedDate;
            }

            ResetChanges();
            return value;
        }

        public PatchRocket ResetChanges()
        {
            SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(SerialNumber);
            Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(Type);
            PlannedDate = Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant>.Empty(PlannedDate);
            return this;
        }

        void IPropertyTracking<Request>.ResetChanges()
        {
            ResetChanges();
        }
    }
}"
            );
        }
        else if (type == RocketModelType.Class)
        {
            AddSources(
                @"
namespace Sample.Core.Operations.Rockets
    {
    public partial class PatchRocket : IPropertyTracking<Request>, IRequest<RocketModel>
    {
        public Guid Id { get; init; }
    }

    public partial class PatchRocket
    {
        public Rocket.Surgery.LaunchPad.Foundation.Assigned<string> SerialNumber { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(default);
        public Rocket.Surgery.LaunchPad.Foundation.Assigned<int> Type { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(default);
        public Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant> PlannedDate { get; set; } = Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant>.Empty(default);

        public record Changes
        {
            public bool SerialNumber { get; init; }

            public bool Type { get; init; }

            public bool PlannedDate { get; init; }
        }

        public Changes GetChangedState()
        {
            return new Changes()
            {SerialNumber = SerialNumber.HasBeenSet(), Type = Type.HasBeenSet(), PlannedDate = PlannedDate.HasBeenSet()};
        }

        public Request ApplyChanges(Request value)
        {
            if (SerialNumber.HasBeenSet())
            {
                value.SerialNumber = SerialNumber;
            }

            if (Type.HasBeenSet())
            {
                value.Type = Type;
            }

            if (PlannedDate.HasBeenSet())
            {
                value.PlannedDate = PlannedDate;
            }

            ResetChanges();
            return value;
        }

        public PatchRocket ResetChanges()
        {
            SerialNumber = Rocket.Surgery.LaunchPad.Foundation.Assigned<string>.Empty(SerialNumber);
            Type = Rocket.Surgery.LaunchPad.Foundation.Assigned<int>.Empty(Type);
            PlannedDate = Rocket.Surgery.LaunchPad.Foundation.Assigned<Instant>.Empty(PlannedDate);
            return this;
        }

        void IPropertyTracking<Request>.ResetChanges()
        {
            ResetChanges();
        }
    }
}
"
            );
        }
    }
}