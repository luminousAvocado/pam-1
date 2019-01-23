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
        [HttpPost]
        public IActionResult RequestSelf(Request req)
        {
            HttpContext.Session.SetObject("Request", req);
            return RedirectToAction("RequestType", "RequestType", req);
        }

        [HttpPost]
        public IActionResult RequestOther(Request req)
        {
            HttpContext.Session.SetObject("Request", req);
            Requester newRequester = new Requester();
            return View("../Request/RequesterInfo", newRequester);
        }
    }
}
