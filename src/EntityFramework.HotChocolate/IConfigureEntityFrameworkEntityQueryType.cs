using HotChocolate.Types;
using System.Reflection;

namespace Rocket.Surgery.LaunchPad.EntityFramework.HotChocolate
{
    public interface IConfigureEntityFrameworkEntityQueryType
    {
        void Configure(IObjectFieldDescriptor fieldDescriptor);
        bool Match(PropertyInfo propertyInfo);
    }
}