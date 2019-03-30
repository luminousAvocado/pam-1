using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Models;
using PAM.Extensions;
using System;
using Newtonsoft.Json;

namespace PAM.Controllers
{
    public class BureauController : Controller
    {
        private readonly OrganizationService _organizationService;
        private readonly IMapper _mapper;
        private readonly ILogger<BureauController> _logger;
        private readonly AuditLogService _auditService;

        public BureauController(OrganizationService organizationService, IMapper mapper, ILogger<BureauController> logger, AuditLogService auditService)
        {
            _organizationService = organizationService;
            _mapper = mapper;
            _logger = logger;
            _auditService = auditService;
        }

        public IActionResult Bureaus()
        {
            return View(_organizationService.GetBureaus());
        }


        public IActionResult ViewBureau(int id)
        {
            return View(_organizationService.GetBureau(id));
        }

        [HttpGet]
        public IActionResult AddBureau()
        {
            ViewData["BureauTypes"] = new SelectList(_organizationService.GetBureauTypes(), "BureauTypeId", "DisplayCode");
            return View(new Bureau());
        }

        [HttpPost]
        public IActionResult AddBureau(Bureau bureau)
        {
            bureau = _organizationService.AddBureau(bureau);

            _auditService.CreateAuditLog(Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId")), Models.Action.Create, ResourceType.Bureau, bureau.BureauId, null, null);

            return RedirectToAction(nameof(ViewBureau), new { id = bureau.BureauId });
        }

        [HttpGet]
        public IActionResult EditBureau(int id)
        {
            ViewData["BureauTypes"] = new SelectList(_organizationService.GetBureauTypes(), "BureauTypeId", "DisplayCode");
            return View(_organizationService.GetBureau(id));
        }

        [HttpPost]
        public IActionResult EditBureau(int id, Bureau update)
        {
            var bureau = _organizationService.GetBureau(id);
            var oldValue = JsonConvert.SerializeObject(bureau);        

            _mapper.Map(update, bureau);
            _organizationService.SaveChanges();
            var newValue = JsonConvert.SerializeObject(_organizationService.GetBureau(id));

            _auditService.CreateAuditLog(Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId")), Models.Action.Edit, ResourceType.Bureau, id, oldValue, newValue);

            return RedirectToAction(nameof(ViewBureau), new { id });
        }

        public IActionResult RemoveBureau(int id)
        {
            var bureau = _organizationService.GetBureau(id);
            bureau.Deleted = true;
            removeChildren(bureau);
            _organizationService.SaveChanges();

            _auditService.CreateAuditLog(Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId")), Models.Action.Delete, ResourceType.Bureau, id, null, null);

            return RedirectToAction(nameof(Bureaus));
        }

        private void removeChildren(Bureau bureau)
        {
            var units = _organizationService.GetBureauChildren(bureau.BureauId);
            foreach (var unit in units)
            {
                unit.Deleted = true;
                removeChildren(unit);
            }
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
