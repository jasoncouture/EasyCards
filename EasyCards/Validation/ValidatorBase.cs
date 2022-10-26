namespace EasyCards.Validation;

public abstract class ValidatorBase<T> : IValidator
{
    public ResultOrError<T1> TryValidate<T1>(T1 item)
    {
        if (typeof(T1) == typeof(T))
        {
            return (ResultOrError<T1>)(object)Validate(((T)(object)item!));
        }

        return item;
    }

    protected abstract ResultOrError<T> Validate(T item);
}