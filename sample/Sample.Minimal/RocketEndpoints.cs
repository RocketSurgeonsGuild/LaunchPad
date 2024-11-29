using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sample.Core.Domain;
using Sample.Core.Models;
using Sample.Core.Operations.Rockets;

internal static partial class RocketEndpoints
{
    public static void UseRockets(this WebApplication app)
    {
        app.MapGet("/rockets", ListRockets);
        app.MapGet("/rockets/{id:guid}", GetRocket);
        app.MapPost("/rockets", CreateRocket);
        app.MapPut("/rockets/{id:guid}", EditRocket);
        app.MapPatch("/rockets/{id:guid}", PatchRocket);
        app.MapDelete("/rockets/{id:guid}", DeleteRocket);
    }

    private static Ok<IAsyncEnumerable<RocketModel>> ListRockets(IMediator mediator, RocketType? rocketType) =>
        TypedResults.Ok(mediator.CreateStream(new ListRockets.Request(rocketType)));

    private static async Task<Results<Ok<RocketModel>, NotFound, ProblemHttpResult>> GetRocket(
        IMediator mediator,
        [FromRoute]
        RocketId id,
        CancellationToken cancellationToken
    ) => TypedResults.Ok(await mediator.Send(new GetRocket.Request(id), cancellationToken));

    private static async Task<Results<CreatedAtRoute<CreateRocket.Response>, ProblemHttpResult, BadRequest>> CreateRocket(
        IMediator mediator,
        CreateRocket.Request request
    )
    {
        return TypedResults.CreatedAtRoute(await mediator.Send(request), nameof(GetRocket));
    }

    private static async Task<Results<Ok<RocketModel>, NotFound, ProblemHttpResult, BadRequest>> EditRocket(
        IMediator mediator,
        [FromRoute]
        RocketId id,
        EditRocket.Request model
    ) => TypedResults.Ok(await mediator.Send(model with { Id = id }));

    private static async Task<Results<Ok<RocketModel>, NotFound, ProblemHttpResult, BadRequest>> PatchRocket(
        IMediator mediator,
        [FromRoute]
        RocketId id,
        EditRocket.PatchRequest model
    ) => TypedResults.Ok(await mediator.Send(model with { Id = id }));

    [HttpDelete("{id:guid}")]
    private static async Task<Results<NoContent, NotFound>> DeleteRocket(IMediator mediator, [FromRoute] RocketId id)
    {
        await mediator.Send(new DeleteRocket.Request(id));
        return TypedResults.NoContent();
    }
}
