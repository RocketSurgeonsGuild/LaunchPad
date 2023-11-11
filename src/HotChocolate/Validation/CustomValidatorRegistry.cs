using FairyBread;
using FluentValidation;

namespace Rocket.Surgery.LaunchPad.HotChocolate.Validation;


class CustomValidatorRegistry : ICustomValidatorRegistry, IValidatorRegistry
{
    private readonly IServiceProvider _serviceProvider;

    private readonly Dictionary<Type, ValidatorDescriptor> _cache = new();

    public CustomValidatorRegistry(IServiceProvider serviceProvider)
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
        if (_serviceProvider.GetService(typeof(IValidator<>).MakeGenericType(type)) is { } service)
        {
            descriptor = _cache[type] = new ValidatorDescriptor(validatorType);
            return true;
        }

        descriptor = null;
        return false;
    }

    Dictionary<Type, List<ValidatorDescriptor>> IValidatorRegistry.Cache { get; } = new();
}
