using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PAM.Models;
using PAM.Services;
using Xunit;

namespace PAM.Test.Services
{
    public class ADServiceTests : IClassFixture<ADServiceFixture>
    {
        private readonly IADService _adService;
        private readonly string _username, _password;

        public ADServiceTests(ADServiceFixture fixture)
        {
            _adService = fixture.ADService;
            _username = fixture.Configuration.GetValue<string>("ActiveDirectory:Username");
            _password = fixture.Configuration.GetValue<string>("ActiveDirectory:Password");
        }

        [Fact]
        public void AuthenticateTest()
        {
            Assert.True(_adService.Authenticate(_username, _password));
        }

        [Fact]
        public void GetEmployeeTest()
        {
            Employee employee = _adService.GetEmployee(_username);
            Assert.NotNull(employee);
            Assert.Equal("Sun", employee.LastName);
        }

        [Fact]
        public void GetEmployeesTest()
        {
            ICollection<Employee> employees = _adService.GetEmployees("Chengyu", "Sun");
            Assert.NotEmpty(employees);
            Assert.Equal(1, employees.Count);
        }
    }
}
