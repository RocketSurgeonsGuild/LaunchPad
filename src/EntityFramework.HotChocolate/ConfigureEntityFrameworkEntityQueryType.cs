using HotChocolate.Resolvers;
using HotChocolate.Types;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Rocket.Surgery.LaunchPad.EntityFramework.HotChocolate
{
    public class ConfigureEntityFrameworkEntityQueryType<TEntity> : ObjectTypeExtension, IConfigureEntityFrameworkEntityQueryType
        where TEntity : class
    {
        private readonly Action<IObjectFieldDescriptor>? _action;

        public ConfigureEntityFrameworkEntityQueryType() { }

        public ConfigureEntityFrameworkEntityQueryType(Action<IObjectFieldDescriptor> action) => _action = action;

        public virtual void Configure(IObjectFieldDescriptor fieldDescriptor)
        {
            if (_action == null)
                throw new NotImplementedException("Action was not implemented!");
            _action?.Invoke(fieldDescriptor);
        }

        bool IConfigureEntityFrameworkEntityQueryType.Match(PropertyInfo propertyInfo) => propertyInfo.PropertyType == typeof(DbSet<TEntity>);
    }
}