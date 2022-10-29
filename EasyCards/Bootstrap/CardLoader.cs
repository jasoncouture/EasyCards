using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EasyCards.Extensions;
using EasyCards.Helpers;
using EasyCards.Models.Templates;
using EasyCards.Services;
using Microsoft.Extensions.Logging;
using ModGenesia;
using RogueGenesia.Data;

namespace EasyCards.Bootstrap;

public sealed class CardLoader : ICardLoader
{
    public CardLoader(ILogger<CardLoader> logger, IJsonDeserializer jsonDeserializer, IDebugHelper debugHelper, ISpriteLoader spriteLoader, ICardRepository cardRepository)
    {
        Logger = logger;
        _jsonDeserializer = jsonDeserializer;
        _debugHelper = debugHelper;
        _spriteLoader = spriteLoader;
        _cardRepository = cardRepository;
    }

    public int LoadOrder => 75;

    private ILogger Logger { get; }
    private readonly IJsonDeserializer _jsonDeserializer;
    private readonly IDebugHelper _debugHelper;
    private readonly ISpriteLoader _spriteLoader;
    private readonly ICardRepository _cardRepository;

    public void Initialize()
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
                Logger.LogError(ex, "Unable to load cards from file {jsonFile}", jsonFile);
            }
        }
    }


    public void AddCardsFromFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            Logger.LogError("File does not exist: {fileName}", fileName);
        }

        Logger.LogInformation("Loading cards from file {fileName}", fileName);

        var json = File.ReadAllText(fileName);
        var templateFile = _jsonDeserializer.Deserialize<TemplateFile>(json);

        Logger.LogInformation($"Loaded {templateFile.Stats.Count} cards");

        var modSource = templateFile.ModSource ?? MyPluginInfo.PLUGIN_NAME;

        var successFullyAddedCards = new Dictionary<string, CardTemplate>();

        foreach (var cardTemplate in templateFile.Stats)
        {
            try
            {
                var soulCardData = ConvertCardTemplate(modSource, cardTemplate);
                Logger.LogInformation($"\tAdding card {cardTemplate.Name}");
                ModGenesia.ModGenesia.AddCustomStatCard(cardTemplate.Name, soulCardData);
                successFullyAddedCards.Add(cardTemplate.Name, cardTemplate);
            }
            catch (Exception ex)
            {
                Logger.LogInformation(ex, $"Error adding {cardTemplate.Name}");
            }
        }

        var allCards = _cardRepository.GetAllCards().ToDictionary(card => card.name);

        Localization.PostProcessDescriptions(allCards, successFullyAddedCards);
        PostProcessBanishes(allCards, successFullyAddedCards);
        PostProcessRemovals(allCards, successFullyAddedCards);
        PostProcessRequirements(allCards, successFullyAddedCards);
    }

    private void PostProcessRequirements(Dictionary<string, SoulCardScriptableObject> allCards,
        Dictionary<string, CardTemplate> addedCards)
    {
        Logger.LogInformation($"=== Post processing requirements for {addedCards.Count} cards ===");

        var addedCardNames = addedCards.Keys;
        foreach (var cardName in addedCardNames)
        {
            Logger.LogInformation($"Processing {cardName}");
            var cardTemplate = addedCards[cardName];
            var cardScso = allCards[cardName];

            if (cardTemplate.RequiresAny != null)
            {
                Logger.LogInformation($"\t{cardName} - RequiresAny");
                cardScso.CardRequirement = cardTemplate.RequiresAny.ToRequirementList();
                _debugHelper.LogRequirements(cardScso.CardRequirement, "\t\t");
            }

            if (cardTemplate.RequiresAll != null)
            {
                Logger.LogInformation($"\t{cardName} - RequiresAll");
                cardScso.HardCardRequirement = cardTemplate.RequiresAll.ToRequirementList();
                _debugHelper.LogRequirements(cardScso.HardCardRequirement, "\t\t");
            }
        }
    }

    private void PostProcessRemovals(Dictionary<string, SoulCardScriptableObject> allCards,
        Dictionary<string, CardTemplate> addedCards)
    {
        Logger.LogInformation($"=== Post processing removals for {addedCards.Count} cards ===");

        var addedCardNames = addedCards.Keys;
        foreach (var cardName in addedCardNames)
        {
            Logger.LogInformation($"Processing {cardName}");
            var cardTemplate = addedCards[cardName];
            var cardScso = allCards[cardName];

            var cardsToRemove = GetCardsForIdentifiers(allCards, cardTemplate.RemovesCards);


            if (cardsToRemove.Count > 0)
            {
                Logger.LogInformation($"\tRemoves {cardsToRemove.Count} cards:");
                foreach (var cardToRemove in cardsToRemove)
                {
                    Logger.LogInformation($"\t\t{cardToRemove.name}");
                }
            }


            cardScso.CardToRemove = cardsToRemove.ToIl2CppReferenceArray();
        }
    }

    private void PostProcessBanishes(Dictionary<string, SoulCardScriptableObject> allCards,
        Dictionary<string, CardTemplate> addedCards)
    {
        Logger.LogInformation($"=== Post processing banishes for {addedCards.Count} cards ===");

        var addedCardNames = addedCards.Keys;
        var statToCardMap = _cardRepository.GetAllCards()
            .Select(card => new { card, modifiers = card.StatsModifier.ModifiersList.ToArray() })
            .SelectMany(cardData =>
                cardData.modifiers.Select(modifier => new { cardData.card, modifier = modifier.Key }))
            .GroupBy(i => i.modifier)
            .Select(i => new { i.Key, Value = i.Select(x => x.card).ToList() })
            .ToDictionary(i => i.Key, i => i.Value);

        foreach (var cardName in addedCardNames)
        {
            Logger.LogInformation($"Processing {cardName}");
            var cardTemplate = addedCards[cardName];
            var cardScso = allCards[cardName];

            if (cardTemplate == null || cardScso == null)
            {
                Logger.LogInformation($"\tTemplate and SCSO are null! bailing!");
                continue;
            }

            var explicitlyBanishedCards = GetCardsForIdentifiers(allCards, cardTemplate.BanishesCardsByName);


            {
                Logger.LogInformation($"\tExplicitly banished cards: {explicitlyBanishedCards.Count}");
                foreach (var card in explicitlyBanishedCards)
                {
                    Logger.LogInformation($"\t\t{card.name}");
                }
            }

            var banishedCardsByStat =
                GetCardsWithStatModifiers(statToCardMap, cardTemplate.BanishesCardsWithStatsOfType);

            {
                Logger.LogInformation($"\tBanished cards by stat: {banishedCardsByStat.Count}");
                foreach (var card in banishedCardsByStat)
                {
                    Logger.LogInformation($"\t\t{card.name}");
                }
            }

            var finalList = new List<SoulCardScriptableObject>(explicitlyBanishedCards);
            finalList.AddRange(banishedCardsByStat);

            finalList = finalList.Distinct().ToList();

            // Make sure we don't accidentally banish the card that is being processed right now.
            var removedCards = finalList.RemoveAll(card => card.name == cardName);
            if (removedCards > 0)
            {
                Logger.LogInformation(
                    $"\tRemoved {cardName} from the list of banished cards! We don't want to banish ourselves, do we?");
            }


            {
                Logger.LogInformation($"\tFinal list of banished cards: {finalList.Count}");
                foreach (var banishedCard in finalList)
                {
                    Logger.LogInformation($"\t\t{banishedCard.name}");
                }
            }

            cardScso.CardExclusion = finalList.ToIl2CppReferenceArray();
        }
    }

    private List<SoulCardScriptableObject> GetCardsWithStatModifiers(
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

    private List<SoulCardScriptableObject> GetCardsForIdentifiers(
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

    private SoulCardCreationData ConvertCardTemplate(string modSource, CardTemplate cardTemplate)
    {
        Logger.LogInformation($"Converting {cardTemplate.Name}");
        var soulCardData = new SoulCardCreationData();

        soulCardData.ModSource = modSource;

        var texturePath = Path.Combine(Paths.Assets, cardTemplate.TexturePath);

        var sprite = _spriteLoader.LoadSprite(texturePath);

        Logger.LogInformation($"\tSprite loaded: {sprite != null}");
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
