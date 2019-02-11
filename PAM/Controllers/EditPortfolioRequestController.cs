using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Models;
using PAM.Services;

namespace PAM.Controllers
{
    [Authorize]
    public class EditPortfolioRequestController : Controller
    {
        private readonly UserService _userService;
        private readonly RequestService _requestService;
        private readonly OrganizationService _organizationService;
        private readonly TreeViewService _treeViewService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EditPortfolioRequestController(UserService userService, RequestService requestService, OrganizationService organizationService,
            TreeViewService treeViewService, IMapper mapper, ILogger<EditPortfolioRequestController> logger)
        {
            _userService = userService;
            _requestService = requestService;
            _organizationService = organizationService;
            _treeViewService = treeViewService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult RequesterInfo(int id)
        {
            var request = _requestService.GetRequest(id);
            ViewData["request"] = request;
            return View(request.RequestedFor);
        }

        [HttpPost]
        public IActionResult RequesterInfo(int id, Requester requester)
        {
            _userService.UpdateRequester(requester);
            return RedirectToAction(nameof(UnitSelection), new { id });
        }

        [HttpGet]
        public IActionResult UnitSelection(int id)
        {
            var request = _requestService.GetRequest(id);
            ViewData["request"] = request;
            ViewData["tree"] = _treeViewService.GenerateTreeInJson();

            return View();
        }

        [HttpPost]
        public IActionResult UnitSelection(int id, int unitId)
        {
            var request = _requestService.GetRequest(id);
            var unit = _organizationService.GetUnit(unitId);

            request.RequestedFor.BureauId = unit.BureauId;
            request.RequestedFor.UnitId = unit.UnitId;
            request.Systems.Clear();
            foreach(var us in unit.Systems)
                request.Systems.Add(new RequestedSystem(request.RequestId, us.SystemId));
            _requestService.SaveChanges();
            return RedirectToAction("AdditionalInfo");
        }

/*
        [HttpGet]
        public IActionResult RequestInfo()
        {
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

            _requestService.UpdateRequest(update);
            HttpContext.Session.SetObject("Request", update);
            return RedirectToAction("Supervisors");
        }

        [HttpGet]
        public IActionResult Supervisors()
        {
            var employees = _adService.GetAllEmployees();
            List<String> employeeName = new List<string>();
            foreach (var employee in employees)
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
            ViewData["Request"] = _requestService.GetRequest(req.RequestId);
            ViewData["Supervisor"] = supervisor;

            return View();
        }

        [HttpPost]
        public IActionResult Review()
        {
            var update = HttpContext.Session.GetObject<Request>("Request");
            update.RequestStatus = RequestStatus.UnderReview;
            _requestService.UpdateRequest(update);

            return RedirectToAction("EmailApprover", "NewRequest");
        }

        public IActionResult EmailApprover()
        {
            var supervisor = _userService.GetEmployeeByName((string)TempData["Supervisor"]);

            var req = HttpContext.Session.GetObject<Request>("Request");
            Request Request = _requestService.GetRequest(req.RequestId);

            string receipient = supervisor.Email;
            string emailName = "ReviewRequest";

            var model = new { _emailHelper.AppUrl, _emailHelper.AppEmail, Request };

            string subject = _emailHelper.GetSubjectFromTemplate(emailName, model, _email.Renderer);
            _email.To(receipient)
                .Subject(subject)
                .UsingTemplateFromFile(_emailHelper.GetBodyTemplateFile(emailName), model)
                .SendAsync();

            ViewData["Receipient"] = receipient;
            ViewData["Subject"] = subject;

            return RedirectToAction("MyRequests", "Request");
        }

        public Requester updateInfo(Requester current, Requester req)
        {
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
        */
    }
}
