using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Sample.Graphql
{
    public abstract class DbContextConfigureQueryEntity<TEntity> : IDbContextConfigureQueryEntity
        where TEntity : class
    {
        public abstract void Configure(IObjectFieldDescriptor fieldDescriptor);
        bool IDbContextConfigureQueryEntity.Match(PropertyInfo propertyInfo) => propertyInfo.PropertyType == typeof(DbSet<TEntity>);
    }
}