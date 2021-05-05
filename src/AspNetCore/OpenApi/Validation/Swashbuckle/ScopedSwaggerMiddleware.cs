using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Threading.Tasks;

namespace Rocket.Surgery.LaunchPad.AspNetCore.OpenApi.Validation.Swashbuckle
{
    /// <summary>
    /// SwaggerMiddleware that resolves <see cref="ISwaggerProvider"/> on scope.
    /// Resolves problems with validators with dependency on scoped services.
    /// </summary>
    public class ScopedSwaggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SwaggerOptions _options;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="options"></param>
        public ScopedSwaggerMiddleware(RequestDelegate next, SwaggerOptions options)
        {
            _next = next;
            _options = options;
        }

        /// <summary>
        /// The scoped middleware
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="swaggerProvider"></param>
        public async Task Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)
        {
            await new SwaggerMiddleware(_next, _options).Invoke(httpContext, swaggerProvider);
        }
    }

    /// <summary>
    /// Allows using scoped swagger middleware
    /// </summary>
    public static class SwaggerBuilderExtensions
    {
        /// <summary>
        /// Replaces standard <see cref="SwaggerMiddleware"/> with <see cref="ScopedSwaggerMiddleware"/>.
        /// Use instead of <see cref="SwaggerBuilderExtensions.UseSwagger"/> if you have services with scoped services like DbContext.
        /// </summary>
        public static IApplicationBuilder UseScopedSwagger(this IApplicationBuilder app, Action<SwaggerOptions> setupAction = null)
        {
            SwaggerOptions swaggerOptions = new SwaggerOptions();
            setupAction?.Invoke(swaggerOptions);
            return app.UseMiddleware<ScopedSwaggerMiddleware>(swaggerOptions);
        }
    }
}