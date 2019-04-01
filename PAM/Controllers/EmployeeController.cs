using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Models;
using PAM.Services;

namespace PAM.Controllers
{
    [Authorize("IsAdmin")]
    public class EmployeeController : Controller
    {
        private readonly UserService _userService;
        private readonly SystemService _systemService;
        private readonly IADService _adService;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(UserService userService, SystemService systemSerivce,
            IADService adService, IMapper mapper, ILogger<EmployeeController> logger)
        {
            _userService = userService;
            _systemService = systemSerivce;
            _adService = adService;
            _mapper = mapper;
            _logger = logger;
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
            _mapper.Map(update, employee);
            _userService.SaveChanges();
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
                }
            }
            return RedirectToAction(nameof(Employees), new { term });
        }
    }
}
