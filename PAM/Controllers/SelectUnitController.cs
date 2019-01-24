using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PAM.Models;

namespace PAM.Controllers
{
    public class SelectUnitController : Controller
    {
        // RETURN A VIEW TO SYSTEMS
        [HttpGet]
        public IActionResult Index()
        {
            return View("../Request/SelectUnit");
        }
    }
}
