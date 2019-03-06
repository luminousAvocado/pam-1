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

        [Route("api/portfolio/{unitId}")]
        public List<Models.System> GetSystemPortfolio(int unitId)
        {
            Unit unit = _organizationSevice.GetUnit(unitId);
            var systems = new List<Models.System>();
            foreach (var unitSystem in unit.Systems)
                systems.Add(unitSystem.System);
            return systems;
        }

        [Route("api/portfolio")]
        public List<Models.System> GetAllSystemPortfolios()
        {
            var systems = new List<Models.System>();
            var allSystems = _organizationSevice.GetAllSystems();
            foreach (var system in allSystems)
                systems.Add(system);
            return systems;
        }
    }
}
