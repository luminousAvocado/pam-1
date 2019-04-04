using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PAM.Extensions;
using PAM.Models;

namespace PAM.Security
{
    public class CanViewRequestRequirement : IAuthorizationRequirement
    {
    }

    public class CanViewRequestHandler : AuthorizationHandler<CanViewRequestRequirement, Request>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanViewRequestRequirement requirement, Request request)
        {
            var identity = (ClaimsIdentity)context.User.Identity;

            int currentUserId = identity.GetClaimAsInt("EmployeeId");
            bool isRequestedBy = currentUserId == request.RequestedBy.EmployeeId;
            bool isRequester = isRequestedBy || currentUserId == request.RequestedFor.EmployeeId;
            bool hasRequiredClaim = identity.HasClaim(c => c.Type == "IsAdmin") || identity.HasClaim(c => c.Type == "ProcessingUnitId");
            bool isReviewer = hasRequiredClaim || request.Reviews.Any(r => r.ReviewerId == currentUserId);

            if (request.RequestStatus == RequestStatus.Draft && isRequestedBy
                || request.RequestStatus != RequestStatus.Draft && (isRequester || hasRequiredClaim || isReviewer))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
