using System.Collections.Generic;
using FastFood.Services.DTO.Employee;

namespace FastFood.Services.Interfaces
{
    public interface IEmployeeService
    {
        void Register(RegisterEmployeeDTO dto);

        ICollection<ListAllEmployeesDTO> All();
    }
}
