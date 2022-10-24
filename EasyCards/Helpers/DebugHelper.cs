using BepInEx.Logging;
using RogueGenesia.Data;
using UnityEngine.InputSystem;

namespace EasyCards.Helpers;

public static class DebugHelper
{
    private static ManualLogSource Logger => EasyCards.Instance.Log;

    public static void Initialize()
    {
        InputHelper.AddKeyListener(Key.L, OnDebugLogKeyPressed);
    }

    private static void OnDebugLogKeyPressed()
    {
        if (!EasyCards.ShouldLogCardDetails) return;
        
        var allCards = CardRepository.GetAllCards();

        Logger.LogInfo("=== Listing All Cards ===");

        var cardIdx = 0;
        foreach (var card in allCards)
        {
            cardIdx++;
            Logger.LogInfo($"=== Card {cardIdx}: {card.name} ===");

            var cardsToRemove = card.CardToRemove;
            Logger.LogInfo($"Removes cards: {cardsToRemove.Count}");
            foreach (var cardToRemove in cardsToRemove)
            {
                Logger.LogInfo($"\t{cardToRemove.name}");
            }

            var cardsToBanish = card.CardExclusion;
            Logger.LogInfo($"Banishes cards: {cardsToBanish.Count}");
            foreach (var cardToBanish in cardsToBanish)
            {
                Logger.LogInfo($"\t{cardToBanish.name}");
            }

            var requiresAnyCard = card.CardRequirement;
            Logger.LogInfo($"Requires ANY of the following cards:");
            LogRequirements(requiresAnyCard);

            var requiresAllCard = card.HardCardRequirement;
            Logger.LogInfo($"Requires ALL of the following:");
            LogRequirements(requiresAllCard);
        }

        Logger.LogInfo("=== Listing All Currently Excluded Cards ===");

        var excludedCards = SoulCardScriptableObject.GetExcludedSoulCard();
        foreach (var excludedCard in excludedCards)
        {
            Logger.LogInfo($"\t{excludedCard.name}");
        }
    }

    public static void LogRequirements(SCSORequirementList requirementList, string prefix = "\t")
    {
        if (requirementList == null) return;

        var cardRequirements = requirementList.CardRequirement;
        if (cardRequirements is { Count: > 0 })
        {
            Logger.LogInfo($"{prefix}Cards:");
            foreach (var cardRequirement in cardRequirements)
            {
                Logger.LogInfo($"{prefix}{prefix}{cardRequirement.RequiredCard.name}, Lvl {cardRequirement.RequiredCardLevel}");
            }
        }

        var statReqs = requirementList.StatsRequirement;
        if (statReqs is { Count: > 0 })
        {
            Logger.LogInfo($"{prefix}Stats:");
            foreach (var statReq in statReqs)
            {
                Logger.LogInfo($"{prefix}{prefix}Modifiers:");
                foreach (var statModifier in statReq.RequiredStats.ModifiersList)
                {
                    Logger.LogInfo(
                        $"{prefix}{prefix}{prefix}{statModifier.Key}: {statModifier.Value.Value} ({statModifier.Value.ModifierType})");
                }

                var typeString = statReq.RequireMore ? "Min" : "Max";

                Logger.LogInfo($"{prefix}{prefix}{prefix}Type: {typeString}");
            }
        }
    }
}