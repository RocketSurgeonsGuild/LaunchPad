using System;
using AutoMapper;
using AutoMapper.Features;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Extensions.AutoMapper {
    class AutoMapperLogger : IRuntimeFeature, IGlobalFeature, ILogger
    {
        private readonly ILogger _iLoggerImplementation;
        public AutoMapperLogger(ILogger iLoggerImplementation) => _iLoggerImplementation = iLoggerImplementation;

        void ILogger.Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter
        ) => _iLoggerImplementation.Log(logLevel, eventId, state, exception, formatter);

        bool ILogger.IsEnabled(LogLevel logLevel) => _iLoggerImplementation.IsEnabled(logLevel);
        IDisposable ILogger.BeginScope<TState>(TState state) => _iLoggerImplementation.BeginScope(state);
        void IRuntimeFeature.Seal(IConfigurationProvider configurationProvider) { }

        void IGlobalFeature.Configure(IConfigurationProvider configurationProvider)
        {
            configurationProvider.Features.Set(this);
        }
    }
}