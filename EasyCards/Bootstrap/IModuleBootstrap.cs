namespace EasyCards.Bootstrap;

public interface IModuleBootstrap
{
    public int LoadOrder => 50;
    void Initialize();
}