using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Services;

namespace PAM.Controllers
{
    [Authorize]
    public class RemoveAccessRequestController : AbstractEditRequestController
    {
        public RemoveAccessRequestController(UserService userService, FormService formService, RequestService requestService,
            SystemService systemService, OrganizationService orgnizationService, IAuthorizationService authService,
            TreeViewService treeViewService, ILogger<RemoveAccessRequestController> logger)
            : base(userService, formService, requestService, systemService, orgnizationService, authService, treeViewService, logger)
        {
        }

        protected override string[] Steps { get; } = {
            nameof(RequesterInfo),
            nameof(UnitSelection),
            nameof(SystemsToRemove),
            nameof(Signatures),
            nameof(Summary)
        };
    }
}
