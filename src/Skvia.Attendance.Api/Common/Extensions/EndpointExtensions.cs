using System.Reflection;

using Skvia.Attendance.Api.Endpoints;

namespace Skvia.Attendance.Api.Common.Extensions;

public static class EndpointExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app, Assembly assembly)
    {
        var endpointTypes = assembly.DefinedTypes
            .Where(t => t is { IsAbstract: false, IsInterface: false }
                        && t.IsAssignableTo(typeof(IEndpoint)));

        // 🌟 Agrupamos de forma segura usando el último segmento del Namespace
        var grouped = endpointTypes.GroupBy(t =>
        {
            var ns = t.Namespace ?? "";
            var segments = ns.Split('.');

            // Retorna el último segmento (ej: "Auth", "Branches") convertido a minúsculas
            return segments[^1].ToLower();
        });

        foreach (var group in grouped)
        {
            // Esto creará correctamente: /api/auth, /api/branches, etc.
            var routeGroup = app.MapGroup($"/api/{group.Key}")
                .WithTags(group.Key);

            foreach (var type in group)
            {
                var method = type.GetMethod("Map");
                method?.Invoke(null, [routeGroup]);
            }
        }

        return app;
    }

    // public static RouteHandlerBuilder WithPermission(this RouteHandlerBuilder app, string permission)
    // {
    //     return app.RequireAuthorization(permission)
    //         .ProducesProblemForbidden();
    // }
}
