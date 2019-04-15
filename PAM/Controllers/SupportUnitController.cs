using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PAM.Data;
using PAM.Extensions;
using PAM.Models;

namespace PAM.Controllers
{
    [Authorize]
    public class SupportUnitController : Controller
    {
        private readonly UserService _userService;
        private readonly SystemService _systemService;
        private readonly OrganizationService _organizationService;
        private readonly AuditLogService _auditLog;
        private readonly IMapper _mapper;
        private readonly ILogger<SupportUnitController> _logger;

        public SupportUnitController(UserService userService, SystemService systemSerivce, OrganizationService organizationService,
            AuditLogService auditLog, IMapper mapper, ILogger<SupportUnitController> logger)
        {
            _userService = userService;
            _systemService = systemSerivce;
            _organizationService = organizationService;
            _auditLog = auditLog;
            _mapper = mapper;
            _logger = logger;
        }

        public IActionResult SupportUnits()
        {
            return View(_organizationService.GetSupportUnits());
        }

        public IActionResult ViewSupportUnit(int id)
        {
            ViewData["employees"] = _userService.GetEmployeesOfSupportUnit(id);
            ViewData["systems"] = _systemService.GetSystemsOfSupportUnit(id);
            return View(_organizationService.GetSupportUnit(id));
        }

        [HttpGet, Authorize("IsAdmin")]
        public IActionResult AddSupportUnit()
        {
            ViewData["systems"] = JsonConvert.SerializeObject(_systemService.GetSystemsWithoutSupportUnit());
            return View(new SupportUnit());
        }

        [HttpPost, Authorize("IsAdmin")]
        public async Task<IActionResult> AddSupportUnit(SupportUnit unit, List<int> employeeIds, List<int> systemIds)
        {
            unit = _organizationService.AddSupportUnit(unit);

            var employees = _userService.GetEmployees(employeeIds);
            foreach (var employee in employees)
                employee.SupportUnitId = unit.SupportUnitId;
            _userService.SaveChanges();

            var systems = _systemService.GetSystems(systemIds);
            foreach (var system in systems)
                system.SupportUnitId = unit.SupportUnitId;
            _systemService.SaveChanges();

            var identity = (ClaimsIdentity)User.Identity;
            await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Create, LogResourceType.SupportUnit, unit.SupportUnitId,
                $"{identity.GetClaim(ClaimTypes.Name)} created support unit with id {unit.SupportUnitId}");

            return RedirectToAction(nameof(ViewSupportUnit), new { id = unit.SupportUnitId });
        }

        [HttpGet, Authorize("IsAdmin")]
        public IActionResult EditSupportUnit(int id)
        {
            ViewData["employees"] = _userService.GetEmployeesOfSupportUnit(id);
            ViewData["systems"] = _systemService.GetSystemsOfSupportUnit(id);
            ViewData["allSystems"] = JsonConvert.SerializeObject(_systemService.GetSystemsWithoutSupportUnit());
            return View(_organizationService.GetSupportUnit(id));
        }

        [HttpPost, Authorize("IsAdmin")]
        public async Task<IActionResult> EditSupportUnit(int id, SupportUnit unit, List<int> employeeIds, List<int> systemIds)
        {
            var supportUnit = _organizationService.GetSupportUnit(id);
            var oldValue = JsonConvert.SerializeObject(supportUnit, Formatting.Indented);
            supportUnit.Name = unit.Name;
            supportUnit.Email = unit.Email;
            _organizationService.SaveChanges();
            var newValue = JsonConvert.SerializeObject(supportUnit, Formatting.Indented);

            var identity = (ClaimsIdentity)User.Identity;
            await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Update, LogResourceType.SupportUnit, supportUnit.SupportUnitId,
                $"{identity.GetClaim(ClaimTypes.Name)} updated support unit with id {supportUnit.SupportUnitId}", oldValue, newValue);

            var employees = _userService.GetEmployees(employeeIds);
            foreach (var employee in employees)
                employee.SupportUnitId = supportUnit.SupportUnitId;
            _userService.SaveChanges();

            var systems = _systemService.GetSystems(systemIds);
            foreach (var system in systems)
                system.SupportUnitId = supportUnit.SupportUnitId;
            _systemService.SaveChanges();

            return RedirectToAction(nameof(ViewSupportUnit), new { id });
        }

        [Authorize("IsAdmin")]
        public async Task<IActionResult> RemoveSupportUnit(int id)
        {
            var unit = _organizationService.GetSupportUnit(id);
            unit.Deleted = true;
            _organizationService.SaveChanges();

            var identity = (ClaimsIdentity)User.Identity;
            await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Remove, LogResourceType.SupportUnit, unit.SupportUnitId,
                $"{identity.GetClaim(ClaimTypes.Name)} removed support unit with id {unit.SupportUnitId}");

            var employees = _userService.GetEmployeesOfSupportUnit(unit.SupportUnitId);
            foreach (var employee in employees)
                employee.SupportUnitId = null;
            _userService.SaveChanges();

            var systems = _systemService.GetSystemsOfSupportUnit(unit.SupportUnitId);
            foreach (var system in systems)
                system.SupportUnitId = null;
            _systemService.SaveChanges();

            return RedirectToAction(nameof(SupportUnits));
        }
    }
}
