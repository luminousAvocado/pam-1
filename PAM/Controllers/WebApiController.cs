﻿using System;
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
        private readonly UserService _userService;
        private readonly SystemService _systemService;
        private readonly OrganizationService _organizationSevice;
        private readonly AuditLogService _auditService;

        public WebApiController(UserService userService, SystemService systemService, OrganizationService organizationService, AuditLogService auditService)
        {
            _userService = userService;
            _systemService = systemService;
            _organizationSevice = organizationService;
            _auditService = auditService;
        }

        [Route("api/auditLogs/{start:datetime}/{end:datetime}")]
        public ICollection<AuditLog> GetAuditLogsByDate(DateTime start, DateTime end)
        {
            return _auditService.GetLogsInRange(start, end); 
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

        [Route("api/employee/search")]
        public IList<Employee> SearchEmployees(string term)
        {
            return _userService.SearchEmployees(term);
        }

        [HttpDelete]
        [Route("api/processingUnit/{unitId}/employees/{employeeId}")]
        public IActionResult RemoveEmployeeFromProcessingUnit(int unitId, int employeeId)
        {
            var employee = _userService.GetEmployee(employeeId);
            if (employee.ProcessingUnitId == unitId)
            {
                employee.ProcessingUnitId = null;
                _userService.SaveChanges();
            }
            return Ok();
        }

        [HttpDelete]
        [Route("api/processingUnit/{unitId}/systems/{systemId}")]
        public IActionResult RemoveSystemFromProcessingUnit(int unitId, int systemId)
        {
            var system = _systemService.GetSystem(systemId);
            if (system.ProcessingUnitId == unitId)
            {
                system.ProcessingUnitId = null;
                _systemService.SaveChanges();
            }
            return Ok();
        }
    }
}
