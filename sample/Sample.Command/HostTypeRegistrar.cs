using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli;

namespace Sample.Command;

/// <summary>
///     Lets Spectre.Console.Cli create its commands with the services of the host.
/// </summary>
/// <remarks>
///     What Spectre registers (its commands) stays in a collection of its own, and each command is created with the services of the host:
///     taken from the host's container when that knows it, otherwise built from the services it has. A type Spectre did not register is
///     resolved from the host or left to Spectre, which creates its settings itself when the resolver returns <see langword="null" />.
/// </remarks>
internal sealed class HostTypeRegistrar(IServiceProvider hostServices) : ITypeRegistrar
{
    public ITypeResolver Build() => new Resolver(hostServices, _spectre.BuildServiceProvider());

    public void Register(Type service, Type implementation) =>
        _spectre.AddSingleton(service, _ => ActivatorUtilities.GetServiceOrCreateInstance(hostServices, implementation));

    public void RegisterInstance(Type service, object implementation) => _spectre.AddSingleton(service, implementation);

    public void RegisterLazy(Type service, Func<object> factory) => _spectre.AddSingleton(service, _ => factory());

    private readonly ServiceCollection _spectre = [];

    private sealed class Resolver(IServiceProvider hostServices, ServiceProvider spectre) : ITypeResolver, IDisposable
    {
        public object? Resolve(Type? type) => type is null ? null : spectre.GetService(type) ?? hostServices.GetService(type);

        public void Dispose() => spectre.Dispose();
    }
}
