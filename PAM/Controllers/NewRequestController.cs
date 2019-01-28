using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PAM.Data;
using PAM.Extensions;
using PAM.Models;
using PAM.Services;


namespace PAM.Controllers
{
    public class NewRequestController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserService _userService;

        private IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public NewRequestController(IADService adService, UserService userService,
            AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = context;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult CreateRequester()
        {
            Requester requester = new Requester
            {
                Email = ((ClaimsIdentity)User.Identity).GetClaim(ClaimTypes.Email),
                FirstName = ((ClaimsIdentity)User.Identity).GetClaim(ClaimTypes.GivenName),
                LastName = ((ClaimsIdentity)User.Identity).GetClaim(ClaimTypes.Surname),
                Username = ((ClaimsIdentity)User.Identity).GetClaim(ClaimTypes.NameIdentifier),
                Name = ((ClaimsIdentity)User.Identity).GetClaim("Name"),
            };

            requester = _userService.SaveRequester(requester);
            HttpContext.Session.SetObject("Requester", requester);

            return View("NewRequest");
        }

        [HttpGet]
        public IActionResult SelfInfo(Request req)
        {
            HttpContext.Session.SetObject("Request", req);
            return View();
        }

        [HttpPost]
        public IActionResult SelfInfo(Requester requester)
        {
            return View();
        }

        [HttpGet]
        public IActionResult OtherInfo(Request req)
        {
            HttpContext.Session.SetObject("Request", req);
            return View();
        }

        [HttpGet]
        public IActionResult RequestType(Requester requester)
        {
            var update = HttpContext.Session.GetObject<Request>("Request");
            
            return View();
        }

        [HttpGet]
        public IActionResult RequestInfo(Request req)
        {
            var update = HttpContext.Session.GetObject<Request>("Request");
            update.RequestTypeId = req.RequestTypeId;
            HttpContext.Session.SetObject("Request", update);
            return View(req);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest (Request req)
        {
            var update = HttpContext.Session.GetObject<Request>("Request");
            update.IsContractor = req.IsContractor;
            update.IsHighProfileAccess = req.IsHighProfileAccess;
            update.IsGlobalAccess = req.IsGlobalAccess;
            update.CaseloadType = req.CaseloadType;
            update.CaseloadFunction = req.CaseloadFunction;
            update.CaseloadNumber = req.CaseloadNumber;
            update.DepartureReason = req.DepartureReason;

            _dbContext.Add(update);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Self", "Request");
        }
    }
}
