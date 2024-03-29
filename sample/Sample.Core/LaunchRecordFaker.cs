﻿using Bogus;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core;

public class LaunchRecordFaker : Faker<LaunchRecord>
{
    public LaunchRecordFaker(IReadOnlyCollection<ReadyRocket> rockets)
    {
        RuleFor(x => x.Id, x => new LaunchRecordId(x.Random.Guid()));
        RuleFor(x => x.Partner, x => x.Company.CompanyName());
        RuleFor(x => x.Rocket, x => x.PickRandom(rockets.AsEnumerable()));
        RuleFor(x => x.RocketId, (_, v) => v.Rocket.Id);
        RuleFor(x => x.ActualLaunchDate, f => f.Date.PastOffset().OrNull(f, 0.2f));
        RuleFor(x => x.ScheduledLaunchDate, (f, v) => f.Date.PastOffset(refDate: v.ActualLaunchDate));
        RuleFor(x => x.PayloadWeightKg, f => f.Random.Number(100, 1000000));
        RuleFor(x => x.Payload, f => f.Lorem.Paragraphs());
    }
}
