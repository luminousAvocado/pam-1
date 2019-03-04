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
using System;

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

            request.TransferredFromUnitId = unit.UnitId;
            request.TransferredFromUnit = unit;

            request.Systems.Clear();
            foreach (var us in unit.Systems)
                request.Systems.Add(new RequestedSystem(request.RequestId, us.SystemId));
            _requestService.SaveChanges();
            Console.WriteLine("HMMMM" + request.TransferredFromUnit.Bureau.Description);

            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction("UnitTransfer", new { id });
        }

        [HttpGet]
        public IActionResult UnitTransfer(int id){
            var request = _requestService.GetRequest(id);
            ViewData["tree"] = _treeViewService.GenerateTreeInJson();

            return View(request);
        }

        [HttpPost]
        public IActionResult UnitTransfer(int id, int transferUnitId, bool saveDraft = false){
            var request = _requestService.GetRequest(id);

            List<RequestedSystem> currentSystems = new List<RequestedSystem>(request.Systems);
            var transferUnit = _organizationService.GetUnit(transferUnitId);

            request.RequestedFor.BureauId = transferUnit.BureauId;
            request.RequestedFor.UnitId = transferUnit.UnitId;

            request.Systems.Clear();
            foreach(var cs in currentSystems){
                foreach(var ts in transferUnit.Systems){
                    if (ts.SystemId == cs.SystemId) {
                        //Keep matching systems
                        request.Systems.Add(new RequestedSystem(request.RequestId, cs.SystemId) { AccessType = SystemAccessType.Update });
                        break;
                    }
                    else if(transferUnit.Systems.IndexOf(ts) == transferUnit.Systems.Count - 1 ){
                        //Remove system not in new bureau
                        request.Systems.Add(new RequestedSystem(request.RequestId, cs.SystemId) { AccessType = SystemAccessType.Remove });
                    }
                }
            }
            foreach(var ts in transferUnit.Systems){
                foreach(var cs in currentSystems){
                    if (cs.SystemId == ts.SystemId) {
                        break;
                    }
                    else if(currentSystems.IndexOf(cs) == currentSystems.Count - 1 ){
                        request.Systems.Add(new RequestedSystem(request.RequestId, ts.SystemId) { AccessType = SystemAccessType.Add });
                    }
                }
            }
            _requestService.SaveChanges();

            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction("AdditionalInfo", new { id });
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
            var request = _requestService.GetRequest(id);
            int unitId = request.TransferredFromUnitId ?? default(int);
            var unit = _organizationService.GetUnit(unitId);
            request.TransferredFromUnit = unit;
            return View(_requestService.GetRequest(id));
        }
    }
}
