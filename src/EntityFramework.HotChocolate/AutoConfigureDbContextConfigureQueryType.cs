using System.Linq.Expressions;
using System.Reflection;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace Rocket.Surgery.LaunchPad.EntityFramework.HotChocolate;

/// <summary>
///     Configure the GraphQl values for a given Entity Framework type.
/// </summary>
/// <typeparam name="TContext"></typeparam>
public class ConfigureConfigureEntityFrameworkContextQueryType<TContext> : ObjectTypeExtension
    where TContext : DbContext
{
    private static IObjectFieldDescriptor ConfigureResolve<TEntity>(IObjectFieldDescriptor typeDescriptor, PropertyInfo propertyInfo)
    {
        var resolverContextProperty = Expression.Parameter(typeof(IPureResolverContext), "ctx");
        var cancellationTokenProperty = Expression.Parameter(typeof(CancellationToken), "ct");

        var serviceCall = Expression.Call(resolverContextProperty, nameof(IPureResolverContext.Service), new[] { typeof(TContext) });
        var contextProperty = Expression.Property(serviceCall, propertyInfo);

        var method = Expression.Lambda<Func<IResolverContext, CancellationToken, TEntity>>(contextProperty, resolverContextProperty, cancellationTokenProperty)
                               .Compile();
        return typeDescriptor
              .UseDbContext<TContext>()
              .Resolve(method);
    }

    private readonly IEnumerable<IConfigureEntityFrameworkEntityQueryType> _configureQueryEntities;

    /// <summary>
    ///     The constructor
    /// </summary>
    /// <param name="configureQueryEntities"></param>
    public ConfigureConfigureEntityFrameworkContextQueryType(IEnumerable<IConfigureEntityFrameworkEntityQueryType> configureQueryEntities)
    {
        _configureQueryEntities = configureQueryEntities;
    }

    /// <summary>
    ///     The constructor
    /// </summary>
    /// <param name="configureQueryEntities"></param>
    public ConfigureConfigureEntityFrameworkContextQueryType(params IConfigureEntityFrameworkEntityQueryType[] configureQueryEntities)
    {
        _configureQueryEntities = configureQueryEntities;
    }

    /// <inheritdoc />
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Name(OperationTypeNames.Query);
        var configure = typeof(ConfigureConfigureEntityFrameworkContextQueryType<TContext>).GetMethod(
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

    /// <summary>
    ///     Allows further customization by the child class.
    /// </summary>
    /// <param name="fieldDescriptor"></param>
    /// <param name="propertyInfo"></param>
    protected virtual void Configure(IObjectFieldDescriptor fieldDescriptor, PropertyInfo propertyInfo)
    {
    }
}
