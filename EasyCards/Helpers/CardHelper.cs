using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using EasyCards.Templates;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using ModGenesia;
using RogueGenesia.Data;

namespace EasyCards.Helpers
{
    public static class CardHelper
    {
        private static ManualLogSource s_log = EasyCards.Log;
        private static JsonSerializer s_jsonSerializer = new();

        private static Dictionary<string, List<SoulCardScriptableObject>> GetAllCardsByStatsType()
        {
            var result = new Dictionary<string, List<SoulCardScriptableObject>>();

            foreach (var card in GetAllCards())
            {
                foreach (var modifier in card.StatsModifier.ModifiersList)
                {
                    if (!result.ContainsKey(modifier.Key))
                    {
                        result[modifier.Key] = new();
                    }

                    result[modifier.Key].Add(card);
                }
            }

            return result;
        }
        
        public static ImmutableList<SoulCardScriptableObject> GetAllCards()
        {
            return GameData.GetAllSoulBonus().ToImmutableList();
        }

        public static Dictionary<string, SoulCardScriptableObject> GetAllCardsAsDictionary()
        {
            return GetAllCards().ToDictionary(card => card.name);
        }

        public static void LogCards()
        {
            var cardBonus = GetAllCards();

            foreach (var card in cardBonus)
            {
                LogCard(card);
            }
        }

        private static void LogCard(SoulCardScriptableObject card)
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

        private static void LogStatsModifier(StatsModifier statsMod)
        {
            for (var i = 0; i < statsMod.ModifiersList.Count; i++)
            {
                var modifier = statsMod.ModifiersList[i];
                s_log.LogInfo($"Modifier: {i}");
                LogStatModifier(modifier);
            }
        }

        private static void LogStatModifier(StatModifier modifier)
        {
            s_log.LogInfo($"\tKey: {modifier.Key}");
            LOGSingularModifier(modifier.Value);
        }

        private static void LOGSingularModifier(SingularModifier modifier)
        {
            s_log.LogInfo($"\tValue: {modifier.Value}");
            s_log.LogInfo($"\tModifierType: {modifier.ModifierType}");
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

            PostProcess(successFullyAddedCards);
        }

        private static void PostProcess(Dictionary<string,CardTemplate> successFullyAddedCards)
        {
            s_log.LogInfo($"Post processing {successFullyAddedCards.Count} cards");
            
            var allCards = GetAllCardsAsDictionary();
            var addedCardNames = successFullyAddedCards.Keys.ToList();
            var statToCardMap = GetAllCardsByStatsType();
            
            foreach (var cardName in addedCardNames)
            {
                s_log.LogInfo($"Processing {cardName}");
                var cardTemplate = successFullyAddedCards[cardName];
                var cardScso = allCards[cardName];

                if (cardTemplate == null || cardScso == null)
                {
                    s_log.LogInfo($"Template and SCSO are null! bailing!");
                    continue;
                }

                var explicitlyBanishedCards = GetCardsForIdentifiers(allCards, cardTemplate.BanishesCardNames);
                
                s_log.LogInfo($"Explicitly banished cards: {explicitlyBanishedCards.Count}");
                
                var banishedCardsByStat = GetCardsWithStatModifiers(statToCardMap, cardTemplate.BanishesCardsWithStatModifiers);
                s_log.LogInfo($"Banished cards by stat: {banishedCardsByStat.Count}");

                var finalList = new List<SoulCardScriptableObject>(explicitlyBanishedCards);
                finalList.AddRange(banishedCardsByStat);

                finalList = finalList.Distinct().ToList();
                
                s_log.LogInfo($"Final list of banished cards: {finalList.Count}");
                cardScso.CardExclusion = new Il2CppReferenceArray<SoulCardScriptableObject>(finalList.ToArray());

                var cardsToRemove = GetCardsForIdentifiers(allCards, cardTemplate.RemovesCards);
                cardScso.CardToRemove = new Il2CppReferenceArray<SoulCardScriptableObject>(cardsToRemove.ToArray());
            }
        }
        
        private static List<SoulCardScriptableObject> GetCardsWithStatModifiers(
            Dictionary<string, List<SoulCardScriptableObject>> statToCardsMap, List<StatsType> modifiersToCheck)
        {
            var result = new List<SoulCardScriptableObject>();

            foreach (var modifier in modifiersToCheck)
            {
                var cardsForModifier = statToCardsMap[modifier.ToString()];
                result.AddRange(cardsForModifier);
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

            soulCardData.Rarity = cardTemplate.Rarity;

            var tags = cardTemplate.Tags.Aggregate<CardTag, CardTag>(0, (current, tag) => current | tag);

            soulCardData.Tags = tags;
            soulCardData.DropWeight = cardTemplate.DropWeight;
            soulCardData.LevelUpWeight = cardTemplate.LevelUpWeight;
            soulCardData.MaxLevel = cardTemplate.MaxLevel;

            s_log.LogInfo($"\tLoading localizations");
            foreach (var (localizationKey, translation) in cardTemplate.NameLocalization)
            {
                var locale = GetLocaleForKey(localizationKey);

                if (locale == null)
                {
                    s_log.LogError($"\tLocale {localizationKey} not supported!");
                    continue;
                }

                var ld = new LocalizationData
                {
                    Key = locale,
                    Value = translation
                };

                s_log.LogInfo($"\tAdding {cardTemplate.Name} translation for {locale.Identifier.Code}: {translation}");

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

        public static void LogCardWeights()
        {
            s_log.LogInfo($"Current card weights:");
            
            var cards = from card in GetAllCards()
                orderby card.SoulRarity 
                select card;
            
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
    }
}