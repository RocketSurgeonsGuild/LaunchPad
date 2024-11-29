using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;

internal static partial class LaunchRecordEndpoints
{
    public static void UseLaunchRecords(this WebApplication app)
    {
        app.MapGet("/launch-records", ListLaunchRecords);
        app.MapGet("/launch-records/{id:guid}", GetLaunchRecord);
        app.MapPost("/launch-records", CreateLaunchRecord);
        app.MapPut("/launch-records/{id:guid}", EditLaunchRecord);
        app.MapPatch("/launch-records/{id:guid}", PatchLaunchRecord);
        app.MapDelete("/launch-records/{id:guid}", DeleteLaunchRecord);
    }

    [EndpointName(nameof(ListLaunchRecords))]
    private static Ok<IAsyncEnumerable<LaunchRecordModel>> ListLaunchRecords(IMediator mediator, RocketType? rocketType) =>
        TypedResults.Ok(mediator.CreateStream(new ListLaunchRecords.Request(rocketType)));

    [EndpointName(nameof(GetLaunchRecord))]
    private static async Task<Results<Ok<LaunchRecordModel>, NotFound, ProblemHttpResult>> GetLaunchRecord(
        IMediator mediator,
        LaunchRecordId id,
        CancellationToken cancellationToken
    ) => TypedResults.Ok(await mediator.Send(new GetLaunchRecord.Request(id), cancellationToken));

    [EndpointName(nameof(CreateLaunchRecord))]
    private static async Task<Results<CreatedAtRoute<CreateLaunchRecord.Response>, ProblemHttpResult, BadRequest>> CreateLaunchRecord(
        IMediator mediator,
        CreateLaunchRecord.Request request
    )
    {
        return TypedResults.CreatedAtRoute(await mediator.Send(request), nameof(GetLaunchRecord));
    }

    /// <summary>
    /// Does this comment get picked up?
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [EndpointName(nameof(EditLaunchRecord))]
    private static async Task<Results<Ok<LaunchRecordModel>, NotFound, ProblemHttpResult, BadRequest>> EditLaunchRecord(
        IMediator mediator,
        [FromRoute]
        LaunchRecordId id,
        EditLaunchRecord.Request model
    ) => TypedResults.Ok(await mediator.Send(model with { Id = id }));

    [EndpointName(nameof(PatchLaunchRecord))]
    private static async Task<Results<Ok<LaunchRecordModel>, NotFound, ProblemHttpResult, BadRequest>> PatchLaunchRecord(
        IMediator mediator,
        [FromRoute]
        LaunchRecordId id,
        EditLaunchRecord.PatchRequest model
    ) => TypedResults.Ok(await mediator.Send(model with { Id = id }));

    [EndpointName(nameof(DeleteLaunchRecord))]
    private static async Task<Results<NoContent, NotFound>> DeleteLaunchRecord(IMediator mediator, [FromRoute] LaunchRecordId id)
    {
        await mediator.Send(new DeleteLaunchRecord.Request(id));
        return TypedResults.NoContent();
    }
}
