using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PAM.Models;

namespace PAM.Controllers
{
    public class RequesterInfoController : Controller
    {
        [HttpGet]
        public IActionResult RequesterInfo(Requester requester)
        {
            return View("../Request/RequesterInfo");
        }
    }
}
