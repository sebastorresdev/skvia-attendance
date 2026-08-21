using ErrorOr;
namespace Skvia.Erp.Domain.Employees;

public static class EmployeeErrors
{
    public static Error CodeExists(string code) => Error.Conflict(
        code: "Employee.CodeExists",
        description: $"El código de empleado '{code}' ya se encuentra registrado.");

    public static Error DocumentExists(string documentNumber) => Error.Conflict(
        code: "Employee.DocumentExists",
        description: $"El documento de identidad '{documentNumber}' ya está asignado a otro empleado.");

    public static Error NotFound => Error.NotFound(
        code: "Employee.NotFound",
        description: "El empleado no se encuentra registrado.");

    public static Error UserAlreadyLinked => Error.Conflict(
        code: "Employee.UserAlreadyLinked",
        description: "El usuario seleccionado ya se encuentra vinculado a otro empleado.");
}


