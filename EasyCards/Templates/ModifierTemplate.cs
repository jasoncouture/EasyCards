using RogueGenesia.Data;

namespace EasyCards.Templates;

public class ModifierTemplate
{
    public float ModifierValue { get; set; }

    public ModifierType ModifierType { get; set; }

    public StatsType Stat { get; set; }

    public override string ToString()
    {
        return $"{nameof(ModifierValue)}: {ModifierValue}, {nameof(ModifierType)}: {ModifierType}, {nameof(Stat)}: {Stat}";
    }

    /// <summary>
    /// Converts this ModifierTemplate into something the game can understand.
    /// </summary>
    /// <returns>A <c>StatModifier</c> based on this <c>ModifierTemplate</c></returns>
    public StatModifier ToStatModifier()
    {
        var singMod = new SingularModifier();
        singMod.Value = ModifierValue;
        singMod.ModifierType = ModifierType;

        var statModifier = new StatModifier();
        statModifier.Value = singMod;

        var modifierKey = Stat.ToString();

        statModifier.Key = modifierKey;

        return statModifier;
    }
}