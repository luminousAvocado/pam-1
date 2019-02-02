using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PAM.Data;
using PAM.Models;
using PAM.Services;

namespace PAM.Controllers
{
    public class SelectUnitController : Controller
    {
        private readonly TreeViewService _treeService;
        private readonly OrganizationService _orgService;

        public SelectUnitController(TreeViewService treeService, OrganizationService orgService)
        {
            _treeService = treeService;
            _orgService = orgService;
        }

        [HttpGet]
        public async Task<IActionResult> PickUnit()
        {
            var myTree = _treeService.GenerateTree();
            ViewData["MyTree"] = myTree;

            return View("../SelectUnit/SelectUnit");
        }

        [HttpPost]
        public IActionResult PickUnit(int selectedUnit)
        {
            TempData["selectedUnit"] = selectedUnit;

            return RedirectToAction("SelectSystems", "SelectUnit");
        }

        [HttpGet]
        public IActionResult SelectSystems()
        {
            // List of UnitSystem with related Systems
            var systemsList = _orgService.GetRelatedSystems((int)TempData["selectedUnit"]);

            return View(systemsList);
        }

        [HttpPost]
        public IActionResult SystemSelected()
        {
            // Need to create a RequestedSystem object, save in session, get requestId and create entry at end

            return RedirectToAction("RequestInfo", "NewRequest");
        }
    }
}
