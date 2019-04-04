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
    public class LocationController : Controller
    {
        private readonly OrganizationService _organizationService;
        private readonly AuditLogService _auditLog;
        private readonly IMapper _mapper;
        private readonly ILogger<LocationController> _logger;

        public LocationController(OrganizationService organizationService, AuditLogService auditLog, IMapper mapper, ILogger<LocationController> logger)
        {
            _organizationService = organizationService;
            _auditLog = auditLog;
            _mapper = mapper;
            _logger = logger;
        }

        public IActionResult Locations()
        {
            return View(_organizationService.GetLocations());
        }

        public IActionResult ViewLocation(int id)
        {
            return View(_organizationService.GetLocation(id));
        }

        [HttpGet, Authorize("IsAdmin")]
        public IActionResult AddLocation()
        {
            return View(new Location());
        }

        [HttpPost, Authorize("IsAdmin")]
        public async Task<IActionResult> AddLocation(Location location)
        {
            location = _organizationService.AddLocation(location);

            var identity = (ClaimsIdentity)User.Identity;
            await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Create, LogResourceType.Location, location.LocationId,
                $"{identity.GetClaim(ClaimTypes.Name)} created location with id {location.LocationId}");

            return RedirectToAction(nameof(ViewLocation), new { id = location.LocationId });
        }

        [HttpGet, Authorize("IsAdmin")]
        public IActionResult EditLocation(int id)
        {
            return View(_organizationService.GetLocation(id));
        }

        [HttpPost, Authorize("IsAdmin")]
        public async Task<IActionResult> EditLocation(int id, Location update)
        {
            var location = _organizationService.GetLocation(id);

            var oldValue = JsonConvert.SerializeObject(location, Formatting.Indented);
            _mapper.Map(update, location);
            _organizationService.SaveChanges();
            var newValue = JsonConvert.SerializeObject(location, Formatting.Indented);

            var identity = (ClaimsIdentity)User.Identity;
            await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Update, LogResourceType.Location, location.LocationId,
                $"{identity.GetClaim(ClaimTypes.Name)} updated location with id {location.LocationId}", oldValue, newValue);

            return RedirectToAction(nameof(ViewLocation), new { id });
        }

        [Authorize("IsAdmin")]
        public async Task<IActionResult> RemoveLocation(int id)
        {
            var location = _organizationService.GetLocation(id);
            location.Deleted = true;
            _organizationService.SaveChanges();

            var identity = (ClaimsIdentity)User.Identity;
            await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Remove, LogResourceType.Location, location.LocationId,
                $"{identity.GetClaim(ClaimTypes.Name)} removed location with id {location.LocationId}");

            return RedirectToAction(nameof(Locations));
        }
    }
}
