using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PAM.Data;
using PAM.Models;
using PAM.Services;

namespace PAM.Controllers
{
    // This is the super class for all the request workflow controllers like AddAccessRequestController,
    // RemoveAccessRequestController, PortfolioAssignmentRequestController and so on.
    public abstract class AbstractEditRequestController : Controller
    {
        protected readonly UserService _userService;
        protected readonly FormService _formService;
        protected readonly RequestService _requestService;
        protected readonly SystemService _systemService;
        protected readonly OrganizationService _organizationService;
        protected readonly IAuthorizationService _authService;
        protected readonly TreeViewService _treeViewService;
        protected readonly ILogger _logger;

        public AbstractEditRequestController(UserService userService, FormService formService, RequestService requestService, SystemService systemService,
            OrganizationService orgnizationService, IAuthorizationService authService, TreeViewService treeViewService, ILogger logger)
        {
            _userService = userService;
            _formService = formService;
            _requestService = requestService;
            _systemService = systemService;
            _organizationService = orgnizationService;
            _authService = authService;
            _treeViewService = treeViewService;
            _logger = logger;
        }

        protected abstract string[] Steps { get; }

        protected string GetNextStep(string currentStep)
        {
            var index = Array.IndexOf(Steps, currentStep);
            return index < 0 || index > Steps.Length - 1 ? currentStep : Steps[index + 1];
        }

        [HttpGet]
        public virtual async Task<IActionResult> RequesterInfo(int id)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            ViewData["request"] = request;
            return View(request.RequestedFor);
        }

