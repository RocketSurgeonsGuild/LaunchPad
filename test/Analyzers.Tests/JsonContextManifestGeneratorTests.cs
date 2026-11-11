using Rocket.Surgery.LaunchPad.Analyzers;
using Rocket.Surgery.LaunchPad.Foundation;

namespace Analyzers.Tests;

public class JsonContextManifestGeneratorTests() : GeneratorTest()
{
    [Test]
    public async Task Should_List_Requests_Responses_Patches_And_Opted_In_Types()
    {
        var builder = Builder
                     .WithGenerator<JsonContextManifestGenerator>()
                     .AddReferences(typeof(JsonContextTypeAttribute), typeof(IPropertyTracking<>), typeof(ZeroAlloc.Mediator.IRequest<>));
        // The union types: Polyfill's on .NET 10, the BCL's on .NET 11 (see UnionGeneratorTests).
        if (UnionGeneratorTests.PolyfillReference is { } polyfill) builder = builder.AddReferences(polyfill);
        var result = await builder
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

                                       [System.Runtime.CompilerServices.Union]
                                       public readonly struct Result
                                       {
                                           public Result(RocketModel value) { }
                                           public Result(NotFound value) { }
                                           public Result(RocketConflict value) { }
                                           public object? Value => null;
                                       }
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
}
