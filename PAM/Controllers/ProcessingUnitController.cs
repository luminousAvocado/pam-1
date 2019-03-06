using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PAM.Data;
using PAM.Models;

namespace PAM.Controllers
{
    public class ProcessingUnitController : Controller
    {
        private readonly UserService _userService;
        private readonly SystemService _systemService;
        private readonly OrganizationService _organizationService;
        private readonly IMapper _mapper;
        private readonly ILogger<ProcessingUnitController> _logger;

        public ProcessingUnitController(UserService userService, SystemService systemSerivce, OrganizationService organizationService,
            IMapper mapper, ILogger<ProcessingUnitController> logger)
        {
            _userService = userService;
            _systemService = systemSerivce;
            _organizationService = organizationService;
            _mapper = mapper;
            _logger = logger;
        }

        public IActionResult ProcessingUnits()
        {
            return View(_organizationService.GetProcessingUnits());
        }

        public IActionResult ViewProcessingUnit(int id)
        {
            ViewData["employees"] = _userService.GetEmployeesOfProcessingUnit(id);
            ViewData["systems"] = _systemService.GetSystemsOfProcessingUnit(id);
            return View(_organizationService.GetProcessingUnit(id));
        }

        [HttpGet]
        public IActionResult AddProcessingUnit()
        {
            ViewData["systems"] = JsonConvert.SerializeObject(_systemService.GetSystemsWithoutProcessingUnit());
            return View(new ProcessingUnit());
        }

        [HttpPost]
        public IActionResult AddProcessingUnit(ProcessingUnit unit, List<int> employeeIds, List<int> systemIds)
        {
            unit = _organizationService.AddProcessingUnit(unit);

            var employees = _userService.GetEmployees(employeeIds);
            foreach (var employee in employees)
                employee.ProcessingUnitId = unit.ProcessingUnitId;
            _userService.SaveChanges();

            var systems = _systemService.GetSystems(systemIds);
            foreach (var system in systems)
                system.ProcessingUnitId = unit.ProcessingUnitId;
            _systemService.SaveChanges();

            return RedirectToAction(nameof(ViewProcessingUnit), new { id = unit.ProcessingUnitId });
        }

        [HttpGet]
        public IActionResult EditProcessingUnit(int id)
        {
            ViewData["employees"] = _userService.GetEmployeesOfProcessingUnit(id);
            ViewData["systems"] = _systemService.GetSystemsOfProcessingUnit(id);
            ViewData["allSystems"] = JsonConvert.SerializeObject(_systemService.GetSystemsWithoutProcessingUnit());
            return View(_organizationService.GetProcessingUnit(id));
        }

        [HttpPost]
        public IActionResult EditProcessingUnit(int id, ProcessingUnit unit, List<int> employeeIds, List<int> systemIds)
        {
            var processingUnit = _organizationService.GetProcessingUnit(id);
            processingUnit.Name = unit.Name;
            processingUnit.Email = unit.Email;
            _organizationService.SaveChanges();

            var employees = _userService.GetEmployees(employeeIds);
            foreach (var employee in employees)
                employee.ProcessingUnitId = unit.ProcessingUnitId;
            _userService.SaveChanges();

            var systems = _systemService.GetSystems(systemIds);
            foreach (var system in systems)
                system.ProcessingUnitId = unit.ProcessingUnitId;
            _systemService.SaveChanges();

            return RedirectToAction(nameof(ViewProcessingUnit), new { id });
        }

        public IActionResult RemoveProcessingUnit(int id)
        {
            var unit = _organizationService.GetProcessingUnit(id);
            unit.Deleted = true;
            _organizationService.SaveChanges();

            var employees = _userService.GetEmployeesOfProcessingUnit(unit.ProcessingUnitId);
            foreach (var employee in employees)
                employee.ProcessingUnitId = null;
            _userService.SaveChanges();

            var systems = _systemService.GetSystemsOfProcessingUnit(unit.ProcessingUnitId);
            foreach (var system in systems)
                system.ProcessingUnitId = null;
            _systemService.SaveChanges();

            return RedirectToAction(nameof(ProcessingUnits));
        }
    }
}
