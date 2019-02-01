using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PAM.Services;

namespace PAM.Controllers
{
    public class SelectUnitController : Controller
    {
        private readonly TreeViewService _treeService;

        public SelectUnitController(TreeViewService treeService)
        {
            _treeService = treeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var myTree = _treeService.GenerateTree();
            ViewData["MyTree"] = myTree;

            return View("../NewRequest/SelectUnit");
        }

        [HttpPost]
        public IActionResult Index(int selectedUnit)
        {
            TempData["selectedUnit"] = selectedUnit;

            return RedirectToAction("SelectSystems");
        }
    }
}
