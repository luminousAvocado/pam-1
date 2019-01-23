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
    public class NewRequestController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserService _userService;

        private IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public NewRequestController(IADService adService, UserService userService,
            AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = context;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult CreateRequester()
        {
            var employee = HttpContext.Session.GetObject<Employee>("Employee");
            Requester requester = new Requester
            {
                Email = employee.Email,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Username = employee.Username,
                Name = employee.FirstName + " " + employee.LastName + "(" + employee.Username + ")"
            };

            requester = _userService.SaveRequester(requester);
            HttpContext.Session.SetObject("Requester", requester);

            return View("../Request/NewRequest");
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest (Request req)
        {
            Console.WriteLine("UHH" + req.RequestTypeId);
            Console.WriteLine("UHH1" + req.RequestedById);
            Console.WriteLine("UHH2" + req.RequestedForId);
            _dbContext.Add(req);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Registrations", "Registrations");
        }
    }
}
