using ModGenesia;

namespace EasyCards.Templates;

public class CardRequirementTemplate
{
    public string Name { get; set; }
    public int Level { get; set; }

    public ModCardRequirement ToModCardRequirement()
    {
        var requirement = new ModCardRequirement();
        
        EasyCards.Log.LogInfo($"Card Requirement: Name: {Name}, Level: {Level}");
        
        requirement.cardName = Name;
        requirement.requiredLevel = Level;

        return requirement;
    }
}