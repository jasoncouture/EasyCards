using System.Text.Json.Serialization;
using EasyCards.Models.Templates.Generated;
namespace EasyCards.Models.Templates;

public class CardTemplate
{
    private const string SchemaFileName = "schema.json";

    [JsonPropertyName("$schema")]
    public string Schema
    {
        get => SchemaFileName;
        // ReSharper disable once ValueParameterNotUsed - This is only here to appease the schema generation gods.
        set { }
    }

    public string Name { get; set; }
    public string TexturePath { get; set; }

    public TemplateCardRarity Rarity { get; set; }

    public List<TemplateCardTag> Tags { get; set; } = new();
    public float DropWeight { get; set; }
    public float LevelUpWeight { get; set; }
    public int MaxLevel { get; set; }
    public List<ModifierTemplate> Modifiers { get; set; } = new();
    public Dictionary<string, string> NameLocalization { get; set; } = new();
    public Dictionary<string, string> DescriptionLocalization { get; set; } = new();

    public List<string> BanishesCardsByName { get; set; } = new();
    public List<string> BanishesCardsWithStatsOfType { get; set; } = new();
    public List<string> RemovesCards { get; set; } = new();
    public RequirementTemplate RequiresAny { get; set; }
    public RequirementTemplate RequiresAll { get; set; }

    public override string ToString()
    {
        return $"{nameof(Name)}: {Name}, {nameof(TexturePath)}: {TexturePath}, {nameof(Rarity)}: {Rarity}, {nameof(Tags)}: {Tags}, {nameof(DropWeight)}: {DropWeight}, {nameof(LevelUpWeight)}: {LevelUpWeight}, {nameof(MaxLevel)}: {MaxLevel}, {nameof(Modifiers)}: {Modifiers}";
    }
}