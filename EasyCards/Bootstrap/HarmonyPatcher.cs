using System.Reflection;
using HarmonyLib;

namespace EasyCards.Bootstrap;

public sealed class HarmonyPatcher : IHarmonyPatcher
{
    public int LoadOrder => 0;
    public void Initialize() => Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
}