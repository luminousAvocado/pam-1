using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PAM.Models;
using PAM.Extensions;

namespace PAM.Controllers
{
    public class RequestForController : Controller
    {
        [HttpGet]
        public IActionResult RequestFor()
        {
            return View("../Request/NewRequest");
        }
        [HttpPost]
        public IActionResult RequestSelf(Request req)
        {
            HttpContext.Session.SetObject("Request", req);
            return View("../Request/RequesterInfo");
            //return RedirectToAction("RequestType", "RequestType", req);
        }

        [HttpPost]
        public IActionResult RequestOther(Request req)
        {
            HttpContext.Session.SetObject("Request", req);
            Requester newRequester = new Requester();
            return View("../Request/RequestForInfo", newRequester);
        }
    }
}
