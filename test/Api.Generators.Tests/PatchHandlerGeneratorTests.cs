using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Rocket.Surgery.LaunchPad.Api.Generators;

namespace Api.Generators.Tests;

/// <summary>
///     The handle method of a patch operation: the application declares the handler class and the mediator's handler interface, and the
///     generator writes the method that applies the patch and sends the full request.
/// </summary>
public class PatchHandlerGeneratorTests() : GeneratorTest()
{
    // Foundation's interface, declared here so the test needs no reference to Foundation; the generator binds it by metadata name.
    private const string PropertyTracking = """
        namespace Rocket.Surgery.LaunchPad.Foundation
        {
            public interface IPropertyTracking<T>
            {
                T ApplyChanges(T state);
                void ResetChanges();
            }
        }
        """;

    // ZeroAlloc generates IMediator into the application; here it is declared by hand, as its generator would.
    private const string ZeroAllocMediator = """
        namespace ZeroAlloc.Mediator
        {
            internal interface IMediator
            {
                System.Threading.Tasks.ValueTask<Sample.RocketModel> Send(Sample.EditRocket.Request request, System.Threading.CancellationToken ct = default);
            }
        }
        """;

    private static string Operations(Mediator mediator, string handler)
    {
        var (usings, request, handlerInterface, sender) = mediator switch
        {
            Mediator.MediatR => ("using MediatR;", "IRequest<RocketModel>", "IRequestHandler<PatchRequest, RocketModel>", "ISender"),
            Mediator.Mediator => ("using Mediator;", "IRequest<RocketModel>", "IRequestHandler<PatchRequest, RocketModel>", "ISender"),
            Mediator.ZeroAlloc => ("using ZeroAlloc.Mediator;", "IRequest<RocketModel>", "IRequestHandler<PatchRequest, RocketModel>", "IMediator"),
            Mediator.ChannelMediator => throw new NotImplementedException(),
            Mediator.ImmediateHandlers => throw new NotImplementedException(),
            Mediator.Foundatio => throw new NotImplementedException(),
            Mediator.Custom => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(mediator)),
        };
        return $$"""
            #nullable enable
            {{usings}}
            using System.Threading;
            using System.Threading.Tasks;
            using Rocket.Surgery.LaunchPad.Foundation;

            namespace Sample;

            public record RocketModel(string Id, string SerialNumber);

            public static partial class EditRocket
            {
                public record Request(string Id, string SerialNumber) : {{request}};

                public record PatchRequest(string Id) : {{request}}, IPropertyTracking<Request>
                {
                    public string? SerialNumber { get; init; }

                    public Request ApplyChanges(Request state) => SerialNumber is { } serialNumber ? state with { SerialNumber = serialNumber } : state;

                    public void ResetChanges() { }
                }

                {{handler.Replace("{sender}", sender).Replace("{handlerInterface}", handlerInterface)}}
            }
            """;
    }

    private const string Handler = """
        internal partial class EditRocketHandler({sender} mediator) : {handlerInterface}
                {
                    private partial async Task<Request> GetRequest(PatchRequest request, CancellationToken cancellationToken) =>
                        await Task.FromResult(new Request(request.Id, "SN12345678")).ConfigureAwait(false);
                }
        """;

    private const string ValueTaskHandler = """
        internal partial class EditRocketHandler({sender} mediator) : {handlerInterface}
                {
                    private partial ValueTask<Request> GetRequest(PatchRequest request, CancellationToken cancellationToken) =>
                        new(new Request(request.Id, "SN12345678"));
                }
        """;

    private async Task<GeneratorTestResults> Generate(Mediator mediator, string source, params string[] sources) =>
        await GeneratorTestContextBuilder
             .Create()
             .WithLogger(Logger)
             .WithAssemblyLoadContext(AssemblyLoadContext)
             .WithProjectName("Server")
             .AddPreprocessorSymbol("NET7_0_OR_GREATER")
             .WithGenerator<PatchHandlerGenerator>()
             .AddReferences(FrameworkReferences)
             .AddReferences(MediatorAssemblies(mediator))
             .AddSources(PropertyTracking)
             .AddSources(sources)
             .AddSources(source)
             .Build()
             .GenerateAsync(TestContext.CancellationToken);

    [Test]
    [Arguments(Mediator.MediatR)]
    [Arguments(Mediator.Mediator)]
    [Arguments(Mediator.ZeroAlloc)]
    public async Task Should_Generate_A_Patch_Handler(Mediator mediator)
    {
        var result = await Generate(
            mediator,
            Operations(mediator, mediator == Mediator.MediatR ? Handler : ValueTaskHandler),
            mediator == Mediator.ZeroAlloc ? [ZeroAllocMediator] : []
        );

        AssertCompiles(result);
        await Verify(result).UseParameters(mediator);
    }

    [Test]
    public async Task Should_Find_A_Sender_The_Mediator_Generates()
    {
        // Without ZeroAlloc's generator the IMediator is an unresolved type, as it is to this generator in a real build.
        var result = await Generate(Mediator.ZeroAlloc, Operations(Mediator.ZeroAlloc, ValueTaskHandler));

        await Verify(result);
    }

    [Test]
    public async Task Should_Report_A_Handler_Without_A_Sender()
    {
        var result = await Generate(
            Mediator.MediatR,
            Operations(Mediator.MediatR, Handler.Replace("({sender} mediator)", "(string name)"))
        );

        await VerifyDiagnostics(result);
    }

    [Test]
    public async Task Should_Report_A_Handler_In_A_Type_That_Is_Not_Partial()
    {
        var result = await Generate(
            Mediator.MediatR,
            Operations(Mediator.MediatR, Handler).Replace("public static partial class EditRocket", "public static class EditRocket")
        );

        await VerifyDiagnostics(result);
    }

    [Test]
    public async Task Should_Leave_A_Handler_That_Implements_The_Patch()
    {
        var result = await Generate(
            Mediator.MediatR,
            Operations(
                Mediator.MediatR,
                """
                internal partial class EditRocketHandler(ISender mediator) : IRequestHandler<PatchRequest, RocketModel>
                        {
                            public Task<RocketModel> Handle(PatchRequest request, CancellationToken cancellationToken) =>
                                mediator.Send(request.ApplyChanges(new Request(request.Id, "SN12345678")), cancellationToken);
                        }
                """
            )
        );

        AssertCompiles(result);
        await Verify(result);
    }
}
