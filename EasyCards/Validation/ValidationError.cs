namespace EasyCards.Validation;

public sealed record ValidationError(string Message, bool Fatal = true)
{
    public static implicit operator ValidationError(string message)
    {
        return new ValidationError(message);
    }
}
