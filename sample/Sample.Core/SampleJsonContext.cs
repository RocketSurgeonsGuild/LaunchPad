using System.Text.Json.Serialization;

using Joinery.Dovetail.Attributes;
using Joinery.Dovetail.Joints;

using Microsoft.Extensions.DependencyInjection;

using Rocket.Surgery.LaunchPad.Primitives;

using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;
using Sample.Core.Operations.Rockets;

namespace Sample.Core;

/// <summary>
///     The JSON context of the samples' models and operations, so LaunchPad writes them without reflection.
/// </summary>
/// <remarks>
///     The operations' nested <c>X.Request</c> types share a simple name, so each needs its own <c>TypeInfoPropertyName</c>. The enums the context
///     reaches (<see cref="Domain.RocketType" />) get camel case converters from LaunchPad's generator.
/// </remarks>
[JsonSerializable(typeof(RocketModel))]
[JsonSerializable(typeof(LaunchRecordModel))]
[JsonSerializable(typeof(CreateRocket.Request), TypeInfoPropertyName = "CreateRocketRequest")]
[JsonSerializable(typeof(CreateRocket.Response), TypeInfoPropertyName = "CreateRocketResponse")]
[JsonSerializable(typeof(EditRocket.Request), TypeInfoPropertyName = "EditRocketRequest")]
[JsonSerializable(typeof(CreateLaunchRecord.Request), TypeInfoPropertyName = "CreateLaunchRecordRequest")]
[JsonSerializable(typeof(CreateLaunchRecord.Response), TypeInfoPropertyName = "CreateLaunchRecordResponse")]
[JsonSerializable(typeof(EditLaunchRecord.Request), TypeInfoPropertyName = "EditLaunchRecordRequest")]
public partial class SampleJsonContext : JsonSerializerContext;

/// <summary>
///     Chains <see cref="SampleJsonContext" /> into the samples' JSON options.
/// </summary>
[DovetailExport]
internal sealed class SampleJsonJoint : IServiceJoint
{
    public void Register(IDovetailContext context, IServiceCollection services) => services.AddJsonTypeInfoResolver(SampleJsonContext.Default);
}
