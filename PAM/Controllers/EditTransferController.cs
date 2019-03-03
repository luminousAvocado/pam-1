using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Extensions;
using PAM.Models;
using PAM.Services;
using Microsoft.EntityFrameworkCore.Internal;

namespace PAM.Controllers
{
    [Authorize]
    public class EditTransferController: Controller
    {
        private readonly IADService _adService;
        private readonly UserService _userService;
        private readonly RequestService _requestService;
        private readonly SystemService _systemService;
        private readonly OrganizationService _organizationService;
        private readonly TreeViewService _treeViewService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EditTransferController(IADService adService, UserService userService, RequestService requestService,
            SystemService systemService, OrganizationService organizationService, TreeViewService treeViewService, IMapper mapper,
            ILogger<EditPortfolioRequestController> logger)
        {
            _adService = adService;
            _userService = userService;
            _requestService = requestService;
            _systemService = systemService;
            _organizationService = organizationService;
            _treeViewService = treeViewService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult UnitSelection(int id)
        {
            var request = _requestService.GetRequest(id);
            ViewData["tree"] = _treeViewService.GenerateTreeInJson();
            return View(request);
        }

        [HttpPost]
        public IActionResult UnitSelection(int id, int unitId, bool saveDraft = false)
        {
            var request = _requestService.GetRequest(id);
            var unit = _organizationService.GetUnit(unitId);

            request.RequestedFor.BureauId = unit.BureauId;
            request.RequestedFor.UnitId = unit.UnitId;
            request.Systems.Clear();
            foreach (var us in unit.Systems)
                request.Systems.Add(new RequestedSystem(request.RequestId, us.SystemId));
            _requestService.SaveChanges();

            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction("UnitTransfer", new { id });
        }

        [HttpGet]
        public IActionResult UnitTransfer(int id){
            var request = _requestService.GetRequest(id);
            ViewData["tree"] = _treeViewService.GenerateTreeInJson();

            /*
            var requestFor = _userService.GetRequester(request.RequestedForId);
            var systemAccess = _systemService.GetSystemAccessesByEmployeeId(requestFor.EmployeeId);
            ViewData["SystemAccess"] = systemAccess;
            */
            return View(request);
        }

        [HttpPost]
        public IActionResult UnitTransfer(int id, int transferUnitId, bool saveDraft = false){
            var request = _requestService.GetRequest(id);
            var temp = request.Systems;
            var transferUnit = _organizationService.GetUnit(transferUnitId);

            request.Systems.Clear();
            foreach(var us in transferUnit.Systems){
                foreach(var ds in temp){
                    if (ds.SystemId == us.SystemId) {
                        request.Systems.Add(new RequestedSystem(request.RequestId, ds.SystemId) { InPortfolio = false });
                        continue; //keep matching ones
                    }
                    if(temp.IndexOf(ds) == temp.Count - 1){
                        request.Systems.Add(new RequestedSystem(request.RequestId, ds.SystemId) { AccessType = SystemAccessType.Remove });
                    }
                }
                request.Systems.Add(new RequestedSystem(request.RequestId, us.SystemId) { InPortfolio = true }); //add new ones
            }

            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction("Signatures", new { id });
        }

        [HttpGet]
        public IActionResult AdditionalInfo(int id)
        {
            return View(_requestService.GetRequest(id));
        }

        [HttpPost]
        public IActionResult AdditionalInfo(int id, Request update, bool saveDraft = false)
        {
            var request = _requestService.GetRequest(id);
            request.IsContractor = update.IsContractor;
            request.IsGlobalAccess = update.IsGlobalAccess;
            request.IsHighProfileAccess = update.IsHighProfileAccess;
            request.CaseloadType = update.CaseloadType;
            request.CaseloadFunction = update.CaseloadFunction;
            request.CaseloadNumber = update.CaseloadNumber;
            _requestService.SaveChanges();
            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction("Signatures", new { id });
        }


        [HttpGet]
        public IActionResult Signatures(int id)
        {
            var request = _requestService.GetRequest(id);
            var reviews = request.OrderedReviews;
            ViewData["request"] = request;
            return View(reviews);
        }

        [HttpPost]
        public IActionResult Signatures(int id, List<Review> reviews, bool saveDraft)
        {
            var request = _requestService.GetRequest(id);
            request.Reviews = reviews;
            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction("Summary", new { id });
        }

        public IActionResult Summary(int id)
        {
            return View(_requestService.GetRequest(id));
        }
    }
}
