using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasyCards.EnumGenerator
{
    public static class GeneratorExecutionContextExtensions
    {
        public static string GetAttributeValue(this ISymbol symbol, string name)
        {
            var attributes = symbol.GetAttributes();
            var attribute = attributes.FirstOrDefault(i => string.Equals(i.AttributeClass?.MetadataName ?? string.Empty,
                                name, StringComparison.OrdinalIgnoreCase))
                            ?? attributes.FirstOrDefault(i =>
                                string.Equals(i.AttributeClass?.MetadataName ?? string.Empty, $"{name}Attribute",
                                    StringComparison.OrdinalIgnoreCase));


            // We should visit the desecendent nodes instead, otherwise this will only work when the attribute parameter
            // is declared as an inline string literal (IE: [Attribute("Value")])
            return attribute?.ApplicationSyntaxReference?.GetSyntax()
                .DescendantNodes()
                .OfType<LiteralExpressionSyntax>()
                .Select(i => i.Token.ValueText)
                .FirstOrDefault();
        }
    }
}