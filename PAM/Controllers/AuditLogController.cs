using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAM.Data;

namespace PAM.Controllers
{
    [Authorize("IsAdmin")]
    public class AuditLogController : Controller
    {
        private readonly AuditLogService _auditLog;

        public AuditLogController(AuditLogService auditLog)
        {
            _auditLog = auditLog;
        }

        public IActionResult SearchLog(DateTime? startDate, DateTime? endDate, string term)
        {
            if (startDate == null)
                startDate = DateTime.Now.AddDays(-7);

            ViewData["startDate"] = ((DateTime)startDate).ToString("yyyy-MM-dd");
            return View(_auditLog.Search((DateTime)startDate, endDate, term));
        }

        public IActionResult ViewLogEntry(int id)
        {
            return View(_auditLog.GetAuditLogEntry(id));
        }
    }
}
