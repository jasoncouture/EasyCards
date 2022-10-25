using BepInEx.Configuration;
using Microsoft.Extensions.Logging;

namespace EasyCards.Logging;

public class LoggerConfiguration : ILoggerConfiguration
{
    private readonly ConfigEntry<bool> _logCardDebuggingConfigEntry;

    public LoggerConfiguration(ConfigFile configFile)
    {
        _logCardDebuggingConfigEntry = configFile.Bind("Debug", "LogCards", false,
            "Logs details about the added cards at the end of initialization");
    }

    public bool IsLoggerEnabled(string category) => true;

    public bool IsLogLevelEnabled(string category, LogLevel logLevel) => IsLoggerEnabled(category) && (logLevel > LogLevel.Debug || _logCardDebuggingConfigEntry.Value);
}