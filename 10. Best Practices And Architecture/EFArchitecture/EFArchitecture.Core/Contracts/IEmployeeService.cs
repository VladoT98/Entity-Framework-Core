using EFArchitecture.Core.DTOs;

namespace EFArchitecture.Core.Contracts
{
    public interface IEmployeeService
    {
        EmployeeDto GetEmployeeWithHighestSalary();
    }
}
