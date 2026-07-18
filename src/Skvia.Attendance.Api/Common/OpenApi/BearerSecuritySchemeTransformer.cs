using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

using Skvia.Attendance.Api.Common.Constants;

namespace Skvia.Attendance.Api.Common.OpenApi;

internal sealed class BearerSecuritySchemeTransformer(
        IAuthenticationSchemeProvider authenticationSchemeProvider)
            : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        IEnumerable<AuthenticationScheme> authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();

        if (authenticationSchemes.Any(authScheme => authScheme.Name == AuthScheme.Bearer))
        {
            var securitySchemes = new Dictionary<string, IOpenApiSecurityScheme>
            {
                [AuthScheme.Bearer] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Name = "Authorization",
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}.",
                    BearerFormat = "Json Web Token",
                },
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = securitySchemes;
        }
    }
}
