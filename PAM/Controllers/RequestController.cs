using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAM.Data;
using PAM.Extensions;
using PAM.Models;

namespace PAM.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly RequestService _requestService;

        public RequestController(RequestService requestService)
        {
            _requestService = requestService;
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

        [HttpGet]
        public IActionResult ReviewRequests()
        {
            //-----TODO-----
            ViewData["Requests"] = _requestService.GetRequests();
            return View();
        }

        [HttpGet]
        public IActionResult AllRequests()
        {
            ViewData["Requests"] = _requestService.GetRequests();
            return View();
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            string username = ((ClaimsIdentity)User.Identity).GetClaim(ClaimTypes.NameIdentifier);
            var requests = _requestService.GetRequestsByUsername(username);
            if (requests == null) return RedirectToAction("MyRequests");
            else return View();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int id)
        {
            string username = ((ClaimsIdentity)User.Identity).GetClaim(ClaimTypes.NameIdentifier);
            var requests = _requestService.GetRequestsByUsername(username);

            foreach (var request in requests)
            {
                //   if (request.RequestId == id) _requestService.RemoveRequest(request);
            }
            return RedirectToAction("MyRequests");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
