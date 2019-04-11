using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Models;
using PAM.Services;

namespace PAM.Controllers
{
    [Authorize]
    public class TransferRequestController : Controller
    {
        private readonly UserService _userService;
        private readonly RequestService _requestService;
        private readonly OrganizationService _organizationService;
        private readonly TreeViewService _treeViewService;
        private readonly ILogger _logger;

        public TransferRequestController(UserService userService, RequestService requestService,
            OrganizationService organizationService, TreeViewService treeViewService, ILogger<TransferRequestController> logger)
        {
            _userService = userService;
            _requestService = requestService;
            _organizationService = organizationService;
            _treeViewService = treeViewService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult RequesterInfo(int id)
        {
            var request = _requestService.GetRequest(id);
            ViewData["request"] = request;
            return View(request.RequestedFor);
        }

        [HttpPost]
        public IActionResult RequesterInfo(int id, Requester requester, bool saveDraft = false)
        {
            _userService.UpdateRequester(requester);
            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction(nameof(TransferFromUnit), new { id });
        }

        [HttpGet]
        public IActionResult TransferFromUnit(int id)
        {
            var request = _requestService.GetRequest(id);
            ViewData["tree"] = _treeViewService.GenerateTreeInJson();
            return View(request);
        }

        [HttpPost]
        public IActionResult TransferFromUnit(int id, int unitId, bool saveDraft = false)
        {
            var request = _requestService.GetRequest(id);
            if (request.TransferredFromUnitId != unitId)
            {
                request.TransferredFromUnitId = unitId;
                setRequestedSystems(request);
                _requestService.SaveChanges();
            }

            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction(nameof(TransferToUnit), new { id });
        }

        [HttpGet]
        public IActionResult TransferToUnit(int id)
        {
            var request = _requestService.GetRequest(id);
            ViewData["tree"] = _treeViewService.GenerateTreeInJson();
            return View(request);
        }

        [HttpPost]
        public IActionResult TransferToUnit(int id, int unitId, bool saveDraft = false)
        {
            var request = _requestService.GetRequest(id);
            if (request.RequestedFor.UnitId != unitId)
            {
                request.RequestedFor.UnitId = unitId;
                setRequestedSystems(request);
                _requestService.SaveChanges();
            }

            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction("AdditionalInfo", new { id });
        }

        [HttpGet]
        public IActionResult AdditionalInfo(int id)
        {
            return View(_requestService.GetRequest(id));
        }

        [HttpPost]
        public IActionResult AdditionalInfo(int id, Request update, bool saveDraft = false)
        {
            var request = _requestService.GetRequest(id);
            request.IsContractor = update.IsContractor;
            request.CaseloadType = update.CaseloadType;
            request.CaseloadFunction = update.CaseloadFunction;
            request.CaseloadNumber = update.CaseloadNumber;
            _requestService.SaveChanges();
            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction("Forms", new { id });
        }

        [HttpGet]
        public IActionResult Forms(int id)
        {
            var request = _requestService.GetRequest(id);

            var formAssociations = _organizationService.GetFormAssociations();
            var requestedSystems = request.Systems;
            var requiredForms = new List<Form>();

            foreach(var rs in requestedSystems){
                foreach(var fa in formAssociations){
                    if(rs.SystemId == fa.SystemId){
                        if (requiredForms.Exists(f => f.FormId == fa.FormId)) continue;
                        else requiredForms.Add(_organizationService.GetFormById(fa.FormId));
                    }
                }
            }

            if (!request.IsContractor)
                requiredForms.RemoveAll(f => f.ForContractorOnly);
            else
                requiredForms.RemoveAll(f => f.ForEmployeeOnly);

            ViewData["request"] = request;
            ViewData["requiredForms"] = requiredForms;
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Forms(int id, List<IFormFile> completedFiles, bool saveDraft)
        {
            var request = _requestService.GetRequest(id);
            request.Forms.Clear();
            var formAssociations = _organizationService.GetFormAssociations();
            var requestedSystems = request.Systems;
            var requiredForms = new List<Form>();
            var filledForms = new List<FilledForm>();

            foreach(var rs in requestedSystems){
                foreach(var fa in formAssociations){
                    if(rs.SystemId == fa.SystemId){
                        if (requiredForms.Exists(f => f.FormId == fa.FormId)) continue;
                        else requiredForms.Add(_organizationService.GetFormById(fa.FormId));
                    }
                }
            }

            if (!request.IsContractor)
                requiredForms.RemoveAll(f => f.ForContractorOnly);
            else
                requiredForms.RemoveAll(f => f.ForEmployeeOnly);

            foreach (var cr in completedFiles.Zip(requiredForms, Tuple.Create))
            {
                long size = cr.Item1.Length;

                var filePath = Path.GetTempFileName();
                var pdfFileName = Path.GetFileNameWithoutExtension(cr.Item1.FileName);

                if (cr.Item1.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await cr.Item1.CopyToAsync(stream);
                    }
                }

                var saveFile = new Models.File
                {
                    Name = pdfFileName,
                    ContentType = cr.Item1.ContentType,
                    Length = cr.Item1.Length,
                };

                using (var memoryStream = new MemoryStream())
                {
                    await cr.Item1.CopyToAsync(memoryStream);
                    saveFile.Content = memoryStream.ToArray();
                }

                var file = _organizationService.AddFile(saveFile);
                var form = _organizationService.GetFormById(cr.Item2.FormId);

                filledForms.Add(new FilledForm(request.RequestId, form.FormId, file.FileId));
            }
            request.Forms = filledForms;
            _requestService.SaveChanges();
            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction("Signatures", new { id });
        }

        [HttpGet]
        public IActionResult Signatures(int id)
        {
            var request = _requestService.GetRequest(id);
            var reviews = request.OrderedReviews;
            ViewData["request"] = request;
            return View(reviews);
        }

        [HttpPost]
        public IActionResult Signatures(int id, List<Review> reviews, bool saveDraft)
        {
            var request = _requestService.GetRequest(id);
            request.Reviews = reviews;
            return saveDraft ? RedirectToAction("MyRequests", "Request") :
                RedirectToAction("Summary", new { id });
        }

        public IActionResult Summary(int id)
        {
            return View(_requestService.GetRequest(id));
        }

        private void setRequestedSystems(Request request)
        {
            if (request.TransferredFromUnitId == null || request.RequestedFor?.UnitId == null)
                return;

            request.Systems.Clear();
            var fromUnit = _organizationService.GetUnit((int)request.TransferredFromUnitId);
            var fromSystemIds = fromUnit.Systems.Select(s => s.SystemId);
            var toUnit = _organizationService.GetUnit((int)request.RequestedFor.UnitId);
            var toSystemIds = toUnit.Systems.Select(s => s.SystemId);

            request.Systems.AddRange(toUnit.Systems.Where(s => !fromSystemIds.Contains(s.SystemId))
                .Select(us => new RequestedSystem(request.RequestId, us.SystemId)
                {
                    AccessType = SystemAccessType.Add,
                    InPortfolio = true
                }).ToList());
            request.Systems.AddRange(fromUnit.Systems.Where(s => !toSystemIds.Contains(s.SystemId))
                .Select(us => new RequestedSystem(request.RequestId, us.SystemId)
                {
                    AccessType = SystemAccessType.Remove
                }).ToList());
        }
    }
}
