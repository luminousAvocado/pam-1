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
        private readonly IFluentEmail _email;
        private readonly EmailHelper _emailHelper;
        private readonly ILogger _logger;

        public ReviewController(IADService adService, UserService userService, RequestService requestService,
            IFluentEmail email, EmailHelper emailHelper, ILogger<ReviewController> logger)
        {
            _adService = adService;
            _userService = userService;
            _requestService = requestService;
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

        [HttpGet]
        public IActionResult EditReview(int id)
        {
            var review = _requestService.GetReview(id);
            ViewData["request"] = _requestService.GetRequest(review.RequestId);
            return View(review);
        }

        [HttpPost]
        public IActionResult Approve(int id, string password, string comments)
        {
            string username = ((ClaimsIdentity)User.Identity).GetClaim(ClaimTypes.NameIdentifier);
            if (!_adService.Authenticate(username, password))
                RedirectToAction(nameof(EditReview), new { id });

            Review review = _requestService.GetReview(id);
            review.Approve(comments);
            _requestService.SaveChanges();

            Request request = _requestService.GetRequest(review.RequestId);
            if (review.ReviewOrder == request.Reviews.Count - 1) // last review
            {
                request.RequestStatus = RequestStatus.Approved;
                _requestService.SaveChanges();
                // send approval email to requester
            }
            else
            {
                // send notifcation email to the next reviewer
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
            _requestService.SaveChanges();

            // send denial email to requester

            return RedirectToAction(nameof(MyReviews));
        }
    }
}
