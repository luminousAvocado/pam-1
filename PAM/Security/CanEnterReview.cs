using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PAM.Extensions;
using PAM.Models;

namespace PAM.Security
{
    public class CanEnterReviewRequirement : IAuthorizationRequirement
    {
    }

    public class CanEnterReviewHandler : AuthorizationHandler<CanEnterReviewRequirement, Review>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanEnterReviewRequirement requirement, Review review)
        {
            var identity = (ClaimsIdentity)context.User.Identity;

            if (!review.Completed && identity.GetClaimAsInt("EmployeeId") == review.ReviewerId)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
