using EasyCards.Services;
using StrongInject;

namespace EasyCards.Bootstrap;

[Register(typeof(JsonDeserializer), Scope.SingleInstance, typeof(IJsonDeserializer))]
[Register(typeof(EasyCardsPluginLoader), Scope.SingleInstance, typeof(IEasyCardsPluginLoader))]
// The order of these 3 is important. The order they appear here, is the order they will load in.
[Register(typeof(HarmonyPatcher), Scope.SingleInstance, typeof(IHarmonyPatcher), typeof(IModuleBootstrap))]
[Register(typeof(DebugHelper), Scope.SingleInstance, typeof(IDebugHelper), typeof(IModuleBootstrap), typeof(IInputEventSubscriber))]
[Register(typeof(CardLoader), Scope.SingleInstance, typeof(ICardLoader), typeof(IModuleBootstrap))]
[Register(typeof(InputBootstrapper), Scope.SingleInstance, typeof(IModuleBootstrap))]
public sealed class BootstrapModule { }