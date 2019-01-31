﻿using System.Collections.Generic;
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
        public IActionResult Self()
        {
            string username = ((ClaimsIdentity)User.Identity).GetClaim(ClaimTypes.NameIdentifier);
            ViewData["Requests"] = _requestService.GetRequests(username);
            return View();
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            string username = ((ClaimsIdentity)User.Identity).GetClaim(ClaimTypes.NameIdentifier);
            var requests = _requestService.GetRequests(username);
            if (requests == null) return RedirectToAction("Self");
            else return View();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirm(int id)
        {
            string username = ((ClaimsIdentity)User.Identity).GetClaim(ClaimTypes.NameIdentifier);
            var requests = _requestService.GetRequests(username);

            foreach(var request in requests)
            {
                if (request.RequestId == id) _requestService.RemoveRequest(request);
            }
            return RedirectToAction("Self");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        } 
    }
}