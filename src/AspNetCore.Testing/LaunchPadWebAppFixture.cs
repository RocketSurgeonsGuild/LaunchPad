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
                          // ReSharper disable once NullableWarningSuppressionIsUsed
                          _sqlExtension!
                      }
                      // ReSharper disable once NullableWarningSuppressionIsUsed
                     .Where(z => z != null!)
                     .Concat(extensions)
                     .ToArray();
    }

    // ReSharper disable once NullableWarningSuppressionIsUsed
    /// <summary>
    /// The underlying alba host
    /// </summary>
    public IAlbaHost AlbaHost => _host!;

    /// <summary>
    /// Set the loggerfactory when initializing the test
    /// </summary>
    /// <param name="loggerFactory"></param>
    public void SetLoggerFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory.SetLoggerFactory(loggerFactory);
    }

    /// <summary>
    /// Method to reset the database if provided
    /// </summary>
    public void Reset()
    {
        if (_sqlExtension is null || _host is null) return;
        _sqlExtension.Reset(AlbaHost.Services);
    }

    /// <summary>
    /// Method to reset the database if provided
    /// </summary>
    public async Task ResetAsync()
    {
        if (_sqlExtension is null || _host is null) return;

        await _sqlExtension.ResetAsync(AlbaHost.Services);
    }

    /// <summary>
    /// Method used to start the alba host
    /// </summary>
    public async Task InitializeAsync()
    {
        _host = await Alba.AlbaHost.For<TEntryPoint>(_extensions);
    }

    /// <summary>
    /// Method used to dispose the alba host
    /// </summary>
    public async Task DisposeAsync()
    {
        await AlbaHost.DisposeAsync();
        _loggerFactory.Dispose();

        if (_sqlExtension != null)
        {
            await _sqlExtension.DisposeAsync();
        }
    }

#pragma warning disable CA1816
    async ValueTask IAsyncDisposable.DisposeAsync()
#pragma warning restore CA1816
    {
        await DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// The dispose method
    /// </summary>
    /// <param name="disposing"></param>
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
        private List<DeferredLogger> _deferredLoggers = new();

        public void SetLoggerFactory(ILoggerFactory loggerFactory)
        {
            _innerLoggerFactory = loggerFactory;
            foreach (var logger in _deferredLoggers)
            {
                logger.SetLogger(loggerFactory.CreateLogger(logger.CategoryName));
            }
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
            return _innerLoggerFactory?.CreateLogger(categoryName) ?? AddDeferredLogger(categoryName);
        }

        private DeferredLogger AddDeferredLogger(string categoryName)
        {
            var logger = new DeferredLogger(categoryName);
            _deferredLoggers.Add(logger);
            return logger;
        }
    }

    class DeferredLogger : ILogger
    {
        public string CategoryName { get; }
        private List<(LogLevel logLevel, EventId eventId, string text)> _deferredLogs = new();
        private ILogger? _logger;

        public DeferredLogger(string categoryName)
        {
            CategoryName = categoryName;
        }
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (_logger is null)
            {
                _deferredLogs.Add((logLevel, eventId, formatter(state, exception)));
                return;
            }
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger?.IsEnabled(logLevel) ?? true;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return _logger?.BeginScope(state);
        }

        public void SetLogger(ILogger logger)
        {
            _logger = logger;
            foreach (var log in _deferredLogs)
            {
#pragma warning disable CA1848
#pragma warning disable CA2254
                // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
                _logger.Log(log.logLevel, log.eventId, log.text);
#pragma warning restore CA2254
#pragma warning restore CA1848
            }
        }
    }

    /// <summary>
    /// The dispose method
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
