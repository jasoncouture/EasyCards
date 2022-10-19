using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using Cpp2IL.Core.Analysis.PostProcessActions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Reflection;
using ModGenesia;
using RogueGenesia.Data;

namespace EasyCards.Helpers
{
    public static class CardHelper
    {
        private static ManualLogSource l = EasyCards.Log;
        private static JsonSerializer _jsonSerializer = new();

        public static ImmutableList<SoulCardScriptableObject> GetAllCards()
        {
            return GameData.GetAllSoulBonus().ToImmutableList();
        }

        public static Dictionary<string, SoulCardScriptableObject> GetAllCardsAsDictionary()
        {
            return GetAllCards().ToDictionary(card => card.GetLocalizationName);
        }

        public static void LogCards()
        {
            var cardBonus = GetAllCards();

            foreach (var card in cardBonus)
            {
                logCard(card);
            }
        }

        private static void logCard(SoulCardScriptableObject card)
        {
            l.LogInfo($"=== Card: {card.GetLocalizationName} =============================");
            l.LogInfo($"ID: {card.ID}");
            l.LogInfo($"Texture: {card.Texture}");
            l.LogInfo($"Unlocked: {card.Unlocked}");
            l.LogInfo($"canBeUnlocked: {card.canBeUnlocked}");
            l.LogInfo($"DropWeight: {card.DropWeight}");
            l.LogInfo($"MaxLevel: {card.MaxLevel}");
            l.LogInfo($"SoulRarity: {card.SoulRarity}");
            l.LogInfo($"WeaponType: {card.WeaponType}");
            l.LogInfo($"LevelUpWeight: {card.LevelUpWeight}");
            l.LogInfo($"ModifyPlayerStat: {card.ModifyPlayerStat}");
            l.LogInfo($"SoulCardTags: {card.SoulCardTags}");
            l.LogInfo($"SoulCardType: {card.SoulCardType}");
            l.LogInfo($"CardRequirementOnEverylevel: {card.CardRequirementOnEverylevel}");
            l.LogInfo($"UnlockedByDefault: {card.UnlockedByDefault}");
            l.LogInfo($"OverrideWeaponType: {card.OverrideWeaponType}");
            l.LogInfo($"CardRequirement: {card.CardRequirement}");
            l.LogInfo($"CardRequirement: {card.CardRequirement}");
            l.LogInfo($"StatsModifier:");
            logStatsModifier(card.StatsModifier);
        }

        private static void logStatsModifier(StatsModifier statsMod)
        {
            for (var i = 0; i < statsMod.ModifiersList.Count; i++)
            {
                var modifier = statsMod.ModifiersList[i];
                l.LogInfo($"Modifier: {i}");
                logStatModifier(modifier);
            }
        }

        private static void logStatModifier(StatModifier modifier)
        {
            l.LogInfo($"\tKey: {modifier.Key}");
            logSingularModifier(modifier.Value);
        }

        private static void logSingularModifier(SingularModifier modifier)
        {
            l.LogInfo($"\tValue: {modifier.Value}");
            l.LogInfo($"\tModifierType: {modifier.ModifierType}");
        }

        private static UnityEngine.Localization.Locale getLocaleForKey(string localizationKey)
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
                l.LogError($"File does not exist: {fileName}");
            }

            l.LogInfo($"Loading cards from file {fileName}");

            var json = File.ReadAllText(fileName);
            var templateFile = _jsonSerializer.Deserialize<TemplateFile>(json);

            l.LogInfo($"Loaded {templateFile.Stats.Count} cards");

            var modSource = templateFile.ModSource ?? MyPluginInfo.PLUGIN_NAME;

            var successFullyAddedCards = new Dictionary<string, CardTemplate>();
            
            foreach (var cardTemplate in templateFile.Stats)
            {
                try
                {
                    var soulCardData = convertCardTemplate(modSource, cardTemplate);
                    l.LogInfo($"\tAdding card {cardTemplate.Name}");
                    ModGenesia.ModGenesia.AddCustomStatCard(cardTemplate.Name, soulCardData);
                    successFullyAddedCards.Add($"Stats_{cardTemplate.Name}", cardTemplate);
                }
                catch (Exception ex)
                {
                    l.LogInfo($"Error adding {cardTemplate.Name}: {ex}");
                }
            }

