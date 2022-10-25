namespace EasyCards.Models.Templates;

public class TemplateFile
{
    public string ModSource { get; set; } = string.Empty;
    public List<CardTemplate> Stats { get; set; } = new();
}