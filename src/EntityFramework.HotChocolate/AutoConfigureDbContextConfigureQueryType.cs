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
    private static IObjectFieldDescriptor ConfigureResolve<TDbSet>(IObjectFieldDescriptor typeDescriptor, PropertyInfo propertyInfo)
    {
        var resolverContextProperty = Expression.Parameter(typeof(IPureResolverContext), "ctx");
        var cancellationTokenProperty = Expression.Parameter(typeof(CancellationToken), "ct");

        var serviceCall = Expression.Call(resolverContextProperty, nameof(IPureResolverContext.Service), new[] { typeof(TContext) });
        var contextProperty = Expression.Property(serviceCall, propertyInfo);

        var method = Expression.Lambda<Func<IResolverContext, CancellationToken, TDbSet>>(contextProperty, resolverContextProperty, cancellationTokenProperty)
                               .Compile();
        return typeDescriptor
              .UseDbContext<TContext>()
              .Resolve(method);
    }

    private static IObjectFieldDescriptor ConfigureResolveModel<TEntity, TModel>(IObjectFieldDescriptor typeDescriptor, PropertyInfo propertyInfo)
    {
        var resolverContextProperty = Expression.Parameter(typeof(IResolverContext), "ctx");
        var cancellationTokenProperty = Expression.Parameter(typeof(CancellationToken), "ct");

        var serviceCall = Expression.Call(
            resolverContextProperty,
            typeof(IPureResolverContext).GetMethod(nameof(IPureResolverContext.Service), BindingFlags.Public | BindingFlags.Instance)!.MakeGenericMethod(
                typeof(TContext)
            )
        );
        var contextProperty = Expression.Property(serviceCall, propertyInfo);

        var projectToMethod = typeof(AutoMapperQueryableExtensions).GetMethod(
            nameof(AutoMapperQueryableExtensions.ProjectTo), BindingFlags.Static | BindingFlags.Public
        );
        var projectTo = Expression.Call(null, projectToMethod!.MakeGenericMethod(typeof(TEntity), typeof(TModel)), contextProperty, resolverContextProperty);

        var method = Expression
                    .Lambda<Func<IResolverContext, CancellationToken, IQueryable<TModel>>>(projectTo, resolverContextProperty, cancellationTokenProperty)
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

    public Func<Type, Type> MapModel { get; set; }

    /// <inheritdoc />
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor.Name(OperationTypeNames.Query);
        var configure = typeof(ConfigureConfigureEntityFrameworkContextQueryType<TContext>).GetMethod(
            nameof(ConfigureResolve),
            BindingFlags.NonPublic | BindingFlags.Static
        )!;
        var configureModel = typeof(ConfigureConfigureEntityFrameworkContextQueryType<TContext>).GetMethod(
            nameof(ConfigureResolveModel),
            BindingFlags.NonPublic | BindingFlags.Static
        )!;
        var sets = typeof(TContext)
                  .GetProperties()
                  .Where(z => z.PropertyType.IsGenericType && z.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                  .ToArray();

        foreach (var set in sets)
        {
            var field = descriptor.Field(set.Name.Humanize().Pluralize().Dehumanize().Camelize());
            var entityType = set.PropertyType.GetGenericArguments()[0];
            var modelType = MapModel?.Invoke(entityType);
            if (modelType == entityType)
            {
                configure.MakeGenericMethod(set.PropertyType).Invoke(null, new object[] { field, set });
            }
            else if (modelType is { })
            {
                configureModel.MakeGenericMethod(set.PropertyType.GetGenericArguments()[0], modelType).Invoke(null, new object[] { field, set });
            }

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
