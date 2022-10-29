using System;
using BepInEx.Logging;
using EasyCards.Helpers;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace EasyCards.Logging;

public sealed class EasyCardsLogger : ILogger
{
    private readonly ManualLogSource _logSource;
    private readonly string _category;
    private readonly ILoggerConfiguration _loggerConfiguration;

    public EasyCardsLogger(string category, ILoggerConfiguration loggerConfiguration)
    {
        _category = category;
        _loggerConfiguration = loggerConfiguration;
        _logSource = Logger.CreateLogSource(category);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
        Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;
        _logSource.Log(ConvertLogLevel(logLevel), formatter.Invoke(state, exception));
    }

    private BepInEx.Logging.LogLevel ConvertLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Debug => BepInEx.Logging.LogLevel.Debug,
            LogLevel.Trace => BepInEx.Logging.LogLevel.Debug,
            LogLevel.Information => BepInEx.Logging.LogLevel.Info,
            LogLevel.Warning => BepInEx.Logging.LogLevel.Warning,
            LogLevel.Error => BepInEx.Logging.LogLevel.Error,
            LogLevel.Critical => BepInEx.Logging.LogLevel.Fatal,
            LogLevel.None => BepInEx.Logging.LogLevel.None,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };
    }

    public bool IsEnabled(LogLevel logLevel) => _loggerConfiguration.IsLogLevelEnabled(_category, logLevel);

    public IDisposable BeginScope<TState>(TState state)
    {
        // Not supported, no-op.
        return null;
    }
}