            // PostProcess(successFullyAddedCards);
        }

        private static void PostProcess(Dictionary<string,CardTemplate> successFullyAddedCards)
        {
            var allCards = GetAllCardsAsDictionary();
            var addedCardNames = successFullyAddedCards.Keys.ToList();
            
            foreach (var cardName in addedCardNames)
            {
                var cardTemplate = successFullyAddedCards[cardName];
                var cardScso = allCards[cardName];
                
                if (cardTemplate == null || cardScso == null) continue;

                var banishedCards = GetCardsForIdentifiers(allCards, cardTemplate.BanishesCards);
                cardScso.CardExclusion = new Il2CppReferenceArray<SoulCardScriptableObject>(banishedCards.ToArray());

                var cardsToRemove = GetCardsForIdentifiers(allCards, cardTemplate.RemovesCards);
                cardScso.CardToRemove = new Il2CppReferenceArray<SoulCardScriptableObject>(cardsToRemove.ToArray());
            }
        }

        private static List<SoulCardScriptableObject> GetCardsForIdentifiers(
            Dictionary<string, SoulCardScriptableObject> allCards, List<string> cardsToGet)
        {
            var result = new List<SoulCardScriptableObject>();
            foreach (var cardToGet in cardsToGet)
            {
                var cardScso = allCards[cardToGet];
                if (cardScso != null)
                {
                    result.Add(cardScso);
                }
            }

            return result;
        }

        private static SoulCardCreationData convertCardTemplate(string modSource, CardTemplate cardTemplate)
        {
            l.LogInfo($"Converting {cardTemplate.Name}");
            var soulCardData = new SoulCardCreationData();

            soulCardData.ModSource = modSource;
            
            var texturePath = Path.Combine(Paths.Assets, cardTemplate.TexturePath);

            var sprite = SpriteHelper.LoadSpriteFromFile(texturePath);

            l.LogInfo($"\tSprite loaded: {sprite != null}");
            soulCardData.Texture = sprite;

            soulCardData.Rarity = cardTemplate.Rarity;

            var tags = cardTemplate.Tags.Aggregate<CardTag, CardTag>(0, (current, tag) => current | tag);

            soulCardData.Tags = tags;
            soulCardData.DropWeight = cardTemplate.DropWeight;
            soulCardData.LevelUpWeight = cardTemplate.LevelUpWeight;
            soulCardData.MaxLevel = cardTemplate.MaxLevel;

            l.LogInfo($"\tLoading localizations");
            foreach (var (localizationKey, translation) in cardTemplate.NameLocalization)
            {
                var locale = getLocaleForKey(localizationKey);

                if (locale == null)
                {
                    l.LogError($"\tLocale {localizationKey} not supported!");
                    continue;
                }

                var ld = new LocalizationData
                {
                    Key = locale,
                    Value = translation
                };

                l.LogInfo($"\tAdding {cardTemplate.Name} translation for {locale.Identifier.Code}: {translation}");

                soulCardData.NameOverride.Add(ld);
            }

            var statsMod = new StatsModifier();

            foreach (var modifier in cardTemplate.Modifiers)
            {
                var singMod = new SingularModifier();
                singMod.Value = modifier.ModifierValue;
                singMod.ModifierType = modifier.ModifierType;

                var sm = new StatModifier();
                sm.Value = singMod;

                var modifierKey = modifier.Stat.ToString();

                sm.Key = modifierKey;

                statsMod.ModifiersList.Add(sm);
            }

            soulCardData.StatsModifier = statsMod;
            
            return soulCardData;
        }

        private static Il2CppReferenceArray<SoulCardScriptableObject> findCardsByLocalizationName(
            ImmutableList<SoulCardScriptableObject> existingCards, List<string> cards)
        {
            var foundCards = existingCards.Where(card => cards.Contains(card.GetLocalizationName)).ToArray();
            return new Il2CppReferenceArray<SoulCardScriptableObject>(foundCards);
        }

        public static void LogCardWeights()
        {
            l.LogInfo($"Current card weights:");
            
            var cards = from card in GetAllCards()
                orderby card.SoulRarity 
                select card;
            
            CardRarity? lastRarity = null;
            
            foreach (var card in cards)
            {
                if (card.SoulRarity != lastRarity)
                {
                    l.LogInfo($"");
                    l.LogInfo($"***{card.SoulRarity.ToString()} cards:***");
                    lastRarity = card.SoulRarity;
                }
                
                l.LogInfo($"{card.GetLocalizationName}: [Drop: {card.DropWeight}, LevelUp: {card.LevelUpWeight}]");
            }
        }

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
                    l.LogError($"Unable to load cards from file {jsonFile}: {ex}");
                }
            }
        }
    }
}