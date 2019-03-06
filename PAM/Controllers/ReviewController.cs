using System;
using System.Collections.Generic;
using System.Security.Claims;
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
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IADService _adService;
        private readonly UserService _userService;
        private readonly RequestService _requestService;
        private readonly SystemService _systemService;
        private readonly OrganizationService _organizationService;
        private readonly IFluentEmail _email;
        private readonly EmailHelper _emailHelper;
        private readonly ILogger _logger;

        public ReviewController(IADService adService, UserService userService, RequestService requestService, SystemService systemService,
            OrganizationService organizationService, IFluentEmail email, EmailHelper emailHelper, ILogger<ReviewController> logger)
        {
            _adService = adService;
            _userService = userService;
            _requestService = requestService;
            _systemService = systemService;
            _organizationService = organizationService;
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

        public IActionResult ViewReview(int id)
        {
            var review = _requestService.GetReview(id);
            var request = _requestService.GetRequest(review.RequestId);
            if(request.RequestTypeId == 2){
                int unitId = request.TransferredFromUnitId ?? default(int);
                var unit = _organizationService.GetUnit(unitId);
                request.TransferredFromUnit = unit;
            }
            ViewData["request"] = request;
            return View(review);
        }

        [HttpGet]
        public IActionResult EditReview(int id)
        {
            var review = _requestService.GetReview(id);
            var request = _requestService.GetRequest(review.RequestId);
            if(request.RequestTypeId == 2){
                int unitId = request.TransferredFromUnitId ?? default(int);
                var unit = _organizationService.GetUnit(unitId);
                request.TransferredFromUnit = unit;
            }
            ViewData["request"] = request;
            return View(review);
        }

        [HttpPost]
        public IActionResult Approve(int id, string password, string comments)
        {
            string username = ((ClaimsIdentity)User.Identity).GetClaim(ClaimTypes.NameIdentifier);
            if (!_adService.Authenticate(username, password))
                RedirectToAction(nameof(EditReview), new { id });

            Review review = _requestService.GetReview(id);
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

                switch (request.RequestTypeId)
                {
                    //Transfer Request
                    case 2:
                        foreach (var requestedSystem in request.Systems){
                            if(requestedSystem.AccessType == SystemAccessType.Add || requestedSystem.AccessType == SystemAccessType.Update){
                                var systemAccess = new SystemAccess(request, requestedSystem);
                                _systemService.AddSystemAccess(systemAccess);
                            }
                        }
                        break;
                    //Portfolio Assignment Request
                    case 4:
                        foreach (var requestedSystem in request.Systems)
                        {
                            var systemAccess = new SystemAccess(request, requestedSystem);
                            _systemService.AddSystemAccess(systemAccess);
                        }
                        break;
                    //Add Access Request
                    case 11:
                        foreach (var requestedSystem in request.Systems)
                        {
                            if(!(bool)requestedSystem.InPortfolio){
                                requestedSystem.InPortfolio = true;
                            }
                            var systemAccess = new SystemAccess(request, requestedSystem);
                            _systemService.AddSystemAccess(systemAccess);
                        }
                        break;
                    //Remove Access Request
                    case 12:
                        foreach (var requestedSystem in request.Systems)
                        {
                           // _systemService.RemoveSystemAccess(requestedSystem.SystemId);
                        }
                        break;
                    default:
                        break;
                } 

                string emailName = "RequestApproved";
                var model = new { _emailHelper.AppUrl, _emailHelper.AppEmail, Request = request };
                string subject = _emailHelper.GetSubjectFromTemplate(emailName, model, _email.Renderer);
                string receipient = request.RequestedBy.Email;
                _email.To(receipient)
                    .Subject(subject)
                    .UsingTemplateFromFile(_emailHelper.GetBodyTemplateFile(emailName), model)
                    .SendAsync();

                // send email notification to processing units
                // ...
            }

            return RedirectToAction(nameof(MyReviews));
        }

        [HttpPost]
        public IActionResult Deny(int id, string password, string comments)
        {
            string username = ((ClaimsIdentity)User.Identity).GetClaim(ClaimTypes.NameIdentifier);
            if (!_adService.Authenticate(username, password))
                RedirectToAction(nameof(EditReview), new { id });

            Review review = _requestService.GetReview(id);
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
            _email.To(receipient)
                .Subject(subject)
                .UsingTemplateFromFile(_emailHelper.GetBodyTemplateFile(emailName), model)
                .SendAsync();

            return RedirectToAction(nameof(MyReviews));
        }
    }
}
