using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using EasyCards.Extensions;
using EasyCards.Models.Templates;
using ModGenesia;
using RogueGenesia.Data;
using UnityEngine;

namespace EasyCards.Helpers;

public static partial class CardHelper
{
    private static ManualLogSource s_log = EasyCards.Log;
    private static JsonSerializer s_jsonSerializer = new();

    public static void LoadCustomCards()
    {
        var jsonFiles = Directory.GetFiles(Paths.Data, "*.json");
        foreach (var jsonFile in jsonFiles)
        {
            try
            {
                AddCardsFromFile(jsonFile);
            }
            catch (Exception ex)
            {
                s_log.LogError($"Unable to load cards from file {jsonFile}: {ex}");
            }
        }
    }

    private static UnityEngine.Localization.Locale GetLocaleForKey(string localizationKey)
    {
        foreach (var locale in ModGenesia.ModGenesia.GetLocales())
        {
            if (locale.Identifier.Code == localizationKey)
            {
                return locale;
            }
        }

        return null;
    }

    public static void AddCardsFromFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            s_log.LogError($"File does not exist: {fileName}");
        }

        s_log.LogInfo($"Loading cards from file {fileName}");

        var json = File.ReadAllText(fileName);
        var templateFile = s_jsonSerializer.Deserialize<TemplateFile>(json);

        s_log.LogInfo($"Loaded {templateFile.Stats.Count} cards");

        var modSource = templateFile.ModSource ?? MyPluginInfo.PLUGIN_NAME;

        var successFullyAddedCards = new Dictionary<string, CardTemplate>();

        foreach (var cardTemplate in templateFile.Stats)
        {
            try
            {
                var soulCardData = ConvertCardTemplate(modSource, cardTemplate);
                s_log.LogInfo($"\tAdding card {cardTemplate.Name}");
                ModGenesia.ModGenesia.AddCustomStatCard(cardTemplate.Name, soulCardData);
                successFullyAddedCards.Add(cardTemplate.Name, cardTemplate);
            }
            catch (Exception ex)
            {
                s_log.LogInfo($"Error adding {cardTemplate.Name}: {ex}");
            }
        }

        var allCards = GetAllCardsAsDictionary();

        PostProcessDescriptions(allCards, successFullyAddedCards);
        PostProcessBanishes(allCards, successFullyAddedCards);
        PostProcessRemovals(allCards, successFullyAddedCards);
        PostProcessRequirements(allCards, successFullyAddedCards);
    }

    private static void PostProcessRequirements(Dictionary<string, SoulCardScriptableObject> allCards,
        Dictionary<string, CardTemplate> addedCards)
    {
        if (EasyCards.ShouldLogCardDetails)
            s_log.LogInfo($"=== Post processing requirements for {addedCards.Count} cards ===");

        var addedCardNames = addedCards.Keys;
        foreach (var cardName in addedCardNames)
        {
            if (EasyCards.ShouldLogCardDetails) s_log.LogInfo($"Processing {cardName}");
            var cardTemplate = addedCards[cardName];
            var cardScso = allCards[cardName];

            if (cardTemplate.RequiresAny != null)
            {
                if (EasyCards.ShouldLogCardDetails) s_log.LogInfo($"\t{cardName} - RequiresAny");
                cardScso.CardRequirement = cardTemplate.RequiresAny.ToRequirementList();
                if (EasyCards.ShouldLogCardDetails) DebugHelper.LogRequirements(cardScso.CardRequirement, "\t\t");
            }

            if (cardTemplate.RequiresAll != null)
            {
                if (EasyCards.ShouldLogCardDetails) s_log.LogInfo($"\t{cardName} - RequiresAll");
                cardScso.HardCardRequirement = cardTemplate.RequiresAll.ToRequirementList();
                if (EasyCards.ShouldLogCardDetails) DebugHelper.LogRequirements(cardScso.HardCardRequirement, "\t\t");
            }
        }
    }

    private static void PostProcessRemovals(Dictionary<string, SoulCardScriptableObject> allCards,
        Dictionary<string, CardTemplate> addedCards)
    {
        if (EasyCards.ShouldLogCardDetails)
            s_log.LogInfo($"=== Post processing removals for {addedCards.Count} cards ===");

        var addedCardNames = addedCards.Keys;
        foreach (var cardName in addedCardNames)
        {
            if (EasyCards.ShouldLogCardDetails) s_log.LogInfo($"Processing {cardName}");
            var cardTemplate = addedCards[cardName];
            var cardScso = allCards[cardName];

            var cardsToRemove = GetCardsForIdentifiers(allCards, cardTemplate.RemovesCards);

            if (EasyCards.ShouldLogCardDetails)
            {
                if (cardsToRemove.Count > 0)
                {
                    s_log.LogInfo($"\tRemoves {cardsToRemove.Count} cards:");
                    foreach (var cardToRemove in cardsToRemove)
                    {
                        s_log.LogInfo($"\t\t{cardToRemove.name}");
                    }
                }
            }

            cardScso.CardToRemove = cardsToRemove.ToIl2CppReferenceArray();
        }
    }

    private static void PostProcessBanishes(Dictionary<string, SoulCardScriptableObject> allCards,
        Dictionary<string, CardTemplate> addedCards)
    {
        if (EasyCards.ShouldLogCardDetails)
            s_log.LogInfo($"=== Post processing banishes for {addedCards.Count} cards ===");

        var addedCardNames = addedCards.Keys;
        var statToCardMap = GetAllCardsByStatsType();

        foreach (var cardName in addedCardNames)
        {
            if (EasyCards.ShouldLogCardDetails) s_log.LogInfo($"Processing {cardName}");
            var cardTemplate = addedCards[cardName];
            var cardScso = allCards[cardName];

            if (cardTemplate == null || cardScso == null)
            {
                if (EasyCards.ShouldLogCardDetails) s_log.LogInfo($"\tTemplate and SCSO are null! bailing!");
                continue;
            }

            var explicitlyBanishedCards = GetCardsForIdentifiers(allCards, cardTemplate.BanishesCardsByName);

            if (EasyCards.ShouldLogCardDetails)
            {
                s_log.LogInfo($"\tExplicitly banished cards: {explicitlyBanishedCards.Count}");
                foreach (var card in explicitlyBanishedCards)
                {
                    s_log.LogInfo($"\t\t{card.name}");
                }
            }

            var banishedCardsByStat =
                GetCardsWithStatModifiers(statToCardMap, cardTemplate.BanishesCardsWithStatsOfType);
            if (EasyCards.ShouldLogCardDetails)
            {
                s_log.LogInfo($"\tBanished cards by stat: {banishedCardsByStat.Count}");
                foreach (var card in banishedCardsByStat)
                {
                    s_log.LogInfo($"\t\t{card.name}");
                }
            }

            var finalList = new List<SoulCardScriptableObject>(explicitlyBanishedCards);
            finalList.AddRange(banishedCardsByStat);

            finalList = finalList.Distinct().ToList();

            // Make sure we don't accidentally banish the card that is being processed right now.
            var removedCards = finalList.RemoveAll(card => card.name == cardName);
            if (removedCards > 0)
            {
                s_log.LogInfo(
                    $"\tRemoved {cardName} from the list of banished cards! We don't want to banish ourselves, do we?");
            }

            if (EasyCards.ShouldLogCardDetails)
            {
                s_log.LogInfo($"\tFinal list of banished cards: {finalList.Count}");
                foreach (var banishedCard in finalList)
                {
                    s_log.LogInfo($"\t\t{banishedCard.name}");
                }
            }
            
            cardScso.CardExclusion = finalList.ToIl2CppReferenceArray();
        }
    }

    private static List<SoulCardScriptableObject> GetCardsWithStatModifiers(
        Dictionary<string, List<SoulCardScriptableObject>> statToCardsMap, List<string> modifiersToCheck)
    {
        var result = new List<SoulCardScriptableObject>();

        foreach (var modifier in modifiersToCheck)
        {
            var cardsForModifier = statToCardsMap.GetValueOrDefault(modifier);
            if (cardsForModifier != null)
            {
                result.AddRange(cardsForModifier);
            }
            else
            {
                s_log.LogWarning($"No cards found for modifier {modifier}");
            }
        }

        return result.Distinct().ToList();
    }

    private static List<SoulCardScriptableObject> GetCardsForIdentifiers(
        Dictionary<string, SoulCardScriptableObject> allCards, List<string> cardsToGet)
    {
        var result = new List<SoulCardScriptableObject>();
        foreach (var cardToGet in cardsToGet)
        {
            var cardScso = allCards.GetValueOrDefault(cardToGet);
            if (cardScso != null)
            {
                result.Add(cardScso);
            }
        }

        return result;
    }

    private static SoulCardCreationData ConvertCardTemplate(string modSource, CardTemplate cardTemplate)
    {
        s_log.LogInfo($"Converting {cardTemplate.Name}");
        var soulCardData = new SoulCardCreationData();

        soulCardData.ModSource = modSource;

        var texturePath = Path.Combine(Paths.Assets, cardTemplate.TexturePath);

        var sprite = SpriteHelper.LoadSpriteFromFile(texturePath);

        s_log.LogInfo($"\tSprite loaded: {sprite != null}");
        soulCardData.Texture = sprite;

        soulCardData.Rarity = (CardRarity)(int)cardTemplate.Rarity;

        var tags = (CardTag)cardTemplate.Tags.Aggregate(0, (current, tag) => current | (int)tag);

        soulCardData.Tags = tags;
        soulCardData.DropWeight = cardTemplate.DropWeight;
        soulCardData.LevelUpWeight = cardTemplate.LevelUpWeight;
        soulCardData.MaxLevel = cardTemplate.MaxLevel;
        
        soulCardData.StatsModifier = cardTemplate.CreateStatsModifier();

        if (cardTemplate.NameLocalization.Count > 0)
        {
            var nameLocalizations = GetNameTranslations(cardTemplate);
            foreach (var localization in nameLocalizations)
            {
                soulCardData.NameOverride.Add(localization);
            }
        }
        else
        {
            s_log.LogWarning($"No Name localizations provided for {cardTemplate.Name}!");
        }

        return soulCardData;
    }
}