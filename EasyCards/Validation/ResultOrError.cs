using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EasyCards.Validation;

public record struct ResultOrError<T>(T? Result, ImmutableArray<ValidationError> Errors = default)
{
    private readonly T? _result = Result;

    public ResultOrError(T? result, params ValidationError[] errors) : this(result, errors.ToImmutableArray())
    {
    }

    public ResultOrError(params ValidationError[] errors) : this(default, errors.ToImmutableArray()) 
    {
    }


    public T? Result
    {
        readonly get
        {
            if (HasResult) return _result;
            return _result;
        }
        init => _result = value;
    }

    public readonly bool HasResult => _result is not null && (Errors.IsDefaultOrEmpty || Errors.All(i => !i.Fatal));

    public static implicit operator ResultOrError<T>(T result) => new(result);
    public static implicit operator ResultOrError<T>(ImmutableArray<ValidationError> errors) => new(default(T), errors);
    public static implicit operator ResultOrError<T>(ValidationError[] errors) => new(default(T), errors.ToImmutableArray());
    public static implicit operator ResultOrError<T>(List<ValidationError> errors) => new(default(T), errors.ToImmutableArray());
    public static implicit operator ResultOrError<T>(ValidationError error) => new (default(T), new[] { error }.ToImmutableArray());

    public static ResultOrError<T> Merge(params ResultOrError<T>[] toMerge)
    {
        var errorSet = toMerge.SelectMany(i => i.Errors).ToImmutableArray();
        var value = toMerge.Select(i => i.Result).LastOrDefault(i => i is not null);

        return new ResultOrError<T>(value, errorSet);
    }
    public readonly ImmutableArray<ValidationError> Errors { get; init; } = Errors;

    public static explicit operator T(ResultOrError<T?> resultOrError)
    {
        return resultOrError.Result!;
    }
}