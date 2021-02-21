using HotChocolate.Language;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Rocket.Surgery.LaunchPad.HotChocolate.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Rocket.Surgery.LaunchPad.EntityFramework.HotChocolate
{
    public class AutoConfigureDbContextConfigureQueryType<TContext> : ConfigureGraphqlRootTypeBase
        where TContext : DbContext
    {
        private readonly IEnumerable<IDbContextConfigureQueryEntity> _configureQueryEntities;

        public AutoConfigureDbContextConfigureQueryType(IEnumerable<IDbContextConfigureQueryEntity> configureQueryEntities) : base(OperationType.Query)
        {
            _configureQueryEntities = configureQueryEntities;
        }

        public override void Configure(IObjectTypeDescriptor descriptor)
        {
            var configure = typeof(AutoConfigureDbContextConfigureQueryType<TContext>).GetMethod(
                nameof(ConfigureResolve),
                BindingFlags.NonPublic | BindingFlags.Static
            )!;
            var sets = typeof(TContext)
               .GetProperties()
               .Where(z => z.PropertyType.IsGenericType && z.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
               .ToArray();

            foreach (var set in sets)
            {
                var field = descriptor.Field(set.Name.Humanize().Pluralize().Dehumanize().Camelize());
                configure.MakeGenericMethod(set.PropertyType).Invoke(null, new object[] { field, set });
                Configure(field, set);
                foreach (var item in _configureQueryEntities.Where(z => z.Match(set)))
                {
                    item.Configure(field);
                }
            }
        }

        protected virtual void Configure(IObjectFieldDescriptor fieldDescriptor, PropertyInfo propertyInfo) { }

        private static IObjectFieldDescriptor ConfigureResolve<TEntity>(IObjectFieldDescriptor typeDescriptor, PropertyInfo propertyInfo)
        {
            var resolverContextProperty = Expression.Parameter(typeof(IResolverContext), "ctx");
            var cancellationTokenProperty = Expression.Parameter(typeof(CancellationToken), "ct");

            var serviceCall = Expression.Call(resolverContextProperty, nameof(IResolverContext.Service), new[] { typeof(TContext) });
            var contextProperty = Expression.Property(serviceCall, propertyInfo);

            var method = Expression.Lambda<Func<IResolverContext, CancellationToken, TEntity>>(contextProperty, resolverContextProperty, cancellationTokenProperty)
               .Compile();
            return typeDescriptor
               .UseDbContext<TContext>()
               .Resolver(method);
        }
    }
}