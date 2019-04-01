using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Models;

namespace PAM.Controllers
{
    [Authorize]
    public class LocationController : Controller
    {
        private readonly OrganizationService _organizationService;
        private readonly IMapper _mapper;
        private readonly ILogger<LocationController> _logger;

        public LocationController(OrganizationService organizationService, IMapper mapper, ILogger<LocationController> logger)
        {
            _organizationService = organizationService;
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
        public IActionResult AddLocation(Location location)
        {
            location = _organizationService.AddLocation(location);
            return RedirectToAction(nameof(ViewLocation), new { id = location.LocationId });
        }

        [HttpGet, Authorize("IsAdmin")]
        public IActionResult EditLocation(int id)
        {
            return View(_organizationService.GetLocation(id));
        }

        [HttpPost, Authorize("IsAdmin")]
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
