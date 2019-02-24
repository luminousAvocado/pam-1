using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;

namespace PAM.Controllers
{
    public class SystemController : Controller
    {
        private readonly SystemService _systemService;
        private readonly IMapper _mapper;
        private readonly ILogger<SystemController> _logger;

        public SystemController(SystemService systemService, IMapper mapper, ILogger<SystemController> logger)
        {
            _systemService = systemService;
            _mapper = mapper;
            _logger = logger;
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
            return View(new Models.System());
        }

        [HttpPost]
        public IActionResult AddSystem(Models.System system)
        {
            system = _systemService.AddSystem(system);
            return RedirectToAction(nameof(ViewSystem), new { id = system.SystemId });
        }

        [HttpGet]
        public IActionResult EditSystem(int id)
        {
            return View(_systemService.GetSystem(id));
        }

        [HttpPost]
        public IActionResult EditSystem(int id, Models.System update)
        {
            var system = _systemService.GetSystem(id);
            _mapper.Map(update, system);
            _systemService.SaveChanges();
            return RedirectToAction(nameof(ViewSystem), new { id });
        }

        public IActionResult RemoveSystem(int id)
        {
            var system = _systemService.GetSystem(id);
            system.Retired = true;
            _systemService.SaveChanges();
            return RedirectToAction(nameof(Systems));
        }
    }
}
