using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace EasyCards.Validation;

public class ValidationException : Exception
{
    public ImmutableArray<ValidationError> Errors { get; }
    public ValidationException(IEnumerable<ValidationError> validationErrors)
        : base("One or more fatal validation errors occurred, see the Errors property for more information.")
    {
        Errors = validationErrors.ToImmutableArray();
    }
}