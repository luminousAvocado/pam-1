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
    public class FilesController: Controller
    {
        private readonly OrganizationService _organizationService;
        private readonly ILogger _logger;

        public FilesController(OrganizationService organizationService, ILogger<PortfolioAssignmentRequestController> logger)
        {
            _organizationService = organizationService;
            _logger = logger;
        }

        public ActionResult DownloadFile(int id)
        {
            var fileData = _organizationService.GetFileById(id);
            Stream stream = new MemoryStream(fileData.Content);
            return File(stream, "application/pdf", fileData.Name + ".pdf");
        }

        [Route("ViewForm")]
        public FileStreamResult ViewFile(int id)
        {
            var fileData = _organizationService.GetFormById(id);
            Stream stream = new MemoryStream(fileData.File.Content);
            return new FileStreamResult(stream, fileData.Name);
        }
    }
}
