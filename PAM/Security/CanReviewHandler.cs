using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PAM.Extensions;
using PAM.Models;

namespace PAM.Security
{
    public class CanReviewHandler : AuthorizationHandler<CanReviewRequirement, Review>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
          CanReviewRequirement requirement,
          Review resource)
        {
            var user = context.User;
            Debug.WriteLine("handler for review");

            if (Int32.Parse(((ClaimsIdentity)context.User.Identity).GetClaim("EmployeeId")) == resource.Reviewer.EmployeeId)
            {
                if (resource.Approved == null)
                {
                    Debug.WriteLine("this is still a draft ");
                    context.Succeed(requirement);
                    Debug.WriteLine("context succeeded");

                }
            }
            else { return; }
        }
    }
}
