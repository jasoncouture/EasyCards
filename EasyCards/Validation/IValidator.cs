namespace EasyCards.Validation;

public interface IValidator
{
    public ResultOrError<T> TryValidate<T>(T item);
}
