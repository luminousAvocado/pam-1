using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Models;

namespace PAM.Controllers
{
    public class UpdateInfoRequestController : Controller
    {
        private readonly UserService _userService;
        private readonly RequestService _requestService;
        private readonly SystemService _systemService;
        private readonly OrganizationService _organizationService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UpdateInfoRequestController(UserService userService, RequestService requestService, SystemService systemService, OrganizationService organizationService, IMapper mapper, ILogger<UpdateInfoRequestController> logger)
        {
            _userService = userService;
            _requestService = requestService;
            _systemService = systemService;
            _organizationService = organizationService;
            _mapper = mapper;
            _logger = logger;
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
                RedirectToAction("SystemsToUpdate", new { id });
        }

        [HttpGet]
        public IActionResult SystemsToUpdate(int id)
        {
            var request = _requestService.GetRequest(id);
            var requestFor = _userService.GetRequester(request.RequestedForId);
            var systemAccesses = _systemService.GetSystemAccessesByEmployeeId(requestFor.EmployeeId);
            ViewData["systemAccesses"] = systemAccesses;
            return View(request);
        }

        [HttpPost]
        public IActionResult SystemsToUpdate(int id, int[] selectedSystems, bool saveDraft = false)
        {
            var request = _requestService.GetRequest(id);

            foreach (var systemId in selectedSystems)
            {
                var temp = new RequestedSystem(request.RequestId, systemId);
                temp.AccessType = SystemAccessType.Update;
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
