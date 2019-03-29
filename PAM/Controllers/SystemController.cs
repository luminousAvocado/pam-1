using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Extensions;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using PAM.Models;

namespace PAM.Controllers
{
    public class SystemController : Controller
    {
        private readonly SystemService _systemService;
        private readonly OrganizationService _organizationService;
        private readonly IMapper _mapper;
        private readonly ILogger<SystemController> _logger;
        private readonly AuditLogService _auditService;

        public SystemController(SystemService systemService, OrganizationService organizationService, IMapper mapper, ILogger<SystemController> logger, AuditLogService auditService)
        {
            _systemService = systemService;
            _organizationService = organizationService;
            _mapper = mapper;
            _logger = logger;
            _auditService = auditService;
        }


        public IActionResult Systems()
        {
            return View(_systemService.GetSystems());
        }

        public IActionResult ViewSystem(int id)
        {
            return View(_systemService.GetSystem(id));
        }

        [HttpGet]
        public IActionResult AddSystem()
        {
            ViewData["processingUnits"] = new SelectList(_organizationService.GetProcessingUnits(), "ProcessingUnitId", "Name");
            return View(new Models.System());
        }

        [HttpPost]
        public IActionResult AddSystem(Models.System system)
        {
            system = _systemService.AddSystem(system);

            _auditService.CreateAuditLog(Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId")), Models.Action.Create, ResourceType.System, system.SystemId);

            return RedirectToAction(nameof(ViewSystem), new { id = system.SystemId });
        }

        [HttpGet]
        public IActionResult EditSystem(int id)
        {
            ViewData["processingUnits"] = new SelectList(_organizationService.GetProcessingUnits(), "ProcessingUnitId", "Name");
            return View(_systemService.GetSystem(id));
        }

        [HttpPost]
        public IActionResult EditSystem(int id, Models.System update)
        {
            var system = _systemService.GetSystem(id);
            var oldValue = JsonConvert.SerializeObject(system);

            _mapper.Map(update, system);
            _systemService.SaveChanges();
            var newValue = JsonConvert.SerializeObject(_systemService.GetSystem(id));

            _auditService.CreateAuditLog(Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId")), Models.Action.Edit, ResourceType.System, id, oldValue, newValue);

            return RedirectToAction(nameof(ViewSystem), new { id });
        }

        public IActionResult RemoveSystem(int id)
        {
            var system = _systemService.GetSystem(id);
            system.Retired = true;
            _systemService.SaveChanges();

            _auditService.CreateAuditLog(Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId")), Models.Action.Delete, ResourceType.System, id);

            return RedirectToAction(nameof(Systems));
        }
    }
}
