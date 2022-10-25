using System.Collections.Immutable;
using RogueGenesia.Data;

namespace EasyCards.Services;

public interface ICardRepository
{
    ImmutableArray<SoulCardScriptableObject> GetAllCards();
}