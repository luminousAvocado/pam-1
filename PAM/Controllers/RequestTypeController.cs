using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PAM.Models;

namespace PAM.Controllers
{
    public class RequestTypeController : Controller
    {
        [HttpGet]
        public IActionResult RequestType(Request req)
        {
            return View("../Request/RequestType", req);
        }
    }
}
