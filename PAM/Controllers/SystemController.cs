using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PAM.Data;
using PAM.Extensions;
using PAM.Models;

namespace PAM.Controllers
{
    [Authorize]
    public class SystemController : Controller
    {
        private readonly SystemService _systemService;
        private readonly OrganizationService _organizationService;
        private readonly AuditLogService _auditLog;
        private readonly IMapper _mapper;
        private readonly ILogger<SystemController> _logger;
        private readonly FormService _formService;

        public SystemController(SystemService systemService, OrganizationService organizationService, AuditLogService auditLog,
            IMapper mapper, ILogger<SystemController> logger, FormService formService)
        {
            _systemService = systemService;
            _organizationService = organizationService;
            _auditLog = auditLog;
            _mapper = mapper;
            _logger = logger;
            _formService = formService;
        }


        public IActionResult Systems()
        {
            return View(_systemService.GetSystems());
        }

        public IActionResult ViewSystem(int id)
        {
            return View(_systemService.GetSystem(id));
        }

        [HttpGet, Authorize("IsAdmin")]
        public IActionResult AddSystem()
        {
            ViewData["supportUnits"] = new SelectList(_organizationService.GetSupportUnits(), "SupportUnitId", "Name");
            return View(new Models.System());
        }

        [HttpPost, Authorize("IsAdmin")]
        public async Task<IActionResult> AddSystem(Models.System system)
        {
            system = _systemService.AddSystem(system);

            var identity = (ClaimsIdentity)User.Identity;
            await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Create, LogResourceType.System, system.SystemId,
                $"{identity.GetClaim(ClaimTypes.Name)} created system with id {system.SystemId}");

            return RedirectToAction(nameof(ViewSystem), new { id = system.SystemId });
        }

        [HttpGet, Authorize("IsAdmin")]
        public IActionResult EditSystem(int id)
        {
            ViewData["supportUnits"] = new SelectList(_organizationService.GetSupportUnits(), "SupportUnitId", "Name");
            ViewData["allForms"] = JsonConvert.SerializeObject(_formService.GetForms());
            return View(_systemService.GetSystem(id));
        }

        [HttpPost, Authorize("IsAdmin")]
        public async Task<IActionResult> EditSystem(int id, Models.System update, List<int> formIds)
        {
            var system = _systemService.GetSystem(id);

            var oldValue = JsonConvert.SerializeObject(system, Formatting.Indented);
            _mapper.Map(update, system);
            _systemService.SaveChanges();

            if (formIds.Count > 0)
            {
                foreach (int formId in formIds)
                {
                    _formService.AddSystemForm(new SystemForm { FormId = formId, SystemId = id });
                }
            }

            var newValue = JsonConvert.SerializeObject(system, Formatting.Indented);

            var identity = (ClaimsIdentity)User.Identity;
            await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Update, LogResourceType.System, system.SystemId,
                $"{identity.GetClaim(ClaimTypes.Name)} updated system with id {system.SystemId}", oldValue, newValue);

            return RedirectToAction(nameof(ViewSystem), new { id });
        }

        [Authorize("IsAdmin")]
        public async Task<IActionResult> RemoveSystem(int id)
        {
            var system = _systemService.GetSystem(id);
            system.Retired = true;
            _systemService.SaveChanges();

            var identity = (ClaimsIdentity)User.Identity;
            await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Remove, LogResourceType.System, system.SystemId,
                $"{identity.GetClaim(ClaimTypes.Name)} removed system with id {system.SystemId}");

            return RedirectToAction(nameof(Systems));
        }
    }
}
