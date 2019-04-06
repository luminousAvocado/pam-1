using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using PAM.Models;

namespace PAM.Data
{
    public class UserService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(AppDbContext dbContext, IMapper mapper, ILogger<UserService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public bool HasEmployee(string username)
        {
            return _dbContext.Employees.Any(e => e.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public Employee CreateEmployee(Employee employee)
        {
            _dbContext.Add(employee);
            _dbContext.SaveChanges();
            return employee;
        }

        public Employee UpdateEmployee(Employee update)
        {
            var employee = _dbContext.Employees.Where(e => e.Username == update.Username).FirstOrDefault();
            update.EmployeeId = employee.EmployeeId;
            update.IsAdmin = employee.IsAdmin;
            update.IsApprover = employee.IsApprover;
            _mapper.Map(update, employee);
            _dbContext.SaveChanges();
            return employee;
        }

        public Employee GetEmployee(int id)
        {
            return _dbContext.Employees.Find(id);
        }

        public Employee GetEmployeeByUsername(string username)
        {
            return _dbContext.Employees
                .Where(e => e.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
        }

        public Employee GetEmployeeByName(string name)
        {
            return _dbContext.Employees
                .Where(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
        }

        public IList<Employee> GetEmployeesOfSupportUnit(int supportUnitId)
        {
            return _dbContext.Employees.Where(e => e.SupportUnitId == supportUnitId)
                .OrderBy(e => e.Username).ToList();
        }

        public IList<Employee> GetEmployees(List<int> ids)
        {
            return _dbContext.Employees.Where(e => ids.Contains(e.EmployeeId)).ToList();
        }

        public IList<Employee> SearchEmployees(string term)
        {
            return _dbContext.Employees.Where(e => e.Username.StartsWith(term, StringComparison.OrdinalIgnoreCase)
               || e.FirstName.StartsWith(term, StringComparison.OrdinalIgnoreCase)
               || e.LastName.StartsWith(term, StringComparison.OrdinalIgnoreCase)
               || e.Name.StartsWith(term, StringComparison.OrdinalIgnoreCase)).Take(10)
                .ToList();
        }

        public Requester CreateRequester(Requester requester)
        {
            _dbContext.Add(requester);
            _dbContext.SaveChanges();
            return requester;
        }

        public Requester UpdateRequester(Requester update)
        {
            var requester = _dbContext.Requesters.Find(update.RequesterId);
            _mapper.Map(update, requester);
            _dbContext.SaveChanges();
            return requester;
        }

        public Requester GetRequester(int id)
        {
            return _dbContext.Requesters.Find(id);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
