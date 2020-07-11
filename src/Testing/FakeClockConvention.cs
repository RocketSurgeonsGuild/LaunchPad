using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
    public class FakeClockConvention : IServiceConvention, IHostingConvention
    {
        private readonly int _unixTimeSeconds;
        private readonly Duration _advanceBy;

        public FakeClockConvention(
            int? unixTimeSeconds = null,
            Duration? advanceBy = null
        )
        {
            _unixTimeSeconds = unixTimeSeconds ?? 1577836800;
            _advanceBy = advanceBy ?? Duration.FromSeconds(1);
        }

        public void Register(IServiceConventionContext context)
        {
            context.Services.RemoveAll<IClock>();
            context.Services.AddSingleton<IClock>(new FakeClock(Instant.FromUnixTimeSeconds(_unixTimeSeconds), _advanceBy));
        }

        public void Register(IHostingConventionContext context)
        {
            try
            {
                var type = Type.GetType("Rocket.Surgery.LaunchPad.Extensions.Conventions.NodaTimeConvention");
                context.Get<IConventionScanner>().ExceptConvention(type);
            }
            catch
            {
                // ignore
            }
        }
    }
}