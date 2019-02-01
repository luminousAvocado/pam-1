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

            return View("../NewRequest/SelectUnit");
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
            var unitSystemsList = _orgService.GetRelatedUnitSystems((int)TempData["selectedUnit"]);

            return View("../NewRequest/SelectSystems");
        }
    }
}
