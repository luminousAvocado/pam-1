using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentEmail.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Extensions;
using PAM.Models;
using PAM.Services;

namespace PAM.Controllers
{
    [Authorize("IsApprover")]
    public class ReviewController : Controller
    {
        private readonly IADService _adService;
        private readonly UserService _userService;
        private readonly RequestService _requestService;
        private readonly SystemService _systemService;
        private readonly OrganizationService _organizationService;
        private readonly IAuthorizationService _authService;
        private readonly IFluentEmail _email;
        private readonly EmailHelper _emailHelper;
        private readonly ILogger _logger;

        public ReviewController(IADService adService, UserService userService, RequestService requestService, SystemService systemService,
            OrganizationService organizationService, IAuthorizationService authService, IFluentEmail email, EmailHelper emailHelper,
            ILogger<ReviewController> logger)
        {
            _adService = adService;
            _userService = userService;
            _requestService = requestService;
            _systemService = systemService;
            _organizationService = organizationService;
            _authService = authService;
            _email = email;
            _emailHelper = emailHelper;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult MyReviews()
        {
            int employeeId = Int32.Parse(((ClaimsIdentity)User.Identity).GetClaim("EmployeeId"));
            var allReviews = _requestService.GetReviewsByReviewerId(employeeId);
            List<Review> currentReviews = new List<Review>();
            List<Review> completedReviews = new List<Review>();
            foreach (var review in allReviews)
            {
                if (review.Completed)
                    completedReviews.Add(review);
                else
                    currentReviews.Add(review);
            }
            ViewData["allReviews"] = allReviews;
            ViewData["currentReviews"] = currentReviews;
            ViewData["completedReviews"] = completedReviews;
            return View();
        }

        public IActionResult ViewRequest(int id)
        {
            var review = _requestService.GetReview(id);
            return View(_requestService.GetRequest(review.RequestId));
        }

        [HttpGet]
        public async Task<IActionResult> EditReview(int id)
        {
            var review = _requestService.GetReview(id);
            var authResult = await _authService.AuthorizeAsync(User, review, "CanEnterReview");
            if (!authResult.Succeeded)
                return new ForbidResult();

            var request = _requestService.GetRequest(review.RequestId);
            ViewData["request"] = request;
            ViewData["reviewsBefore"] = request.Reviews.Where(r => r.ReviewOrder < review.ReviewOrder).OrderBy(r => r.ReviewOrder).ToList();
            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id, string password, string comments)
        {
            string username = ((ClaimsIdentity)User.Identity).GetClaim(ClaimTypes.NameIdentifier);
            if (!_adService.Authenticate(username, password))
                RedirectToAction(nameof(EditReview), new { id });

            Review review = _requestService.GetReview(id);
            var authResult = await _authService.AuthorizeAsync(User, review, "CanEnterReview");
            if (!authResult.Succeeded)
                return new ForbidResult();

            Request request = _requestService.GetRequest(review.RequestId);
            review.Approve(comments);
            request.UpdatedOn = DateTime.Now;
            _requestService.SaveChanges();

            if (review.ReviewOrder < request.Reviews.Count - 1)
            {
                Review nextReview = request.OrderedReviews[review.ReviewOrder + 1];
                string emailName = "ReviewRequest";
                var model = new { _emailHelper.AppUrl, _emailHelper.AppEmail, Request = request };
                string subject = _emailHelper.GetSubjectFromTemplate(emailName, model, _email.Renderer);
                string receipient = nextReview.Reviewer.Email;
                _email.To(receipient)
                    .Subject(subject)
                    .UsingTemplateFromFile(_emailHelper.GetBodyTemplateFile(emailName), model)
                    .Send();

                emailName = "RequestUpdated";
                subject = _emailHelper.GetSubjectFromTemplate(emailName, model, _email.Renderer);
                receipient = request.RequestedBy.Email;
                _email.To(receipient)
                    .Subject(subject)
                    .UsingTemplateFromFile(_emailHelper.GetBodyTemplateFile(emailName), model)
                    .Send();
            }
            else // last review
            {
                request.RequestStatus = RequestStatus.Approved;
                request.CompletedOn = DateTime.Now;
                _requestService.SaveChanges();

                foreach (var requestedSystem in request.Systems)
                {
                    var systemAccess = new SystemAccess(request, requestedSystem);
                    _systemService.AddSystemAccess(systemAccess);
                }

                string emailName = "RequestApproved";
                var model = new { _emailHelper.AppUrl, _emailHelper.AppEmail, Request = request };
                string subject = _emailHelper.GetSubjectFromTemplate(emailName, model, _email.Renderer);
                string receipient = request.RequestedBy.Email;
                _email.To(receipient)
                    .Subject(subject)
                    .UsingTemplateFromFile(_emailHelper.GetBodyTemplateFile(emailName), model)
                    .Send();

                emailName = "ProcessRequest";
                _email.Subject(_emailHelper.GetSubjectFromTemplate(emailName, model, _email.Renderer))
                    .UsingTemplateFromFile(_emailHelper.GetBodyTemplateFile(emailName), model);
                _email.Data.ToAddresses.Clear();
                var processingUnitIds = request.Systems.GroupBy(s => s.System.ProcessingUnitId, s => s).Select(g => g.Key).ToList();
                foreach (var processingUnitId in processingUnitIds)
                {
                    var processingUnit = _organizationService.GetProcessingUnit((int)processingUnitId);
                    _email.To(processingUnit.Email);
                }
                await _email.SendAsync();
            }

            return RedirectToAction(nameof(MyReviews));
        }

        [HttpPost]
        public async Task<IActionResult> Deny(int id, string password, string comments)
        {
            string username = ((ClaimsIdentity)User.Identity).GetClaim(ClaimTypes.NameIdentifier);
            if (!_adService.Authenticate(username, password))
                RedirectToAction(nameof(EditReview), new { id });

            Review review = _requestService.GetReview(id);
            var authResult = await _authService.AuthorizeAsync(User, review, "CanEnterReview");
            if (!authResult.Succeeded)
                return new ForbidResult();

            review.Deny(comments);
            Request request = _requestService.GetRequest(review.RequestId);
            request.RequestStatus = RequestStatus.Denied;
            request.UpdatedOn = DateTime.Now;
            request.CompletedOn = DateTime.Now;
            _requestService.SaveChanges();

            string emailName = "RequestDenied";
            var model = new { _emailHelper.AppUrl, _emailHelper.AppEmail, Request = request, Review = review };
            string subject = _emailHelper.GetSubjectFromTemplate(emailName, model, _email.Renderer);
            string receipient = request.RequestedBy.Email;
            await _email.To(receipient)
                .Subject(subject)
                .UsingTemplateFromFile(_emailHelper.GetBodyTemplateFile(emailName), model)
                .SendAsync();

            return RedirectToAction(nameof(MyReviews));
        }
    }
}
