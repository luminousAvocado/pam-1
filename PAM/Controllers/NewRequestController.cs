using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAM.Data;
using PAM.Extensions;
using PAM.Models;
using PAM.Services;


namespace PAM.Controllers
{
    public class NewRequestController : Controller
    {
        private readonly IADService _adService;
        private readonly UserService _userService;
        private readonly AppDbContext _dbContext;

        private IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public NewRequestController(IADService adService, UserService userService,
            AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _adService = adService;
            _dbContext = context;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
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
            return RedirectToAction("NewRequest");
        }

        [HttpGet]
        public IActionResult NewRequest(){
            return View();
        }

        [HttpPost]
        public IActionResult NewRequest(Request req, string selfOrFor){
            if(selfOrFor == "for"){
                Requester requestFor = new Requester();
                HttpContext.Session.SetObject("RequestFor", requestFor);
            }
            HttpContext.Session.SetObject("Request", req);
            return RedirectToAction("RequesterInfo");
        }

        [HttpGet]
        public IActionResult RequesterInfo(){
            return View();
        }

        [HttpPost]
        public IActionResult RequesterInfo(Requester req){
            /*
            var requestFor = HttpContext.Session.GetObject<Requester>("RequestFor");
            if(requestFor != null){
                requestFor = _userService.SaveRequester(requestFor);
                ViewBag.Requester = requestFor;
                Console.WriteLine(ViewBag.Requester.RequesterId);
            }
            */
            var original = HttpContext.Session.GetObject<Requester>("Requester");
            original = updateInfo(original, req);
            var request = HttpContext.Session.GetObject<Request>("Request");
            request.RequestedForId = original.RequesterId;
            HttpContext.Session.SetObject("Request", request);
            Console.WriteLine("UHHH" + request.RequestedForId);
            return RedirectToAction("RequestType");
        }

        [HttpGet]
        public IActionResult RequestType(){
            return View();
        }
        [HttpPost]
        public IActionResult RequestType(Request req)
        {
            var update = HttpContext.Session.GetObject<Request>("Request");
            update.RequestTypeId = req.RequestTypeId;
            HttpContext.Session.SetObject("Request", update);
            return RedirectToAction("RequestInfo");
        }

        [HttpGet]
        public IActionResult RequestInfo(){
            var update = HttpContext.Session.GetObject<Request>("Request");
            return View();
        }

        [HttpPost]
        public IActionResult RequestInfo(Request req)
        {
            var update = HttpContext.Session.GetObject<Request>("Request");
            update.IsContractor = req.IsContractor;
            update.IsHighProfileAccess = req.IsHighProfileAccess;
            update.IsGlobalAccess = req.IsGlobalAccess;
            update.CaseloadType = req.CaseloadType;
            update.CaseloadFunction = req.CaseloadFunction;
            update.CaseloadNumber = req.CaseloadNumber;
            update.DepartureReason = req.DepartureReason;
            HttpContext.Session.SetObject("Request", update);
            return RedirectToAction("Supervisors");
        }

        [HttpGet]
        public IActionResult Supervisors(){
            var employees = _adService.GetAllEmployees();
            List<String> employeeName = new List<string>();
            foreach(var employee in employees)
            {
                employeeName.Add(employee.Name);
            }
            ViewData["adEmployees"] = employeeName;
            return View();
        }

        [HttpPost]
        public IActionResult Supervisors(string nothing = "")
        {
            return RedirectToAction("Review");
        }

        [HttpGet]
        public IActionResult Review()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Review(string nothing = ""){
            return RedirectToAction("CreateRequest"); 
        }

        [HttpGet]
        public async Task<IActionResult> CreateRequest ()
        {
            var update = HttpContext.Session.GetObject<Request>("Request");
            _dbContext.Add(update);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Self", "Request");
        }

        public Requester updateInfo(Requester original, Requester req){
            original.FirstName = req.FirstName;
            original.LastName = req.LastName;
            original.WorkAddress = req.WorkAddress;
            original.WorkCity = req.WorkCity;
            original.WorkState = req.WorkState;
            original.WorkZip = req.WorkZip;
            original.Email = req.Email;
            original.WorkPhone = req.WorkPhone;
            original.CellPhone = req.CellPhone;

            return original;
        }
    }
}
