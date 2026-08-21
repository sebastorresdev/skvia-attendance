using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Common.Security;
using ErrorOr;
using System.Reflection;

using Skvia.Erp.Application.Common.Messaging;

namespace Skvia.Erp.Application.Common.Behaviors;

public class AuthorizationDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> innerHandler,
    ICurrentUserProvider currentUserProvider) 
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : IErrorOr
{
    public async Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        var authorizeAttributes = command.GetType().GetCustomAttributes<AuthorizeCommandAttribute>();

        if (authorizeAttributes.Any())
        {
            var currentUser = currentUserProvider.GetCurrentUser();

            foreach (var attribute in authorizeAttributes)
            {
                if (!string.IsNullOrWhiteSpace(attribute.Roles))
                {
                    var requiredRoles = attribute.Roles.Split(',').Select(r => r.Trim());
                    var hasRole = currentUser.Roles.Any(r => requiredRoles.Contains(r));
                    if (!hasRole)
                    {
                        return (TResponse)(dynamic)Error.Forbidden(description: "No tienes el rol requerido para ejecutar esta acción.");
                    }
                }

                if (!string.IsNullOrWhiteSpace(attribute.Permissions))
                {
                    var requiredPermissions = attribute.Permissions.Split(',').Select(p => p.Trim());
                    var hasPermission = currentUser.Permissions.Any(p => requiredPermissions.Contains(p));
                    if (!hasPermission)
                    {
                        return (TResponse)(dynamic)Error.Forbidden(description: "No tienes los permisos requeridos para ejecutar esta acción.");
                    }
                }
            }
        }

        return await innerHandler.HandleAsync(command, cancellationToken);
    }
}

public class AuthorizationQueryDecorator<TQuery, TResponse>(
    IQueryHandler<TQuery, TResponse> innerHandler,
    ICurrentUserProvider currentUserProvider) 
    : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : IErrorOr
{
    public async Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken)
    {
        var authorizeAttributes = query.GetType().GetCustomAttributes<AuthorizeCommandAttribute>();

        if (authorizeAttributes.Any())
        {
            var currentUser = currentUserProvider.GetCurrentUser();

            foreach (var attribute in authorizeAttributes)
            {
                if (!string.IsNullOrWhiteSpace(attribute.Roles))
                {
                    var requiredRoles = attribute.Roles.Split(',').Select(r => r.Trim());
                    var hasRole = currentUser.Roles.Any(r => requiredRoles.Contains(r));
                    if (!hasRole)
                    {
                        return (TResponse)(dynamic)Error.Forbidden(description: "No tienes el rol requerido para ejecutar esta acción.");
                    }
                }

                if (!string.IsNullOrWhiteSpace(attribute.Permissions))
                {
                    var requiredPermissions = attribute.Permissions.Split(',').Select(p => p.Trim());
                    var hasPermission = currentUser.Permissions.Any(p => requiredPermissions.Contains(p));
                    if (!hasPermission)
                    {
                        return (TResponse)(dynamic)Error.Forbidden(description: "No tienes los permisos requeridos para ejecutar esta acción.");
                    }
                }
            }
        }

        return await innerHandler.HandleAsync(query, cancellationToken);
    }
}

