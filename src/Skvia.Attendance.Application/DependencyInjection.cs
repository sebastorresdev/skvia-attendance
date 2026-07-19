using FluentValidation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Skvia.Attendance.Application.Common.Behaviors;

using System.Reflection;

namespace Skvia.Attendance.Application;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();

        builder.Services.Scan(scan => scan
            // 1. Apuntamos al ensamblado actual usando la variable 'assembly'
            .FromAssemblies(assembly)
            // 1. Queries con retorno
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            // 3. Commands con retorno
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        // 1. Capa de Validación (Se ejecuta primero)
        builder.Services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));

        // 2. Capa de Logs (Envuelve a la validación para registrarlo todo)
        builder.Services.TryDecorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
        builder.Services.TryDecorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));

        // FluentValidation: Registrar todos los validadores en el ensamblado actual, incluyendo tipos internos
        builder.Services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        return builder;
    }
}
