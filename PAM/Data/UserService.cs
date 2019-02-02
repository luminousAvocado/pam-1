using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PAM.Models;

namespace PAM.Data
{
    public class UserService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public UserService(AppDbContext dbContext, ILogger<UserService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public Employee GetEmployee(string username)
        {
            return _dbContext.Employees
                .Where(e => e.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
        }

        public Employee SaveEmployee(Employee employee)
        {
            if (employee.EmployeeId == 0) _dbContext.Add(employee);
            _dbContext.SaveChanges();
            return employee;
        }
        
        public Requester SaveRequester(Requester requester)
        {
            if (requester.RequesterId == 0) _dbContext.Add(requester);
            _dbContext.SaveChanges();
            return requester;
        }

        public void UpdateRequester(Requester requester){
            _dbContext.Update(requester);
            _dbContext.SaveChanges();
        }
    }
}
