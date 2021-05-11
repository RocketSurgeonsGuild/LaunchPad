using FluentAssertions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.Foundation;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Analyzers.Tests
{
    public class MutableGeneratorTests : GeneratorTest
    {
        public MutableGeneratorTests([NotNull] ITestOutputHelper testOutputHelper) : base(testOutputHelper, LogLevel.Trace)
        {
            WithGenerator<MutableGenerator>();
            AddReferences(typeof(MutableAttribute));
        }

        [Fact]
        public async Task Should_Require_Partial_Type_Declaration()
        {
            var source = @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Players
{
    [Mutable]
    public record Player(string Name)
    {
        public int Health { get; init; }
    }
}
";
            var result = await GenerateAsync(source);
            result.TryGetResult<InheritFromGenerator>(out var output).Should().BeTrue();
            var diagnostic = output!.Diagnostics.Should().HaveCount(1).And.Subject.First();
            diagnostic.Id.Should().Be("LPAD0001");
            diagnostic.ToString().Should().Contain("Type Sample.Players.Player must be made partial.");
        }

        [Fact]
        public async Task Should_Require_Partial_Parent_Type_Declaration()
        {
            var source = @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Players
{
    public class Parent
    {
        [Mutable]
        public record Player(string Name)
        {
            public int Health { get; init; }
        }
    }
}
";
            var result = await GenerateAsync(source);
            result.TryGetResult<InheritFromGenerator>(out var output).Should().BeTrue();
            var diagnostic = output!.Diagnostics.Should().HaveCount(1).And.Subject.First();
            diagnostic.Id.Should().Be("LPAD0001");
            diagnostic.ToString().Should().Contain("Type Sample.Players.Parent+Player must be made partial.");
        }

        [Fact]
        public async Task Should_Generate_Mutable_Record()
        {
            var source = @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Players
{
    [Mutable]
    public record Player(string Name)
    {
        public int Health { get; init; }
    }
}
";

            var expected = @"
#nullable enable
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Players
{
    public partial record PlayerViewModel
    {
        public string Name { get; set; }
        public string Health { get; set; }
        public static implicit operator Player (PlayerViewModel model) => new Player(model.Name) { Health = model.Health };
        public static implicit operator PlayerViewModel (Player model) => new PlayerViewModel { Name = model.Name, Health = model.Health };
    }
}
#nullable restore
";

            var result = await GenerateAsync(source);
            result.EnsureDiagnosticSeverity();
            result.AssertGeneratedAsExpected<InheritFromGenerator>(expected);
        }

    }

    public class InheritFromGeneratorTests : GeneratorTest
    {
        public InheritFromGeneratorTests([NotNull] ITestOutputHelper testOutputHelper) : base(testOutputHelper, LogLevel.Trace)
        {
            WithGenerator<InheritFromGenerator>();
            AddReferences(typeof(InheritFromAttribute), typeof(IMediator));
        }

        [Fact]
        public async Task Should_Require_Partial_Type_Declaration()
        {
            var source = @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial record Model
        {
            public string SerialNumber { get; set; }
        }

        [InheritFrom(typeof(Model))]
        public record Request : IRequest<Response>
        {
            public Guid Id { get; init; }
        }

        public partial record Response {}
    }
}
";
            var result = await GenerateAsync(source);
            result.TryGetResult<InheritFromGenerator>(out var output).Should().BeTrue();
            var diagnostic = output!.Diagnostics.Should().HaveCount(1).And.Subject.First();
            diagnostic.Id.Should().Be("LPAD0001");
            diagnostic.ToString().Should().Contain("Type Sample.Core.Operations.Rockets.CreateRocket+Request must be made partial.");
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
    public static class CreateRocket
    {
        public partial record Model
        {
            public string SerialNumber { get; set; }
        }

        [InheritFrom(typeof(Model))]
        public partial record Request : IRequest<Response>
        {
            public Guid Id { get; init; }
        }

        public partial record Response {}
    }
}
";
            var result = await GenerateAsync(source);
            result.TryGetResult<InheritFromGenerator>(out var output).Should().BeTrue();
            var diagnostic = output!.Diagnostics.Should().HaveCount(1).And.Subject.First();
            diagnostic.Id.Should().Be("LPAD0001");
            diagnostic.ToString().Should().Contain("Type Sample.Core.Operations.Rockets.CreateRocket must be made partial.");
        }

        [Fact]
        public async Task Should_Generate_With_Method_For_Record()
        {
            var source = @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial record Model
        {
            public string SerialNumber { get; set; }
        }

        [InheritFrom(typeof(Model))]
        public partial record Request : IRequest<Response>
        {
            public Guid Id { get; init; }
        }

        public partial record Response {}
    }
}
";

            var expected = @"
#nullable enable
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial record Request
        {
            public string SerialNumber { get; set; }

            public Request With(Model value) => this with {SerialNumber = value.SerialNumber};
        }
    }
}
#nullable restore
";

            var result = await GenerateAsync(source);
            result.EnsureDiagnosticSeverity();
            result.AssertGeneratedAsExpected<InheritFromGenerator>(expected);
        }

        [Fact]
        public async Task Should_Generate_With_Method_For_Record_That_Inherits()
        {
            var source = @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial record Model
        {
            public string SerialNumber { get; set; }
        }

        [InheritFrom(typeof(Model))]
        public partial record Request : Model, IRequest<Response>
        {
            public Guid Id { get; init; }
        }

        public partial record Response {}
    }
}
";

            var expected = @"
#nullable enable
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial record Request
        {
            public Request With(Model value) => this with {SerialNumber = value.SerialNumber};
        }
    }
}
#nullable restore
";

            var result = await GenerateAsync(source);
            result.EnsureDiagnosticSeverity();
            result.AssertGeneratedAsExpected<InheritFromGenerator>(expected);
        }

        [Fact]
        public async Task Should_Generate_With_Method_For_Class()
        {
            var source = @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial class Model
        {
            public string SerialNumber { get; set; }
        }

        [InheritFrom(typeof(Model))]
        public partial class Request : IRequest<Response>
        {
            public Guid Id { get; init; }
        }

        public partial record Response {}
    }
}
";

            var expected = @"
#nullable enable
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial class Request
        {
            public string SerialNumber { get; set; }

            public Request With(Model value) => new Request{Id = this.Id, SerialNumber = value.SerialNumber};
        }
    }
}
#nullable restore
";

            var result = await GenerateAsync(source);
            result.EnsureDiagnosticSeverity();
            result.AssertGeneratedAsExpected<InheritFromGenerator>(expected);
        }
    }
}