using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;

namespace PAM.Controllers
{
    public class AuditLogController : Controller
    {
        private readonly AuditLogService _auditService;
        private readonly ILogger<AuditLogController> _logger;

        public AuditLogController(AuditLogService auditService, ILogger<AuditLogController> logger)
        {
            _auditService = auditService;
            _logger = logger;
        }

        public IActionResult ListLogs()
        {
            return View(_auditService.GetAllLogs());
        } 
    }
}
