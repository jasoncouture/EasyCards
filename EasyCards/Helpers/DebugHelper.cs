using BepInEx.Logging;
using RogueGenesia.Data;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace EasyCards.Helpers;

public static class DebugHelper
{
    private static ManualLogSource s_log = EasyCards.Log;

    public static void Initialize()
    {
        InputHelper.AddKeyListener(Key.L, OnDebugLogKeyPressed);
    }

    private static void OnDebugLogKeyPressed()
    {
        if (!EasyCards.ShouldLogCardDetails) return;
        
        var allCards = CardHelper.GetAllCards();

        s_log.LogInfo("=== Listing All Cards ===");

        var cardIdx = 0;
        foreach (var card in allCards)
        {
            cardIdx++;
            s_log.LogInfo($"=== Card {cardIdx}: {card.name} ===");

            var cardsToRemove = card.CardToRemove;
            s_log.LogInfo($"Removes cards: {cardsToRemove.Count}");
            foreach (var cardToRemove in cardsToRemove)
            {
                s_log.LogInfo($"\t{cardToRemove.name}");
            }

            var cardsToBanish = card.CardExclusion;
            s_log.LogInfo($"Banishes cards: {cardsToBanish.Count}");
            foreach (var cardToBanish in cardsToBanish)
            {
                s_log.LogInfo($"\t{cardToBanish.name}");
            }

            var requiresAnyCard = card.CardRequirement;
            s_log.LogInfo($"Requires ANY of the following cards:");
            LogRequirements(requiresAnyCard);

            var requiresAllCard = card.HardCardRequirement;
            s_log.LogInfo($"Requires ALL of the following:");
            LogRequirements(requiresAllCard);
        }

        s_log.LogInfo("=== Listing All Currently Excluded Cards ===");

        var excludedCards = SoulCardScriptableObject.GetExcludedSoulCard();
        foreach (var excludedCard in excludedCards)
        {
            EasyCards.Log.LogInfo($"\t{excludedCard.name}");
        }
    }

    public static void LogRequirements(SCSORequirementList requirementList, string prefix = "\t")
    {
        if (requirementList == null) return;

        var cardRequirements = requirementList.CardRequirement;
        if (cardRequirements is { Count: > 0 })
        {
            s_log.LogInfo($"{prefix}Cards:");
            foreach (var cardRequirement in cardRequirements)
            {
                s_log.LogInfo($"{prefix}{prefix}{cardRequirement.RequiredCard.name}, Lvl {cardRequirement.RequiredCardLevel}");
            }
        }

        var statReqs = requirementList.StatsRequirement;
        if (statReqs is { Count: > 0 })
        {
            s_log.LogInfo($"{prefix}Stats:");
            foreach (var statReq in statReqs)
            {
                s_log.LogInfo($"{prefix}{prefix}Modifiers:");
                foreach (var statModifier in statReq.RequiredStats.ModifiersList)
                {
                    s_log.LogInfo(
                        $"{prefix}{prefix}{prefix}{statModifier.Key}: {statModifier.Value.Value} ({statModifier.Value.ModifierType})");
                }

                var typeString = statReq.RequireMore ? "Min" : "Max";

                s_log.LogInfo($"{prefix}{prefix}{prefix}Type: {typeString}");
            }
        }
    }
}