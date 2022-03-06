using Bogus;
using Sample.Core.Domain;
using Sample.Core.Models;

namespace Sample.Core;

public class RocketFaker : Faker<ReadyRocket>
{
    public RocketFaker()
    {
        RuleFor(x => x.Id, x => new RocketId(x.Random.Guid()));
        RuleFor(x => x.Type, x => x.PickRandom<RocketType>());
        RuleFor(x => x.SerialNumber, x => x.Vehicle.Vin());
    }
}
