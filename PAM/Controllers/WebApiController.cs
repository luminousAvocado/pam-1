﻿using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAM.Data;
using PAM.Extensions;
using PAM.Models;

namespace PAM.Controllers
{
    [ApiController, Authorize]
    public class WebApiController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly SystemService _systemService;
        private readonly OrganizationService _organizationSevice;
        private readonly AuditLogService _auditLog;

        public WebApiController(UserService userService, SystemService systemService, OrganizationService organizationService,
            AuditLogService auditLog)
        {
            _userService = userService;
            _systemService = systemService;
            _organizationSevice = organizationService;
            _auditLog = auditLog;
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

        [Route("api/employee/search"), Authorize("IsAdmin")]
        public IList<Employee> SearchEmployees(string term)
        {
            return _userService.SearchEmployees(term);
        }

        [HttpDelete, Authorize("IsAdmin")]
        [Route("api/processingUnit/{unitId}/employees/{employeeId}")]
        public async Task<IActionResult> RemoveEmployeeFromProcessingUnit(int unitId, int employeeId)
        {
            var employee = _userService.GetEmployee(employeeId);
            if (employee.ProcessingUnitId == unitId)
            {
                employee.ProcessingUnitId = null;
                _userService.SaveChanges();

                var identity = (ClaimsIdentity)User.Identity;
                await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Update, LogResourceType.ProcessingUnit, unitId,
                    $"{identity.GetClaim(ClaimTypes.Name)} removed employee {employeeId} from processing unit {unitId}");
            }
            return Ok();
        }

        [HttpDelete, Authorize("IsAdmin")]
        [Route("api/processingUnit/{unitId}/systems/{systemId}")]
        public async Task<IActionResult> RemoveSystemFromProcessingUnit(int unitId, int systemId)
        {
            var system = _systemService.GetSystem(systemId);
            if (system.ProcessingUnitId == unitId)
            {
                system.ProcessingUnitId = null;
                _systemService.SaveChanges();

                var identity = (ClaimsIdentity)User.Identity;
                await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Update, LogResourceType.ProcessingUnit, unitId,
                    $"{identity.GetClaim(ClaimTypes.Name)} removed system {systemId} from processing unit {unitId}");
            }
            return Ok();
        }
    }
}
