//#define DEBUG_ENUM_GENERATOR // This will deadlock the build until the debugger is attached to the compiler, you have been warned.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EasyCards.EnumGenerator.EnumScannerModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
#if DEBUG && DEBUG_ENUM_GENERATOR
using System.Diagnostics;
using System.Threading;
#endif


namespace EasyCards.EnumGenerator
{
    [Generator]
    public class EnumGenerator : DeclarationBuilderSourceGenerator
    {
        static EnumGenerator()
        {
#if DEBUG && DEBUG_ENUM_GENERATOR
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
                SpinWait.SpinUntil(() => Debugger.IsAttached);
            }

            Debugger.Break();
#endif
            EnumBaseTypeLookup = typeof(int).Assembly.GetTypes()
                .Where(i => i.IsPrimitive && (i.Name.ToLower().Contains("int") || i.Name.ToLower().Contains("byte")))
                .ToDictionary(i => i.Name, i => i, StringComparer.OrdinalIgnoreCase);
            // This line gives us: MakeString<>, which we can then use to build the dictionary using types.
            var methodHandle = typeof(EnumGenerator).GetMethods()
                .Single(i => i.IsStatic && i.IsGenericMethod && i.Name.StartsWith(nameof(MakeString)))
                .GetGenericMethodDefinition();
            EnumBaseTypeFormatter = EnumBaseTypeLookup.Values
                .Select(i => new { i.Name, method = methodHandle.MakeGenericMethod(i) })
                .ToDictionary(i => i.Name, i => CreateFunctionPointer(i.method),
                    StringComparer.OrdinalIgnoreCase);
        }

        private static Func<ulong, string> CreateFunctionPointer(MethodInfo methodInfo)
        {
            // We need this conversion, because some enums may have negative values, which we can't represent with an unsigned.
            // We also can't guarantee that the enum value will fit within an int64 either.
            // ex: enum SomeEnum : ulong { Max = ulong.MaxValue }
            var input = Expression.Parameter(typeof(ulong), "i");
            return Expression.Lambda<Func<ulong, string>>(
                Expression.Call(null, methodInfo, input), input).Compile();
        }

        public static string MakeString<T>(ulong arg) where T : struct
        {
            return Convert.ChangeType(arg, typeof(T)).ToString();
        }

        private static readonly Dictionary<string, Func<ulong, string>> EnumBaseTypeFormatter;
        private static readonly Dictionary<string, Type> EnumBaseTypeLookup;

        public override void Execute(GeneratorExecutionContext context)
        {
            var targetNamespace = GeneratorExecutionContextExtensions.GetAttributeValue(context.Compilation.Assembly, "EnumNamespace") ??
                                  context.Compilation.AssemblyName;
            var enumNamePrefix = GeneratorExecutionContextExtensions.GetAttributeValue(context.Compilation.Assembly, "EnumPrefix") ?? string.Empty;
            var enumNamePostfix = GeneratorExecutionContextExtensions.GetAttributeValue(context.Compilation.Assembly, "EnumPostfix") ?? string.Empty;
            var visibilityModifier = "public";

            foreach (var enumDefinition in EnumScanResultsLoader.GetEnumDefinitions().Enums)
            {
                DefineEnum(context, enumDefinition, enumNamePrefix, enumNamePostfix, visibilityModifier,
                    targetNamespace);
                DefineEnumExtension(context, enumDefinition, enumNamePrefix, enumNamePostfix, visibilityModifier,
                    targetNamespace);
            }
        }


        private void DefineEnumExtension(GeneratorExecutionContext context, EnumDefinition enumDefinition,
            string enumNamePrefix, string enumNamePostfix, string visibilityModifier, string targetNamespace)
        {
            var namespaces = new HashSet<string>();
            namespaces.Add("System");

            var declaration = new SyntaxBuilder();
            var enumTargetName = $"{enumNamePrefix}{enumDefinition.Name}{enumNamePostfix}";
            var className = $"{enumTargetName}Extensions";
            declaration
                .BeginBlock($"{visibilityModifier} static class {className}")
                .BeginBlock($"public static T CastTo<T>(this {enumTargetName} val) where T : struct")
                .BeginBlock("if (!typeof(T).IsEnum)")
                .EndBlock("throw new ArgumentException(\"Type argument must be an enum\", nameof(T));")
                .AppendLine($"return (T)Convert.ChangeType(val, typeof(T));")
                .EndAllBlocks();

            CreateSourceContent(context, className, targetNamespace, declaration.ToString(), namespaces.ToArray());
        }

        private void DefineEnum(GeneratorExecutionContext context, EnumDefinition enumDefinition, string enumNamePrefix,
            string enumNamePostfix, string visibilityModifier, string targetNamespace)
        {
            if (string.IsNullOrWhiteSpace(enumDefinition.BaseType) ||
                !EnumBaseTypeLookup.TryGetValue(enumDefinition.BaseType, out var baseType) ||
                !EnumBaseTypeFormatter.TryGetValue(enumDefinition.BaseType, out var formatter)
               )
                // TODO: Add diagnostic to compiler log, explaining why we skipped this one.
                return;
            var namespaces = new HashSet<string>();
            namespaces.Add("System");

            var declaration = new SyntaxBuilder();
            if (enumDefinition.Flags)
            {
                declaration.AppendLine("[Flags]");
            }

            var enumTargetName = $"{enumNamePrefix}{enumDefinition.Name}{enumNamePostfix}";
            declaration.BeginBlock($"{visibilityModifier} enum {enumTargetName} : {baseType.Name}");
            bool first = true;
            foreach (var enumMember in enumDefinition.Members)
            {
                if (!first)
                    declaration.AppendLine(",");
                else
                    first = false;
                declaration.Append(enumMember.Name).Append(" = ").Append(formatter.Invoke(enumMember.Value));
            }

            declaration.AppendLine().EndAllBlocks();

            CreateSourceContent(context, enumTargetName, targetNamespace, declaration.ToString(), namespaces.ToArray());
        }

    }
}