using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Models;
using PAM.Services;

namespace PAM.Controllers
{
    [Authorize]
    public class TransferRequestController : AbstractEditRequestController
    {
        public TransferRequestController(UserService userService, RequestService requestService,
            SystemService systemService, OrganizationService orgnizationService, IAuthorizationService authService,
            TreeViewService treeViewService, ILogger<TransferRequestController> logger)
            : base(userService, requestService, systemService, orgnizationService, authService, treeViewService, logger)
        {
        }

        protected override string[] Steps { get; } = {
            nameof(RequesterInfo),
            nameof(TransferFromUnit),
            nameof(TransferToUnit),
            nameof(AdditionalInfo),
            nameof(Signatures),
            nameof(Summary)
        };

        [HttpGet]
        public async Task<IActionResult> TransferFromUnit(int id)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            ViewData["tree"] = _treeViewService.GenerateTreeInJson();
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> TransferFromUnit(int id, int unitId, bool saveDraft = false)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            if (request.TransferredFromUnitId != unitId)
            {
                request.TransferredFromUnitId = unitId;
                setRequestedSystems(request);
                _requestService.SaveChanges();
            }

            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction(GetNextStep(nameof(TransferFromUnit)), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> TransferToUnit(int id)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            ViewData["tree"] = _treeViewService.GenerateTreeInJson();
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> TransferToUnit(int id, int unitId, bool saveDraft = false)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            if (request.RequestedFor.UnitId != unitId)
            {
                request.RequestedFor.UnitId = unitId;
                setRequestedSystems(request);
                _requestService.SaveChanges();
            }

            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction(GetNextStep(nameof(TransferToUnit)), new { id });
        }

        private void setRequestedSystems(Request request)
        {
            if (request.TransferredFromUnitId == null || request.RequestedFor?.UnitId == null)
                return;

            request.Systems.Clear();
            var fromUnit = _organizationService.GetUnit((int)request.TransferredFromUnitId);
            var fromSystemIds = fromUnit.Systems.Select(s => s.SystemId);
            var toUnit = _organizationService.GetUnit((int)request.RequestedFor.UnitId);
            var toSystemIds = toUnit.Systems.Select(s => s.SystemId);

            request.Systems.AddRange(toUnit.Systems.Where(s => !fromSystemIds.Contains(s.SystemId))
                .Select(us => new RequestedSystem(request.RequestId, us.SystemId)
                {
                    AccessType = SystemAccessType.Add,
                    InPortfolio = true
                }).ToList());
            request.Systems.AddRange(fromUnit.Systems.Where(s => !toSystemIds.Contains(s.SystemId))
                .Select(us => new RequestedSystem(request.RequestId, us.SystemId)
                {
                    AccessType = SystemAccessType.Remove
                }).ToList());
        }
    }
}
