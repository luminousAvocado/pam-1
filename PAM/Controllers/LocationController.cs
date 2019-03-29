using System;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Models;
using PAM.Extensions;
using Newtonsoft.Json;

namespace PAM.Controllers
{
    public class LocationController : Controller
    {
        private readonly OrganizationService _organizationService;
        private readonly IMapper _mapper;
        private readonly ILogger<LocationController> _logger;
        private readonly AuditLogService _auditService;

        public LocationController(OrganizationService organizationService, IMapper mapper, ILogger<LocationController> logger, AuditLogService auditService)
        {
            _organizationService = organizationService;
            _mapper = mapper;
            _logger = logger;
            _auditService = auditService;
        }

        public IActionResult Locations()
        {
            return View(_organizationService.GetLocations());
        }

        public IActionResult ViewLocation(int id)
        {
            return View(_organizationService.GetLocation(id));
        }

        [HttpGet]
        public IActionResult AddLocation()
        {
            return View(new Location());
        }

        [HttpPost]
        public IActionResult AddLocation(Location location)
        {
            location = _organizationService.AddLocation(location);

            _auditService.CreateAuditLog(Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId")), Models.Action.Create, ResourceType.Location, location.LocationId);

            return RedirectToAction(nameof(ViewLocation), new { id = location.LocationId });
        }

        public IActionResult EditLocation(int id)
        {
            return View(_organizationService.GetLocation(id));
        }

        [HttpPost]
        public IActionResult EditLocation(int id, Location update)
        {
            var location = _organizationService.GetLocation(id);
            var oldValue = JsonConvert.SerializeObject(location);

            _mapper.Map(update, location);
            _organizationService.SaveChanges();
            var newValue = JsonConvert.SerializeObject(_organizationService.GetLocation(id));

            _auditService.CreateAuditLog(Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId")), Models.Action.Edit, ResourceType.Location, id, oldValue, newValue);

            return RedirectToAction(nameof(ViewLocation), new { id });
        }

        public IActionResult RemoveLocation(int id)
        {
            var location = _organizationService.GetLocation(id);
            location.Deleted = true;
            _organizationService.SaveChanges();

            _auditService.CreateAuditLog(Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId")), Models.Action.Delete, ResourceType.Location, id);

            return RedirectToAction(nameof(Locations));
        }
    }
}
