using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PAM.Models;
using PAM.Data;
using PAM.Extensions;

namespace PAM.Controllers
{
    public class RequestForInfoController : Controller
    {
        private readonly UserService _userService;

        public RequestForInfoController(UserService userService)
        {
            _userService = userService;
        }
        
        [HttpGet]
        public IActionResult RequestForInfo(Requester requester)
        {
            var newRequester = _userService.SaveRequester(requester);

            return View("../Request/RequesterInfo");
        }
    }
}
