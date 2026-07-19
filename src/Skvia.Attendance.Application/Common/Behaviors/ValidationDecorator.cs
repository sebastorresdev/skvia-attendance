using FluentValidation;
using FluentValidation.Results;

namespace Skvia.Attendance.Application.Common.Behaviors;

internal static class ValidationDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
        where TResponse : IErrorOr
    {
        public async Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            var failures = await ValidateAsync(command, validators);

            if (failures.Count == 0)
            {
                return await innerHandler.HandleAsync(command, cancellationToken);
            }

            var errors = failures.ConvertAll(error => Error.Validation(
                code: error.PropertyName,
                description: error.ErrorMessage));

            return (dynamic)errors;
        }
    }

    private static async Task<List<ValidationFailure>> ValidateAsync<TCommand>(
        TCommand command,
        IEnumerable<IValidator<TCommand>> validators)
    {
        if (!validators.Any()) return [];

        var context = new ValidationContext<TCommand>(command);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context)));

        return [.. validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)];
    }
}
