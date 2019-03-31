using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace PAM.Controllers
{
    [Authorize]
    public class LocationController : Controller
    {
        private readonly OrganizationService _organizationService;
        private readonly IMapper _mapper;
        private readonly ILogger<LocationController> _logger;
        private readonly IAuthorizationService _authService;

        public LocationController(IAuthorizationService authService,OrganizationService organizationService, IMapper mapper, ILogger<LocationController> logger)
        {
            _authService = authService;
            _organizationService = organizationService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IActionResult> Locations()
        {
            var authResult = await _authService.AuthorizeAsync(User, "IsAdmin");
            ViewData["Admin"] = authResult.Succeeded;
            return View(_organizationService.GetLocations());
        }

        public async Task<IActionResult> ViewLocation(int id)
        {
            var authResult = await _authService.AuthorizeAsync(User, "IsAdmin");
            ViewData["Admin"] = authResult.Succeeded;
            return View(_organizationService.GetLocation(id));
        }

        [HttpGet]
        [Authorize("IsAdmin")]
        public IActionResult AddLocation()
        {
            return View(new Location());
        }

        [HttpPost]
        [Authorize("IsAdmin")]
        public IActionResult AddLocation(Location location)
        {
            location = _organizationService.AddLocation(location);
            return RedirectToAction(nameof(ViewLocation), new { id = location.LocationId });
        }

        [Authorize("IsAdmin")]
        public IActionResult EditLocation(int id)
        {
            return View(_organizationService.GetLocation(id));
        }

        [HttpPost]
        [Authorize("IsAdmin")]
        public IActionResult EditLocation(int id, Location update)
        {
            var location = _organizationService.GetLocation(id);
            _mapper.Map(update, location);
            _organizationService.SaveChanges();
            return RedirectToAction(nameof(ViewLocation), new { id });
        }

        [Authorize("IsAdmin")]
        public IActionResult RemoveLocation(int id)
        {
            var location = _organizationService.GetLocation(id);
            location.Deleted = true;
            _organizationService.SaveChanges();
            return RedirectToAction(nameof(Locations));
        }
    }
}
