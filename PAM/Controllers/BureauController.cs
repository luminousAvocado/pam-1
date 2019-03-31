using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace PAM.Controllers
{
    [Authorize]
    public class BureauController : Controller
    {
        private readonly OrganizationService _organizationService;
        private readonly IMapper _mapper;
        private readonly ILogger<BureauController> _logger;
        private readonly IAuthorizationService _authService;
        public BureauController(IAuthorizationService authResult, OrganizationService organizationService, IMapper mapper, ILogger<BureauController> logger)
        {
            _authService = authResult;
            _organizationService = organizationService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IActionResult> Bureaus()
        {
            var authResult = await _authService.AuthorizeAsync(User, "IsAdmin");
            ViewData["Admin"] = authResult.Succeeded;
            return View(_organizationService.GetBureaus());
        }


        public async Task<IActionResult> ViewBureau(int id)
        {
            var authResult = await _authService.AuthorizeAsync(User, "IsAdmin");
            ViewData["Admin"] = authResult.Succeeded;
            return View(_organizationService.GetBureau(id));
        }

        [HttpGet]
        [Authorize("IsAdmin")]
        public IActionResult AddBureau()
        {
            ViewData["BureauTypes"] = new SelectList(_organizationService.GetBureauTypes(), "BureauTypeId", "DisplayCode");
            return View(new Bureau());
        }

        [HttpPost]
        [Authorize("IsAdmin")]
        public IActionResult AddBureau(Bureau bureau)
        {
            bureau = _organizationService.AddBureau(bureau);
            return RedirectToAction(nameof(ViewBureau), new { id = bureau.BureauId });
        }

        [HttpGet]
        [Authorize("IsAdmin")]
        public IActionResult EditBureau(int id)
        {
            ViewData["BureauTypes"] = new SelectList(_organizationService.GetBureauTypes(), "BureauTypeId", "DisplayCode");
            return View(_organizationService.GetBureau(id));
        }

        [HttpPost]
        [Authorize("IsAdmin")]
        public IActionResult EditBureau(int id, Bureau update)
        {
            var bureau = _organizationService.GetBureau(id);
            _mapper.Map(update, bureau);
            _organizationService.SaveChanges();
            return RedirectToAction(nameof(ViewBureau), new { id });
        }

        [Authorize("IsAdmin")]
        public IActionResult RemoveBureau(int id)
        {
            var bureau = _organizationService.GetBureau(id);
            bureau.Deleted = true;
            removeChildren(bureau);
            _organizationService.SaveChanges();
            return RedirectToAction(nameof(Bureaus));
        }

        [Authorize("IsAdmin")]
        private void removeChildren(Bureau bureau)
        {
            var units = _organizationService.GetBureauChildren(bureau.BureauId);
            foreach (var unit in units)
            {
                unit.Deleted = true;
                removeChildren(unit);
            }
        }

        [Authorize("IsAdmin")]
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
