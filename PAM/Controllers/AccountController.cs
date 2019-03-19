using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        [HttpGet, AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return User.Identity.IsAuthenticated ? RedirectToAction(nameof(Welcome)) : (IActionResult)View();
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password, string returnUrl)
        {
            if (!_adService.Authenticate(username, password))
                return View(new ErrorViewModel
                {
                    Message = "Authentication Failed"
                });

            Employee employee = _adService.GetEmployeeByUsername(username);
            employee = _userService.HasEmployee(username) ?
                _userService.UpdateEmployee(employee) : _userService.CreateEmployee(employee);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(employee.ToClaimsIdentity()),
                new AuthenticationProperties());

            _logger.LogInformation($"{employee.Username} logged in at {DateTime.Now}.");

            return !string.IsNullOrEmpty(returnUrl) ? (IActionResult)LocalRedirect(returnUrl)
                : RedirectToAction(nameof(Welcome));
        }

        [HttpGet]
        public IActionResult Welcome()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation($"{User.Identity.Name} logged out at {DateTime.Now}.");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied(string returnUrl)
        {
            string resource = $"URL: [{returnUrl}]";

            _logger.LogWarning($"Access to {resource} denied for {User.Identity.Name} at {DateTime.Now}.");

            return View();
        }
    }
}
