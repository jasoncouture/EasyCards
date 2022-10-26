using EasyCards.Helpers;
using Microsoft.Extensions.Logging;

namespace EasyCards.Logging;

public sealed class EasyCardsLoggerFactoryWrapper : ILoggerFactory
{
    private readonly ILoggerConfiguration _loggerConfiguration;

    public EasyCardsLoggerFactoryWrapper(ILoggerConfiguration loggerConfiguration)
    {
        _loggerConfiguration = loggerConfiguration;
    }

    public void Dispose()
    {
    }


    public ILogger CreateLogger(string categoryName) => new EasyCardsLogger(categoryName, _loggerConfiguration);

    public void AddProvider(ILoggerProvider provider)
    {
        // Not supported, and will be ignored.
    }
}