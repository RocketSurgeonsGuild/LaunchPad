using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Riok.Mapperly.Abstractions;
using Sample.Core.Operations;
using Sample.Core.Operations.LaunchRecords;
using ZeroAlloc.Mediator;
using NodaTimeMapper = Sample.Core.Models.NodaTimeMapper;

namespace Sample.Grpc.Services;

[Mapper]
[UseStaticMapper(typeof(NodaTimeMapper))]
[UseStaticMapper(typeof(WellKnownGrpcTypesMapper))]
public partial class LaunchRecordsService : LaunchRecords.LaunchRecordsBase
{
    private static IMediator Mediator(ServerCallContext context) => context.GetHttpContext().RequestServices.GetRequiredService<IMediator>();

    public static partial CreateLaunchRecord.Request Map(CreateLaunchRecordRequest request);
    public static partial CreateLaunchRecordResponse Map(CreateLaunchRecord.Response request);
    public static partial GetLaunchRecord.Request Map(GetLaunchRecordRequest request);
    public static partial EditLaunchRecord.Request Map(UpdateLaunchRecordRequest request);
    public static partial ListLaunchRecords.Request Map(ListLaunchRecordsRequest request);
    public static partial DeleteLaunchRecord.Request Map(DeleteLaunchRecordRequest request);
    public static partial LaunchRecordModel Map(Core.Models.LaunchRecordModel request);

    public override async Task<CreateLaunchRecordResponse> CreateLaunchRecord(CreateLaunchRecordRequest request, ServerCallContext context) =>
        Map(( await Mediator(context).Send(Map(request), context.CancellationToken) ).Value.Expect<CreateLaunchRecord.Response>());

    public override async Task<LaunchRecordModel> EditLaunchRecord(UpdateLaunchRecordRequest request, ServerCallContext context) => Map(( await Mediator(context).Send(Map(request), context.CancellationToken) ).Value.Expect<Core.Models.LaunchRecordModel>());

    public override async Task<Empty> DeleteLaunchRecord(DeleteLaunchRecordRequest request, ServerCallContext context)
    {
        ( await Mediator(context).Send(Map(request), context.CancellationToken) ).Value.Expect<Unit>();
        return new();
    }

    public override async Task<LaunchRecordModel> GetLaunchRecords(GetLaunchRecordRequest request, ServerCallContext context) => Map(( await Mediator(context).Send(Map(request), context.CancellationToken) ).Value.Expect<Core.Models.LaunchRecordModel>());

    public override async Task ListLaunchRecords(
        ListLaunchRecordsRequest request,
        IServerStreamWriter<LaunchRecordModel> responseStream,
        ServerCallContext context
    )
    {
        await foreach (var item in Mediator(context).CreateStream(Map(request), context.CancellationToken))
        {
            await responseStream.WriteAsync(Map(item));
        }
    }
}
