using Skvia.Attendance.Domain.Employees;


namespace Skvia.Attendance.Application.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<CreateEmployeeCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> HandleAsync(CreateEmployeeCommand command, CancellationToken cancellationToken)
    {
        var normalizedCode = command.Code.Trim().ToUpperInvariant();

        if (await dbContext.Employees.AnyAsync(e => e.Code == normalizedCode, cancellationToken))
        {
            return EmployeeErrors.CodeExists(command.Code);
        }

        var document = command.DocumentNumber.Trim();
        
        if (await dbContext.Employees.AnyAsync(e => e.DocumentNumber == document, cancellationToken))
        {
            return EmployeeErrors.DocumentExists(command.DocumentNumber);
        }

        var employee = Employee.Create(
            code: command.Code,
            firstName: command.FirstName,
            lastName: command.LastName,
            documentType: (DocumentType)command.DocumentType,
            documentNumber: command.DocumentNumber,
            hireDate: command.HireDate,
            email: command.Email,
            phone: command.Phone,
            position: command.Position,
            department: command.Department,
            photoUrl: command.PhotoUrl);
        
        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync(cancellationToken);

        return employee.Id;
    }
}
