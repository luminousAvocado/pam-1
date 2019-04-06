using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;

namespace PAM.Controllers
{
    public class FormController : Controller
    {
        private readonly OrganizationService _organizationService;
        private readonly AuditLogService _auditLog;
        private readonly IMapper _mapper;
        private readonly ILogger<BureauController> _logger;

        public FormController(OrganizationService organizationService, AuditLogService auditLog, IMapper mapper, ILogger<BureauController> logger)
        {
            _organizationService = organizationService;
            _auditLog = auditLog;
            _mapper = mapper;
            _logger = logger;
        }


        public IActionResult Forms()
        {
            return View( _organizationService.GetForms() );
        }

        public IActionResult ViewForm()
        {
            return View();
        }
        public IActionResult AddForm()
        {
            return View();
        }
        public IActionResult EditForm()
        {
            return View();
        }
    }
}
