using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PAM.Extensions;
using PAM.Models;

namespace PAM.Controllers
{
    public class RequestInfoController : Controller
    {
        [HttpGet]
        public IActionResult RequestInfo(Request req)
        {
            var update = HttpContext.Session.GetObject<Request>("Request");
            update.RequestTypeId = req.RequestTypeId;
            HttpContext.Session.SetObject("Request", update);
            Console.WriteLine("ZZZ " + req.RequestTypeId);
            Console.WriteLine("ZZZZ " + req.RequestedById);
            Console.WriteLine("ZZZZZ " + req.RequestedForId);
            return View("../Request/RequestInfo", req);
        }
    }
}
