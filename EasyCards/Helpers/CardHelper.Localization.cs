using System.Collections.Generic;
using BepInEx.Logging;
using EasyCards.Models.Templates;
using RogueGenesia.Data;

namespace EasyCards.Helpers;

public static class Localization
{
    private static ManualLogSource Logger => EasyCards.Instance.Log;  
    private static List<LocalizationData> GetTranslations(Dictionary<string, string> translations)
    {
        var result = new List<LocalizationData>();
        
        foreach (var (localizationKey, translation) in translations)
        {
            var locale = GetLocaleForKey(localizationKey);

            if (locale == null)
            {
                Logger.LogWarning($"\tLocale {localizationKey} not supported!");
                continue;
            }

            var ld = new LocalizationData
            {
                Key = locale,
                Value = translation
            };

            Logger.LogInfo($"\tAdding translation for {locale.Identifier.Code}: {translation}");

            result.Add(ld);
        }

        return result;
    }

    public static List<LocalizationData> GetNameTranslations(CardTemplate cardTemplate)
    {
        Logger.LogInfo($"\tHandling Name translations");
        return GetTranslations(cardTemplate.NameLocalization);
    }

    private static List<LocalizationData> GetDescriptionTranslations(CardTemplate cardTemplate)
    {
        Logger.LogInfo($"\tHandling Description translations");
        return GetTranslations(cardTemplate.DescriptionLocalization);
    }

    public static void PostProcessDescriptions(Dictionary<string,SoulCardScriptableObject> allCards, Dictionary<string,CardTemplate> addedCards)
    { 
        if (EasyCards.ShouldLogCardDetails) Logger.LogInfo($"=== Post processing descriptions for {addedCards.Count} cards ===");

        foreach (var cardName in addedCards.Keys)
        {
            var cardTemplate = addedCards[cardName];
            if (cardTemplate.DescriptionLocalization.Count == 0) continue;

            var cardScso = allCards[cardName];
            var translations = GetDescriptionTranslations(cardTemplate);
            if (EasyCards.ShouldLogCardDetails) Logger.LogInfo($"\tGot {translations.Count} description translations for {cardName}");

            foreach (var translation in translations)
            {
                cardScso.DescriptionOverride.Add(translation);
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
}