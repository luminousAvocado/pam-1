using System.Collections.Generic;
using System.Linq;
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
        private readonly FormService _formService;
        private readonly SystemService _systemService;
        private readonly OrganizationService _organizationSevice;
        private readonly AuditLogService _auditLog;

        public WebApiController(UserService userService, FormService formService, SystemService systemService,
            OrganizationService organizationService, AuditLogService auditLog)
        {
            _userService = userService;
            _formService = formService;
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
        [Route("api/supportUnit/{unitId}/employees/{employeeId}")]
        public async Task<IActionResult> RemoveEmployeeFromSupportUnit(int unitId, int employeeId)
        {
            var employee = _userService.GetEmployee(employeeId);
            if (employee.SupportUnitId == unitId)
            {
                employee.SupportUnitId = null;
                _userService.SaveChanges();

                var identity = (ClaimsIdentity)User.Identity;
                await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Update, LogResourceType.SupportUnit, unitId,
                    $"{identity.GetClaim(ClaimTypes.Name)} removed employee {employeeId} from support unit {unitId}");
            }
            return Ok();
        }

        [HttpDelete, Authorize("IsAdmin")]
        [Route("api/supportUnit/{unitId}/systems/{systemId}")]
        public async Task<IActionResult> RemoveSystemFromSupportUnit(int unitId, int systemId)
        {
            var system = _systemService.GetSystem(systemId);
            if (system.SupportUnitId == unitId)
            {
                system.SupportUnitId = null;
                _systemService.SaveChanges();

                var identity = (ClaimsIdentity)User.Identity;
                await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Update, LogResourceType.SupportUnit, unitId,
                    $"{identity.GetClaim(ClaimTypes.Name)} removed system {systemId} from support unit {unitId}");
            }
            return Ok();
        }

        [HttpPost, Authorize("IsAdmin")]
        [Route("api/system/{systemId}/form/{formId}")]
        public async Task<IActionResult> AddFormToSystem(int systemId, int formId)
        {
            var system = _systemService.GetSystem(systemId);
            if (!system.Forms.Any(f => f.FormId == formId))
            {
                system.Forms.Add(new SystemForm(systemId, formId));
                _systemService.SaveChanges();

                var identity = (ClaimsIdentity)User.Identity;
                await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Update, LogResourceType.System, systemId,
                    $"{identity.GetClaim(ClaimTypes.Name)} added form {formId} to system {systemId}");

                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete, Authorize("IsAdmin")]
        [Route("api/system/{systemId}/form/{formId}")]
        public async Task<IActionResult> RemoveFormFromSystem(int systemId, int formId)
        {
            var system = _systemService.GetSystem(systemId);
            if (system.Forms.Any(f => f.FormId == formId))
            {
                system.Forms.RemoveAll(f => f.FormId == formId);
                _systemService.SaveChanges();

                var identity = (ClaimsIdentity)User.Identity;
                await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Update, LogResourceType.System, systemId,
                    $"{identity.GetClaim(ClaimTypes.Name)} removed form {formId} from system {systemId}");

                return Ok();
            }
            return BadRequest();
        }

        [HttpPost, Authorize("IsAdmin")]
        [Route("api/form/{formId}/system/{systemId}")]
        public async Task<IActionResult> AddSystemToForm(int formId, int systemId)
        {
            var form = _formService.GetForm(formId);
            if (!form.Systems.Any(s => s.SystemId == systemId))
            {
                form.Systems.Add(new SystemForm(systemId, formId));
                _formService.SaveChanges();

                var identity = (ClaimsIdentity)User.Identity;
                await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Update, LogResourceType.Form, formId,
                    $"{identity.GetClaim(ClaimTypes.Name)} added system {systemId} to form {formId}");

                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete, Authorize("IsAdmin")]
        [Route("api/form/{formId}/system/{systemId}")]
        public async Task<IActionResult> RemoveSystemFromForm(int formId, int systemId)
        {
            var form = _formService.GetForm(formId);
            if (form.Systems.Any(s => s.SystemId == systemId))
            {
                form.Systems.RemoveAll(s => s.SystemId == systemId);
                _formService.SaveChanges();

                var identity = (ClaimsIdentity)User.Identity;
                await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Update, LogResourceType.Form, formId,
                    $"{identity.GetClaim(ClaimTypes.Name)} removed system {systemId} from form {formId}");

                return Ok();
            }
            return BadRequest();
        }
    }
}
