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
    public class LeavingProbationRequestController : AbstractEditRequestController
    {
        public LeavingProbationRequestController(UserService userService, FormService formService, RequestService requestService,
            SystemService systemService, OrganizationService orgnizationService, IAuthorizationService authService,
            TreeViewService treeViewService, ILogger<LeavingProbationRequestController> logger)
            : base(userService, formService, requestService, systemService, orgnizationService, authService, treeViewService, logger)
        {
        }

        protected override string[] Steps { get; } = {
            nameof(RequesterInfo),
            nameof(UnitSelection),
            nameof(AdditionalInfo),
            nameof(SystemsToRemove),
            nameof(Signatures),
            nameof(Summary)
        };

        [HttpPost]
        public override async Task<IActionResult> AdditionalInfo(int id, Request update, bool saveDraft = false)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            request.DepartureReason = update.DepartureReason;
            _requestService.SaveChanges();
            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction(nameof(SystemsToRemove), new { id });
        }
    }
}
