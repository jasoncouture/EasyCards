// See https://aka.ms/new-console-template for more information

using System.Reflection;
using System.Text.Json;

var exportedTypes = typeof(RogueGenesia.GameManager.GameManager).Assembly.GetExportedTypes();
var enums = exportedTypes.Where(i => i.IsEnum);

var target = Path.GetFullPath(args.Length > 0 ? args[0] : "enums.json");

var enumDefinitionList = new List<EnumDefinition>();
foreach (var @enum in enums)
{
    Console.Write("Processing enum: {0} ({1})...", @enum.Name, @enum.FullName);
    var name = @enum.Name;
    var baseType = @enum.GetEnumUnderlyingType();
    var enumNames = @enum.GetEnumNames();
    var rawValues = @enum.GetEnumValues();
    var enumNamesAndValues = enumNames.Zip(rawValues.Cast<object>().Select(Convert.ToUInt64));
    var isFlags = @enum.GetCustomAttribute<FlagsAttribute>() is not null;
    var definition = new EnumDefinition(name, baseType.Name, isFlags,
        enumNamesAndValues.Where(i => !i.First.StartsWith("PLACEHOLDER", StringComparison.OrdinalIgnoreCase)).Select(keyValuePair => new EnumMemberDefinition(keyValuePair.First, keyValuePair.Second))
            .ToArray());
    enumDefinitionList.Add(definition);
    Console.WriteLine(" Done! ({0} items processed)", definition.Members.Length);
}

var enumDefinitionsObject = new EnumDefinitions(enumDefinitionList.ToArray());
var bytes = JsonSerializer.SerializeToUtf8Bytes(enumDefinitionsObject, JsonContext.Default.EnumDefinitions);
Console.WriteLine("Writing {0} enum definitions, with {1} members total, to {2}", enumDefinitionsObject.Enums.Length, enumDefinitionsObject.Enums.Sum(x => x.Members.Length), target);
await File.WriteAllBytesAsync(target, bytes, CancellationToken.None);