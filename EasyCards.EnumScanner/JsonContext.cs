using System.Text.Json.Serialization;

[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Serialization,
    DefaultIgnoreCondition = JsonIgnoreCondition.Never,
    IgnoreReadOnlyProperties = false,
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
)]
[JsonSerializable(typeof(EnumDefinition))]
[JsonSerializable(typeof(EnumMemberDefinition))]
[JsonSerializable(typeof(EnumDefinitions))]
internal partial class JsonContext : JsonSerializerContext
{
}
