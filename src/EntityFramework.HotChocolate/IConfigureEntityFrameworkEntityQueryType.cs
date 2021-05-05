using HotChocolate.Types;
using System.Reflection;

namespace Rocket.Surgery.LaunchPad.EntityFramework.HotChocolate
{
    /// <summary>
    /// Interface for configuring entity framework query types with graphql
    /// </summary>
    public interface IConfigureEntityFrameworkEntityQueryType
    {
        /// <summary>
        /// The method used to the configure the field
        /// </summary>
        /// <param name="fieldDescriptor"></param>
        void Configure(IObjectFieldDescriptor fieldDescriptor);
        /// <summary>
        /// Determine if the given property is a match or not.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        bool Match(PropertyInfo propertyInfo);
    }
}