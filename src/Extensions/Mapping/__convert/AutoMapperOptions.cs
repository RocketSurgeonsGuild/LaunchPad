using AutoMapper;
using AutoMapper.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Conventions.AutoMapper
{
    /// <summary>
    /// Class AutoMapperOptions.
    /// </summary>
    public class AutoMapperOptions : IRuntimeFeature, IGlobalFeature
    {
        /// <summary>
        /// Gets or sets the service lifetime.
        /// </summary>
        /// <value>The service lifetime.</value>
        public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Transient;

        void IRuntimeFeature.Seal(IConfigurationProvider configurationProvider) { }

        void IGlobalFeature.Configure(IConfigurationProvider configurationProvider) => configurationProvider.Features.Set(this);
    }
}