using System.Text.Json;
using Native = System.Text.Json.JsonSerializer;

namespace EasyCards.Services
{
    public sealed class JsonDeserializer : IJsonDeserializer
    {
        public JsonDeserializer()
        {
            Options = new()
            {
                AllowTrailingCommas = true,
                IncludeFields = true,
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true
            };
            Options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        }


        public T Deserialize<T>(string json)
        {
            return Native.Deserialize<T>(json, Options);
        }

        public JsonSerializerOptions Options;
    }
}
