using System.Linq;
using Microsoft.Extensions.Logging;

namespace EasyCards.Bootstrap;

public class EasyCardsPluginLoader : IEasyCardsPluginLoader
{
    private readonly ILogger<EasyCardsPluginLoader> _logger;
    private readonly IModuleBootstrap[] _moduleBootstrappers;

    public EasyCardsPluginLoader(ILogger<EasyCardsPluginLoader> logger, IModuleBootstrap[] moduleBootstrappers)
    {
        _logger = logger;
        _moduleBootstrappers = moduleBootstrappers;
    }

    public void Load()
    {
        _logger.LogInformation($"Plugin {MyPluginInfo.PLUGIN_GUID} is loading! Patching!");
        foreach (var bootstrapper in _moduleBootstrappers.OrderBy(i => i.LoadOrder))
        {
            _logger.LogDebug("Initializing bootstrapper for {module}", bootstrapper.GetType().Name);
            bootstrapper.Initialize();
            _logger.LogDebug("Initialized bootstrapper for {module}", bootstrapper.GetType().Name);
        }
        _logger.LogInformation($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}