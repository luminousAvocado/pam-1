using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Services;

namespace PAM.Controllers
{
    [Authorize]
    public class UpdateInfoRequestController : AbstractEditRequestController
    {
        public UpdateInfoRequestController(UserService userService, RequestService requestService,
            SystemService systemService, OrganizationService orgnizationService, IAuthorizationService authService,
            TreeViewService treeViewService, ILogger<UpdateInfoRequestController> logger)
            : base(userService, requestService, systemService, orgnizationService, authService, treeViewService, logger)
        {
        }

        protected override string[] Steps { get; } = {
            nameof(RequesterInfo),
            nameof(UnitSelection),
            nameof(AdditionalInfo),
            nameof(SystemsToUpdate),
            nameof(Signatures),
            nameof(Summary)
        };
    }
}
