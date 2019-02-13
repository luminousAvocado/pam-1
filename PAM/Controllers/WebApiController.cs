using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAM.Data;
using PAM.Models;

namespace PAM.Controllers
{
    [ApiController]
    [Authorize]
    public class WebApiController : ControllerBase
    {
        private readonly OrganizationService _organizationSevice;

        public WebApiController(OrganizationService organizationService)
        {
            _organizationSevice = organizationService;
        }

        [Route("api/portfolio/{unitId}")]
        public List<PAM.Models.System> GetSystemPortfolio(int unitId)
        {
            Unit unit = _organizationSevice.GetUnit(unitId);
            var systems = new List<PAM.Models.System>();
            foreach (var unitSystem in unit.Systems)
                systems.Add(unitSystem.System);
            return systems;
        }
    }
}