        [HttpPost]
        public virtual async Task<IActionResult> RequesterInfo(int id, Requester requester, bool saveDraft = false)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            _userService.UpdateRequester(requester);
            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction(GetNextStep(nameof(RequesterInfo)), new { id });
        }

        [HttpGet]
        public virtual async Task<IActionResult> UnitSelection(int id)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            ViewData["tree"] = _treeViewService.GenerateTreeInJson();
            return View(request);
        }

        [HttpPost]
        public virtual async Task<IActionResult> UnitSelection(int id, int unitId, bool saveDraft = false)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            request.RequestedFor.UnitId = unitId;
            _requestService.SaveChanges();

            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction(GetNextStep(nameof(UnitSelection)), new { id });
        }

        [HttpGet]
        public virtual async Task<IActionResult> SystemsToAdd(int id)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            var systemAccesses = _systemService.GetCurrentSystemAccessesByEmployeeId(request.RequestedFor.EmployeeId);

            ViewData["systemAccesses"] = systemAccesses;
            ViewData["systems"] = JsonConvert.SerializeObject(_systemService.GetSystems());
            ViewData["requestedSystemIds"] = JsonConvert.SerializeObject(request.Systems.Select(s => s.SystemId).ToList());
            ViewData["accessSystemIds"] = JsonConvert.SerializeObject(systemAccesses.Select(s => s.SystemId).ToList());
            return View(request);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SystemsToAdd(int id, List<int> systemIds, bool saveDraft = false)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            var requestedSystems = request.Systems.ToDictionary(s => s.SystemId, s => s);

            foreach (var requestedSystemId in requestedSystems.Keys)
                if (!systemIds.Contains(requestedSystemId))
                    request.Systems.Remove(requestedSystems.GetValueOrDefault(requestedSystemId));

            foreach (var systemId in systemIds)
                if (!requestedSystems.Keys.Contains(systemId))
                    request.Systems.Add(new RequestedSystem(request.RequestId, systemId)
                    {
                        AccessType = SystemAccessType.Add,
                        InPortfolio = false
                    });

            _requestService.SaveChanges();

            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction(GetNextStep(nameof(SystemsToAdd)), new { id });
        }

        [HttpGet]
        public virtual async Task<IActionResult> SystemsToRemove(int id)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            var systemAccesses = _systemService.GetCurrentSystemAccessesByEmployeeId(request.RequestedFor.EmployeeId);

            ViewData["systemAccesses"] = systemAccesses;
            ViewData["systems"] = JsonConvert.SerializeObject(_systemService.GetSystems());
            ViewData["requestedSystemIds"] = JsonConvert.SerializeObject(request.Systems.Select(s => s.SystemId).ToList());
            ViewData["accessSystemIds"] = JsonConvert.SerializeObject(systemAccesses.Select(s => s.SystemId).ToList());
            return View(request);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SystemsToRemove(int id, List<int> systemIds, bool saveDraft = false)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

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
                RedirectToAction(GetNextStep(nameof(SystemsToRemove)), new { id });
        }

        [HttpGet]
        public virtual async Task<IActionResult> SystemsToUpdate(int id)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            var systemAccesses = _systemService.GetCurrentSystemAccessesByEmployeeId(request.RequestedFor.EmployeeId);

            ViewData["systemAccesses"] = systemAccesses;
            ViewData["systems"] = JsonConvert.SerializeObject(_systemService.GetSystems());
            ViewData["requestedSystemIds"] = JsonConvert.SerializeObject(request.Systems.Select(s => s.SystemId).ToList());
            ViewData["accessSystemIds"] = JsonConvert.SerializeObject(systemAccesses.Select(s => s.SystemId).ToList());
            return View(request);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SystemsToUpdate(int id, List<int> systemIds, bool saveDraft = false)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            var requestedSystems = request.Systems.ToDictionary(s => s.SystemId, s => s);

            foreach (var requestedSystemId in requestedSystems.Keys)
                if (!systemIds.Contains(requestedSystemId))
                    request.Systems.Remove(requestedSystems.GetValueOrDefault(requestedSystemId));

            foreach (var systemId in systemIds)
                if (!requestedSystems.Keys.Contains(systemId))
                    request.Systems.Add(new RequestedSystem(request.RequestId, systemId)
                    {
                        AccessType = SystemAccessType.Update
                    });

            _requestService.SaveChanges();

            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction(GetNextStep(nameof(SystemsToUpdate)), new { id });
        }

        [HttpGet]
        public virtual async Task<IActionResult> AdditionalInfo(int id)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            return View(request);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AdditionalInfo(int id, Request update, bool saveDraft = false)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            request.IsContractor = update.IsContractor;
            request.IsGlobalAccess = update.IsGlobalAccess;
            request.IsHighProfileAccess = update.IsHighProfileAccess;
            request.CaseloadType = update.CaseloadType;
            request.CaseloadFunction = update.CaseloadFunction;
            request.CaseloadNumber = update.CaseloadNumber;
            _requestService.SaveChanges();

            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction(GetNextStep(nameof(AdditionalInfo)), new { id });
        }

        [HttpGet]
        public virtual async Task<IActionResult> Forms(int id)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            var formIds = new HashSet<int>();
            foreach (var system in request.Systems)
                if (system.AccessType == SystemAccessType.Add)
                    formIds.UnionWith(system.System.Forms.Select(f => f.FormId).ToHashSet());

            var forms = _formService.GetForms(formIds);
            forms.RemoveAll(f => request.IsContractor && f.ForEmployeeOnly || !request.IsContractor && f.ForContractorOnly);

            request.Forms.RemoveAll(rf => !forms.Select(f => f.FormId).Contains(rf.FormId));
            forms.RemoveAll(f => request.Forms.Select(rf => rf.FormId).Contains(f.FormId));

            foreach (var form in forms)
            {
                request.Forms.Add(new CompletedForm()
                {
                    RequestId = id,
                    Form = form
                });
            }
            _requestService.SaveChanges();

            return View(request);
        }

        [HttpGet]
        public virtual async Task<IActionResult> Signatures(int id)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            var reviews = request.OrderedReviews;
            ViewData["request"] = request;
            return View(reviews);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Signatures(int id, List<Review> reviews, bool saveDraft)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            request.Reviews = reviews;
            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction(GetNextStep(nameof(Signatures)), new { id });
        }

        public virtual async Task<IActionResult> Summary(int id)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            return View(request);
        }
    }
}
