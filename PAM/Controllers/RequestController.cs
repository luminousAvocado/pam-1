﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FluentEmail.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Extensions;
using PAM.Models;
using PAM.Services;

namespace PAM.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly IADService _adService;
        private readonly UserService _userService;
        private readonly RequestService _requestService;
        private readonly FormService _formService;
        private readonly SystemService _systemService;
        private readonly OrganizationService _organizationService;
        private readonly AuditLogService _auditLog;
        private readonly IAuthorizationService _authService;
        private readonly IFluentEmail _email;
        private readonly EmailHelper _emailHelper;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public RequestController(IADService adService, UserService userService, RequestService requestService, FormService formService,
            SystemService systemService, OrganizationService orgnizationService, AuditLogService auditLog, IAuthorizationService authService,
            IFluentEmail email, EmailHelper emailHelper, IMapper mapper, ILogger<RequestController> logger)
        {
            _adService = adService;
            _userService = userService;
            _requestService = requestService;
            _formService = formService;
            _systemService = systemService;
            _organizationService = orgnizationService;
            _auditLog = auditLog;
            _authService = authService;
            _email = email;
            _emailHelper = emailHelper;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult CreateRequest()
        {
            return View(_requestService.GetRequestTypes());
        }

        [HttpPost]
        public IActionResult CreateRequest(string forSelf, string requestedForUsername, int requestTypeId)
        {
            Employee employee = new Employee((ClaimsIdentity)User.Identity);
            Requester requestedBy = _mapper.Map<Requester>(employee);
            requestedBy = _userService.CreateRequester(requestedBy);

            Requester requestedFor;
            if (forSelf.Equals("yes"))
            {
                requestedFor = requestedBy;
            }
            else if (requestedForUsername != null)
            {
                employee = _adService.GetEmployeeByUsername(requestedForUsername);
                employee = _userService.HasEmployee(requestedForUsername) ?
                    _userService.UpdateEmployee(employee) : _userService.CreateEmployee(employee);
                requestedFor = _mapper.Map<Requester>(employee);
                requestedFor = _userService.CreateRequester(requestedFor);
            }
            else
            {
                // If RequestedFor doesn't have an AD account, we need their username, email, etc.
                // to create a PAM Employee record for them. This function is not implemented yet.
                throw new NotImplementedException();
            }

            Request request = new Request();
            request.RequestedBy = requestedBy;
            request.RequestedFor = requestedFor;
            request.RequestTypeId = requestTypeId;

            request.Reviews = new List<Review>();
            var requestType = _requestService.GetRequestType(requestTypeId);
            var requiredSignatures = requestType.RequiredSignatures.OrderBy(s => s.Order).ToList();
            for (int i = 0; i < requiredSignatures.Count; ++i)
            {
                var review = new Review
                {
                    ReviewOrder = i,
                    ReviewerTitle = requiredSignatures[i].Title
                };
                if (review.ReviewerTitle.Equals("Supervisor"))
                {
                    Employee supervisor = _adService.GetEmployeeByName(request.RequestedFor.SupervisorName);
                    supervisor = _userService.HasEmployee(supervisor.Username) ?
                        _userService.UpdateEmployee(supervisor) : _userService.CreateEmployee(supervisor);
                    supervisor.IsApprover = true;
                    _userService.SaveChanges();
                    review.ReviewerId = supervisor.EmployeeId;
                }
                request.Reviews.Add(review);
            }

            request = _requestService.CreateRequest(request);

            _logger.LogInformation($"User {User.Identity.Name} created request {request.RequestId}.");

            return RedirectEditRequest(request.RequestId, requestType);
        }

        public async Task<IActionResult> ViewRequest(int id)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanViewRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            return View(request);
        }

        public async Task<IActionResult> EditRequest(int id)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            return RedirectEditRequest(id, request.RequestType);
        }

        [HttpPost]
        public async Task<IActionResult> UploadCompletedForm(int id, int completedFormId, [FromForm(Name = "file")] IFormFile uploadedFile)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            foreach (var completedForm in request.Forms)
                if (completedForm.CompletedFormId == completedFormId)
                {
                    completedForm.File = await saveFile(uploadedFile);
                    _requestService.SaveChanges();
                    break;
                }

            return Ok();
        }

        public async Task<IActionResult> DownloadCompletedForm(int id, int completedFormId)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanViewRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            foreach (var completedForm in request.Forms)
                if (completedForm.CompletedFormId == completedFormId)
                {
                    if (completedForm.FileId == null) break;
                    else
                    {
                        var file = _formService.GetFile((int)completedForm.FileId);
                        return File(file.Content, file.ContentType, file.Name);
                    }
                }

            return NotFound();
        }

        public async Task<IActionResult> SubmitRequest(int id)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            request.RequestStatus = RequestStatus.UnderReview;
            request.SubmittedOn = DateTime.Now;
            _requestService.SaveChanges();

            var identity = (ClaimsIdentity)User.Identity;
            await _auditLog.Append(identity.GetClaimAsInt("EmployeeId"), LogActionType.Submit, LogResourceType.Request, id,
                $"{identity.GetClaim(ClaimTypes.Name)} submitted request with id {id}");

            if (request.Reviews.Count > 0)
            {
                Employee reviewer = request.OrderedReviews[0].Reviewer;
                string receipient = reviewer.Email;
                string emailName = "ReviewRequest";
                var model = new { _emailHelper.AppUrl, _emailHelper.AppEmail, Request = request };
                string subject = _emailHelper.GetSubjectFromTemplate(emailName, model, _email.Renderer);
                await _email.To(receipient)
                    .Subject(subject)
                    .UsingTemplateFromFile(_emailHelper.GetBodyTemplateFile(emailName), model)
                    .SendAsync();
            }
            else
            {
                request.RequestStatus = RequestStatus.Approved;
                request.CompletedOn = DateTime.Now;
                _requestService.SaveChanges();

                foreach (var requestedSystem in request.Systems)
                {
                    var systemAccess = new SystemAccess(request, requestedSystem);
                    _systemService.AddSystemAccess(systemAccess);
                }

                string emailName = "ProcessRequest";
                var model = new { _emailHelper.AppUrl, _emailHelper.AppEmail, Request = request };
                _email.Subject(_emailHelper.GetSubjectFromTemplate(emailName, model, _email.Renderer))
                    .UsingTemplateFromFile(_emailHelper.GetBodyTemplateFile(emailName), model);
                _email.Data.ToAddresses.Clear();
                var supportUnitIds = request.Systems.GroupBy(s => s.System.SupportUnitId, s => s).Select(g => g.Key).ToList();
                foreach (var supportUnitId in supportUnitIds)
                {
                    var supportUnit = _organizationService.GetSupportUnit((int)supportUnitId);
                    _email.To(supportUnit.Email);
                }
                await _email.SendAsync();
            }

            return RedirectToAction("MyRequests");
        }

        public async Task<IActionResult> DeleteRequest(int id)
        {
            var request = _requestService.GetRequest(id);
            var authResult = await _authService.AuthorizeAsync(User, request, "CanEditRequest");
            if (!authResult.Succeeded)
                return new ForbidResult();

            request.Deleted = true;
            _requestService.SaveChanges();
            return RedirectToAction("MyRequests");
        }

        [HttpGet]
        public IActionResult MyRequests()
        {
            string username = ((ClaimsIdentity)User.Identity).GetClaim(ClaimTypes.NameIdentifier);
            var allRequests = _requestService.GetRequestsByUsername(username);
            List<Request> underReviewRequests = new List<Request>();
            List<Request> completedRequests = new List<Request>();
            List<Request> draftRequests = new List<Request>();
            foreach (var request in allRequests)
            {
                if (request.RequestStatus == RequestStatus.Draft)
                    draftRequests.Add(request);
                else if (request.RequestStatus == RequestStatus.UnderReview)
                    underReviewRequests.Add(request);
                else
                    completedRequests.Add(request);
            }
            ViewData["allRequests"] = allRequests;
            ViewData["underReviewRequests"] = underReviewRequests;
            ViewData["completedRequests"] = completedRequests;
            ViewData["draftRequests"] = draftRequests;
            return View();
        }

        private IActionResult RedirectEditRequest(int id, RequestType requestType)
        {
            switch (requestType.DisplayCode)
            {
                case "Add Access":
                    return RedirectToAction("RequesterInfo", "AddAccessRequest", new { id });
                case "Remove Access":
                    return RedirectToAction("RequesterInfo", "RemoveAccessRequest", new { id });
                case "Update Information":
                    return RedirectToAction("RequesterInfo", "UpdateInfoRequest", new { id });
                case "Transfer":
                    return RedirectToAction("RequesterInfo", "TransferRequest", new { id });
                case "Leaving Probation":
                    return RedirectToAction("RequesterInfo", "LeavingProbationRequest", new { id });
                default:
                    return RedirectToAction("RequesterInfo", "PortfolioAssignmentRequest", new { id });
            }
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

            return _formService.AddFile(file);
        }
    }
}
