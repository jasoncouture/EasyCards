namespace EasyCards.Models.Templates;

public class StatRequirement
{
    public string Name { get; set; } = string.Empty;
    public float Value { get; set; }

    public override string ToString()
    {
        return $"{nameof(Name)}: {Name}, {nameof(Value)}: {Value}";
    }
}