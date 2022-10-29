using EasyCards.Services;

namespace EasyCards.Bootstrap;

public sealed class InputBootstrapper : IModuleBootstrap
{
    public int LoadOrder => 100;
    public void Initialize()
    {
        EasyCards.Instance.AddComponent<KeyBehaviour>();
    }
}
