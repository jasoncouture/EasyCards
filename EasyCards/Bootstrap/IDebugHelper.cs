using RogueGenesia.Data;

namespace EasyCards.Bootstrap;

public interface IDebugHelper : IModuleBootstrap
{
    void LogRequirements(SCSORequirementList requirementList, string prefix = "\t");
    bool Enabled { get; }
}
