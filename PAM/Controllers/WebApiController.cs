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
        private readonly SystemService _systemService;

        public WebApiController(OrganizationService organizationService, SystemService systemService)
        {
            _organizationSevice = organizationService;
            _systemService = systemService;
        }

        [Route("api/portfolio")]
        public List<PAM.Models.System> GetSystemPortfolio()
        {
            /*
                Unit unit = _organizationSevice.GetUnit(unitId);
                var systems = new List<PAM.Models.System>();
                foreach (var unitSystem in unit.Systems)
                    systems.Add(unitSystem.System);
                    */
            var systems = (List<Models.System>)_systemService.GetAllSystems();
            return systems;
        }
    }
}
