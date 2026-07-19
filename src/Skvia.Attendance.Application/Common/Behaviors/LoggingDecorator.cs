using Microsoft.Extensions.Logging;

using Serilog.Context;

namespace Skvia.Attendance.Application.Common.Behaviors;

internal static class LoggingDecorator
{
    // =========================================================================
    // 🚀 1. DECORADOR PARA TODOS LOS COMANDOS (Commands)
    // =========================================================================
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
        where TResponse : IErrorOr
    {
        public async Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            logger.LogInformation("Processing command {Command}", commandName);

            // Sincronizado con tu método HandleAsync nativo
            TResponse result = await innerHandler.HandleAsync(command, cancellationToken);

            if (!result.IsError) // Cambiado a las propiedades de IErrorOr
            {
                logger.LogInformation("Completed command {Command}", commandName);
            }
            else
            {
                // Extraemos la lista de errores para inyectarla enriquecida en Serilog
                using (LogContext.PushProperty("Errors", result.Errors, true))
                {
                    logger.LogError("Completed command {Command} with error(s)", commandName);
                }
            }

            return result;
        }
    }

    // =========================================================================
    // 🔍 2. DECORADOR PARA TODAS LAS CONSULTAS (Queries)
    // =========================================================================
    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
        where TResponse : IErrorOr
    {
        public async Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken)
        {
            string queryName = typeof(TQuery).Name;

            logger.LogInformation("Processing query {Query}", queryName);

            // Sincronizado con tu método HandleAsync nativo
            TResponse result = await innerHandler.HandleAsync(query, cancellationToken);

            if (!result.IsError)
            {
                logger.LogInformation("Completed query {Query}", queryName);
            }
            else
            {
                using (LogContext.PushProperty("Errors", result.Errors, true))
                {
                    logger.LogError("Completed query {Query} with error(s)", queryName);
                }
            }

            return result;
        }
    }
}
