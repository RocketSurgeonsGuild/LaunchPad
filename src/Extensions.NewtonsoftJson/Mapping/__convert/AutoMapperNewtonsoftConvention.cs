using System;
using JetBrains.Annotations;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.AutoMapper.NewtonsoftJson;
using Rocket.Surgery.Conventions.DependencyInjection;

[assembly: Convention(typeof(AutoMapperNewtonsoftJsonConvention))]

namespace Rocket.Surgery.Conventions.AutoMapper.NewtonsoftJson
{
    /// <summary>
    /// AutoMapperConvention.
    /// Implements the <see cref="IServiceConvention" />
    /// </summary>
    /// <seealso cref="IServiceConvention" />
    [DependsOnConvention(typeof(AutoMapperConvention))]
    public class AutoMapperNewtonsoftJsonConvention : IServiceConvention
    {
        /// <summary>
        /// Registers the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Register([NotNull] IServiceConventionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
        }
    }
}