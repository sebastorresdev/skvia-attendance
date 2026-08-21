using ErrorOr;
using Microsoft.AspNetCore.Identity;

namespace Skvia.Erp.Infrastructure.Services;

public static class IdentityResultExtensions
{
    public static List<Error> ToApplicationError(this IdentityResult result)
    {
        return [.. result.Errors
            .Select(error => Error.Validation(
                code: error.Code,
                description: error.Description))];
    }
}


