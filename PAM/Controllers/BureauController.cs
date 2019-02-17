using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Models;

namespace PAM.Controllers
{
    public class BureauController : Controller
    {
        private readonly OrganizationService _organizationService;
        private readonly IMapper _mapper;
        private readonly ILogger<BureauController> _logger;

        public BureauController(OrganizationService organizationService, IMapper mapper, ILogger<BureauController> logger)
        {
            _organizationService = organizationService;
            _mapper = mapper;
            _logger = logger;
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
            _mapper.Map(update, bureau);
            _organizationService.SaveChanges();
            return RedirectToAction(nameof(ViewBureau), new { id });
        }

        public IActionResult RemoveBureau(int id)
        {
            var bureau = _organizationService.GetBureau(id);
            bureau.Deleted = true;
            _organizationService.SaveChanges();
            return RedirectToAction(nameof(Bureaus));
        }
    }
}
