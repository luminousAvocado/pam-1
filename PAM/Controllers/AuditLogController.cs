using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PAM.Controllers
{
    public class AuditLogController : Controller
    {
        public AuditLogController()
        {

        }

        public IActionResult ListLogs()
        {
            return View();
        }
    }
}
