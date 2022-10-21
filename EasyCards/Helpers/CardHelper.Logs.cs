using System.Linq;
using RogueGenesia.Data;

namespace EasyCards.Helpers;

public static partial class CardHelper
{
    public static void LogCardWeights()
    {
        s_log.LogInfo($"Current card weights:");

        var cards = GetAllCards().OrderBy(card => card.SoulRarity);
        
        CardRarity? lastRarity = null;
            
        foreach (var card in cards)
        {
            if (card.SoulRarity != lastRarity)
            {
                s_log.LogInfo($"");
                s_log.LogInfo($"***{card.SoulRarity.ToString()} cards:***");
                lastRarity = card.SoulRarity;
            }
                
            s_log.LogInfo($"{card.name}: [Drop: {card.DropWeight}, LevelUp: {card.LevelUpWeight}]");
        }
    }
    
    public static void LogCards()
    {
        var cardBonus = GetAllCards();

        foreach (var card in cardBonus)
        {
            LogCard(card);
        }
    }

    public static void LogCard(SoulCardScriptableObject card)
    {
        s_log.LogInfo($"=== Card: {card.name} =============================");
        s_log.LogInfo($"ID: {card.ID}");
        s_log.LogInfo($"Texture: {card.Texture}");
        s_log.LogInfo($"Unlocked: {card.Unlocked}");
        s_log.LogInfo($"canBeUnlocked: {card.canBeUnlocked}");
        s_log.LogInfo($"DropWeight: {card.DropWeight}");
        s_log.LogInfo($"MaxLevel: {card.MaxLevel}");
        s_log.LogInfo($"SoulRarity: {card.SoulRarity}");
        s_log.LogInfo($"WeaponType: {card.WeaponType}");
        s_log.LogInfo($"LevelUpWeight: {card.LevelUpWeight}");
        s_log.LogInfo($"ModifyPlayerStat: {card.ModifyPlayerStat}");
        s_log.LogInfo($"SoulCardTags: {card.SoulCardTags}");
        s_log.LogInfo($"SoulCardType: {card.SoulCardType}");
        s_log.LogInfo($"CardRequirementOnEverylevel: {card.CardRequirementOnEverylevel}");
        s_log.LogInfo($"UnlockedByDefault: {card.UnlockedByDefault}");
        s_log.LogInfo($"OverrideWeaponType: {card.OverrideWeaponType}");
        s_log.LogInfo($"CardRequirement: {card.CardRequirement}");
        s_log.LogInfo($"CardRequirement: {card.CardRequirement}");
        s_log.LogInfo($"StatsModifier:");
        LogStatsModifier(card.StatsModifier);
    }

    public static void LogStatsModifier(StatsModifier statsMod)
    {
        for (var i = 0; i < statsMod.ModifiersList.Count; i++)
        {
            var modifier = statsMod.ModifiersList[i];
            s_log.LogInfo($"Modifier: {i}");
            LogStatModifier(modifier);
        }
    }

    public static void LogStatModifier(StatModifier modifier)
    {
        s_log.LogInfo($"\tKey: {modifier.Key}");
        LogSingularModifier(modifier.Value);
    }

    public static void LogSingularModifier(SingularModifier modifier)
    {
        s_log.LogInfo($"\tValue: {modifier.Value}");
        s_log.LogInfo($"\tModifierType: {modifier.ModifierType}");
    }

}