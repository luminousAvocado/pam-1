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
    public class PortfolioAssignmentRequestController : AbstractEditRequestController
    {
        public PortfolioAssignmentRequestController(UserService userService, FormService formService, RequestService requestService,
            SystemService systemService, OrganizationService orgnizationService, IAuthorizationService authService,
            TreeViewService treeViewService, ILogger<PortfolioAssignmentRequestController> logger)
            : base(userService, formService, requestService, systemService, orgnizationService, authService, treeViewService, logger)
        {
        }

        protected override string[] Steps { get; } = {
            nameof(RequesterInfo),
            nameof(UnitSelection),
            nameof(AdditionalInfo),
            nameof(Forms),
            nameof(Signatures),
            nameof(Summary)
        };

        [HttpPost]
        public override async Task<IActionResult> UnitSelection(int id, int unitId, bool saveDraft = false)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

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
                RedirectToAction(GetNextStep(nameof(UnitSelection)), new { id });
        }
    }
}
