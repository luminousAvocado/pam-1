using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Models;
using PAM.Services;
using PAM.Extensions;
using Newtonsoft.Json;
using System;
using System.Security.Claims;

namespace PAM.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly UserService _userService;
        private readonly SystemService _systemService;
        private readonly IADService _adService;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeController> _logger;
        private readonly AuditLogService _auditService;

        public EmployeeController(UserService userService, SystemService systemSerivce,
            IADService adService, IMapper mapper, ILogger<EmployeeController> logger, AuditLogService auditService)
        {
            _userService = userService;
            _systemService = systemSerivce;
            _adService = adService;
            _mapper = mapper;
            _logger = logger;
            _auditService = auditService;
        }

        public IActionResult Employees(string term)
        {
            return term != null ? View(_userService.SearchEmployees(term)) : View();
        }

        public IActionResult ViewEmployee(int id)
        {
            ViewData["systemAccesses"] = _systemService.GetSystemAccessesByEmployeeId(id);
            return View(_userService.GetEmployee(id));
        }

        [HttpGet]
        public IActionResult EditEmployee(int id)
        {
            var employee = _userService.GetEmployee(id);
            return View(_mapper.Map<EmployeeBindingModel>(employee));
        }

        [HttpPost]
        public IActionResult EditEmployee(int id, Employee update)
        {
            var employee = _userService.GetEmployee(id);
            var oldValue = JsonConvert.SerializeObject(employee);

            _mapper.Map(update, employee);
            _userService.SaveChanges();
            var newValue = JsonConvert.SerializeObject(_userService.GetEmployee(id));

            _auditService.CreateAuditLog(Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId")), Models.Action.Edit, ResourceType.Employee, id, oldValue, newValue);

            return RedirectToAction(nameof(ViewEmployee), new { id });
        }

        [HttpGet]
        public IActionResult AddEmployeesFromAD(string firstName, string lastName, string username)
        {
            if (username != null)
                return View(new List<Employee>() { _adService.GetEmployeeByUsername(username) });
            else if (firstName != null && lastName != null)
                return View(_adService.GetEmployees(firstName, lastName));
            return View();
        }

        [HttpPost]
        public IActionResult AddEmployeesFromAD(List<string> usernames)
        {
            string term = "";
            foreach (var username in usernames)
            {
                if (!_userService.HasEmployee(username))
                {
                    var employee = _userService.CreateEmployee(_adService.GetEmployeeByUsername(username));
                    term = employee.FirstName + " " + employee.LastName;
                    _auditService.CreateAuditLog(Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId")), Models.Action.Create, ResourceType.Employee, employee.EmployeeId);
                }
            }
            return RedirectToAction(nameof(Employees), new { term });
        }
    }
}
