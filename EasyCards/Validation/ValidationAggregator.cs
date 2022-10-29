namespace EasyCards.Validation;

public sealed class ValidationAggregator : IValidationAggregator
{
    private readonly IValidator[] _validators;

    public ValidationAggregator(IValidator[] validators)
    {
        _validators = validators;
    }
    public ResultOrError<T> TryValidate<T>(T item)
    {
        ResultOrError<T> finalResult = default;

        foreach (var validator in _validators)
        {
            var result = validator.TryValidate(item);
            if (result.HasResult && finalResult.HasResult && result.Errors.Length == 0)
                continue;
            finalResult = ResultOrError<T>.Merge(finalResult, result);
        }

        return finalResult;
    }
}
