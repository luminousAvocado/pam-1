using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAM.Data;
using PAM.Models;
using PAM.Services;


namespace PAM.Controllers
{
    [Authorize]
    public class NewRequestController : Controller
    {
        private readonly IADService _adService;
        private readonly UserService _userService;
        private readonly RequestService _requestService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public NewRequestController(IADService adService, UserService userService,
            RequestService requestService, IMapper mapper, ILogger<AccountController> logger)
        {
            _adService = adService;
            _userService = userService;
            _requestService = requestService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult CreateRequest()
        {
            return View(_requestService.GetRequestTypes());
        }

        [HttpPost]
        public IActionResult CreateRequest(string forSelf, string requestedForUsername, int requestTypeId)
        {
            Employee employee = new Employee((ClaimsIdentity)User.Identity);
            Requester requestedBy = _mapper.Map<Requester>(employee);
            requestedBy = _userService.CreateRequester(requestedBy);

            Requester requestedFor;
            if (forSelf.Equals("yes"))
            {
                requestedFor = requestedBy;
            }
            else if (requestedForUsername != null)
            {
                employee = _adService.GetEmployeeByUsername(requestedForUsername);
                employee = _userService.HasEmployee(requestedForUsername) ?
                    _userService.UpdateEmployee(employee) : _userService.CreateEmployee(employee);
                requestedFor = _mapper.Map<Requester>(employee);
                requestedFor = _userService.CreateRequester(requestedFor);
            }
            else
                requestedFor = _userService.CreateRequester(new Requester());

            Request request = new Request();
            request.RequestedBy = requestedBy;
            request.RequestedFor = requestedFor;
            request.RequestTypeId = requestTypeId;
            request = _requestService.CreateRequest(request);

            _logger.LogInformation($"User {User.Identity.Name} created request {request.RequestId}.");

            switch (request.RequestTypeId)
            {
                default:
                    return RedirectToAction("RequesterInfo", "EditPortfolioRequest", new { id = request.RequestId });
            }
        }

    }
}
