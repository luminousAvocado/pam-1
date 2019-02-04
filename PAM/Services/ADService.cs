using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PAM.Extensions;
using PAM.Models;

namespace PAM.Services
{
    public interface IADService
    {
        bool Authenticate(string username, string password);

        Employee GetEmployee(string username);

        ICollection<Employee> GetEmployees(string firstName, string lastName);

        ICollection<Employee> GetAllEmployees();
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

        public static readonly string[] Properties =
        {
            "sAMAccountName", // Username, a.k.a. Employee Number
            "cn", // Name (Full Name + Employee Number)
            "givenName", // First Name
            "middleName", // Middle Name
            "sn", // Last Name
            "mail", "userPrincipalName", // Email
            "title", // Title
            "department", // Department
            "section", // Section
            "service", // Service
            "streetAddress", // Address
            "l", // City
            "st", // State
            "postalCode", // Zip
            "telephoneNumber", // Phone
            "mobile", // CellPhone
            "supervisor", "manager" // Supervisor
        };

        private void setSearchProperties(DirectorySearcher searcher)
        {
            foreach (var property in Properties)
                searcher.PropertiesToLoad.Add(property);
        }

        private Employee getEmployee(SearchResult result)
        {
            Employee employee = new Employee();
            employee.Username = result.GetProperty("sAMAccountName");
            employee.Name = result.GetProperty("cn");
            employee.FirstName = result.GetProperty("givenName");
            employee.MiddleName = result.GetProperty("middleName");
            employee.LastName = result.GetProperty("sn");
            employee.Email = result.GetProperty("mail") ?? result.GetProperty("userPrincipalName");

            employee.Title = result.GetProperty("title");
            employee.Department = result.GetProperty("department");
            employee.Section = result.GetProperty("section");
            employee.Service = result.GetProperty("service");

            employee.Address = result.GetProperty("streetAddress");
            employee.City = result.GetProperty("l");
            employee.State = result.GetProperty("st");
            employee.Zip = result.GetProperty("postalCode");

            employee.Phone = result.GetProperty("telephoneNumber");
            employee.CellPhone = result.GetProperty("mobile");

            string supervisor = result.GetProperty("supervisor") ?? result.GetProperty("manager");
            int startIndex = supervisor.IndexOf("CN=") + 3;
            int endIndex = supervisor.IndexOf(',', startIndex);
            employee.SupervisorName = supervisor.Substring(startIndex, (endIndex - startIndex));

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

        public ICollection<Employee> GetAllEmployees()
        {
            ICollection<Employee> employees = new List<Employee>();
            using (DirectoryEntry entry = new DirectoryEntry(_url, _username, _password))
            {
                using (DirectorySearcher searcher = new DirectorySearcher(entry))
                {
                    searcher.CacheResults = true;
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
            },
            new Employee()
            {
                Username = "e456789",
                Name = "Brandon Lam (e456789)",
                FirstName = "Brandon",
                LastName = "Lam",
                Email = "blam@localhost.localdomain",
                Title = "Director",
                Department = "IT Systems",
                Phone = "345-678-9012"
            },
            new Employee()
            {
                Username = "e567891",
                Name = "Jaime Borunda (e567891)",
                FirstName = "Jaime",
                LastName = "Borunda",
                Email = "jborunda@localhost.localdomain",
                Title = "Director",
                Department = "IT Systems",
                Phone = "345-678-9012"
            },
            new Employee()
            {
                Username = "e678912",
                Name = "James Kang (e678912)",
                FirstName = "James",
                LastName = "Kang",
                Email = "jkang@localhost.localdomain",
                Title = "Director",
                Department = "IT Systems",
                Phone = "345-678-9012"
            },
            new Employee()
            {
                Username = "e789123",
                Name = "Kevork Gib (e789123)",
                FirstName = "Kevork",
                LastName = "Gilabouchian",
                Email = "kgilabouchian@localhost.localdomain",
                Title = "Director",
                Department = "IT Systems",
                Phone = "345-678-9012"
            },
            new Employee()
            {
                Username = "e891234",
                Name = "Chengyu Sun (e891234)",
                FirstName = "Chengyu",
                LastName = "Sun",
                Email = "csun@localhost.localdomain",
                Title = "Director",
                Department = "IT Systems",
                Phone = "345-678-9012"
            },
            new Employee()
            {
                Username = "e912345",
                Name = "Zilong Yi (e912345)",
                FirstName = "Zilong",
                LastName = "Yi",
                Email = "zyi@localhost.localdomain",
                Title = "Director",
                Department = "IT Systems",
                Phone = "345-678-9012"
            },
            new Employee()
            {
                Username = "e987654",
                Name = "Diana Salazar (e987654)",
                FirstName = "Diana",
                LastName = "Salazar",
                Email = "dsalazar@localhost.localdomain",
                Title = "Director",
                Department = "IT Systems",
                Phone = "345-678-9012"
            },
            new Employee()
            {
                Username = "e876543",
                Name = "Benjamin Morales (e876543)",
                FirstName = "Benjamin",
                LastName = "Morales",
                Email = "bmorales@localhost.localdomain",
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

        public ICollection<Employee> GetAllEmployees()
        {
            return employees.ToList();
        }
    }

}
