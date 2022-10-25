using Microsoft.CodeAnalysis;

namespace EasyCards.EnumGenerator
{
    [Generator]
    public class EnumNamespaceAttributeGenerator : DeclarationBuilderSourceGenerator
    {
        // The attribute does not need to store the parameter, we're going to read this from the compiler.
        private const string EnumNamespaceAttribute = @"
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
internal class EnumNamespaceAttribute : Attribute 
{
    public EnumNamespaceAttribute(string name) { }
}
";

        private const string EnumPrefixAttribute = @"
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
internal class EnumPrefixAttribute : Attribute 
{
    public EnumPrefixAttribute(string prefix) { }
}
";

        private const string EnumPostfixAttribute = @"
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
internal class EnumPostfixAttribute : Attribute 
{
    public EnumPostfixAttribute(string prefix) { }
}
";

        public override void Execute(GeneratorExecutionContext context)
        {
            CreateSourceContentInDefaultNamespace(context, nameof(EnumNamespaceAttribute), EnumNamespaceAttribute,
                "System");
            CreateSourceContentInDefaultNamespace(context, nameof(EnumPrefixAttribute), EnumPrefixAttribute, "System");
            CreateSourceContentInDefaultNamespace(context, nameof(EnumPostfixAttribute), EnumPostfixAttribute,
                "System");
        }
    }
}