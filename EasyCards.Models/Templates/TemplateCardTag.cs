namespace EasyCards.Models.Templates;

[Flags]
public enum TemplateCardTag
{
    None = 0,
    Order = 1,
    Critical = 2,
    Defence = 4,
    Body = 8,
    Might = 16, // 0x00000010
    Evolution = 32, // 0x00000020
}