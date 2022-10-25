namespace EasyCards.Models.Templates;

public class RequirementTemplate
{
    public List<CardRequirementTemplate> Cards { get; } = new();
    public StatRequirementTemplate Stats { get; set; } = new();
}