using System;
using System.Collections.Generic;
using App.Metrics.AspNetCore;
using App.Metrics.AspNetCore.Endpoints;
using App.Metrics.AspNetCore.Endpoints.Middleware;
using App.Metrics.Formatters;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Rocket.Surgery.LaunchPad.AspNetCore.AppMetrics;

/// <summary>
///     App Metrics extenison methods
/// </summary>
public static class EndpointRoutingExtensions
{
    /// <summary>
    ///     Map app metrics
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureEndpointMetadata"></param>
    /// <returns></returns>
    [PublicAPI]
    public static IEndpointRouteBuilder MapAppMetrics(
        this IEndpointRouteBuilder builder,
        Action<IEndpointConventionBuilder>? configureEndpointMetadata = null
    )
    {
        configureEndpointMetadata ??= _ => { };
        // configureEndpointMetadata(builder.MapMetricsTextEndpoint());
        configureEndpointMetadata(builder.MapMetricsEndpoint());
        // configureEndpointMetadata(builder.MapEnvInfoEndpoint());
        return builder;
    }

    /// <summary>
    ///     Map text endpoints
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    [PublicAPI]
    public static IEndpointConventionBuilder MapMetricsTextEndpoint(this IEndpointRouteBuilder endpoints)
    {
        var app = endpoints.CreateApplicationBuilder();
        var responseWriter = GetMetricsTextResponseWriter(app.ApplicationServices);
        return endpoints.Map("/metrics/text", endpoints.CreateApplicationBuilder().UseMiddleware<MetricsEndpointMiddleware>(responseWriter).Build());
    }

    /// <summary>
    ///     Map metrics endpoint
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    [PublicAPI]
    public static IEndpointConventionBuilder MapMetricsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        var app = endpoints.CreateApplicationBuilder();
        var responseWriter = GetMetricsResponseWriter(app.ApplicationServices);
        return endpoints.Map("/metrics", endpoints.CreateApplicationBuilder().UseMiddleware<MetricsEndpointMiddleware>(responseWriter).Build());
    }

    /// <summary>
    ///     Map env info endpoint
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    [PublicAPI]
    public static IEndpointConventionBuilder MapEnvInfoEndpoint(this IEndpointRouteBuilder endpoints)
    {
        var app = endpoints.CreateApplicationBuilder();
        var responseWriter = GetEnvInfoResponseWriter(app.ApplicationServices);
        return endpoints.Map("/metrics/env", endpoints.CreateApplicationBuilder().UseMiddleware<EnvInfoMiddleware>(responseWriter).Build());
    }

    // public static IEndpointConventionBuilder MapHealthChecks(IEndpointRouteBuilder builder)
    // {
    //     var app = builder.CreateApplicationBuilder()
    //        .UseMetricsTextEndpoint()
    //        .UseEnvInfoEndpoint()
    //        .UseMetricsEndpoint();
    //     return builder.Map(RoutePatternFactory.Parse("/metrics"), app.Build());
    // }

    private static DefaultMetricsResponseWriter GetMetricsResponseWriter(IServiceProvider serviceProvider)
    {
        var formatters = serviceProvider.GetRequiredService<IReadOnlyCollection<IMetricsOutputFormatter>>();

        var options = serviceProvider.GetRequiredService<IOptions<MetricEndpointsOptions>>();
        return new DefaultMetricsResponseWriter(options.Value.MetricsEndpointOutputFormatter, formatters);
    }

    private static IMetricsResponseWriter GetMetricsTextResponseWriter(IServiceProvider serviceProvider)
    {
        var formatters = serviceProvider.GetRequiredService<IReadOnlyCollection<IMetricsOutputFormatter>>();

        var options = serviceProvider.GetRequiredService<IOptions<MetricEndpointsOptions>>();
        return new DefaultMetricsResponseWriter(options.Value.MetricsTextEndpointOutputFormatter, formatters);
    }

    private static IEnvResponseWriter GetEnvInfoResponseWriter(IServiceProvider serviceProvider)
    {
        var formatters = serviceProvider.GetRequiredService<IReadOnlyCollection<IEnvOutputFormatter>>();

        var options = serviceProvider.GetRequiredService<IOptions<MetricEndpointsOptions>>();
        return new DefaultEnvResponseWriter(options.Value.EnvInfoEndpointOutputFormatter, formatters);
    }
}
