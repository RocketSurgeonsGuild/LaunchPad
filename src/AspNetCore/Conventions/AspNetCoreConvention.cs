using FluentValidation;
using FluentValidation.AspNetCore;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.LaunchPad.AspNetCore.Conventions;
using Rocket.Surgery.LaunchPad.AspNetCore.Filters;
using Rocket.Surgery.LaunchPad.AspNetCore.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[assembly: Convention(typeof(AspNetCoreConvention))]

namespace Rocket.Surgery.LaunchPad.AspNetCore.Conventions
{
    /// <summary>
    /// Class MvcConvention.
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    public class AspNetCoreConvention : IServiceConvention
    {
        private readonly ValidatorConfiguration? _validatorConfiguration;
        private readonly FluentValidationMvcConfiguration? _validationMvcConfiguration;

        /// <summary>
        /// Configure aspnet with some logical defaults
        /// </summary>
        /// <param name="validatorConfiguration"></param>
        /// <param name="validationMvcConfiguration"></param>
        public AspNetCoreConvention(
            ValidatorConfiguration? validatorConfiguration = null,
            FluentValidationMvcConfiguration? validationMvcConfiguration = null
            )
        {
            _validatorConfiguration = validatorConfiguration;
            _validationMvcConfiguration = validationMvcConfiguration;
        }

        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="configuration"></param>
        /// <param name="services"></param>
        /// TODO Edit XML Comment Template for Register
        public void Register([NotNull] IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            services.AddMvcCore().AddApiExplorer();
            PopulateDefaultParts(
                GetServiceFromCollection<ApplicationPartManager>(services),
                context.AssemblyCandidateFinder
                   .GetCandidateAssemblies("Rocket.Surgery.LaunchPad.AspNetCore")
                   .SelectMany(GetApplicationPartAssemblies)
                );

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add<NotFoundExceptionFilter>();
                options.Filters.Add<RequestFailedExceptionFilter>();
                options.Filters.Add<SerilogLoggingActionFilter>(0);
                options.Filters.Add<SerilogLoggingPageFilter>(0);
            });

            services.AddFluentValidationExtensions(_validatorConfiguration, _validationMvcConfiguration);
        }

        private static T GetServiceFromCollection<T>(IServiceCollection services) => (T)services
           .LastOrDefault(d => d.ServiceType == typeof(T))
          ?.ImplementationInstance;


        internal static void PopulateDefaultParts(
            ApplicationPartManager manager,
            IEnumerable<Assembly> assemblies)
        {
            var seenAssemblies = new HashSet<Assembly>();

            foreach (var assembly in assemblies)
            {
                if (!seenAssemblies.Add(assembly))
                {
                    // "assemblies" may contain duplicate values, but we want unique ApplicationPart instances.
                    // Note that we prefer using a HashSet over Distinct since the latter isn't
                    // guaranteed to preserve the original ordering.
                    continue;
                }

                var partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
                foreach (var applicationPart in partFactory.GetApplicationParts(assembly))
                {
                    manager.ApplicationParts.Add(applicationPart);
                }
            }
        }

        private static IEnumerable<Assembly> GetApplicationPartAssemblies(Assembly assembly)
        {
            // Use ApplicationPartAttribute to get the closure of direct or transitive dependencies
            // that reference MVC.
            var assembliesFromAttributes = assembly.GetCustomAttributes<ApplicationPartAttribute>()
                .Select(name => Assembly.Load(name.AssemblyName))
                .OrderBy(assembly => assembly.FullName, StringComparer.Ordinal)
                .SelectMany(GetAssemblyClosure);

            // The SDK will not include the entry assembly as an application part. We'll explicitly list it
            // and have it appear before all other assemblies \ ApplicationParts.
            return GetAssemblyClosure(assembly)
                .Concat(assembliesFromAttributes);
        }

        private static IEnumerable<Assembly> GetAssemblyClosure(Assembly assembly)
        {
            yield return assembly;

            var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(assembly, throwOnError: false)
                .OrderBy(assembly => assembly.FullName, StringComparer.Ordinal);

            foreach (var relatedAssembly in relatedAssemblies)
            {
                yield return relatedAssembly;
            }
        }
    }
}
