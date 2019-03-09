using System;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Extensions;
using PAM.Models;

namespace PAM.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserService _userService;
        private readonly SystemService _systemService;
        private readonly IMapper _mapper;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(UserService userService, SystemService systemSerivce,
            IMapper mapper, ILogger<ProfileController> logger)
        {
            _userService = userService;
            _systemService = systemSerivce;
            _mapper = mapper;
            _logger = logger;
        }

        public IActionResult ViewProfile()
        {
            var employeeId = Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId"));
            ViewData["systemAccesses"] = _systemService.GetSystemAccessesByEmployeeId(employeeId);
            return View(_userService.GetEmployee(employeeId));
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            var employeeId = Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId"));
            var employee = _userService.GetEmployee(employeeId);
            return View(_mapper.Map<EmployeeBindingModel>(employee));
        }

        [HttpPost]
        public IActionResult EditProfile(Employee update)
        {
            var employeeId = Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId"));
            var employee = _userService.GetEmployee(employeeId);
            update.IsAdmin = employee.IsAdmin;
            update.IsApprover = employee.IsApprover;
            _mapper.Map(update, employee);
            _userService.SaveChanges();
            return RedirectToAction(nameof(ViewProfile));
        }
    }
}
