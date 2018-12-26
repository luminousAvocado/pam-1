using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PAM.Models;
using PAM.Services;

namespace PAM.Controllers
{
    public class LoginController : Controller
    {
        //Intial method with a GET request from http and returns the view of the LOGIN
        [HttpGet]
        public IActionResult Login()
        {
            var view = View();
            view.ViewData["Login"] = "~/Views/Shared/_LoginLayout.cshtml";
            return view;
        }

        [HttpPost]
        public IActionResult Login(Employee emp)
        {
            string user = emp.Username;
            MockADService mAD = new MockADService();
            if (mAD.Authenticate(user, "abcd"))
                return RedirectToAction("NewRegistrations", "Home");
            else
            {
                var view = View();
                view.ViewData["Login"] = "~/Views/Shared/_LoginLayout.cshtml";
                return view;
            }
        }
    }
}
