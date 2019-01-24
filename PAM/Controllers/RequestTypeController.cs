using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PAM.Data;
using PAM.Extensions;
using PAM.Models;

namespace PAM.Controllers
{
    public class RequestTypeController : Controller
    {
        private readonly UserService _userService;

        public RequestTypeController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult RequestType(Request req)
        {
            var update = HttpContext.Session.GetObject<Request>("Request");
            
            return View("../Request/RequestType", req);
        }

        [HttpGet, ActionName("NewRequester")]
        public IActionResult RequestType(Requester requester)
        {
            var newRequester = _userService.SaveRequester(requester);

            return View("../Request/RequestType");
        }
    }
}
