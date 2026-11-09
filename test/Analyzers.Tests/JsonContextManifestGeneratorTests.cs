using MediatR;

using Rocket.Surgery.LaunchPad.Foundation;
using Rocket.Surgery.LaunchPad.Primitives;
using Rocket.Surgery.LaunchPad.Primitives.Analyzers;

namespace Analyzers.Tests;

public class JsonContextManifestGeneratorTests() : GeneratorTest()
{
    [Test]
    public async Task Should_List_Requests_Responses_Patches_And_Opted_In_Types()
    {
        var result = await Builder
                          .WithGenerator<JsonContextManifestGenerator>()
                          .AddReferences(typeof(JsonContextTypeAttribute), typeof(IPropertyTracking<>), typeof(IRequest<>))
                          .AddSources(
                               """
                               #nullable enable
                               using System.Collections.Generic;
                               using MediatR;
                               using Rocket.Surgery.LaunchPad.Foundation;
                               using Rocket.Surgery.LaunchPad.Primitives;

                               namespace Sample.Core
                               {
                                   public record RocketModel(System.Guid Id, string Name);
                                   public record LaunchModel(string Pad);

                                   public static class GetRocket { public record Request(System.Guid Id) : IRequest<RocketModel?>; }
                                   public static class ListRockets { public record Request(string? Name) : IStreamRequest<RocketModel>; }
                                   public static class ListLaunches { public record Request : IRequest<IEnumerable<LaunchModel>>; }
                                   public static class DeleteRocket { public record Request(System.Guid Id) : IRequest; }
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
    public async Task Should_Write_Nothing_Without_LaunchPad_Primitives()
    {
        var result = await Builder
                          .WithGenerator<JsonContextManifestGenerator>()
                          .AddReferences(typeof(IRequest<>))
                          .AddSources("namespace Sample.Core { public record Request : MediatR.IRequest<int>; }")
                          .Build()
                          .GenerateAsync(TestContext.CancellationToken);

        await Verify(result).UseFileName($"{nameof(JsonContextManifestGeneratorTests)}.{nameof(Should_Write_Nothing_Without_LaunchPad_Primitives)}");
    }
}
