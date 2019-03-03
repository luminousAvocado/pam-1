using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Models;
using PAM.Services;

namespace PAM.Controllers
{
    public class LeavingProbationRequestController : Controller
    {
        private readonly IADService _adService;
        private readonly UserService _userService;
        private readonly RequestService _requestService;
        private readonly SystemService _systemService;
        private readonly OrganizationService _organizationService;
        private readonly TreeViewService _treeViewService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public LeavingProbationRequestController(IADService adService, UserService userService, RequestService requestService, SystemService systemService,
            OrganizationService organizationService, TreeViewService treeViewService, IMapper mapper,
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
        public IActionResult ReasonForLeaving(int id)
        {
            var request = _requestService.GetRequest(id);
            return View(request);
        }

        [HttpPost]
        public IActionResult ReasonForLeaving(int id, DepartureReason selectedReason, bool saveDraft = false)
        {
            var request = _requestService.GetRequest(id);
            request.DepartureReason = selectedReason;
            var requestFor = _userService.GetRequester(request.RequestedForId);
            var systemAccesses = _systemService.GetSystemAccessesByEmployeeId(requestFor.EmployeeId);
            foreach (var sa in systemAccesses)
            {
                var temp = new RequestedSystem(request.RequestId, sa.SystemId);
                temp.AccessType = SystemAccessType.Remove;
                request.Systems.Add(temp);
            }
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
