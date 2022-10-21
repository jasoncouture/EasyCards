using EasyCards.Helpers;
using RogueGenesia.Data;

namespace EasyCards.Templates;

public class StatRequirement
{
    public string Name { get; set; }
    public float Value { get; set; }
    
    public StatModifier ToStatModifier()
    {
        if (EnumHelper.IsValidIdentifierForEnum<StatsType>(Name))
        {
            var statModifier = new StatModifier();

            var singularModifier = new SingularModifier();

            // This is just a placeholder, as it is not used in the comparison
            singularModifier.ModifierType = ModifierType.Additional;
            singularModifier.Value = Value;

            statModifier.Key = Name;
            statModifier.Value = singularModifier;

            return statModifier;
        }

        EasyCards.Log.LogWarning($"{Name} is not valid a valid stat name!");
        return null;
    }

    public override string ToString()
    {
        return $"{nameof(Name)}: {Name}, {nameof(Value)}: {Value}";
    }
}