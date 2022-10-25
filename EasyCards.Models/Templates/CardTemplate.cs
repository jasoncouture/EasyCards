using EasyCards.Models.Templates.Generated;
namespace EasyCards.Models.Templates;

public class CardTemplate
{
    public string Name { get; set; } = string.Empty;
    public string TexturePath { get; set; } = "placeholder.png";

    public TemplateCardRarity Rarity { get; set; }

    public List<TemplateCardTag> Tags { get; } = new();
    public float DropWeight { get; set; }
    public float LevelUpWeight { get; set; }
    public int MaxLevel { get; set; }
    public List<ModifierTemplate> Modifiers { get; } = new();
    public Dictionary<string, string> NameLocalization { get; set; } = new();
    public Dictionary<string, string> DescriptionLocalization { get; set; } = new();

    public List<string> BanishesCardsByName { get; } = new();
    public List<string> BanishesCardsWithStatsOfType { get; } = new();
    public List<string> RemovesCards { get; } = new();
    public RequirementTemplate RequiresAny { get; set; } = new();
    public RequirementTemplate RequiresAll { get; set; } = new();

    public override string ToString()
    {
        return $"{nameof(Name)}: {Name}, {nameof(TexturePath)}: {TexturePath}, {nameof(Rarity)}: {Rarity}, {nameof(Tags)}: {Tags}, {nameof(DropWeight)}: {DropWeight}, {nameof(LevelUpWeight)}: {LevelUpWeight}, {nameof(MaxLevel)}: {MaxLevel}, {nameof(Modifiers)}: {Modifiers}";
    }
}