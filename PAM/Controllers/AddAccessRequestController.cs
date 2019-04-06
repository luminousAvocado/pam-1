using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Services;

namespace PAM.Controllers
{
    [Authorize]
    public class AddAccessRequestController : AbstractEditRequestController
    {
        public AddAccessRequestController(UserService userService, RequestService requestService,
            SystemService systemService, OrganizationService orgnizationService, IAuthorizationService authService,
            TreeViewService treeViewService, ILogger<AddAccessRequestController> logger)
            : base(userService, requestService, systemService, orgnizationService, authService, treeViewService, logger)
        {
        }

        protected override string[] Steps { get; } = {
            nameof(RequesterInfo),
            nameof(UnitSelection),
            nameof(SystemsToAdd),
            nameof(AdditionalInfo),
            nameof(Signatures),
            nameof(Summary)
        };
    }
}
