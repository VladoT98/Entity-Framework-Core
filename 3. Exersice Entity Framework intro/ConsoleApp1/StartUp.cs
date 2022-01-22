using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using SoftUni.Data;
using SoftUni.Models;

namespace SoftUni
{
    using System;
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();
            string result = RemoveTown(context);

            Console.WriteLine(result);
        }

        //3. Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context
                .Employees
                .OrderBy(e => e.EmployeeId)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }


        //4. Employees with salary over 50 000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context
                .Employees
                .Where(employee => employee.Salary > 50000)
                .OrderBy(employee => employee.FirstName)
                .Select(employee => new
                {
                    employee.FirstName,
                    employee.Salary
                })
                .ToArray();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }


        //5. Employees from Research Department
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context
                .Employees
                .Where(e => e.Department.Name == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    DepartmentName = e.Department.Name,
                    e.Salary
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }


        //6. Adding a New Address and Updating Emoployee
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address newAddress = new Address();
            newAddress.TownId = 4;
            newAddress.AddressText = "Vitoshka 15";
            context.Addresses.Add(newAddress);

            Employee nakovEmployee = context
                .Employees
                .FirstOrDefault(e => e.LastName == "Nakov");
            nakovEmployee.Address = newAddress;

            context.SaveChanges();

            string[] employeesAddresses = context
                .Employees
                .OrderByDescending(e => e.AddressId)
                .Select(e => e.Address.AddressText)
                .Take(10)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            return string.Join(Environment.NewLine, employeesAddresses);
        }


        //7. Employees and Projects
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context
                .Employees
                .Where(e => e.EmployeesProjects.Any(ep =>
                    ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager.FirstName,
                    ManagerLastName = e.Manager.LastName,
                    Projects = e.EmployeesProjects.Select(e => e.Project)
                })
                .Take(10)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");

                foreach (var project in e.Projects)
                {
                    if (project.EndDate == null)
                    {
                        sb.AppendLine($"--{project.Name:M/d/yyyy h:mm:ss tt} - {project.StartDate:M/d/yyyy h:mm:ss tt} - not finished");
                        continue;
                    }
                    sb.AppendLine($"--{project.Name} - {project.StartDate:M/d/yyyy h:mm:ss tt} - {project.EndDate:M/d/yyyy h:mm:ss tt}");
                }
            }

            return sb.ToString().TrimEnd();
        }


        //8. Addresses by Town
        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context
                .Addresses
                .OrderByDescending(a => a.Employees.Count)
                .ThenBy(a => a.Town.Name)
                .ThenBy(a => a.AddressText)
                .Select(a => new
                {
                    a.AddressText,
                    TownName = a.Town.Name,
                    EmployeeCount = a.Employees.Count
                })
                .Take(10)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var a in addresses)
            {
                sb.AppendLine($"{a.AddressText}, {a.TownName} - {a.EmployeeCount} employees");
            }

            return sb.ToString().TrimEnd();
        }


        //9. Employee 147
        public static string GetEmployee147(SoftUniContext context)
        {
            Employee employee = context.Employees.Find(147);

            Project[] projects = context
                .Projects
                .Where(p => p.EmployeesProjects.Any(ep => ep.EmployeeId == 147))
                .OrderBy(p => p.Name)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

            foreach (var p in projects)
            {
                sb.AppendLine($"{p.Name}");
            }

            return sb.ToString().TrimEnd();
        }


        //10. Departments with More Than 5 Employees
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context
                .Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    DepartmentName = d.Name,
                    ManagerFirstName = d.Manager.FirstName,
                    ManagerlastName = d.Manager.LastName
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var d in departments)
            {
                sb.AppendLine($"{d.DepartmentName} - {d.ManagerFirstName} {d.ManagerlastName}");

                Employee[] employees = context
                    .Employees
                    .Where(e => e.Department.Name == d.DepartmentName)
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .ToArray();

                foreach (var e in employees)
                {
                    sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }


        //11. Find Latest 10 Projects
        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context
                .Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .OrderBy(p => p.Name)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    p.StartDate
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var p in projects)
            {
                sb.AppendLine($"{p.Name}");
                sb.AppendLine($"{p.Description}");
                sb.AppendLine($"{p.StartDate:M/d/yyyy h:mm:ss tt}");
            }

            return sb.ToString().TrimEnd();
        }


        //12. Increase Salaries
        public static string IncreaseSalaries(SoftUniContext context)
        {
            Employee[] employees = context
                .Employees
                .Where(e => e.Department.Name == "Engineering" ||
                                   e.Department.Name == "Tool Design" ||
                                   e.Department.Name == "Marketing" ||
                                   e.Department.Name == "Information Services")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToArray();

            foreach (var e in employees)
            {
                e.Salary += e.Salary * 0.12m;
            }

            context.SaveChanges();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }


        //13. Find Employees by First Name Starting with "Sa"
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context
                .Employees
                .Where(e => e.FirstName.StartsWith("Sa"))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }


        //14. Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
            var projectsDelete = context.Projects.Find(2);
            var employeesProjectsDelete = context
                .EmployeesProjects
                .Where(ep => ep.ProjectId == 2);

            foreach (var ep in employeesProjectsDelete)
            {
                context.EmployeesProjects.Remove(ep);
            }
            context.Projects.Remove(projectsDelete);

            context.SaveChanges();

            Project[] projects = context
                .Projects
                .Take(10)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var p in projects)
            {
                sb.AppendLine($"{p.Name}");
            }

            return sb.ToString().TrimEnd();
        }


        //15. Remove Town
        public static string RemoveTown(SoftUniContext context)
        {
            Town townDelete = context
                .Towns
                .First(t => t.Name == "Seattle");

            Address[] addressesDelete = context
                .Addresses
                .Where(a => a.Town == townDelete)
                .ToArray();

            foreach (var a in addressesDelete)
            {
                Employee[] employees = context
                    .Employees
                    .Where(e => e.Address == a)
                    .ToArray();

                foreach (var e in employees)
                {
                    e.Address = null;
                }

                context.Addresses.Remove(a);
            }

            context.Towns.Remove(townDelete);

            context.SaveChanges();

            return $"{addressesDelete.Length} addresses in Seattle were deleted";
        }
    }
}