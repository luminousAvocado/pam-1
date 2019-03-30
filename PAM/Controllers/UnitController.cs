using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PAM.Data;
using PAM.Models;
using PAM.Services;
using System;
using System.Security.Claims;
using PAM.Extensions;

namespace PAM.Controllers
{
    public class UnitController : Controller
    {
        private readonly OrganizationService _organizationService;
        private readonly SystemService _systemService;
        private readonly TreeViewService _treeViewService;
        private readonly IMapper _mapper;
        private readonly ILogger<UnitController> _logger;
        private readonly AuditLogService _auditService;

        public UnitController(OrganizationService organizationService, SystemService systemService,
            TreeViewService treeViewService, IMapper mapper, ILogger<UnitController> logger, AuditLogService auditService)
        {
            _organizationService = organizationService;
            _systemService = systemService;
            _treeViewService = treeViewService;
            _mapper = mapper;
            _logger = logger;
            _auditService = auditService;
        }

        public IActionResult Units(int? id)
        {
            ViewData["tree"] = _treeViewService.GenerateTreeInJson();
            return View(id != null ? _organizationService.GetUnit((int)id) : null);
        }

        [HttpGet]
        public IActionResult AddUnit(int? parentId, int? bureauId)
        {
            if (parentId != null)
            {
                Unit parent = _organizationService.GetUnit((int)parentId);
                ViewData["parent"] = parent;
                ViewData["bureau"] = parent.Bureau;
            }
            else
            {
                ViewData["bureau"] = _organizationService.GetBureau((int)bureauId);
            }
            ViewData["unitTypes"] = new SelectList(_organizationService.GetUnitTypes(), "UnitTypeId", "Name");
            ViewData["systems"] = JsonConvert.SerializeObject(_systemService.GetSystems());
            return View(new Unit());
        }

        [HttpPost]
        public IActionResult AddUnit(Unit unit, ICollection<int> systemIds)
        {
            unit.Systems = new List<UnitSystem>();
            foreach (var systemId in systemIds.OrderBy(v => v).ToList())
                unit.Systems.Add(new UnitSystem()
                {
                    SystemId = systemId
                });
            unit = _organizationService.AddUnit(unit);

            _auditService.CreateAuditLog(Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId")), Models.Action.Create, ResourceType.Unit, unit.UnitId, null, null);

            return RedirectToAction(nameof(Units), new { id = unit.UnitId });
        }


        [HttpGet]
        public IActionResult EditUnit(int id)
        {
            var unit = _organizationService.GetUnit(id);
            ViewData["parent"] = unit.Parent;
            ViewData["bureau"] = unit.Bureau;
            ViewData["unitTypes"] = new SelectList(_organizationService.GetUnitTypes(), "UnitTypeId", "Name");
            ViewData["systems"] = JsonConvert.SerializeObject(_systemService.GetSystems());
            return View(unit);
        }

        [HttpPost]
        public IActionResult EditUnit(int id, Unit update, ICollection<int> systemIds)
        {
            var unit = _organizationService.GetUnit(id);
            var oldValue = JsonConvert.SerializeObject(unit);

            unit.Name = update.Name;
            unit.UnitTypeId = update.UnitTypeId;
            unit.DisplayOrder = update.DisplayOrder;
            unit.Systems.Clear();
            foreach (var systemId in systemIds.OrderBy(v => v).ToList())
                unit.Systems.Add(new UnitSystem()
                {
                    UnitId = id,
                    SystemId = systemId
                });
            _organizationService.SaveChanges();
            var newValue = JsonConvert.SerializeObject(_organizationService.GetUnit(id));

            _auditService.CreateAuditLog(Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId")), Models.Action.Edit, ResourceType.Unit, id, oldValue, newValue);

            return RedirectToAction(nameof(Units), new { id });
        }

        public IActionResult RemoveUnit(int id)
        {
            var unit = _organizationService.GetUnit(id);
            unit.Deleted = true;
            removeChildren(unit);
            _organizationService.SaveChanges();

            _auditService.CreateAuditLog(Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId")), Models.Action.Delete, ResourceType.Unit, id, null, null);

            return RedirectToAction(nameof(Units), new { id = unit.ParentId });
        }

        private void removeChildren(Unit parent)
        {
            var units = _organizationService.GetUnitChildren(parent.UnitId);
            foreach (var unit in units)
            {
                unit.Deleted = true;
                removeChildren(unit);
            }
        }
    }
}
