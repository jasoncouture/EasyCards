using System.Linq;
using System.Text.Json;
using EasyCards.EnumGenerator.EnumScannerModels;

namespace EasyCards.EnumGenerator
{
    public static class EnumScanResultsLoader
    {
        public static EnumDefinitions GetEnumDefinitions()
        {
            var jsonDataResourceName = typeof(EnumScanResultsLoader).Assembly
                .GetManifestResourceNames().Single(i => i.Contains("scanned-enums"));

            using (var manifestStream =
                   typeof(EnumScanResultsLoader).Assembly.GetManifestResourceStream(jsonDataResourceName))
            {
                if (manifestStream == null) return null;
                var result = JsonSerializer.Deserialize<EnumDefinitions>(manifestStream, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });

                return result;
            }
        }
    }
}