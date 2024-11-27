using System.Reflection;
using System.Text;
using System.Text.Json;
using FluentValidation;
using Humanizer;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.AspNetCore;
using Sample.Core.Models;
using Sample.Core.Operations.LaunchRecords;
using Sample.Minimal;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddControllersAsServices();
builder.Services.AddHostedService<CustomHostedService>();

var app = await builder
   .LaunchWith(RocketBooster.For(Imports.Instance));
app.UseExceptionHandler();
app.UseHttpsRedirection();

// Should this move into an extension method?
app.UseSerilogRequestLogging(
    x =>
    {
        x.GetLevel = LaunchPadHelpers.DefaultGetLevel;
        x.EnrichDiagnosticContext = LaunchPadHelpers.DefaultEnrichDiagnosticContext;
    }
);

app.UseRouting();

app
   .UseSwaggerUI()
   .UseReDoc();

app.UseAuthorization();
app.MapHealthChecks(
    "/health",
    new()
    {
        ResponseWriter = LaunchPadHelpers.DefaultResponseWriter,
        ResultStatusCodes = new Dictionary<HealthStatus, int>
        {
            { HealthStatus.Healthy, StatusCodes.Status200OK },
            { HealthStatus.Degraded, StatusCodes.Status500InternalServerError },
            { HealthStatus.Unhealthy, StatusCodes.Status503ServiceUnavailable },
        },
    }
);

app.MapGet("/launch-records", LaunchPadApi.ListLaunchRecords);
app.MapGet("/launch-records/{id:guid}", LaunchPadApi.GetLaunchRecord);
//[Route("[controller]")]
//public partial class LaunchRecordController : RestfulApiController
//{
//    /// <summary>
//    ///     Create a new launch record
//    /// </summary>
//    /// <param name="request">The launch record details</param>
//    /// <returns></returns>
//    [HttpPost]
//    [Created(nameof(GetLaunchRecord))]
//    public partial Task<ActionResult<CreateLaunchRecord.Response>> CreateLaunchRecord(CreateLaunchRecord.Request request);
//
//    /// <summary>
//    ///     Update a given launch record
//    /// </summary>
//    /// <param name="id">The id of the launch record</param>
//    /// <param name="model">The request details</param>
//    /// <returns></returns>
//    [HttpPut("{id:guid}")]
//    // ReSharper disable once RouteTemplates.ParameterTypeAndConstraintsMismatch
//    public partial Task<ActionResult> EditLaunchRecord([BindRequired] [FromRoute] LaunchRecordId id, EditLaunchRecord.Request model);
//
//    /// <summary>
//    ///     Update a given launch record
//    /// </summary>
//    /// <param name="id">The id of the launch record</param>
//    /// <param name="model">The request details</param>
//    /// <returns></returns>
//    [HttpPatch("{id:guid}")]
//    // ReSharper disable once RouteTemplates.ParameterTypeAndConstraintsMismatch
//    public partial Task<ActionResult> PatchLaunchRecord([BindRequired] [FromRoute] LaunchRecordId id, EditLaunchRecord.PatchRequest model);
//
//    /// <summary>
//    ///     Remove a launch record
//    /// </summary>
//    /// <param name="request"></param>
//    /// <returns></returns>
//    [HttpDelete("{id:guid}")]
//    public partial Task<ActionResult> DeleteLaunchRecord(DeleteLaunchRecord.Request request);


app.Run();

public partial class Program;

internal class CustomHostedServiceOptions
{
    public string? A { get; set; }

    [UsedImplicitly]
    private sealed class Validator : AbstractValidator<CustomHostedServiceOptions>
    {
        public Validator()
        {
            RuleFor(z => z.A).NotNull();
        }
    }
}

internal class CustomHostedService(IOptions<CustomHostedServiceOptions> options) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // ReSharper disable once UnusedVariable
        var v = options.Value.A;
        return Task.CompletedTask;
    }
}


public static partial class LaunchPadApi
{
    public static Ok<IAsyncEnumerable<LaunchRecordModel>> ListLaunchRecords(IMediator mediator, ListLaunchRecords.Request request) =>
        TypedResults.Ok(mediator.CreateStream(request));

    public static async Task<Ok<LaunchRecordModel>> GetLaunchRecord(
        IMediator mediator,
        [FromRoute]
        GetLaunchRecord.Request request,
        CancellationToken cancellationToken
    ) => TypedResults.Ok(await mediator.Send(request, cancellationToken));
}
