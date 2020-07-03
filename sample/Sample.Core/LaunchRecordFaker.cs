using System.Collections.Generic;
using System.Linq;
using Bogus;
using Bogus.Extensions;
using Sample.Core.Domain;

namespace Sample.Core
{
    class LaunchRecordFaker : Faker<LaunchRecord>
    {
        public LaunchRecordFaker(List<Sample.Core.Domain.ReadyRocket> rockets)
        {
            RuleFor(x => x.Id, x => x.Random.Guid());
            RuleFor(x => x.Partner, x => x.Company.CompanyName());
            RuleFor(x => x.ReadyRocket, x => x.PickRandom(rockets.AsEnumerable()));
            RuleFor(x => x.RocketId, (f, v) => v.ReadyRocket.Id);
            RuleFor(x => x.ActualLaunchDate, f => f.Date.PastOffset().OrNull(f, 0.2f));
            RuleFor(x => x.ScheduledLaunchDate, (f, v) => f.Date.PastOffset(refDate: v.ActualLaunchDate));
            RuleFor(x => x.PayloadWeightKg, f => f.Random.Number(100, 1000000));
            RuleFor(x => x.Payload, f => f.Lorem.Paragraphs(3));
        }
    }
}