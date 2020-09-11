using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NodaTime;
using NodaTime.Testing;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Hosting;
using Rocket.Surgery.LaunchPad.Testing;

[assembly: Convention(typeof(FakeClockConvention))]

namespace Rocket.Surgery.LaunchPad.Testing
{
    [UnitTestConvention]
    public class FakeClockConvention : IServiceConvention
    {
        private readonly int _unixTimeSeconds;
        private readonly Duration _advanceBy;

        public FakeClockConvention(int? unixTimeSeconds = null, Duration? advanceBy = null)
        {
            _unixTimeSeconds = unixTimeSeconds ?? 1577836800;
            _advanceBy = advanceBy ?? Duration.FromSeconds(1);
        }

        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            services.RemoveAll<IClock>();
            services.AddSingleton<IClock>(new FakeClock(Instant.FromUnixTimeSeconds(_unixTimeSeconds), _advanceBy));
        }
    }
}