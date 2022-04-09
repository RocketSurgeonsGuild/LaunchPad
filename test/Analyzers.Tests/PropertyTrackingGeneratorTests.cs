using Analyzers.Tests.Helpers;
using ImTools;
using MediatR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Analyzers.Tests;

public class PropertyTrackingGeneratorTests : GeneratorTest
{
    [Fact]
    public async Task Should_Require_Partial_Type_Declaration()
    {
        var source = @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
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
";
        var result = await GenerateAsync(source);
        result.TryGetResult<PropertyTrackingGenerator>(out var output).Should().BeTrue();
        var diagnostic = output!.Diagnostics.Should().HaveCount(1).And.Subject.First();
        diagnostic.Id.Should().Be("LPAD0001");
        diagnostic.ToString().Should().Contain("Type Sample.Core.Operations.Rockets.PatchRocket must be made partial.");
    }

    [Fact]
    public async Task Should_Require_Partial_Parent_Type_Declaration()
    {
        var source = @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
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
";
        var result = await GenerateAsync(source);
        result.TryGetResult<PropertyTrackingGenerator>(out var output).Should().BeTrue();
        var diagnostic = output!.Diagnostics.Should().HaveCount(1).And.Subject.First();
        diagnostic.Id.Should().Be("LPAD0001");
        diagnostic.ToString().Should().Contain("Type Sample.Core.Operations.Rockets.PublicClass must be made partial.");
    }

    [Fact]
    public async Task Should_Generate_Record_With_Underlying_Properties_And_Track_Changes()
    {
        var source = @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Operations.Rockets;

public record Request : IRequest<RocketModel>
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null!;
    public int Type { get; set; }
}
public partial record PatchRocket : IPropertyTracking<Request>, IRequest<RocketModel>
{
}
";
        var result = await GenerateAsync(source);
        result.TryGetResult<PropertyTrackingGenerator>(out var output).Should().BeTrue();
        output!.Diagnostics.Should().HaveCount(0);

        var assembly = EmitAssembly(result).Should().NotBeNull().And.Subject;
        var type = assembly.DefinedTypes.FindFirst(z => z.Name == "PatchRocket");
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
        var changes = getChangesMethod.Invoke(instance, Array.Empty<object>());

        var serialNumberChanged = (bool)serialNumberChangedProperty.GetValue(changes)!;
        serialNumberChanged.Should().BeTrue();

        typeProperty.SetValue(instance, new Assigned<int>(12345));
        changes = getChangesMethod.Invoke(instance, Array.Empty<object>());

        var typeChanged = (bool)typeChangedProperty.GetValue(changes)!;
        typeChanged.Should().BeTrue();

        idProperty.SetValue(instance, new Assigned<Guid>(Guid.NewGuid()));
        changes = getChangesMethod.Invoke(instance, Array.Empty<object>());

