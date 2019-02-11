using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Models;
using PAM.Services;

namespace PAM.Controllers
{
    public class AccountController : Controller
    {
        private readonly IADService _adService;
        private readonly UserService _userService;
        private readonly ILogger _logger;

        public AccountController(IADService adService, UserService userService, ILogger<AccountController> logger)
        {
            _adService = adService;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated) RedirectToAction("Self", "Request");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (!_adService.Authenticate(username, password))
                return View(new ErrorViewModel
                {
                    Message = "Authentication Failed"
                });

            Employee employee = _adService.GetEmployeeByUsername(username);
            Employee user = _userService.GetEmployee(username);
            if (user != null)
            {
                user.Name = employee.Name;
                user.FirstName = employee.FirstName;
                user.LastName = employee.LastName;
                user.Email = employee.Email;
                user.Title = employee.Title;
                user.Department = employee.Department;
                user.Phone = employee.Phone;
                employee = user;
            }
            employee = _userService.SaveEmployee(employee);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(employee.ToClaimsIdentity()),
                new AuthenticationProperties());

            _logger.LogInformation($"User {employee.Username} logged in at {DateTime.UtcNow}.");
            //return RedirectToAction("Self", "Request");
            return RedirectToAction("Welcome");
        }

        [HttpGet]
        public IActionResult Welcome(){
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation($"User {User.Identity.Name} logged out at {DateTime.UtcNow}.");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/");
        }
    }
}
