using RogueGenesia.Data;

namespace EasyCards.Helpers;

public static class CardTemplateExtensions
{
    public static StatsModifier CreateStatsModifier(this CardTemplate cardTemplate)
    {
        var statsMod = new StatsModifier();

        foreach (var modifier in cardTemplate.Modifiers)
        {
            statsMod.ModifiersList.Add(modifier.CreateStatModifier());
        }

        return statsMod;
    }
    
    /// <summary>
    /// Converts this ModifierTemplate into something the game can understand.
    /// </summary>
    /// <returns>A <c>StatModifier</c> based on this <c>ModifierTemplate</c></returns>
    public static StatModifier CreateStatModifier(this ModifierTemplate modifierTemplate)
    {
        var singMod = new SingularModifier
        {
            Value = modifierTemplate.ModifierValue,
            ModifierType = (ModifierType)(int)modifierTemplate.ModifierType
        };

        var statModifier = new StatModifier
        {
            Value = singMod,
            Key = modifierTemplate.Stat.ToString()
        };

        return statModifier;
    }
}