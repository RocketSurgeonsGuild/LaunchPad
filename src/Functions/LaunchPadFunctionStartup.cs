using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.LaunchPad.Functions;

/// <summary>
///     Default startup class for interacting with Azure Functions
/// </summary>
public abstract class LaunchPadFunctionStartup : FunctionsStartup
{
    internal ConventionContextBuilder _builder;
    internal IConventionContext _context = null!;

    /// <summary>
    ///     The default constuctor
    /// </summary>
    protected LaunchPadFunctionStartup()
    {
        _builder = new ConventionContextBuilder(new Dictionary<object, object?>())
                  .UseAppDomain(AppDomain.CurrentDomain)
                  .Set(HostType.Live);
        if (this is IConvention convention)
        {
            _builder.AppendConvention(convention);
        }

        // TODO: Restore this sometime
        // var functionsAssembly = this.GetType().Assembly;
        // var location = Path.GetDirectoryName(functionsAssembly.Location);
        // DependencyContext? dependencyContext = null;
        // while (!string.IsNullOrEmpty(location))
        // {
        //     var depsFilePath = Path.Combine(location, functionsAssembly.GetName().Name + ".deps.json");
        //     if (File.Exists(depsFilePath))
        //     {
        //         using var stream = File.Open(depsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        //         using var reader = new DependencyContextJsonReader();
        //         dependencyContext = reader.Read(stream);
        //         break;
        //     }
        //
        //     location = Path.GetDirectoryName(location);
        // }
    }

    /// <summary>
    ///     The default constructor with the given configuration method
    /// </summary>
    /// <param name="configure"></param>
    protected LaunchPadFunctionStartup(Func<LaunchPadFunctionStartup, ConventionContextBuilder> configure)
    {
        _builder = configure(this).Set(HostType.Live);
        if (this is IConvention convention)
        {
            _builder.AppendConvention(convention);
        }
    }

    /// <inheritdoc />
    public sealed override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        Setup(_builder);
        _context = ConventionContext.From(_builder);
        builder.ConfigurationBuilder.ApplyConventions(_context);
        ConfigureAppConfiguration(builder, _context);
    }

    /// <inheritdoc />
    public sealed override void Configure(IFunctionsHostBuilder builder)
    {
        var existingHostedServices = builder.Services.Where(x => x.ServiceType == typeof(IHostedService)).ToArray();
        var builderContext = builder.GetContext();

        _context.Set(builderContext.Configuration);
        builder.Services.ApplyConventions(_context);

        builder.Services.RemoveAll<IHostedService>();
        builder.Services.Add(existingHostedServices);
        Configure(builder, _context);
    }

    /// <summary>
    ///     Method called to setup the conventions
    /// </summary>
    /// <param name="contextBuilder"></param>
    public virtual void Setup(ConventionContextBuilder contextBuilder)
    {
    }

    /// <summary>
    ///     Method called to configure the application with the conventions
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="context"></param>
    public virtual void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder, IConventionContext context)
    {
    }

    /// <summary>
    ///     Method called to configure the services with the conventions
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="context"></param>
    public abstract void Configure(IFunctionsHostBuilder builder, IConventionContext context);

    /// <summary>
    ///     Use the given rocket booster
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public LaunchPadFunctionStartup UseRocketBooster(Func<LaunchPadFunctionStartup, ConventionContextBuilder> configure)
    {
        _builder = configure(this);
        return this;
    }

    /// <summary>
    ///     Use the given rocket booster
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public LaunchPadFunctionStartup LaunchWith(Func<LaunchPadFunctionStartup, ConventionContextBuilder> configure)
    {
        _builder = configure(this);
        return this;
    }
}
