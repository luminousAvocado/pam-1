using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PAM.Extensions;
using PAM.Models;

namespace PAM.Security
{
    public class CanEditRequestRequirement : IAuthorizationRequirement
    {
    }

    public class CanEditRequestHandler : AuthorizationHandler<CanEditRequestRequirement, Request>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanEditRequestRequirement requirement, Request request)
        {
            var identity = (ClaimsIdentity)context.User.Identity;

            if (request.RequestStatus == RequestStatus.Draft && identity.GetClaimAsInt("EmployeeId") == request.RequestedBy.EmployeeId)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
