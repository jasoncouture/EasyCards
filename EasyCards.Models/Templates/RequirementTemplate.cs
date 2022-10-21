using System.Collections.Generic;
using EasyCards.Extensions;
using ModGenesia;
using RogueGenesia.Data;

namespace EasyCards.Templates;

public class RequirementTemplate
{
    public List<CardRequirementTemplate> Cards { get; set; }
    public StatRequirementTemplate Stats { get; set; }


    public SCSORequirementList ToRequirementList()
    {
        if (Cards == null && Stats == null)
        {
            EasyCards.Log.LogInfo($"Nothing to convert, leaving");
            return null;
        }

        List<ModCardRequirement> cardRequirements = new();

        if (Cards != null)
        {
            cardRequirements = Cards.ConvertAll(template => template.ToModCardRequirement());
        }
        else
        {
            EasyCards.Log.LogInfo($"No Card requirements");
        }

        StatsModifier statRequirements = null;

        var isMinRequirement = true;
        
        if (Stats != null)
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
}