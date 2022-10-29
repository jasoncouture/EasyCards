namespace EasyCards.Models.Templates;

public class StatRequirementTemplate
{
    public List<StatRequirement> StatRequirements { get; set; } = new();
    public string RequirementType { get; set; } = StatRequirementType.Min.ToString();

    public bool IsMinRequirement() => RequirementType == StatRequirementType.Min.ToString();
}
