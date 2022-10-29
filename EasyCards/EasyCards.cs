using System.Collections.Concurrent;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using EasyCards.Bootstrap;
using Il2CppInterop.Generator.Extensions;

namespace EasyCards
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class EasyCards : BasePlugin
    {
        internal static EasyCards Instance { get; private set; }

        public override void Load()
        {
            // This must be set, before resolving anything from the container.
            // As the container uses this to expose BepInEx types.
            Instance = this;
            Container.Instance.Resolve<IEasyCardsPluginLoader>().Load();
        }
    }
}
