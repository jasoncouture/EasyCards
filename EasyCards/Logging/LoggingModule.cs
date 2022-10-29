using EasyCards.Helpers;
using Microsoft.Extensions.Logging;
using StrongInject;

namespace EasyCards.Logging;

[Register(typeof(EasyCardsLoggerFactoryWrapper), Scope.SingleInstance, typeof(ILoggerFactory))]
[Register(typeof(Logger<>), Scope.SingleInstance, typeof(ILogger<>))]
[Register(typeof(LoggerConfiguration), Scope.SingleInstance, typeof(ILoggerConfiguration))]
public sealed class LoggingModule
{
}
