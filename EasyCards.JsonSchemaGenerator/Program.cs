// See https://aka.ms/new-console-template for more information

using EasyCards.Models.Templates;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonSchema;
using NJsonSchema.Generation;

var target = Path.GetFullPath(args.Length > 0 ? args[0] : "schema.json");

Console.WriteLine("Generating JSON schema for TemplateFile, output path: {0}", target);

var generatorSettings = new JsonSchemaGeneratorSettings()
{
    SchemaType = SchemaType.JsonSchema,
    SerializerSettings = new JsonSerializerSettings()
    {
        Converters =
        {
            new StringEnumConverter()
        }
    }
};
var generator = new JsonSchemaGenerator(generatorSettings);
var schema = generator.Generate(typeof(TemplateFile));

File.WriteAllTextAsync(target, schema.ToJson(Formatting.Indented));

Console.WriteLine("Generated schema for template file at {0}", target);
