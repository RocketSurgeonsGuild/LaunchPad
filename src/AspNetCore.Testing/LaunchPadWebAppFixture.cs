using Alba;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.LaunchPad.AspNetCore.Testing;

/// <summary>
/// A fixture used to bootstrap the alba test server with a custom sql extension
/// </summary>
public abstract class LaunchPadWebAppFixture<TEntryPoint> : ILaunchPadWebAppFixture
    where TEntryPoint : class
{
    private readonly AppFixtureLoggerFactory _loggerFactory;
    private readonly IResettableAlbaExtension? _sqlExtension;
    private readonly IAlbaExtension[] _extensions;
    private IAlbaHost? _host;

    /// <summary>
    /// The base constructor for use with hte web app fixture
    /// </summary>
    /// <param name="resettableAlbaExtension"></param>
    /// <param name="extensions"></param>
    protected LaunchPadWebAppFixture(IResettableAlbaExtension? resettableAlbaExtension, params IAlbaExtension[] extensions)
    {
        _loggerFactory = new AppFixtureLoggerFactory();
        _sqlExtension = resettableAlbaExtension;
        _extensions = new IAlbaExtension[]
                      {
                          new LaunchPadExtension(this, _loggerFactory),
                          _sqlExtension!
                      }
                     .Where(z => z != null!)
                     .Concat(extensions)
                     .ToArray();
    }

    public IAlbaHost AlbaHost => _host!;

    public void SetLoggerFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory.SetLoggerFactory(loggerFactory);
    }

    public void Reset()
    {
        if (_sqlExtension is null || _host is null) return;
        _sqlExtension.Reset(AlbaHost.Services);
    }

    public async Task ResetAsync()
    {
        if (_sqlExtension is null || _host is null) return;

        await _sqlExtension.ResetAsync(AlbaHost.Services);
    }

    public async Task InitializeAsync()
    {
        _host = await Alba.AlbaHost.For<TEntryPoint>(_extensions);
    }

    public async Task DisposeAsync()
    {
        await AlbaHost.DisposeAsync();
        _loggerFactory.Dispose();

        if (_sqlExtension != null)
        {
            await _sqlExtension.DisposeAsync();
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await DisposeAsync().ConfigureAwait(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        _loggerFactory.Dispose();
        _sqlExtension?.Dispose();
        AlbaHost.Dispose();
    }

    class AppFixtureLoggerFactory : ILoggerFactory
    {
        private ILoggerFactory? _innerLoggerFactory;

        public void SetLoggerFactory(ILoggerFactory loggerFactory)
        {
            _innerLoggerFactory = loggerFactory;
        }

        public void Dispose()
        {
            _innerLoggerFactory?.Dispose();
        }

        public void AddProvider(ILoggerProvider provider)
        {
            _innerLoggerFactory?.AddProvider(provider);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _innerLoggerFactory?.CreateLogger(categoryName) ?? NullLogger.Instance;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
