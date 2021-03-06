﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.LaunchPad.HotChocolate.Configuration;
using Rocket.Surgery.LaunchPad.HotChocolate.Conventions;
using Rocket.Surgery.LaunchPad.HotChocolate.Extensions;

[assembly: Convention(typeof(HotChocolateConvention))]

namespace Rocket.Surgery.LaunchPad.HotChocolate.Conventions
{
    /// <summary>
    /// Hot Chocolate convention
    /// </summary>
    public class HotChocolateConvention : IServiceConvention
    {
        /// <inheritdoc />
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            services
               .ConfigureOptions<HotChocolateContextDataConfigureOptions>()
               .AddGraphQL()
               .ConfigureSchema(sb => sb.AddNodaTime())
               ;
        }
    }
}