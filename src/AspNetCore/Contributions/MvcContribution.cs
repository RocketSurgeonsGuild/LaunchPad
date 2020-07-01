using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.AspNetCore.Contributions;
using Rocket.Surgery.AspNetCore.Conventions;
using Rocket.Surgery.AspNetCore.Filters;
using Rocket.Surgery.AspNetCore.Views;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;

[assembly: Convention(typeof(AspNetCoreConvention))]

namespace Rocket.Surgery.AspNetCore.Contributions
{
    /// <summary>
    /// Class MvcConvention.
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    /// TODO Edit XML Comment Template for MvcConvention
    public class AspNetCoreConvention : IServiceConvention
    {
        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// TODO Edit XML Comment Template for Register
        public void Register([NotNull] IServiceConventionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var coreBuilder = context.Services
                .AddMvcCore()
                .AddControllersAsServices()
                .AddApiExplorer();
            foreach (var item in context.AssemblyCandidateFinder.GetCandidateAssemblies("Rocket.Surgery.AspNetCore", "Microsoft.AspNetCore.Mvc"))
            {
                coreBuilder
                    .AddApplicationPart(item);
            }

            context.Services.Configure<RazorViewEngineOptions>(options =>
            {
                // {0} - Action Name
                // {1} - Controller Name
                // {2} - Area Name
                // {3} - Feature Name
                // Replace normal view location entirely
                for (var i = Locations.Length - 1; i >= 0; i--)
                {
                    options.AreaViewLocationFormats.Insert(0, $"/Areas/{{2}}{Locations[i]}");
                }

                for (var i = Locations.Length - 1; i >= 0; i--)
                {
                    options.ViewLocationFormats.Insert(0, Locations[i]);
                }

                options.ViewLocationExpanders.Add(new FeatureViewLocationExpander());
            });

            context.Services.Configure<MvcOptions>(options =>
            {
                options.Conventions.Add(new FeatureConvention());
                options.Filters.Add<NotFoundExceptionFilter>();
                options.Filters.Add<RequestExceptionFilter>();
            });
        }

        /// <summary>
        /// The locations
        /// </summary>
        private static readonly string[] Locations = {
            "/{3}/{1}/{0}.cshtml",
            "/{3}/{0}.cshtml",
            "/{3}/{1}.cshtml",
            "/Shared/{0}.cshtml",
            "/Views/{0}.cshtml",
            "/Views/{1}.cshtml",
        };
    }
}
