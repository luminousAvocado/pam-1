using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PAM.Data;
using PAM.Models;
using PAM.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace PAM.Controllers
{
    [Authorize]
    public class LeavingProbationRequestController : Controller
    {
        private readonly UserService _userService;
        private readonly RequestService _requestService;
        private readonly SystemService _systemService;
        private readonly TreeViewService _treeViewService;
        private readonly ILogger _logger;

        public LeavingProbationRequestController(UserService userService, RequestService requestService, SystemService systemService,
            TreeViewService treeViewService, ILogger<LeavingProbationRequestController> logger)
        {
            _userService = userService;
            _requestService = requestService;
            _systemService = systemService;
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
            request.RequestedFor.UnitId = unitId;
            _requestService.SaveChanges();

            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction(nameof(AdditionalInfo), new { id });
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
            request.DepartureReason = update.DepartureReason;
            _requestService.SaveChanges();
            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction(nameof(SystemsToRemove), new { id });
        }

        [HttpGet]
        public IActionResult SystemsToRemove(int id)
        {
            var request = _requestService.GetRequest(id);
            var systemAccesses = _systemService.GetCurrentSystemAccessesByEmployeeId(request.RequestedFor.EmployeeId);

            ViewData["systemAccesses"] = systemAccesses;
            ViewData["systems"] = JsonConvert.SerializeObject(_systemService.GetSystems());
            ViewData["requestedSystemIds"] = JsonConvert.SerializeObject(request.Systems.Select(s => s.SystemId).ToList());
            ViewData["accessSystemIds"] = JsonConvert.SerializeObject(systemAccesses.Select(s => s.SystemId).ToList());
            return View(request);
        }

        [HttpPost]
        public IActionResult SystemsToRemove(int id, List<int> systemIds, bool saveDraft = false)
        {
            var request = _requestService.GetRequest(id);
            var requestedSystems = request.Systems.ToDictionary(s => s.SystemId, s => s);

            foreach (var requestedSystemId in requestedSystems.Keys)
                if (!systemIds.Contains(requestedSystemId))
                    request.Systems.Remove(requestedSystems.GetValueOrDefault(requestedSystemId));

            foreach (var systemId in systemIds)
                if (!requestedSystems.Keys.Contains(systemId))
                    request.Systems.Add(new RequestedSystem(request.RequestId, systemId)
                    {
                        AccessType = SystemAccessType.Remove
                    });

            _requestService.SaveChanges();

            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction(nameof(Signatures), new { id });
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
                RedirectToAction(nameof(Summary), new { id });
        }

        public IActionResult Summary(int id)
        {
            return View(_requestService.GetRequest(id));
        }
    }
}
