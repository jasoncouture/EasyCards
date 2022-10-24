using System.Collections.Generic;
using EasyCards.Models.Templates;
using RogueGenesia.Data;

namespace EasyCards.Helpers;

public static partial class CardHelper
{
    private static List<LocalizationData> GetTranslations(Dictionary<string, string> translations)
    {
        var result = new List<LocalizationData>();
        
        foreach (var (localizationKey, translation) in translations)
        {
            var locale = GetLocaleForKey(localizationKey);

            if (locale == null)
            {
                s_log.LogWarning($"\tLocale {localizationKey} not supported!");
                continue;
            }

            var ld = new LocalizationData
            {
                Key = locale,
                Value = translation
            };

            s_log.LogInfo($"\tAdding translation for {locale.Identifier.Code}: {translation}");

            result.Add(ld);
        }

        return result;
    }
    
    private static List<LocalizationData> GetNameTranslations(CardTemplate cardTemplate)
    {
        s_log.LogInfo($"\tHandling Name translations");
        return GetTranslations(cardTemplate.NameLocalization);
    }

    private static List<LocalizationData> GetDescriptionTranslations(CardTemplate cardTemplate)
    {
        s_log.LogInfo($"\tHandling Description translations");
        return GetTranslations(cardTemplate.DescriptionLocalization);
    }

    private static void PostProcessDescriptions(Dictionary<string,SoulCardScriptableObject> allCards, Dictionary<string,CardTemplate> addedCards)
    {
        s_log.LogInfo($"Post processing descriptions for {addedCards.Count} cards");

        foreach (var cardName in addedCards.Keys)
        {
            var cardTemplate = addedCards[cardName];
            if (cardTemplate.DescriptionLocalization.Count == 0) continue;

            var cardScso = allCards[cardName];
            var translations = GetDescriptionTranslations(cardTemplate);
            s_log.LogInfo($"Got {translations.Count} description translations");

            foreach (var translation in translations)
            {
                cardScso.DescriptionOverride.Add(translation);
            }
        }
    }
}