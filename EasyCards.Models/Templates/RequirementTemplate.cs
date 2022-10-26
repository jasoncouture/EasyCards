namespace EasyCards.Models.Templates;

public sealed class RequirementTemplate
{
    public List<CardRequirementTemplate> Cards { get; set; }
    public StatRequirementTemplate Stats { get; set; }
}