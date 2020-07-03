using Bogus;
using Sample.Core.Domain;

namespace Sample.Core
{
    class RocketFaker : Faker<Sample.Core.Domain.ReadyRocket>
    {
        public RocketFaker()
        {
            RuleFor(x => x.Id, x => x.Random.Guid());
            RuleFor(x => x.Type, x => x.PickRandom<RocketType>());
            RuleFor(x => x.SerialNumber, x => x.Vehicle.Vin());
        }
    }
}