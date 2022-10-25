using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using EasyCards.Extensions;
using EasyCards.Models.Templates;
using ModGenesia;
using RogueGenesia.Data;

namespace EasyCards.Helpers;

public static class CardHelper
{
    private static ManualLogSource Logger => EasyCards.Instance.Log;
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
                Logger.LogError($"Unable to load cards from file {jsonFile}: {ex}");
            }
        }
    }



    public static void AddCardsFromFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            Logger.LogError($"File does not exist: {fileName}");
        }

        Logger.LogInfo($"Loading cards from file {fileName}");

        var json = File.ReadAllText(fileName);
        var templateFile = s_jsonSerializer.Deserialize<TemplateFile>(json);

        Logger.LogInfo($"Loaded {templateFile.Stats.Count} cards");

        var modSource = templateFile.ModSource ?? MyPluginInfo.PLUGIN_NAME;

        var successFullyAddedCards = new Dictionary<string, CardTemplate>();

        foreach (var cardTemplate in templateFile.Stats)
        {
            try
            {
                var soulCardData = ConvertCardTemplate(modSource, cardTemplate);
                Logger.LogInfo($"\tAdding card {cardTemplate.Name}");
                ModGenesia.ModGenesia.AddCustomStatCard(cardTemplate.Name, soulCardData);
                successFullyAddedCards.Add(cardTemplate.Name, cardTemplate);
            }
            catch (Exception ex)
            {
                Logger.LogInfo($"Error adding {cardTemplate.Name}: {ex}");
            }
        }

        var allCards = CardRepository.GetAllCards().ToDictionary(card => card.name);

        Localization.PostProcessDescriptions(allCards, successFullyAddedCards);
        PostProcessBanishes(allCards, successFullyAddedCards);
        PostProcessRemovals(allCards, successFullyAddedCards);
        PostProcessRequirements(allCards, successFullyAddedCards);
    }

    private static void PostProcessRequirements(Dictionary<string, SoulCardScriptableObject> allCards,
        Dictionary<string, CardTemplate> addedCards)
    {
        if (EasyCards.ShouldLogCardDetails)
            Logger.LogInfo($"=== Post processing requirements for {addedCards.Count} cards ===");

        var addedCardNames = addedCards.Keys;
        foreach (var cardName in addedCardNames)
        {
            if (EasyCards.ShouldLogCardDetails) Logger.LogInfo($"Processing {cardName}");
            var cardTemplate = addedCards[cardName];
            var cardScso = allCards[cardName];

            if (cardTemplate.RequiresAny != null)
            {
                if (EasyCards.ShouldLogCardDetails) Logger.LogInfo($"\t{cardName} - RequiresAny");
                cardScso.CardRequirement = cardTemplate.RequiresAny.ToRequirementList();
                if (EasyCards.ShouldLogCardDetails) DebugHelper.LogRequirements(cardScso.CardRequirement, "\t\t");
            }

            if (cardTemplate.RequiresAll != null)
            {
                if (EasyCards.ShouldLogCardDetails) Logger.LogInfo($"\t{cardName} - RequiresAll");
                cardScso.HardCardRequirement = cardTemplate.RequiresAll.ToRequirementList();
                if (EasyCards.ShouldLogCardDetails) DebugHelper.LogRequirements(cardScso.HardCardRequirement, "\t\t");
            }
        }
    }

    private static void PostProcessRemovals(Dictionary<string, SoulCardScriptableObject> allCards,
        Dictionary<string, CardTemplate> addedCards)
    {
        if (EasyCards.ShouldLogCardDetails)
            Logger.LogInfo($"=== Post processing removals for {addedCards.Count} cards ===");

        var addedCardNames = addedCards.Keys;
        foreach (var cardName in addedCardNames)
        {
            if (EasyCards.ShouldLogCardDetails) Logger.LogInfo($"Processing {cardName}");
            var cardTemplate = addedCards[cardName];
            var cardScso = allCards[cardName];

            var cardsToRemove = GetCardsForIdentifiers(allCards, cardTemplate.RemovesCards);

            if (EasyCards.ShouldLogCardDetails)
            {
                if (cardsToRemove.Count > 0)
                {
                    Logger.LogInfo($"\tRemoves {cardsToRemove.Count} cards:");
                    foreach (var cardToRemove in cardsToRemove)
                    {
                        Logger.LogInfo($"\t\t{cardToRemove.name}");
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
            Logger.LogInfo($"=== Post processing banishes for {addedCards.Count} cards ===");

        var addedCardNames = addedCards.Keys;
        var statToCardMap = CardRepository.GetAllCards()
            .Select(card => new {card,  modifiers = card.StatsModifier.ModifiersList.ToArray() })
            .SelectMany(cardData => cardData.modifiers.Select(modifier => new { cardData.card, modifier = modifier.Key }))
            .GroupBy(i => i.modifier)
            .Select(i => new { i.Key, Value = i.Select(x => x.card).ToList()})
            .ToDictionary(i => i.Key, i => i.Value);
            
        foreach (var cardName in addedCardNames)
        {
            if (EasyCards.ShouldLogCardDetails) Logger.LogInfo($"Processing {cardName}");
            var cardTemplate = addedCards[cardName];
            var cardScso = allCards[cardName];

            if (cardTemplate == null || cardScso == null)
            {
                if (EasyCards.ShouldLogCardDetails) Logger.LogInfo($"\tTemplate and SCSO are null! bailing!");
                continue;
            }

            var explicitlyBanishedCards = GetCardsForIdentifiers(allCards, cardTemplate.BanishesCardsByName);

            if (EasyCards.ShouldLogCardDetails)
            {
                Logger.LogInfo($"\tExplicitly banished cards: {explicitlyBanishedCards.Count}");
                foreach (var card in explicitlyBanishedCards)
                {
                    Logger.LogInfo($"\t\t{card.name}");
                }
            }

            var banishedCardsByStat =
                GetCardsWithStatModifiers(statToCardMap, cardTemplate.BanishesCardsWithStatsOfType);
            if (EasyCards.ShouldLogCardDetails)
            {
                Logger.LogInfo($"\tBanished cards by stat: {banishedCardsByStat.Count}");
                foreach (var card in banishedCardsByStat)
                {
                    Logger.LogInfo($"\t\t{card.name}");
                }
            }

            var finalList = new List<SoulCardScriptableObject>(explicitlyBanishedCards);
            finalList.AddRange(banishedCardsByStat);

            finalList = finalList.Distinct().ToList();

            // Make sure we don't accidentally banish the card that is being processed right now.
            var removedCards = finalList.RemoveAll(card => card.name == cardName);
            if (removedCards > 0)
            {
                Logger.LogInfo(
                    $"\tRemoved {cardName} from the list of banished cards! We don't want to banish ourselves, do we?");
            }

            if (EasyCards.ShouldLogCardDetails)
            {
                Logger.LogInfo($"\tFinal list of banished cards: {finalList.Count}");
                foreach (var banishedCard in finalList)
                {
                    Logger.LogInfo($"\t\t{banishedCard.name}");
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
                Logger.LogWarning($"No cards found for modifier {modifier}");
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
        Logger.LogInfo($"Converting {cardTemplate.Name}");
        var soulCardData = new SoulCardCreationData();

        soulCardData.ModSource = modSource;

        var texturePath = Path.Combine(Paths.Assets, cardTemplate.TexturePath);

        var sprite = SpriteHelper.LoadSpriteFromFile(texturePath);

        Logger.LogInfo($"\tSprite loaded: {sprite != null}");
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
            var nameLocalizations = Localization.GetNameTranslations(cardTemplate);
            foreach (var localization in nameLocalizations)
            {
                soulCardData.NameOverride.Add(localization);
            }
        }
        else
        {
            Logger.LogWarning($"No Name localizations provided for {cardTemplate.Name}!");
        }

        return soulCardData;
    }
}