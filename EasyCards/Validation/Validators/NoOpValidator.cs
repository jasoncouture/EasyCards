namespace EasyCards.Validation;

public class NoOpValidator : IValidator
{
    public ResultOrError<T> TryValidate<T>(T item)
    {
        return item;
    }
}