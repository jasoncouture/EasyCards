using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using RogueGenesia.Data;

namespace EasyCards.Helpers;

public static partial class CardHelper
{
    public static ImmutableList<SoulCardScriptableObject> GetAllCards()
    {
        return GameData.GetAllSoulBonus().ToImmutableList();
    }
    
    public static Dictionary<string, SoulCardScriptableObject> GetAllCardsAsDictionary()
    {
        return GetAllCards().ToDictionary(card => card.name);
    }

    public static Dictionary<string, List<SoulCardScriptableObject>> GetAllCardsByStatsType()
    {
        var result = new Dictionary<string, List<SoulCardScriptableObject>>();

        foreach (var card in GetAllCards())
        {
            foreach (var modifier in card.StatsModifier.ModifiersList)
            {
                if (!result.ContainsKey(modifier.Key))
                {
                    result[modifier.Key] = new();
                }

                result[modifier.Key].Add(card);
            }
        }

        return result;
    }
}