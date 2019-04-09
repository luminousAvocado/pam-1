using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PAM.Data;
using PAM.Extensions;
using PAM.Models;

namespace PAM.Controllers
{
    public class FormController : Controller
    {

        private readonly OrganizationService _organizationService;
        private readonly AuditLogService _auditLog;
        private readonly IMapper _mapper;
        private readonly ILogger<BureauController> _logger;
        private readonly SystemService _systemService;

        public FormController(SystemService systemService, OrganizationService organizationService, AuditLogService auditLog, IMapper mapper, ILogger<BureauController> logger)
        {
            _systemService = systemService;
            _organizationService = organizationService;
            _auditLog = auditLog;
            _mapper = mapper;
            _logger = logger;
        }


        public IActionResult Forms()
        {
            //var testForm = _organizationService.GetForm(6);
            //var testFile = _organizationService.GetFile("ad_contractor");

            //testForm.FileId = testFile.FileId;
            //_organizationService.SaveChanges();

           

            return View( _organizationService.GetForms() );
        }


        public IActionResult DetailsForm(int id)
        {
            ViewData["Systems"] = _systemService.GetSystemFormsByFormId(id);
            return View(_organizationService.GetForm(id));
        }

        [HttpGet]
        public IActionResult AddForm()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddForm(Form form,IFormFile file)
        {
           await Upload(file);
           _organizationService.AddForm(form);
           return RedirectToAction(nameof(DetailsForm), new { id = form.FormId});
        }

        [HttpGet, Authorize("IsAdmin")]
        public IActionResult EditForm(int id)
        {
            ViewData["Systems"] = _systemService.GetSystemFormsByFormId(id);
            return View(_organizationService.GetForm(id));
        }

        [HttpPost, Authorize("IsAdmin")]
        public async Task<IActionResult> EditForm(int id, Form update)
        {
            var form = _organizationService.GetForm(id);

            var oldValue = JsonConvert.SerializeObject(form);
            _mapper.Map(update, form);
            _organizationService.SaveChanges();


            var newValue = JsonConvert.SerializeObject(form);
            var identity = (ClaimsIdentity)User.Identity;
            await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Update, /*this needs to be changed to form*/LogResourceType.Bureau, form.FormId,
                $"{identity.GetClaim(ClaimTypes.Name)} updated bureau with id {form.FormId}", oldValue, newValue);

            return RedirectToAction(nameof(DetailsForm), new { id });
        }

        [Authorize("IsAdmin")]
        public async Task<IActionResult> RemoveForm(int id)
        {
            var form = _organizationService.GetForm(id);
            form.Deleted = true;
            
            _organizationService.SaveChanges();
            Debug.WriteLine(form.Deleted);
            var identity = (ClaimsIdentity)User.Identity;
            await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Remove, LogResourceType.Bureau, form.FormId,
                $"{identity.GetClaim(ClaimTypes.Name)} removed bureau with id {form.FormId}");

            return RedirectToAction(nameof(Forms));
        }

        public FileStreamResult ViewForm(int id)
        {
            var formData = _organizationService.GetForm(id);
            Stream stream = new MemoryStream(formData.File.Content);
            return new FileStreamResult(stream, formData.Name);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile fileUpload)
        {
            long size = fileUpload.Length;
            var filePath = Path.GetTempFileName();
            var pdfFileName = Path.GetFileNameWithoutExtension(fileUpload.FileName);
            if (fileUpload.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileUpload.CopyToAsync(stream);
                }
            }
            var saveFile = new PAM.Models.File
            {
                Name = pdfFileName,
                ContentType = fileUpload.ContentType,
                Length = fileUpload.Length,
            };
            using (var memoryStream = new MemoryStream())
            {
                await fileUpload.CopyToAsync(memoryStream);
                saveFile.Content = memoryStream.ToArray();
            }
            _organizationService.AddFile(saveFile);
            return Ok(new { size, filePath });
        }

        public ActionResult Download(int id)
        {
            var form = _organizationService.GetForm(id);
            var test = _organizationService.GetFileByName(form.File.Name);
            Stream stream = new MemoryStream(test.Content);
            return File(stream, "application/pdf", form.File.Name);
            //return new FileStreamResult(stream, test.ContentType);
        }
    }
}
