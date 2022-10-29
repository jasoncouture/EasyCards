namespace EasyCards.Validation;

public sealed class NoOpValidator : IValidator
{
    public ResultOrError<T> TryValidate<T>(T item)
    {
        return item;
    }
}
