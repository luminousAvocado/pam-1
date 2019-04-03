using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Models;
using PAM.Services;

namespace PAM.Controllers
{
    [Authorize]
    public class FormsController : Controller
    {
        private readonly FormService _formService;
        private readonly ILogger _logger;

        public FormsController(FormService formService, ILogger<PortfolioAssignmentRequestController> logger)
        {
            _formService = formService;
            _logger = logger;
        }

        public ActionResult DownloadForm(int id)
        {
            var formData = _formService.GetFormById(id);
            Stream stream = new MemoryStream(formData.File.Content);
            return File(stream, "application/pdf", formData.Name);
        }

        [Route("ViewForm")]
        public FileStreamResult ViewForm(int id)
        {
            var formData = _formService.GetFormById(id);
            Stream stream = new MemoryStream(formData.File.Content);
            return new FileStreamResult(stream, formData.Name);
        }
    }
}
