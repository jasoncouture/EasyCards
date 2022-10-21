using System.Collections.Generic;
using RogueGenesia.Data;

namespace EasyCards.Templates;

public class CardTemplate
{
    public string Name { get; set; }
    public string TexturePath { get; set; }

    public CardRarity Rarity { get; set; }

    public List<CardTag> Tags { get; set; } = new();
    public float DropWeight { get; set; }
    public float LevelUpWeight { get; set; }
    public int MaxLevel { get; set; }
    public List<ModifierTemplate> Modifiers { get; set; } = new();
    public Dictionary<string, string> NameLocalization { get; set; }

    public List<string> BanishesCardsByName { get; set; } = new();
    public List<string> BanishesCardsWithStatsOfType { get; set; } = new();
    public List<string> RemovesCards { get; set; } = new();
    public List<string> RequiresAny { get; set; } = new();
    public List<string> RequiresAll { get; set; } = new();

    public void Validate()
    {
    }

    public override string ToString()
    {
        return $"{nameof(Name)}: {Name}, {nameof(TexturePath)}: {TexturePath}, {nameof(Rarity)}: {Rarity}, {nameof(Tags)}: {Tags}, {nameof(DropWeight)}: {DropWeight}, {nameof(LevelUpWeight)}: {LevelUpWeight}, {nameof(MaxLevel)}: {MaxLevel}, {nameof(Modifiers)}: {Modifiers}";
    }

    public StatsModifier ConvertModifiersToStatsModifier()
    {
        var statsMod = new StatsModifier();

        foreach (var modifier in Modifiers)
        {
            statsMod.ModifiersList.Add(modifier.ToStatModifier());
        }

        return statsMod;
    }
}