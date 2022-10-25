using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using EasyCards.Helpers;
using HarmonyLib;

namespace EasyCards
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class EasyCards : BasePlugin
    {
        internal static EasyCards Instance { get; private set; }
        private ConfigEntry<bool> _configLogCardDetails;

        public static bool ShouldLogCardDetails => Instance._configLogCardDetails.Value;

        public override void Load()
        {
            Instance = this;

            _configLogCardDetails = Config.Bind("Debug", "LogCards", false,
                "Logs details about the added cards at the end of initialization");

            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loading! Patching!");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            DebugHelper.Initialize();

            CardHelper.LoadCustomCards();

            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}