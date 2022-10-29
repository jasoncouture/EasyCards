using Microsoft.Extensions.Logging;

namespace EasyCards.Logging;

public interface ILoggerConfiguration
{
    bool IsLoggerEnabled(string category);
    bool IsLogLevelEnabled(string category, LogLevel logLevel);
}
