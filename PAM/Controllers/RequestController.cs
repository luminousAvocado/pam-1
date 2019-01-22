using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PAM.Data;
using PAM.Extensions;
using PAM.Models;
using PAM.Services;


namespace PAM.Controllers
{
    public class RequestController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserService _userService;
        //private readonly SessionHelper _sessionHelp;

        private IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public RequestController(IADService adService, UserService userService,
            AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = context;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
           //_sessionHelp = sessionHelp;
        }

        [HttpGet]
        public IActionResult CreateRequester()
        {
            Requester requester = HttpContext.Session.GetObject<Requester>("Requester");
            requester = _userService.SaveRequester(requester);            

            return View("NewRequest");
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest (NewRegistrationViewModel reg)
        {
            if(reg.RequestedFor.FirstName != null)
            {
                _dbContext.Add(reg.RequestedFor);

            }
            _dbContext.Add(reg.Request);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Registrations", "Home");
        }
    }
}
