using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using EasyCards.Helpers;
using HarmonyLib;

namespace EasyCards
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class EasyCards : BasePlugin
    {
        internal new static ManualLogSource Log;

        public override void Load()
        {
            Log = base.Log;
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loading! Patching!");
            
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            // CardHelper.LogCardWeights();
            CardHelper.LoadCustomCards();

            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
