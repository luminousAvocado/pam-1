using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PAM.Models;

namespace PAM.Controllers
{
    public class RequestInfoController : Controller
    {
        public IActionResult RequestInfo(Request req)
        {
            return View("../Request/RequestInfo", req);
        }
    }
}
