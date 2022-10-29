//#define DEBUG_ENUM_GENERATOR // This will deadlock the build until the debugger is attached to the compiler, you have been warned.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EasyCards.EnumGenerator.EnumScannerModels;
using Microsoft.CodeAnalysis;
#if DEBUG && DEBUG_ENUM_GENERATOR
using System.Diagnostics;
using System.Threading;
#endif


namespace EasyCards.EnumGenerator
{
    [Generator]
    public class EnumGenerator : ISourceGenerator
    {
        static readonly DiagnosticDescriptor CreatedEnumMessageDescriptor = new DiagnosticDescriptor("EASYCARDS0000", "Easy Cards Generation Message", "Successfully generated enum {0}", "EasyCards", DiagnosticSeverity.Info, true);
        static readonly DiagnosticDescriptor EnumCreateFailedDiagnosticWarningDescriptor = new DiagnosticDescriptor("EASYCARDS0001", "Enum Generation Failed", "Enum generation for {0} failed: {1}", "EasyCards", DiagnosticSeverity.Warning, true);
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

        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context)
        {
            var targetNamespace = context.Compilation.Assembly.GetAttributeValue("EnumNamespace") ??
                                  context.Compilation.AssemblyName;
            var enumNamePrefix = context.Compilation.Assembly.GetAttributeValue("EnumPrefix") ?? string.Empty;
            var enumNamePostfix = context.Compilation.Assembly.GetAttributeValue("EnumPostfix") ?? string.Empty;
            var visibilityModifier = "public";

            foreach (var enumDefinition in EnumScanResultsLoader.GetEnumDefinitions().Enums)
            {
                var enumTargetName = $"{enumNamePrefix}{enumDefinition.Name}{enumNamePostfix}";
                var builder = DefineEnum(context, enumDefinition, enumTargetName, visibilityModifier);
                builder?.WithClassName(enumTargetName)
                    .FinalizeDeclaration(targetNamespace, "System")
                    .AddSource(context);
                ;
            }
        }

        private SyntaxBuilder DefineEnum(GeneratorExecutionContext context, EnumDefinition enumDefinition, string enumTargetName,
            string visibilityModifier)
        {
            if (string.IsNullOrWhiteSpace(enumDefinition.BaseType) ||
                !EnumBaseTypeLookup.TryGetValue(enumDefinition.BaseType, out var baseType) ||
                !EnumBaseTypeFormatter.TryGetValue(enumDefinition.BaseType, out var formatter)
               )
            {
                context.ReportDiagnostic(Diagnostic.Create(EnumCreateFailedDiagnosticWarningDescriptor, Location.None, $"{enumTargetName} ({enumDefinition.Name})", $"Unable to find base type {enumDefinition.BaseType}"));
                return null;
            }

            var declaration = new SyntaxBuilder();
            if (enumDefinition.Flags)
            {
                declaration.AppendLine("[Flags]");
            }

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
            context.ReportDiagnostic(Diagnostic.Create(CreatedEnumMessageDescriptor, Location.None, $"{enumTargetName} ({enumDefinition.Name})"));
            return declaration.AppendLine();
        }
    }
}
