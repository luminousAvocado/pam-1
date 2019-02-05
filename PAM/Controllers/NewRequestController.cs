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
        private readonly TreeViewService _treeService;
        private readonly OrganizationService _orgService;

        private IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public NewRequestController(IADService adService, UserService userService,
            AppDbContext context, TreeViewService treeService,
            OrganizationService orgService, IHttpContextAccessor httpContextAccessor)
        {
            _adService = adService;
            _dbContext = context;
            _userService = userService;
            _treeService = treeService;
            _orgService = orgService;
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
                Name = ((ClaimsIdentity)User.Identity).GetClaim(ClaimTypes.Name),
            };

            requester = _userService.SaveRequester(requester);
            HttpContext.Session.SetObject("Requester", requester);
            return RedirectToAction("NewRequest");
        }

        [HttpGet]
        public IActionResult NewRequest(){
            Request request = new Request();

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
        public IActionResult RequesterInfo(Requester formData){
            var currRequester = HttpContext.Session.GetObject<Requester>("Requester");
            currRequester = updateInfo(currRequester, formData);
            _userService.UpdateRequester(currRequester);
            var request = HttpContext.Session.GetObject<Request>("Request");
            
            var requestFor = HttpContext.Session.GetObject<Requester>("RequestFor");
            if(requestFor != null){
                requestFor.Name = "placeholder";
                requestFor.Username = "placeholder";
                requestFor = updateInfo(requestFor, formData);
                requestFor = _userService.SaveRequester(requestFor);
                request.RequestedForId = requestFor.RequesterId;
            }
            else request.RequestedForId = currRequester.RequesterId;

            HttpContext.Session.SetObject("Request", request);
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

            // Routing from RequestType to SelectUnit
            //return RedirectToAction("RequestInfo");
            return RedirectToAction("SelectUnit");
        }

        [HttpGet]
        public IActionResult SelectUnit()
        {
            var myTree = _treeService.GenerateTree();
            ViewData["MyTree"] = myTree;

            return View();
            //return View("../SelectUnit/SelectUnit");
        }

        [HttpPost]
        public IActionResult SelectUnit(int selectedUnit)
        {
            TempData["selectedUnit"] = selectedUnit;

            return RedirectToAction("SelectSystems");
        }

        [HttpGet]
        public IActionResult SelectSystems()
        {
            var systemsList = _orgService.GetRelatedSystems((int)TempData["selectedUnit"]);

            // Line below breaks
            //TempData["SystemsList"] = systemsList;

            return View(systemsList);
        }

        [HttpPost]
        public IActionResult SystemSelected()
        {
            // Need to create a RequestedSystem object, save in session, get requestId and create entry at end

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

        [HttpPost]
        public async Task<IActionResult> CreateRequest ()
        {
            var update = HttpContext.Session.GetObject<Request>("Request");
            _dbContext.Add(update);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Self", "Request");
        }

        public Requester updateInfo(Requester current, Requester req){
            current.FirstName = req.FirstName;
            current.LastName = req.LastName;
            current.Address = req.Address;
            current.City = req.City;
            current.State = req.State;
            current.Zip = req.Zip;
            current.Email = req.Email;
            current.Phone = req.Phone;
            current.CellPhone = req.CellPhone;

            return current;
        }
    }
}
