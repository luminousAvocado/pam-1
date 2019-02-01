using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAM.Data;
using PAM.Models;
using Newtonsoft.Json;
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
    }
}
