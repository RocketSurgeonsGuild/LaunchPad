using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.LaunchPad.HotChocolate.FairyBread;

public class DefaultValidatorRegistry : IValidatorRegistry
{
    private readonly IServiceProvider _serviceProvider;

    private readonly Dictionary<Type, ValidatorDescriptor> _cache = new();

    public DefaultValidatorRegistry(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public bool TryGetValidator(Type type, [NotNullWhen(true)] out ValidatorDescriptor? descriptor)
    {
        if (_cache.TryGetValue(type, out var validator))
        {
            descriptor = validator;
            return true;
        }

        var validatorType = typeof(IValidator<>).MakeGenericType(type);
        if (_serviceProvider.GetService(typeof(IValidator<>).MakeGenericType(type)) is { })
        {
            descriptor = _cache[type] = new ValidatorDescriptor(validatorType);
            return true;
        }

        descriptor = null;
        return false;
    }
}
