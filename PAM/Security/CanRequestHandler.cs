using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PAM.Models;
using PAM.Extensions;
using System.Security.Claims;
namespace PAM.Security
{
    public class CanRequestHandler :
        AuthorizationHandler<CanRequestRequirement, Request>
    {

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CanRequestRequirement requirement,
            Request resource)
        {
            var user = context.User;
            Debug.WriteLine("handler for request");

            Debug.WriteLine((Int32.Parse(((ClaimsIdentity)context.User.Identity).GetClaim("EmployeeId"))));
            Debug.WriteLine(resource.RequestedBy.EmployeeId);
            if (Int32.Parse(((ClaimsIdentity)context.User.Identity).GetClaim("EmployeeId")) == resource.RequestedBy.EmployeeId)
            {
                if (resource.RequestStatus.ToString().Equals("Draft"))
                {
                    Debug.WriteLine("this is still a draft ");
                    context.Succeed(requirement);
                    Debug.WriteLine("context succeeded");
                
                }
                Debug.WriteLine("this request has already been submitted");
            }
            else { return; }

        }
    }
}
