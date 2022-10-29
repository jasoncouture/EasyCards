using Microsoft.CodeAnalysis;

namespace EasyCards.EnumGenerator
{
    [Generator]
    public class EnumNamespaceAttributeGenerator : ISourceGenerator
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

        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context)
        {
            EnumNamespaceAttribute.ToSyntaxBuilder()
                .WithClassName(nameof(EnumNamespaceAttribute))
                .FinalizeDeclaration(context.Compilation.AssemblyName, "System")
                .AddSource(context);
            EnumPrefixAttribute.ToSyntaxBuilder()
                .WithClassName(nameof(EnumPrefixAttribute))
                .FinalizeDeclaration(context.Compilation.AssemblyName, "System")
                .AddSource(context);
            EnumPostfixAttribute.ToSyntaxBuilder()
                .WithClassName(nameof(EnumPostfixAttribute))
                .FinalizeDeclaration(context.Compilation.AssemblyName, "System")
                .AddSource(context);
        }
    }
}
