using FluentValidation;
using MediatR;

namespace AuthService.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .Select(failure => failure.ErrorMessage)
            .Distinct()
            .ToArray();

        if (failures.Length == 0)
        {
            return await next();
        }

        return CreateFailureResponse(string.Join(" ", failures));
    }

    private static TResponse CreateFailureResponse(string error)
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }

        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = typeof(TResponse).GetGenericArguments()[0];
            var resultType = typeof(Result<>).MakeGenericType(valueType);
            var failureMethod = resultType.GetMethod(nameof(Result.Failure), [typeof(string)])
                ?? throw new InvalidOperationException("Result failure factory was not found.");

            return (TResponse)failureMethod.Invoke(null, [error])!;
        }

        throw new InvalidOperationException(
            $"Validation behavior cannot create a failure response for '{typeof(TResponse).Name}'.");
    }
}
