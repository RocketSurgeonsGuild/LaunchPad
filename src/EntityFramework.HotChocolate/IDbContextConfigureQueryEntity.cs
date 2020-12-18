using HotChocolate.Types;
using System.Reflection;

namespace Sample.Graphql
{
    public interface IDbContextConfigureQueryEntity
    {
        void Configure(IObjectFieldDescriptor fieldDescriptor);
        bool Match(PropertyInfo propertyInfo);
    }
}