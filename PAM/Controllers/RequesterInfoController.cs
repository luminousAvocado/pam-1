using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PAM.Models;
using PAM.Extensions;

namespace PAM.Controllers
{
    public class RequesterInfoController : Controller
    {
        [HttpGet]
        public IActionResult SelfInfo(Request req)
        {
            HttpContext.Session.SetObject("Request", req);
            return View("SelfInfo");
        }

        [HttpPost]
        public IActionResult SelfInfo(Requester requester)
        {
            return View("SelfInfo");
        }

        [HttpGet]
        public IActionResult OtherInfo(Request req)
        {
            HttpContext.Session.SetObject("Request", req);
            return View("OtherInfo");
        }
    }
}
