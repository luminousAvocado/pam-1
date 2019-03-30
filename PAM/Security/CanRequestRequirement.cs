using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace PAM.Security
{
    public class CanRequestRequirement : IAuthorizationRequirement 
    {
        //public int UserId { get;  }
        //public int ResourceId { get; }
        public CanRequestRequirement() { }

    }
}
