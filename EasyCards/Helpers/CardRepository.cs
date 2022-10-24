using System.Collections.Immutable;
using RogueGenesia.Data;

namespace EasyCards.Helpers;

public static class CardRepository
{
    public static ImmutableList<SoulCardScriptableObject> GetAllCards()
    {
        return GameData.GetAllSoulBonus().ToImmutableList();
    }
}