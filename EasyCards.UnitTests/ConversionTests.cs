using EasyCards.Models.Templates.Generated;
using RogueGenesia.Data;
using Xunit;

namespace EasyCards.UnitTests;

public sealed class ConversionTests
{
    [Fact]
    public void TemplateModifierTypeCanCastToModifierType()
    {
        var expected = ModifierType.Additional;
        var modifierType = TemplateModifierType.Additional;

        var result = modifierType.CastTo<ModifierType>();
        Assert.Equal(expected, result);
    }
}
