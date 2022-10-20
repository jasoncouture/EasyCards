using System.Collections.Generic;

namespace EasyCards.Templates;

public class TemplateFile
{
    public string ModSource { get; set; }
    public List<CardTemplate> Stats { get; set; } = new();
}