using System;
using System.Reflection;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Rocket.Surgery.LaunchPad.EntityFramework.HotChocolate;

/// <summary>
///     Configure the GraphQl values for a given Entity Framework type.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class ConfigureEntityFrameworkEntityQueryType<TEntity> : ObjectTypeExtension, IConfigureEntityFrameworkEntityQueryType
    where TEntity : class
{
    private readonly Action<IObjectFieldDescriptor>? _action;

    /// <summary>
    ///     The constructor
    /// </summary>
    public ConfigureEntityFrameworkEntityQueryType()
    {
    }

    /// <summary>
    ///     The constructor
    /// </summary>
    /// <param name="action"></param>
    public ConfigureEntityFrameworkEntityQueryType(Action<IObjectFieldDescriptor> action)
    {
        _action = action;
    }

    /// <summary>
    ///     Allows further customization by the child class.
    /// </summary>
    /// <param name="fieldDescriptor"></param>
    /// <exception cref="NotImplementedException"></exception>
    public virtual void Configure(IObjectFieldDescriptor fieldDescriptor)
    {
        if (_action == null)
            throw new NotImplementedException("Action was not implemented!");
        _action?.Invoke(fieldDescriptor);
    }

    bool IConfigureEntityFrameworkEntityQueryType.Match(PropertyInfo propertyInfo)
    {
        return propertyInfo.PropertyType == typeof(DbSet<TEntity>);
    }
}
