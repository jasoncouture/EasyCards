using Il2CppSystem.IO;

namespace EasyCards;

public static class Paths
{
    public static string Assets = Path.Combine(BepInEx.Paths.PluginPath, MyPluginInfo.PLUGIN_NAME, "Assets");
    public static string Data = Path.Combine(BepInEx.Paths.PluginPath, MyPluginInfo.PLUGIN_NAME, "Data");
}
