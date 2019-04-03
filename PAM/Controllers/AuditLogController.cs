using Microsoft.AspNetCore.Mvc;
using PAM.Data;

namespace PAM.Controllers
{
    public class AuditLogController : Controller
    {
        private readonly AuditLogService _auditLog;

        public AuditLogController(AuditLogService auditLog)
        {
            _auditLog = auditLog;
        }

        public IActionResult ViewLog()
        {
            int days = 7;
            var recentLog = _auditLog.GetRecentEntries(days);

            ViewData["days"] = days;
            ViewData["recentLog"] = recentLog;
            return View();
        }
    }
}