        var idChanged = (bool)idChangedProperty.GetValue(changes)!;
        idChanged.Should().BeTrue();
    }


    [Fact]
    public async Task Should_Generate_Class_With_Underlying_Properties_And_Track_Changes()
    {
        var source = @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Operations.Rockets;

public class Request : IRequest<RocketModel>
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null!;
    public int Type { get; set; }
}
public partial class PatchRocket : IPropertyTracking<Request>, IRequest<RocketModel>
{
}
";
        var result = await GenerateAsync(source);
        result.TryGetResult<PropertyTrackingGenerator>(out var output).Should().BeTrue();
        output!.Diagnostics.Should().HaveCount(0);

        var assembly = EmitAssembly(result).Should().NotBeNull().And.Subject;
        var type = assembly.DefinedTypes.FindFirst(z => z.Name == "PatchRocket");
        var serialNumberProperty = type.GetProperty("SerialNumber")!;
        var typeProperty = type.GetProperty("Type")!;
        var getChangesMethod = type.GetMethod("GetChangedState")!;
        var changesType = getChangesMethod.ReturnType;
        var serialNumberChangedProperty = changesType.GetProperty("SerialNumber")!;
        var typeChangedProperty = changesType.GetProperty("Type")!;
        var instance = Activator.CreateInstance(type);

        serialNumberProperty.SetValue(instance, new Assigned<string>("12345"));
        var changes = getChangesMethod.Invoke(instance, Array.Empty<object>());

        var serialNumberChanged = (bool)serialNumberChangedProperty.GetValue(changes)!;
        serialNumberChanged.Should().BeTrue();

        typeProperty.SetValue(instance, new Assigned<int>(12345));
        changes = getChangesMethod.Invoke(instance, Array.Empty<object>());

        var typeChanged = (bool)typeChangedProperty.GetValue(changes)!;
        typeChanged.Should().BeTrue();

        changesType.GetProperty("Id").Should().BeNull();
        type.GetProperty("Id").Should().BeNull();
    }

    [Fact]
    public async Task Should_Require_Same_Type_As_Record()
    {
        var source = @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public record Request : IRequest<RocketModel>
    {
        public Guid Id { get; init; }
        public string SerialNumber { get; set; } = null!;
        public int Type { get; set; }
    }
    public partial class PatchRocket : IPropertyTracking<Request>, IRequest<RocketModel>
    {
        public Guid Id { get; init; }
    }
}
";
        var result = await GenerateAsync(source);
        result.TryGetResult<PropertyTrackingGenerator>(out var output).Should().BeTrue();
        var diagnostic = output!.Diagnostics.Should().HaveCount(1).And.Subject.First();
        diagnostic.Id.Should().Be("LPAD0005");
        diagnostic.ToString().Should().Contain("The declaration Sample.Core.Operations.Rockets.PatchRocket must be a record.");
    }

    [Fact]
    public async Task Should_Require_Same_Type_As_Class()
    {
        var source = @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
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
";
        var result = await GenerateAsync(source);
        result.TryGetResult<PropertyTrackingGenerator>(out var output).Should().BeTrue();
        var diagnostic = output!.Diagnostics.Should().HaveCount(1).And.Subject.First();
        diagnostic.Id.Should().Be("LPAD0005");
        diagnostic.ToString().Should().Contain("The declaration Sample.Core.Operations.Rockets.PatchRocket must be a class.");
    }

    public PropertyTrackingGeneratorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper, LogLevel.Trace)
    {
        WithGenerator<PropertyTrackingGenerator>();
        AddReferences(typeof(IPropertyTracking<>), typeof(IMediator), typeof(IBaseRequest));
        AddSources(
            @"
using System;
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
    public async Task Should_Generate_Record_With_Underlying_Properties_And_Apply_Changes(string property, object value)
    {
        var source = @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Operations.Rockets;

public record Request : IRequest<RocketModel>
{
    public Guid Id { get; init; }
    public string SerialNumber { get; set; } = null!;
    public int Type { get; set; }
}
public partial record PatchRocket : IPropertyTracking<Request>, IRequest<RocketModel>
{
}
";
        var result = await GenerateAsync(source);
        result.TryGetResult<PropertyTrackingGenerator>(out var output).Should().BeTrue();
        output!.Diagnostics.Should().HaveCount(0);

        var assembly = EmitAssembly(result).Should().NotBeNull().And.Subject;
        var type = assembly.DefinedTypes.FindFirst(z => z.Name == "PatchRocket");
        var applyChangesMethod = type.GetMethod("ApplyChanges")!;
        var propertyUnderTest = type.GetProperty(property)!;
        var requestType = assembly.DefinedTypes.FindFirst(z => z.Name == "Request");
        var requestPropertyUnderTest = requestType.GetProperty(property)!;
        var otherRequestProperties = requestType.GetProperties().Where(z => z.Name != property);
        var request = Activator.CreateInstance(requestType);
        var instance = Activator.CreateInstance(type);

        var currentPropertyValues = otherRequestProperties.Select(z => z.GetValue(request)).ToArray();

        var assignedType = typeof(Assigned<>).MakeGenericType(value.GetType());
        propertyUnderTest.SetValue(instance, Activator.CreateInstance(assignedType, value));
        request = applyChangesMethod.Invoke(instance, new[] { request });
        var r = requestPropertyUnderTest.GetValue(request);
        r.Should().Be(value);
        currentPropertyValues.Should().ContainInOrder(otherRequestProperties.Select(z => z.GetValue(request)).ToArray());
    }

    [Theory]
    [InlineData("SerialNumber", "12345")]
    [InlineData("Type", 12345)]
    public async Task Should_Generate_Class_With_Underlying_Properties_And_Apply_Changes(string property, object value)
    {
        var source = @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;
using Sample.Core.Operations.Rockets;

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
";
        var result = await GenerateAsync(source);
        result.TryGetResult<PropertyTrackingGenerator>(out var output).Should().BeTrue();
        output!.Diagnostics.Should().HaveCount(0);

        var assembly = EmitAssembly(result).Should().NotBeNull().And.Subject;
        var type = assembly.DefinedTypes.FindFirst(z => z.Name == "PatchRocket");
        var applyChangesMethod = type.GetMethod("ApplyChanges")!;
        var propertyUnderTest = type.GetProperty(property)!;
        var requestType = assembly.DefinedTypes.FindFirst(z => z.Name == "Request");
        var requestPropertyUnderTest = requestType.GetProperty(property)!;
        var otherRequestProperties = requestType.GetProperties().Where(z => z.Name != property);
        var request = Activator.CreateInstance(requestType);
        var instance = Activator.CreateInstance(type);

        var currentPropertyValues = otherRequestProperties.Select(z => z.GetValue(request)).ToArray();

        var assignedType = typeof(Assigned<>).MakeGenericType(value.GetType());
        propertyUnderTest.SetValue(instance, Activator.CreateInstance(assignedType, value));
        applyChangesMethod.Invoke(instance, new[] { request });
        var r = requestPropertyUnderTest.GetValue(request);
        r.Should().Be(value);
        currentPropertyValues.Should().ContainInOrder(otherRequestProperties.Select(z => z.GetValue(request)).ToArray());
    }
}
