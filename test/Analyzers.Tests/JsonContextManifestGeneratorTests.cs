using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Analyzers.Tests;

public class JsonContextManifestGeneratorTests() : GeneratorTest()
{
    [Test]
    public async Task Should_List_Requests_Responses_Patches_And_Opted_In_Types()
    {
        var result = await Builder
                          .WithGenerator<JsonContextManifestGenerator>()
                          // C# 15, for the native union below
                          .WithLanguageVersion(Microsoft.CodeAnalysis.CSharp.LanguageVersion.Preview)
                          .AddReferences(typeof(JsonContextTypeAttribute), typeof(IPropertyTracking<>), typeof(ZeroAlloc.Mediator.IRequest<>))
                          .AddSources(
                               """
                               #nullable enable
                               using System.Collections.Generic;
                               using ZeroAlloc.Mediator;
                               using Rocket.Surgery.LaunchPad.Foundation;
                               using Rocket.Surgery.LaunchPad.Foundation;

                               namespace Sample.Core
                               {
                                   public record RocketModel(System.Guid Id, string Name);
                                   public record LaunchModel(string Pad);

                                   public static class GetRocket { public record Request(System.Guid Id) : IRequest<RocketModel?>; }
                                   public static class ListRockets { public record Request(string? Name) : IStreamRequest<RocketModel>; }
                                   public static class ListLaunches { public record Request : IRequest<IEnumerable<LaunchModel>>; }
                                   public static class DeleteRocket { public record Request(System.Guid Id) : IRequest<Unit>; }
                                   public static class CountRockets { public record Request : IRequest<int>; }
                                   public static partial class EditRocket
                                   {
                                       public record Request(System.Guid Id, string Name) : IRequest<RocketModel>;
                                       public partial record PatchRequest(System.Guid Id) : IRequest<RocketModel>, IPropertyTracking<Request>
                                       {
                                           public Request ApplyChanges(Request state) => state;
                                           void IPropertyTracking<Request>.ResetChanges() { }
                                       }
                                   }

                                   // A union response: its cases are listed, not the union, and LaunchPad's standard problems aren't.
                                   public record RocketConflict(System.Guid ExistingId);
                                   public static class RenameRocket
                                   {
                                       public record Request(System.Guid Id, string Name) : IRequest<Result>;

                                       public union Result(RocketModel, NotFound, RocketConflict);
                                   }

                                   [IncludeInJsonContext]
                                   public record Telemetry(double Altitude);

                                   // Not listed: the application's context couldn't name it.
                                   internal record Hidden(string Value) : IRequest<RocketModel>;
                                   public record Unmarked(string Value);
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseFileName($"{nameof(JsonContextManifestGeneratorTests)}.{nameof(Should_List_Requests_Responses_Patches_And_Opted_In_Types)}");
    }

    [Test]
    public async Task Should_Write_Nothing_Without_LaunchPad_Foundation()
    {
        var result = await Builder
                          .WithGenerator<JsonContextManifestGenerator>()
                          .AddReferences(typeof(ZeroAlloc.Mediator.IRequest<>))
                          .AddSources("namespace Sample.Core { public record Request : ZeroAlloc.Mediator.IRequest<int>; }")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseFileName($"{nameof(JsonContextManifestGeneratorTests)}.{nameof(Should_Write_Nothing_Without_LaunchPad_Foundation)}");
    }

    public enum Profile
    {
        Mediator,
        ChannelMediator,
        ImmediateHandlers,
        Foundatio,
        ZeroAlloc,

        /// <summary>A mediator the compilation describes with <c>[assembly: MediatorRequest]</c>.</summary>
        Custom,
    }

    // Each profile's own request shapes: a unary request answering a model, a stream, a void request (whose unit response isn't listed) and a
    // patch. Immediate.Handlers has no request marker, so none of its requests are listed: only what is opted in.
    private static (System.Reflection.Assembly[] References, string Usings, string Requests) For(Profile profile) => profile switch
    {
        Profile.Mediator => (
            [typeof(Mediator.ISender).Assembly],
            "using Mediator;",
            """
            public static class GetRocket { public record Request(System.Guid Id) : IQuery<RocketModel?>; }
            public static class LaunchRocket { public record Request(System.Guid Id) : ICommand<LaunchModel>; }
            public static class ScrubRocket { public record Request(System.Guid Id) : IRequest<RocketModel>; }
            public static class ListRockets { public record Request(string? Name) : IStreamQuery<RocketModel>; }
            public static class DeleteRocket { public record Request(System.Guid Id) : ICommand; }
            """
        ),
        Profile.ChannelMediator => (
            [typeof(ChannelMediator.IMediator).Assembly, typeof(ChannelMediator.IRequest<>).Assembly],
            "using ChannelMediator;",
            """
            public static class GetRocket { public record Request(System.Guid Id) : IRequest<RocketModel?>; }
            public static class LaunchRocket { public record Request(System.Guid Id) : IRequest<LaunchModel>; }
            public static class ListRockets { public record Request(string? Name) : IStreamRequest<RocketModel>; }
            public static class DeleteRocket { public record Request(System.Guid Id) : IRequest; }
            """
        ),
        Profile.ImmediateHandlers => (
            [typeof(Immediate.Handlers.Shared.IHandler<,>).Assembly],
            "using Immediate.Handlers.Shared;",
            """
            public static class GetRocket { public record Request(System.Guid Id); }
            """
        ),
        Profile.Foundatio => (
            [typeof(Foundatio.Mediator.IMediator).Assembly],
            "using Foundatio.Mediator;",
            """
            public static class GetRocket { public record Request(System.Guid Id) : IQuery<RocketModel?>; }
            public static class LaunchRocket { public record Request(System.Guid Id) : ICommand<LaunchModel>; }
            public static class ScrubRocket { public record Request(System.Guid Id) : IRequest<RocketModel>; }
            public static class DeleteRocket { public record Request(System.Guid Id) : ICommand; }
            """
        ),
        Profile.ZeroAlloc => (
            [typeof(ZeroAlloc.Mediator.IRequest<>).Assembly],
            "using ZeroAlloc.Mediator;",
            """
            public static class GetRocket { public record Request(System.Guid Id) : IRequest<RocketModel?>; }
            public static class LaunchRocket { public record Request(System.Guid Id) : IRequest<LaunchModel>; }
            public static class ListRockets { public record Request(string? Name) : IStreamRequest<RocketModel>; }
            public static class DeleteRocket { public record Request(System.Guid Id) : IRequest<Unit>; }
            """
        ),
        // The response is the second type argument: the manifest reads ResponseTypeArgumentIndex.
        Profile.Custom => (
            [],
            "",
            """
            public interface IMyRequest<TKey, TResponse> { }
            public static class GetRocket { public record Request(System.Guid Id) : IMyRequest<System.Guid, RocketModel?>; }
            public static class LaunchRocket { public record Request(System.Guid Id) : IMyRequest<System.Guid, LaunchModel>; }
            """
        ),
        _ => throw new ArgumentOutOfRangeException(nameof(profile)),
    };

    private const string CustomProfile = """
        [assembly: Rocket.Surgery.LaunchPad.Api.MediatorRequest(typeof(Sample.Core.IMyRequest<,>), ResponseTypeArgumentIndex = 1)]

        namespace Rocket.Surgery.LaunchPad.Api
        {
            [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = true)]
            internal sealed class MediatorRequestAttribute(System.Type requestInterface) : System.Attribute
            {
                public System.Type RequestInterface { get; } = requestInterface;
                public int ResponseTypeArgumentIndex { get; set; }
            }
        }
        """;

    [Test]
    [Arguments(Profile.Mediator)]
    [Arguments(Profile.ChannelMediator)]
    [Arguments(Profile.ImmediateHandlers)]
    [Arguments(Profile.Foundatio)]
    [Arguments(Profile.ZeroAlloc)]
    [Arguments(Profile.Custom)]
    public async Task Should_List_The_Requests_Of_Each_Mediator_Profile(Profile profile)
    {
        var (references, usings, requests) = For(profile);
        var patchMarker = profile switch
        {
            Profile.ImmediateHandlers => "",
            Profile.Custom => "IMyRequest<System.Guid, RocketModel>, ",
            Profile.Mediator => throw new NotImplementedException(),
            Profile.ChannelMediator => throw new NotImplementedException(),
            Profile.Foundatio => throw new NotImplementedException(),
            Profile.ZeroAlloc => throw new NotImplementedException(),
            _ => "IRequest<RocketModel>, ",
        };
        var result = await Builder
                          .WithGenerator<JsonContextManifestGenerator>()
                          .AddReferences(typeof(JsonContextTypeAttribute), typeof(IPropertyTracking<>))
                          .AddReferences(references)
                          .AddSources(profile == Profile.Custom ? [CustomProfile] : [])
                          .AddSources(
                               $$"""
                               #nullable enable
                               {{usings}}
                               using Rocket.Surgery.LaunchPad.Foundation;

                               namespace Sample.Core
                               {
                                   public record RocketModel(System.Guid Id, string Name);
                                   public record LaunchModel(string Pad);

                                   {{requests}}

                                   public static partial class EditRocket
                                   {
                                       public record Request(System.Guid Id, string Name);
                                       public partial record PatchRequest(System.Guid Id) : {{patchMarker}}IPropertyTracking<Request>
                                       {
                                           public Request ApplyChanges(Request state) => state;
                                           void IPropertyTracking<Request>.ResetChanges() { }
                                       }
                                   }

                                   [IncludeInJsonContext]
                                   public record Telemetry(double Altitude);
                               }
                               """
                           )
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseParameters(profile);
    }
}
