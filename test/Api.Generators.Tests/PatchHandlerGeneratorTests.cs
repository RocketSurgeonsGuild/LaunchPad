using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Rocket.Surgery.LaunchPad.Analyzers.Api.Patch;

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

    // A mediator the compilation describes itself: a request marker, a handler interface returning a Task and a sender with a generic Dispatch.
    private const string CustomMediator = """
        [assembly: Rocket.Surgery.LaunchPad.Api.MediatorProfile(SenderType = typeof(Custom.Mediator.IMyDispatcher), SendMethod = "Dispatch", Name = "MyMediator")]
        [assembly: Rocket.Surgery.LaunchPad.Api.MediatorRequest(typeof(Custom.Mediator.IMyRequest<>), HandlerInterface = typeof(Custom.Mediator.IMyHandler<,>), HandleMethod = "Execute")]

        namespace Custom.Mediator
        {
            public interface IMyRequest<TResponse> { }

            public interface IMyHandler<in TRequest, TResponse> where TRequest : IMyRequest<TResponse>
            {
                System.Threading.Tasks.Task<TResponse> Execute(TRequest request, System.Threading.CancellationToken cancellationToken);
            }

            public interface IMyDispatcher
            {
                System.Threading.Tasks.Task<TResponse> Dispatch<TResponse>(IMyRequest<TResponse> request, System.Threading.CancellationToken cancellationToken = default);
            }
        }
        """;

    private static string Operations(Mediator mediator, string handler)
    {
        // The request marker (Immediate.Handlers has none), the handler interface the application declares (Foundatio has only the non-generic
        // IHandler marker) and the service the handler sends through (Immediate.Handlers has no sender: the handler of the full request).
        var (usings, request, handlerInterface, sender) = mediator switch
        {
            Mediator.Mediator => ("using Mediator;", "IRequest<RocketModel>", "IRequestHandler<PatchRequest, RocketModel>", "ISender"),
            Mediator.ZeroAlloc => ("using ZeroAlloc.Mediator;", "IRequest<RocketModel>", "IRequestHandler<PatchRequest, RocketModel>", "IMediator"),
            Mediator.ChannelMediator => ("using ChannelMediator;", "IRequest<RocketModel>", "IRequestHandler<PatchRequest, RocketModel>", "IMediator"),
            Mediator.ImmediateHandlers => ("using Immediate.Handlers.Shared;", null, "IHandler<PatchRequest, RocketModel>", "IHandler<Request, RocketModel>"),
            Mediator.Foundatio => ("using Foundatio.Mediator;", "IRequest<RocketModel>", "IHandler", "IMediator"),
            Mediator.Custom => ("using Custom.Mediator;", "IMyRequest<RocketModel>", "IMyHandler<PatchRequest, RocketModel>", "IMyDispatcher"),
            _ => throw new ArgumentOutOfRangeException(nameof(mediator)),
        };
        // GetRequest returns the task type the handler interface does.
        var task = mediator is Mediator.ChannelMediator or Mediator.Custom ? "Task" : "ValueTask";
        var marker = request is null ? "" : " : " + request;
        var patchMarker = request is null ? " : " : " : " + request + ", ";
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
                public record Request(string Id, string SerialNumber){{marker}};

                public record PatchRequest(string Id){{patchMarker}}IPropertyTracking<Request>
                {
                    public string? SerialNumber { get; init; }

                    public Request ApplyChanges(Request state) => SerialNumber is { } serialNumber ? state with { SerialNumber = serialNumber } : state;

                    public void ResetChanges() { }
                }

                {{handler.Replace("{sender}", sender).Replace("{handlerInterface}", handlerInterface).Replace("{task}", task)}}
            }
            """;
    }

    private const string Handler = """
        internal partial class EditRocketHandler({sender} mediator) : {handlerInterface}
                {
                    private partial {task}<Request> GetRequest(PatchRequest request, CancellationToken cancellationToken) =>
                        {task}.FromResult(new Request(request.Id, "SN12345678"));
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
             .AddSources(mediator == Mediator.Custom ? [AttributeSources, CustomMediator] : [])
             .AddSources(sources)
             .AddSources(source)
             .Build()
             .GenerateAsync(TestContext.CancellationToken);

    [Test]
    [Arguments(Mediator.Mediator)]
    [Arguments(Mediator.ZeroAlloc)]
    [Arguments(Mediator.ChannelMediator)]
    [Arguments(Mediator.ImmediateHandlers)]
    [Arguments(Mediator.Custom)]
    public async Task Should_Generate_A_Patch_Handler(Mediator mediator)
    {
        var result = await Generate(
            mediator,
            Operations(mediator, Handler),
            mediator == Mediator.ZeroAlloc ? [ZeroAllocMediator] : []
        );

        AssertCompiles(result);
        await Verify(result).UseParameters(mediator);
    }

    [Test]
    public async Task Should_Report_A_Handler_Without_The_Handler_Of_The_Full_Request()
    {
        // Immediate.Handlers has no sender: the patched request goes to the handler of the full request, which this handler doesn't take.
        var result = await Generate(
            Mediator.ImmediateHandlers,
            Operations(Mediator.ImmediateHandlers, Handler.Replace("({sender} mediator)", "(string name)"))
        );

        await VerifyDiagnostics(result);
    }

    [Test]
    public async Task Should_Report_A_Handler_When_The_Profile_Has_No_Handler_Interface()
    {
        // Foundatio finds its handlers by convention; with no generic handler interface there is no patch handler to write, so the handler's
        // GetRequest has no defining declaration and LPAD1029 says why.
        var result = await Generate(Mediator.Foundatio, Operations(Mediator.Foundatio, Handler));

        await VerifyDiagnostics(result);
    }

    [Test]
    public async Task Should_Find_A_Sender_The_Mediator_Generates()
    {
        // Without ZeroAlloc's generator the IMediator is an unresolved type, as it is to this generator in a real build.
        var result = await Generate(Mediator.ZeroAlloc, Operations(Mediator.ZeroAlloc, Handler));

        await Verify(result);
    }

    [Test]
    public async Task Should_Report_A_Handler_Without_A_Sender()
    {
        var result = await Generate(
            Mediator.Mediator,
            Operations(Mediator.Mediator, Handler.Replace("({sender} mediator)", "(string name)"))
        );

        await VerifyDiagnostics(result);
    }

    [Test]
    public async Task Should_Report_A_Handler_In_A_Type_That_Is_Not_Partial()
    {
        var result = await Generate(
            Mediator.Mediator,
            Operations(Mediator.Mediator, Handler).Replace("public static partial class EditRocket", "public static class EditRocket")
        );

        await VerifyDiagnostics(result);
    }

    [Test]
    public async Task Should_Leave_A_Handler_That_Implements_The_Patch()
    {
        var result = await Generate(
            Mediator.Mediator,
            Operations(
                Mediator.Mediator,
                """
                internal partial class EditRocketHandler(ISender mediator) : IRequestHandler<PatchRequest, RocketModel>
                        {
                            public ValueTask<RocketModel> Handle(PatchRequest request, CancellationToken cancellationToken) =>
                                mediator.Send(request.ApplyChanges(new Request(request.Id, "SN12345678")), cancellationToken);
                        }
                """
            )
        );

        AssertCompiles(result);
        await Verify(result);
    }
}
