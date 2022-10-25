using System.Collections.Generic;
using EasyCards.EnumGenerator.EnumScannerModels;
using Microsoft.CodeAnalysis;

namespace EasyCards.EnumGenerator
{
    [Generator]
    public class EnumExtensionsGenerator : ISourceGenerator 
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            const string extensionMethodClassName = "GeneratedEnumExtensions";
            var targetNamespace = context.Compilation.Assembly.GetAttributeValue("EnumNamespace") ??
                                  context.Compilation.AssemblyName;
            var enumNamePrefix = context.Compilation.Assembly.GetAttributeValue("EnumPrefix") ?? string.Empty;
            var enumNamePostfix = context.Compilation.Assembly.GetAttributeValue("EnumPostfix") ?? string.Empty;
            
            var visibilityModifier = "public";
            var declaration = new SyntaxBuilder();

            declaration.BeginBlock($"{visibilityModifier} static class {extensionMethodClassName}");

            foreach (var enumDefinition in EnumScanResultsLoader.GetEnumDefinitions().Enums)
            {
                declaration = DefineEnumExtensionMethod(enumDefinition, enumNamePrefix, enumNamePostfix, declaration);
            }

            declaration.WithClassName(extensionMethodClassName)
                .FinalizeDeclaration(targetNamespace, "System")
                .AddSource(context);
        }
        
        private SyntaxBuilder DefineEnumExtensionMethod(EnumDefinition enumDefinition,
            string enumNamePrefix, string enumNamePostfix, SyntaxBuilder declaration)
        {
            var namespaces = new HashSet<string>();
            namespaces.Add("System");

            
            var enumTargetName = $"{enumNamePrefix}{enumDefinition.Name}{enumNamePostfix}";
            declaration
                .BeginBlock($"public static T CastTo<T>(this {enumTargetName} val) where T : struct")
                .BeginBlock("if (!typeof(T).IsEnum)")
                .EndBlock("throw new ArgumentException(\"Type argument must be an enum\", nameof(T));")
                .AppendLine($"return (T)(object)({enumDefinition.BaseType})val;")
                .EndBlock();

            return declaration;
        }
    }
}