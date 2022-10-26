using EasyCards.Services;
using Microsoft.Extensions.Logging;
using RogueGenesia.Data;
using UnityEngine.InputSystem;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace EasyCards.Bootstrap;

public class DebugHelper : IDebugHelper, IInputEventSubscriber
{
    private readonly ICardRepository _cardRepository;

    public DebugHelper(ILogger<DebugHelper> logger, ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
        Logger = logger;
    }

    private ILogger<DebugHelper> Logger { get; }

    public void Initialize()
    {
    }

    private void OnDebugLogKeyPressed()
    {
        if (!Logger.IsEnabled(LogLevel.Debug)) return;
        var allCards = _cardRepository.GetAllCards();

        Logger.LogInformation("=== Listing All Cards ===");

        var cardIdx = 0;
        foreach (var card in allCards)
        {
            cardIdx++;
            Logger.LogInformation($"=== Card {cardIdx}: {card.name} ===");

            var cardsToRemove = card.CardToRemove;
            Logger.LogInformation($"Removes cards: {cardsToRemove.Count}");
            foreach (var cardToRemove in cardsToRemove)
            {
                Logger.LogInformation($"\t{cardToRemove.name}");
            }

            var cardsToBanish = card.CardExclusion;
            Logger.LogInformation($"Banishes cards: {cardsToBanish.Count}");
            foreach (var cardToBanish in cardsToBanish)
            {
                Logger.LogInformation($"\t{cardToBanish.name}");
            }

            var requiresAnyCard = card.CardRequirement;
            Logger.LogInformation($"Requires ANY of the following cards:");
            LogRequirements(requiresAnyCard);

            var requiresAllCard = card.HardCardRequirement;
            Logger.LogInformation($"Requires ALL of the following:");
            LogRequirements(requiresAllCard);
        }

        Logger.LogInformation("=== Listing All Currently Excluded Cards ===");

        var excludedCards = SoulCardScriptableObject.GetExcludedSoulCard();
        foreach (var excludedCard in excludedCards)
        {
            Logger.LogInformation($"\t{excludedCard.name}");
        }
    }

    public bool Enabled => Logger.IsEnabled(LogLevel.Debug);
    public void LogRequirements(SCSORequirementList requirementList, string prefix = "\t")
    {
        if (requirementList == null) return;

        var cardRequirements = requirementList.CardRequirement;
        if (cardRequirements is { Count: > 0 })
        {
            Logger.LogInformation($"{prefix}Cards:");
            foreach (var cardRequirement in cardRequirements)
            {
                Logger.LogInformation($"{prefix}{prefix}{cardRequirement.RequiredCard.name}, Lvl {cardRequirement.RequiredCardLevel}");
            }
        }

        var statReqs = requirementList.StatsRequirement;
        if (statReqs is { Count: > 0 })
        {
            Logger.LogInformation($"{prefix}Stats:");
            foreach (var statReq in statReqs)
            {
                Logger.LogInformation($"{prefix}{prefix}Modifiers:");
                foreach (var statModifier in statReq.RequiredStats.ModifiersList)
                {
                    Logger.LogInformation(
                        $"{prefix}{prefix}{prefix}{statModifier.Key}: {statModifier.Value.Value} ({statModifier.Value.ModifierType})");
                }

                var typeString = statReq.RequireMore ? "Min" : "Max";

                Logger.LogInformation($"{prefix}{prefix}{prefix}Type: {typeString}");
            }
        }
    }

    public bool HandlesKey(Key key) => key == Key.L;

    public void OnInputEvent(Key key)
    {
        OnDebugLogKeyPressed();
    }
}