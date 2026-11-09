using Api.Aot.Contracts;
using Rocket.Surgery.LaunchPad.Api;
namespace Api.Aot.ClientLib;

[Operation<GetRocket.Request>]
[Operation<ListRockets.Request>]
[Operation<CreateRocket.Request>]
[Operation<EditRocket.Request>]
[Operation<DeleteRocket.Request>]
public partial class RocketsClient
{
    static partial void ConfigureJsonOptions(System.Text.Json.JsonSerializerOptions options) => options.TypeInfoResolverChain.Insert(0, RocketsJsonContext.Default);
}
