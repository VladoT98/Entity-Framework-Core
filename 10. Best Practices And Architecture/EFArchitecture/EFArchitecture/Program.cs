using System;
using System.Threading.Tasks;
using EFArchitecture.Core.Services;
using EFArchitecture.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EFArchitecture
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<EFArchitectureDbContext>(options =>
                    options.UseSqlServer(
                        @"Server=VLADOTONCHEV\SQLEXPRESS;Database=PetStoreWebApp;Integrated Security=True;"))
                //.AddScoped<IEFArchitectureRepository, EFArchitectureRepository>()
                //.AddScoped<IEmployeeService, EmployeeService>()
                .BuildServiceProvider();

            var employeeService = serviceProvider.GetService<EmployeeService>();
            var employee = await employeeService.GetEmployeeWithHighestSalary();

            Console.WriteLine($"{employee.FirstName} {employee.LastName}: {employee.Salary:F2}");
        }
    }
}
