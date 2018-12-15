using System;
using System.Collections.Generic;
using System.Linq;
using PAM.Models;

namespace PAM.Services
{
    public interface IADService
    {
        bool Authenticate(string username, string password);

        Employee GetEmployee(string username, string password);

        ICollection<Employee> SearchEmployees(string term);
    }

    public class MockADService : IADService
    {
        private Employee[] employees =
        {
            new Employee()
            {
                EmployeeNumber = "1234",
                Username = "jdoe1",
                FirstName = "John",
                LastName = "Doe",
                Email = "jdoe1@localhost.localdomain",
                Title = "Developer",
                Department = "IT Systems",
                Phone = "123-456-7890"
            },
            new Employee()
            {
                EmployeeNumber = "2345",
                Username = "jdoe2",
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jdoe2@localhost.localdomain",
                Title = "Supervisor",
                Department = "IT Systems",
                Phone = "234-567-8901"
            },
            new Employee()
            {
                EmployeeNumber = "3456",
                Username = "tsmith",
                FirstName = "Tom",
                LastName = "Smith",
                Email = "tsmith@localhost.localdomain",
                Title = "Director",
                Department = "IT Systems",
                Phone = "345-678-9012"
            }
        };

        public bool Authenticate(string username, string password)
        {
            return employees.Any(employee => employee.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public Employee GetEmployee(string username, string password)
        {
            return employees
                .Where(e => e.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
        }

        public ICollection<Employee> SearchEmployees(string term)
        {
            return employees
                .Where(e => e.Username.StartsWith(term, StringComparison.OrdinalIgnoreCase))
                .OrderBy(e => e.FirstName).ToList();
        }
    }

}
