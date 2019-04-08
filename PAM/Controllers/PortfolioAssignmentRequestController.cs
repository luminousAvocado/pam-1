using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Models;
using PAM.Services;

namespace PAM.Controllers
{
    [Authorize]
    public class PortfolioAssignmentRequestController : Controller
    {
        private readonly UserService _userService;
        private readonly RequestService _requestService;
        private readonly OrganizationService _organizationService;
        private readonly TreeViewService _treeViewService;
        private readonly ILogger _logger;

        public PortfolioAssignmentRequestController(UserService userService, RequestService requestService,
            OrganizationService organizationService, TreeViewService treeViewService, ILogger<PortfolioAssignmentRequestController> logger)
        {
            _userService = userService;
            _requestService = requestService;
            _organizationService = organizationService;
            _treeViewService = treeViewService;
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
        public IActionResult RequesterInfo(int id, Requester requester, bool saveDraft = false)
        {
            _userService.UpdateRequester(requester);
            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction(nameof(UnitSelection), new { id });
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

            request.RequestedFor.UnitId = unit.UnitId;
            request.Systems.Clear();
            foreach (var us in unit.Systems)
                request.Systems.Add(new RequestedSystem(request.RequestId, us.SystemId)
                {
                    InPortfolio = true,
                });
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
            request.CaseloadType = update.CaseloadType;
            request.CaseloadFunction = update.CaseloadFunction;
            request.CaseloadNumber = update.CaseloadNumber;
            _requestService.SaveChanges();
            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction("Forms", new { id });
        }

        [HttpGet]
        public IActionResult Forms(int id)
        {
            var request = _requestService.GetRequest(id);
            var allForms = _organizationService.GetAllForms();
            /*
            var requestedSystems = request.Systems;
            foreach(var rs in requestedSystems){
                if(rs.SystemId)
            }
            */
            ViewData["request"] = request;
            ViewData["requiredForms"] = allForms;
            return View(request);
        }

        [HttpPost]
        public IActionResult Forms(int id,bool saveDraft)
        {
            var request = _requestService.GetRequest(id);
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
