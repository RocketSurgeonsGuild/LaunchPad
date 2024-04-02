using System.Linq.Expressions;
using System.Collections.Immutable;
using Analyzers.Tests.Helpers;
using FluentValidation;
using MediatR;
using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Analyzers.Tests;

public class InheritFromGeneratorTests(ITestOutputHelper testOutputHelper) : GeneratorTest(testOutputHelper)
{
    [Fact]
    public async Task Should_Require_Partial_Type_Declaration()
    {
        var result = await Builder
                          .AddSources(
                               @"
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
"
                           )
                          .Build()
                          .GenerateAsync();
        result.TryGetResult<InheritFromGenerator>(out var output).Should().BeTrue();
        var diagnostic = output!.Diagnostics.Should().HaveCount(1).And.Subject.First();
        diagnostic.Id.Should().Be("LPAD0001");
        diagnostic.ToString().Should().Contain("Type Sample.Core.Operations.Rockets.CreateRocket+Request must be made partial.");

        await Verify(result);
    }

    [Fact]
    public async Task Should_Require_Partial_Parent_Type_Declaration()
    {
        var result = await Builder
                          .AddSources(
                               @"
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
"
                           )
                          .Build()
                          .GenerateAsync();
        result.TryGetResult<InheritFromGenerator>(out var output).Should().BeTrue();
        var diagnostic = output!.Diagnostics.Should().HaveCount(1).And.Subject.First();
        diagnostic.Id.Should().Be("LPAD0001");
        diagnostic.ToString().Should().Contain("Type Sample.Core.Operations.Rockets.CreateRocket must be made partial.");

        await Verify(result);
    }

    [Fact]
    public async Task Should_Generate_With_Method_For_Record()
    {
        var result = await Builder
                          .AddSources(
                               @"
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
            public string Excluded { get; set; }
        }

        [InheritFrom(typeof(Model), Exclude = new[] { nameof(Model.Excluded) })]
        public partial record Request : IRequest<Response>
        {
            public Guid Id { get; init; }
        }

        public partial record Response {}
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();
        await Verify(result);
    }

    [Fact]
    public async Task Should_Generate_And_Ignore_Type_Declaration_Members()
    {
        var result = await Builder
                          .AddSources(
                               @"
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

            private class Mapper;
        }

        [InheritFrom(typeof(Model))]
        public partial record Request : IRequest<Response>
        {
            public Guid Id { get; init; }
        }

        public partial record Response {}
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();
        await Verify(result);
    }

    [Fact]
    public async Task Should_Inherit_Multiple_With_Method_For_Record()
    {
        var result = await Builder
                          .AddReferences(typeof(ImmutableArray<>))
                          .AddSources(
                               @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;
using System.Collections.Immutable;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial record Model
        {
            public string SerialNumber { get; set; }
            public ImmutableArray<string> Items { get; set; }
        }
    }
}",
                               @"
using System;
using MediatR;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Sample.Core.Operations.Rockets
{
    public static partial class CreateRocket
    {
        public partial record Other
        {
            public string OtherNumber { get; set; }
        }

        [InheritFrom(typeof(Model))]
        [InheritFrom(typeof(Other))]
        public partial record Request : IRequest<Response>
        {
            public Guid Id { get; init; }
        }

        public partial record Response {}
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    #if !ROSLYN4_0
    [Fact]
    public async Task Should_Inherit_Using_Generic_Type_Arguments()
    {
        var result = await Builder
                          .AddSources(
                               @"
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
            public string Excluded { get; set; }
        }

        public partial record Other
        {
            public string OtherNumber { get; set; }
        }

        [InheritFrom<Model>(Exclude = new[] { nameof(Model.Excluded) })]
        [InheritFrom<Other>]
        public partial record Request : IRequest<Response>
        {
            public Guid Id { get; init; }
        }

        public partial record Response {}
    }
}
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }
    #endif

    [Fact]
    public async Task Should_Generate_With_Method_For_Record_That_Inherits()
    {
        var result = await Builder
                          .AddSources(
                               @"
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
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Fact]
    public async Task Should_Generate_With_Method_For_Class()
    {
        var result = await Builder
                          .AddSources(
                               @"
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
"
                           )
                          .Build()
                          .GenerateAsync();

        await Verify(result);
    }

    [Theory]
    [MemberData(nameof(Should_Generate_Class_With_Underlying_FluentValidation_Validator_Methods_Data))]
    public async Task Should_Generate_Class_With_Underlying_FluentValidation_Validator_Methods(string name, string source)
    {
        var result = await Builder
                          .AddReferences(typeof(AbstractValidator<>))
                          .AddReferences(typeof(Expression))
                          .AddSources(source)
                          .Build()
                          .GenerateAsync();
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

partial class Validator : AbstractValidator<Request>
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

partial class Validator : AbstractValidator<Request>
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

partial class Validator : AbstractValidator<Request>
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

partial class Validator : AbstractValidator<Request>
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

partial class Validator : AbstractValidator<Request>
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

partial class Validator : AbstractValidator<Request>
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

partial class Validator : AbstractValidator<Request>
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

partial class Validator : AbstractValidator<Request>
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
            RuleSet(""Create"",
                () =>
                {
                    RuleFor(x => x.SerialNumber).NotNull();
                    RuleFor(x => x.Id).NotNull();
                    RuleFor(x => x.Something).NotNull();
                });
            this.RuleSet(""OnlySerialNumber"",
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

partial class Validator : AbstractValidator<Request>
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
            "RuleSet_Exclude",
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
            RuleSet(""Create"",
                () =>
                {
                    RuleFor(x => x.SerialNumber).NotNull();
                    RuleFor(x => x.Id).NotNull();
                    RuleFor(x => x.Something).NotNull();
                });
            this.RuleSet(""OnlySerialNumber"",
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

partial class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty();
        InheritFromModel();
    }
}
",
        ];
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        Builder = Builder
                 .WithGenerator<InheritFromGenerator>()
                 .AddReferences(
                      typeof(InheritFromAttribute),
                      typeof(IMediator),
                      typeof(IBaseRequest)
                  )
                 .AddSources(
                      @"
global using System;
global using MediatR;
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
