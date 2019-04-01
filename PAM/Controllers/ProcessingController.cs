using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FluentEmail.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Extensions;
using PAM.Services;

namespace PAM.Controllers
{
    [Authorize("IsProcessor")]
    public class ProcessingController : Controller
    {
        private readonly UserService _userService;
        private readonly RequestService _requestService;
        private readonly SystemService _systemService;
        private readonly OrganizationService _organizationService;
        private readonly IFluentEmail _email;
        private readonly EmailHelper _emailHelper;
        private readonly ILogger _logger;

        public ProcessingController(UserService userService, RequestService requestService, SystemService systemService,
            OrganizationService organizationService, IFluentEmail email, EmailHelper emailHelper, ILogger<ProcessingController> logger)
        {
            _userService = userService;
            _requestService = requestService;
            _systemService = systemService;
            _organizationService = organizationService;
            _email = email;
            _emailHelper = emailHelper;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult MyProcessings()
        {
            int processingUnitId = Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("ProcessingUnitId"));
            var systemAccesses = _systemService.GetCurrentSystemAccessesByProcessingUnitId(processingUnitId);
            var systemsToProcess = systemAccesses.Where(s => s.ProcessedOn == null).ToList();
            var systemsToConfirm = systemAccesses.Where(s => !systemsToProcess.Contains(s)).ToList();

            ViewData["systemAccesses"] = systemAccesses;
            ViewData["systemsToProcess"] = systemsToProcess.GroupBy(s => s.Request, s => s).ToDictionary(g => g.Key, g => g.ToList());
            ViewData["systemsToConfirm"] = systemsToConfirm.GroupBy(s => s.Request, s => s).ToDictionary(g => g.Key, g => g.ToList());
            return View();
        }

        public IActionResult ViewRequest(int id)
        {
            return View(_requestService.GetRequest(id));
        }

        public IActionResult ProcessSystemAccesses(List<int> systemAccessIds)
        {
            int employeeId = Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId"));
            var systemAccesses = _systemService.GetSystemAccesses(systemAccessIds);
            foreach (var systemAccess in systemAccesses)
            {
                systemAccess.ProcessedById = employeeId;
                systemAccess.ProcessedOn = DateTime.Now;
            }
            _systemService.SaveChanges();

            var systemAccessesByRequest = systemAccesses.GroupBy(s => s.Request, s => s).ToDictionary(g => g.Key, g => g.ToList());
            foreach (var request in systemAccessesByRequest.Keys)
            {
                string emailName = "RequestProcessed";
                var model = new
                {
                    _emailHelper.AppUrl,
                    _emailHelper.AppEmail,
                    Request = request,
                    SystemAccesses = systemAccessesByRequest.GetValueOrDefault(request)
                };
                string subject = _emailHelper.GetSubjectFromTemplate(emailName, model, _email.Renderer);
                string receipient = request.RequestedBy.Email;
                _email.To(receipient)
                    .Subject(subject)
                    .UsingTemplateFromFile(_emailHelper.GetBodyTemplateFile(emailName), model)
                    .Send();
            }
            return RedirectToAction(nameof(MyProcessings));
        }

        public IActionResult ConfirmSystemAccesses(List<int> systemAccessIds)
        {
            int employeeId = Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId"));
            var systemAccesses = _systemService.GetSystemAccesses(systemAccessIds);
            foreach (var systemAccess in systemAccesses)
            {
                systemAccess.ConfirmedById = employeeId;
                systemAccess.ConfirmedOn = DateTime.Now;
            }
            _systemService.SaveChanges();

            var systemAccessesByRequest = systemAccesses.GroupBy(s => s.Request, s => s).ToDictionary(g => g.Key, g => g.ToList());
            foreach (var request in systemAccessesByRequest.Keys)
            {
                string emailName = "RequestConfirmed";
                var model = new
                {
                    _emailHelper.AppUrl,
                    _emailHelper.AppEmail,
                    Request = request,
                    SystemAccesses = systemAccessesByRequest.GetValueOrDefault(request)
                };
                string subject = _emailHelper.GetSubjectFromTemplate(emailName, model, _email.Renderer);
                string receipient = request.RequestedBy.Email;
                _email.To(receipient)
                    .Subject(subject)
                    .UsingTemplateFromFile(_emailHelper.GetBodyTemplateFile(emailName), model)
                    .Send();
            }

            return RedirectToAction(nameof(MyProcessings));
        }
    }
}
