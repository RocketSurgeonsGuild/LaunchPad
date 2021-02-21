using HotChocolate.Types;
using System.Reflection;

namespace Rocket.Surgery.LaunchPad.EntityFramework.HotChocolate
{
    public interface IDbContextConfigureQueryEntity
    {
        void Configure(IObjectFieldDescriptor fieldDescriptor);
        bool Match(PropertyInfo propertyInfo);
    }
}