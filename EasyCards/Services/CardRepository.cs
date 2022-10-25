using System.Collections.Immutable;
using RogueGenesia.Data;

namespace EasyCards.Services;

public class CardRepository : ICardRepository
{
    private ImmutableArray<SoulCardScriptableObject>? _cardCache;

    public ImmutableArray<SoulCardScriptableObject> GetAllCards() =>
        _cardCache ??= GameData.GetAllSoulBonus().ToImmutableArray();
    
}