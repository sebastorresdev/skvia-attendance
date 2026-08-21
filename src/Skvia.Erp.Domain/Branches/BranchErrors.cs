using ErrorOr;

namespace Skvia.Erp.Domain.Branches;

public static class BranchErrors
{
    public static Error DuplicateBranch(string code) =>
        Error.Conflict(
            code: "Branch.DuplicateBranch",
            description: $"El Codigo de la sede '{code}' ya está en uso.");

    public static Error NotFound =>
        Error.NotFound(
            code: "Branch.NotFound",
            description: $"Sede no encontrada.");
}

