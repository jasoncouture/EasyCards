using EasyCards.Services;

namespace EasyCards.Bootstrap;

public class InputBootstrapper : IModuleBootstrap
{
    public int LoadOrder => 100;
    public void Initialize()
    {
        EasyCards.Instance.AddComponent<KeyBehaviour>();
    }
}