using System.Collections.Generic;
using EasyCards.Extensions;
using EasyCards.Models.Templates;
using HarmonyLib;
using ModGenesia;
using RogueGenesia.Data;

namespace EasyCards.Helpers;

public static class CardTemplateExtensions
{
    public  static StatsModifier ToStatsModifier(this StatRequirementTemplate template)
    {
        if (EnumHelper.IsValidIdentifierForEnum<StatRequirementType>(template.RequirementType))
        {
            var statsModifier = new StatsModifier();

            var statModifiers = new List<StatModifier>();
            foreach (var statRequirement in template.StatRequirements)
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

        EasyCards.Log.LogWarning($"{template.RequirementType} is not a valid requirement type! Values values are: {System.Enum.GetNames(typeof(RequirementTemplate))}");
        return null;
    }
    public static StatModifier ToStatModifier( this StatRequirement template)
    {
        if (EnumHelper.IsValidIdentifierForEnum<StatsType>(template.Name))
        {
            var statModifier = new StatModifier();

            var singularModifier = new SingularModifier();

            // This is just a placeholder, as it is not used in the comparison
            singularModifier.ModifierType = ModifierType.Additional;
            singularModifier.Value = template.Value;

            statModifier.Key = template.Name;
            statModifier.Value = singularModifier;

            return statModifier;
        }

        EasyCards.Log.LogWarning($"{template.Name} is not valid a valid stat name!");
        return null;
    }
    
    public static SCSORequirementList ToRequirementList(this RequirementTemplate template)
    {
        if (template.Cards == null && template.Stats == null)
        {
            EasyCards.Log.LogInfo($"Nothing to convert, leaving");
            return null;
        }

        List<ModCardRequirement> cardRequirements = new();

        if (template.Cards != null)
        {
            cardRequirements = template.Cards.ConvertAll(template => template.ToModCardRequirement());
        }
        else
        {
            EasyCards.Log.LogInfo($"No Card requirements");
        }

        StatsModifier statRequirements = null;

        var isMinRequirement = true;
        
        if (template.Stats != null)
        {
            EasyCards.Log.LogInfo($"Converting Stat requirements");
            EasyCards.Log.LogWarning($"Stat Requirements don't currently work! Disabled for now!");
            // statRequirements = Stats.ToStatsModifier();
            // isMinRequirement = Stats.IsMinRequirement();
        }
        else
        {
            EasyCards.Log.LogInfo($"No Stat requirements");
        }
        
        var requirementList = ModGenesia.ModGenesia.MakeCardRequirement(cardRequirements?.ToIl2CppReferenceArray(), statRequirements, isMinRequirement);
        
        return requirementList;
    }
    public static ModCardRequirement ToModCardRequirement(this CardRequirementTemplate template)
    {
        var requirement = new ModCardRequirement()
        {
            cardName = template.Name,
            requiredLevel = template.Level
        };
        
        EasyCards.Log.LogInfo($"Card Requirement: Name: {template.Name}, Level: {template.Level}");

        return requirement;
    }
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