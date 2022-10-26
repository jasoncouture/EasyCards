using Microsoft.Extensions.Logging;

namespace EasyCards.Validation.Extensions;

public static class ResultOrErrorExtensions
{
    public static bool LogValidationErrors<T>(this ILogger logger, ResultOrError<T> resultOrError)
    {
        var isValid = resultOrError.HasResult;
        if (!logger.IsEnabled(LogLevel.Error) && !logger.IsEnabled(LogLevel.Warning)) return isValid;
        foreach (var error in resultOrError.Errors)
        {
            logger.LogValidationError(error);
        }

        return isValid;
    }

    public static void LogValidationError(this ILogger logger, ValidationError validationError)
    {
        if(validationError.Fatal)
            logger.LogValidationErrorAsError(validationError);
        else
            logger.LogValidationErrorAsWarning(validationError);
    }

    public static void LogValidationErrorAsWarning(this ILogger logger, ValidationError validationError)
    {
        logger.LogWarning("Non-fatal validation error: {message}", validationError.Message);
    }

    public static void LogValidationErrorAsError(this ILogger logger, ValidationError validationError)
    {
        logger.LogError("Fatal validation error: {message}", validationError.Message);
    }
}