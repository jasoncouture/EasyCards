using BepInEx.Configuration;
using BepInEx.Logging;
using EasyCards.Bootstrap;
using EasyCards.Helpers;
using EasyCards.Logging;
using EasyCards.Services;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Linq;
using StrongInject;
using StrongInject.Modules;

namespace EasyCards;

[RegisterModule(typeof(LoggingModule))]
[RegisterModule(typeof(BootstrapModule))]
[RegisterModule(typeof(ServicesModule))]
[RegisterModule(typeof(CollectionsModule))]
public partial class Container : IContainer<IEasyCardsPluginLoader>, IContainer<IInputEventSubscriber[]>
{
    public static Container Instance { get; } = new();
    public T Resolve<T>()
    {
        return ((IContainer<T>)this).Resolve().Value;
    }

    [Factory]
    private EasyCards CreateEasyCards() => EasyCards.Instance;

    [Factory]
    private ManualLogSource CreateManualLogSource(EasyCards easyCards) => easyCards.Log;

    [Factory]
    private ConfigFile CreateConfigFile(EasyCards easyCards) => easyCards.Config;
}