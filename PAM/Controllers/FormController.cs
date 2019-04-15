using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
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
    [Authorize]
    public class FormController : Controller
    {
        private readonly FormService _formService;
        private readonly SystemService _systemService;
        private readonly AuditLogService _auditLog;
        private readonly ILogger<FormController> _logger;

        public FormController(FormService formService, SystemService systemService, AuditLogService auditLog, ILogger<FormController> logger)
        {
            _formService = formService;
            _systemService = systemService;
            _auditLog = auditLog;
            _logger = logger;
        }

        public IActionResult Forms()
        {
            return View(_formService.GetForms());
        }

        public IActionResult ViewForm(int id)
        {
            return View(_formService.GetForm(id));
        }

        [HttpGet, Authorize("IsAdmin")]
        public IActionResult AddForm()
        {
            ViewData["systems"] = JsonConvert.SerializeObject(_systemService.GetSystems());
            return View(new Form());
        }

        [HttpPost, Authorize("IsAdmin")]
        public async Task<IActionResult> AddForm(Form form, List<int> systemIds, IFormFile uploadedFile)
        {
            var file = uploadedFile != null ? await saveFile(uploadedFile) : null;
            if (file != null) form.FileId = file.FileId;
            form = _formService.AddForm(form);
            if (systemIds.Count > 0)
            {
                foreach (var systemId in systemIds)
                    form.Systems.Add(new SystemForm(systemId, form.FormId));
                _formService.SaveChanges();
            }

            var identity = (ClaimsIdentity)User.Identity;
            await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Create, LogResourceType.Form, form.FormId,
                $"{identity.GetClaim(ClaimTypes.Name)} created form with id {form.FormId}");

            return RedirectToAction(nameof(ViewForm), new { id = form.FormId });
        }

        [HttpGet, Authorize("IsAdmin")]
        public IActionResult EditForm(int id)
        {
            ViewData["systems"] = JsonConvert.SerializeObject(_systemService.GetSystems());
            return View(_formService.GetForm(id));
        }

        [HttpPost, Authorize("IsAdmin")]
        public async Task<IActionResult> EditForm(int id, Form update, IFormFile uploadedFile)
        {
            var file = uploadedFile != null ? await saveFile(uploadedFile) : null;

            var form = _formService.GetForm(id);

            var oldValue = JsonConvert.SerializeObject(form, Formatting.Indented);
            form.Name = update.Name;
            form.DisplayOrder = update.DisplayOrder;
            form.ForEmployeeOnly = update.ForEmployeeOnly;
            form.ForContractorOnly = update.ForContractorOnly;
            if (file != null) form.FileId = file.FileId;
            _systemService.SaveChanges();
            var newValue = JsonConvert.SerializeObject(form, Formatting.Indented);

            var identity = (ClaimsIdentity)User.Identity;
            await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Update, LogResourceType.Form, form.FormId,
                $"{identity.GetClaim(ClaimTypes.Name)} updated form with id {form.FormId}", oldValue, newValue);

            return RedirectToAction(nameof(ViewForm), new { id });
        }

        public IActionResult DownloadForm(int id)
        {
            var form = _formService.GetForm(id);
            if (form.FileId == null)
                return NotFound();

            var file = _formService.GetFile((int)form.FileId);
            return File(file.Content, file.ContentType, file.Name);
        }

        [Authorize("IsAdmin")]
        public async Task<IActionResult> RemoveForm(int id)
        {
            var form = _formService.GetForm(id);
            form.Deleted = true;
            _formService.SaveChanges();

            var identity = (ClaimsIdentity)User.Identity;
            await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Remove, LogResourceType.Form, form.FormId,
                $"{identity.GetClaim(ClaimTypes.Name)} removed form with id {form.FormId}");

            return RedirectToAction(nameof(Forms));
        }

        private async Task<Models.File> saveFile(IFormFile uploadedFile)
        {
            var file = new Models.File()
            {
                Name = Path.GetFileName(uploadedFile.FileName),
                ContentType = uploadedFile.ContentType,
                Length = uploadedFile.Length
            };
            using (var memoryStream = new MemoryStream())
            {
                await uploadedFile.CopyToAsync(memoryStream);
                file.Content = memoryStream.ToArray();
            }

            file = _formService.AddFile(file);

            var identity = (ClaimsIdentity)User.Identity;
            await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Upload, LogResourceType.File, file.FileId,
                $"{identity.GetClaim(ClaimTypes.Name)} uploaded file with id {file.FileId}");

            return file;
        }
    }
}
