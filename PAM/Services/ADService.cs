using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PAM.Models;

namespace PAM.Services
{
    public interface IADService
    {
        bool Authenticate(string username, string password);

        Employee GetEmployee(string username);

        ICollection<Employee> GetEmployees(string firstName, string lastName);
    }

    public class ADService : IADService
    {
        private readonly string _path, _url, _username, _password;

        private readonly ILogger _logger;

        public ADService(IConfiguration config, ILogger<ADService> logger)
        {
            _path = config.GetValue<string>("ActiveDirectory:Path");
            _username = config.GetValue<string>("ActiveDirectory:Username");
            _password = config.GetValue<string>("ActiveDirectory:Password");
            _url = $"LDAP://{_path}";
            _logger = logger;
        }

        private void setSearchProperties(DirectorySearcher searcher)
        {
            searcher.PropertiesToLoad.Add("sAMAccountName"); // aka Employee Number, Username
            searcher.PropertiesToLoad.Add("cn"); // Full Name + Employee Number
            searcher.PropertiesToLoad.Add("givenName"); // First Name
            searcher.PropertiesToLoad.Add("sn"); // Last Name
            searcher.PropertiesToLoad.Add("mail"); // Email (could be empty)
            searcher.PropertiesToLoad.Add("userPrincipalName"); // Email
            searcher.PropertiesToLoad.Add("title"); // Title (could be empty)
            searcher.PropertiesToLoad.Add("department"); // Department
            searcher.PropertiesToLoad.Add("telephoneNumber"); // Phone (could be empty)
        }

        private Employee getEmployee(SearchResult result)
        {
            Employee employee = new Employee();
            employee.Username = result.Properties["sAMAccountName"][0].ToString();
            employee.Name = result.Properties["cn"][0].ToString();
            employee.FirstName = result.Properties["givenName"][0].ToString();
            employee.LastName = result.Properties["sn"][0].ToString();
            employee.Email = result.Properties["userPrincipalName"][0].ToString();
            employee.Department = result.Properties["department"][0].ToString();

            if (result.Properties.Contains("mail"))
                employee.Email = result.Properties["userPrincipalName"][0].ToString();
            if (result.Properties.Contains("title"))
                employee.Title = result.Properties["title"][0].ToString();
            if (result.Properties.Contains("telephoneNumber"))
                employee.Phone = result.Properties["telephoneNumber"][0].ToString();

            return employee;
        }

        public bool Authenticate(string username, string password)
        {
            bool authenticated = false;
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, _path))
            {
                authenticated = pc.ValidateCredentials(username, password);
                _logger.LogInformation("Authenticate user {username}: {result}", username, authenticated);
            }
            return authenticated;
        }

        public Employee GetEmployee(string username)
        {
            Employee employee = null;
            using (DirectoryEntry entry = new DirectoryEntry(_url, _username, _password))
            {
                using (DirectorySearcher searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter = $"(&(SAMAccountName={username}))";
                    setSearchProperties(searcher);
                    employee = getEmployee(searcher.FindOne());
                }
            }
            return employee;
        }

        public ICollection<Employee> GetEmployees(string firstName, string lastName)
        {
            ICollection<Employee> employees = new List<Employee>();
            using (DirectoryEntry entry = new DirectoryEntry(_url, _username, _password))
            {
                using (DirectorySearcher searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter = $"(&(givenName={firstName})(sn={lastName}))";
                    searcher.CacheResults = true;
                    searcher.SizeLimit = 10;
                    setSearchProperties(searcher);
                    var results = searcher.FindAll();
                    foreach (SearchResult result in results)
                        employees.Add(getEmployee(result));
                }
            }
            return employees;
        }
    }

    public class MockADService : IADService
    {
        private Employee[] employees =
        {
            new Employee()
            {
                Username = "e123456",
                Name = "John Doe (e123456)",
                FirstName = "John",
                LastName = "Doe",
                Email = "jdoe1@localhost.localdomain",
                Title = "Developer",
                Department = "IT Systems",
                Phone = "123-456-7890"
            },
            new Employee()
            {
                Username = "e234567",
                Name = "Jane Doe (e234567)",
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jdoe2@localhost.localdomain",
                Title = "Supervisor",
                Department = "IT Systems",
                Phone = "234-567-8901"
            },
            new Employee()
            {
                Username = "e345678",
                Name = "Tom Smith (e345678)",
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

        public Employee GetEmployee(string username)
        {
            return employees
                .Where(e => e.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
        }

        public ICollection<Employee> GetEmployees(string firstName, string lastName)
        {
            return employees
                .Where(e => e.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
                    e.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase))
                .OrderBy(e => e.FirstName).ToList();
        }
    }

}
