using System.Collections.Generic;
using EasyCards.Helpers;
using RogueGenesia.Data;

namespace EasyCards.Templates;

public enum StatRequirementType
{
    Min,
    Max
}

public class StatRequirementTemplate
{
    public List<StatRequirement> StatRequirements { get; set; } = new();
    public string RequirementType { get; set; } = StatRequirementType.Min.ToString();

    public bool IsMinRequirement() => RequirementType == StatRequirementType.Min.ToString();

    public StatsModifier ToStatsModifier()
    {
        if (EnumHelper.IsValidIdentifierForEnum<StatRequirementType>(RequirementType))
        {
            var statsModifier = new StatsModifier();

            var statModifiers = new List<StatModifier>();
            foreach (var statRequirement in StatRequirements)
            {
                var convertedRequirement = statRequirement.ToStatModifier();
                if (convertedRequirement != null)
                {
                    statModifiers.Add(convertedRequirement);
                }
            }
            foreach (var statModifier in statModifiers)
            {
                statsModifier.ModifiersList.Add(statModifier);
            }
            
            return statsModifier;
        }

        EasyCards.Log.LogWarning($"{RequirementType} is not a valid requirement type! Values values are: {System.Enum.GetNames(typeof(RequirementTemplate))}");
        return null;
    }
}