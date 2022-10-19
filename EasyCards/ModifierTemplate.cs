namespace EasyCards;

public class ModifierTemplate
{
    public float ModifierValue { get; set; }

    public ModifierType ModifierType { get; set; }

    public StatsType Stat { get; set; }

    public override string ToString()
    {
        return $"{nameof(ModifierValue)}: {ModifierValue}, {nameof(ModifierType)}: {ModifierType}, {nameof(Stat)}: {Stat}";
    }
}