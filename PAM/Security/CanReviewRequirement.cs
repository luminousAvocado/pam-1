using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace PAM.Security
{
    public class CanReviewRequirement : IAuthorizationRequirement
    {
        public CanReviewRequirement() { }
    }
}
