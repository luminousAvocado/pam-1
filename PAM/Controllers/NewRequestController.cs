using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentEmail.Core;
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
        private readonly IFluentEmail _email;
        private readonly EmailHelper _emailHelper;
        private readonly RequestService _reqService;
        private readonly SystemService _sysService;

        private IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public NewRequestController(IADService adService, UserService userService,
            AppDbContext context, TreeViewService treeService, IHttpContextAccessor httpContextAccessor, IFluentEmail email, EmailHelper emailHelper,
            OrganizationService orgService, RequestService reqService, SystemService sysService)
        {
            _adService = adService;
            _dbContext = context;
            _userService = userService;
            _treeService = treeService;
            _orgService = orgService;
            _reqService = reqService;
            _httpContextAccessor = httpContextAccessor;
            _email = email;
            _emailHelper = emailHelper;
            _sysService = sysService;
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
            HttpContext.Session.SetObject("Requester", currRequester);

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
            update = _reqService.SaveRequest(update);
            HttpContext.Session.SetObject("Request", update);

            return RedirectToAction("SelectUnit");
        }

        [HttpGet]
        public IActionResult SelectUnit()
        {
            var myTree = _treeService.GenerateTree();
            ViewData["MyTree"] = myTree;

            return View();
        }

        [HttpPost]
        public IActionResult SelectUnit(int selectedUnit)
        {
            TempData["selectedUnit"] = selectedUnit;
            HttpContext.Session.SetObject("UnitId", (int)TempData["selectedUnit"]);

            return RedirectToAction("SelectSystems");
        }

        [HttpGet]
        public IActionResult SelectSystems()
        {
            var systemsList = _orgService.GetRelatedSystems((int)TempData["selectedUnit"]);        

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

            _reqService.UpdateRequest(update);
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
        public IActionResult Review(string supervisor)
        {
            TempData["Supervisor"] = supervisor;
            var req = HttpContext.Session.GetObject<Request>("Request");
            var unitId = HttpContext.Session.GetObject<int>("UnitId");
            ViewData["Systems"] = _orgService.GetRelatedSystems(unitId);
            ViewData["Request"] = _reqService.GetRequest(req.RequestId);
            ViewData["Supervisor"] = supervisor;

            return View();
        }

        [HttpPost]
        public IActionResult Review()
        {
            var update = HttpContext.Session.GetObject<Request>("Request");
            update.RequestStatus = RequestStatus.PendingReview;
            _reqService.UpdateRequest(update);

            // Consider making a SendEmail() method that basically does what EmailApprover does and just call in here
            // Ask Dr. Sun about this, pretty sure its gonna be SendEmail()

            return RedirectToAction("EmailApprover", "NewRequest"); 
        }

        public IActionResult EmailApprover()
        {
            var supervisor = _userService.GetEmployeeByName((string)TempData["Supervisor"]);

            var req = HttpContext.Session.GetObject<Request>("Request");
            Request Request = _reqService.GetRequest(req.RequestId);

            // Create Review entry
            Review newReview = new Review
            {
                RequestId = req.RequestId,
                ReviewerId = supervisor.EmployeeId,
                ReviewerTitle = supervisor.Title,
                //ReviewOrder =
            };
            _reqService.SaveReview(newReview);

            // Create RequestedSystems
            var systemsList = _orgService.GetRelatedSystems(HttpContext.Session.GetObject<int>("UnitId"));
            foreach(var system in systemsList)
            {
                RequestedSystem newReqSystem = new RequestedSystem
                {
                    RequestId = req.RequestId,
                    SystemId = system.SystemId
                };
                _sysService.SaveRequestedSystem(newReqSystem);
            }

            string receipient = supervisor.Email;
            string emailName = "ReviewRequest";

            var model = new { _emailHelper.AppUrl, _emailHelper.AppEmail, Request};

            string subject = _emailHelper.GetSubjectFromTemplate(emailName, model, _email.Renderer);
            _email.To(receipient)
                .Subject(subject)
                .UsingTemplateFromFile(_emailHelper.GetBodyTemplateFile(emailName), model)
                .SendAsync();

            ViewData["Receipient"] = receipient;
            ViewData["Subject"] = subject;

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
