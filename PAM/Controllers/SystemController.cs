using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using PAM.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace PAM.Controllers
{
    [Authorize]
    public class SystemController : Controller
    {
        private readonly SystemService _systemService;
        private readonly OrganizationService _organizationService;
        private readonly IMapper _mapper;
        private readonly ILogger<SystemController> _logger;
        private readonly IAuthorizationService _authService;

        public SystemController(IAuthorizationService authService, SystemService systemService, OrganizationService organizationService, IMapper mapper, ILogger<SystemController> logger)
        {
            _authService = authService;
            _systemService = systemService;
            _organizationService = organizationService;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task<IActionResult> Systems()
        {
            var authResult = await _authService.AuthorizeAsync(User, "IsAdmin");
            ViewData["Admin"] = authResult.Succeeded;
            return View(_systemService.GetSystems());
        }

        public async Task<IActionResult> ViewSystem(int id)
        {
            var authResult = await _authService.AuthorizeAsync(User, "IsAdmin");
            ViewData["Admin"] = authResult.Succeeded;
            return View(_systemService.GetSystem(id));
        }

        [HttpGet]
        [Authorize("IsAdmin")]
        public IActionResult AddSystem()
        {
            ViewData["processingUnits"] = new SelectList(_organizationService.GetProcessingUnits(), "ProcessingUnitId", "Name");
            return View(new Models.System());
        }

        [HttpPost]
        [Authorize("IsAdmin")]
        public IActionResult AddSystem(Models.System system)
        {
            system = _systemService.AddSystem(system);
            return RedirectToAction(nameof(ViewSystem), new { id = system.SystemId });
        }

        [HttpGet]
        [Authorize("IsAdmin")]
        public IActionResult EditSystem(int id)
        {
            ViewData["processingUnits"] = new SelectList(_organizationService.GetProcessingUnits(), "ProcessingUnitId", "Name");
            return View(_systemService.GetSystem(id));
        }

        [HttpPost]
        [Authorize("IsAdmin")]
        public IActionResult EditSystem(int id, Models.System update)
        {
            var system = _systemService.GetSystem(id);
            _mapper.Map(update, system);
            _systemService.SaveChanges();
            return RedirectToAction(nameof(ViewSystem), new { id });
        }

        [Authorize("IsAdmin")]
        public IActionResult RemoveSystem(int id)
        {
            var system = _systemService.GetSystem(id);
            system.Retired = true;
            _systemService.SaveChanges();
            return RedirectToAction(nameof(Systems));
        }
    }
}
