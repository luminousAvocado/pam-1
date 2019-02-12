using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;
using PAM.Data;
using PAM.Services;

namespace PAM.Controllers
{
    public class SubmitRequestController : Controller
    {
        private readonly RequestService _requestService;
        private readonly IFluentEmail _email;
        private readonly EmailHelper _emailHelper;

        public SubmitRequestController(RequestService requestService, IFluentEmail email, EmailHelper emailHelper)
        {
            _requestService = requestService;
            _email = email;
            _emailHelper = emailHelper;
        }

        public IActionResult Submit(int id)
        {
            return View();
        }

        /*
              [HttpGet]
              public IActionResult Review(string supervisor)
              {
                  TempData["Supervisor"] = supervisor;
                  var req = HttpContext.Session.GetObject<Request>("Request");
                  var unitId = HttpContext.Session.GetObject<int>("UnitId");
                  ViewData["Systems"] = _orgService.GetRelatedSystems(unitId);
                  ViewData["Request"] = _requestService.GetRequest(req.RequestId);
                  ViewData["Supervisor"] = supervisor;

                  return View();
              }

              [HttpPost]
              public IActionResult Review()
              {
                  var update = HttpContext.Session.GetObject<Request>("Request");
                  update.RequestStatus = RequestStatus.UnderReview;
                  _requestService.UpdateRequest(update);

                  return RedirectToAction("EmailApprover", "NewRequest");
              }

              public IActionResult EmailApprover()
              {
                  var supervisor = _userService.GetEmployeeByName((string)TempData["Supervisor"]);

                  var req = HttpContext.Session.GetObject<Request>("Request");
                  Request Request = _requestService.GetRequest(req.RequestId);

                  string receipient = supervisor.Email;
                  string emailName = "ReviewRequest";

                  var model = new { _emailHelper.AppUrl, _emailHelper.AppEmail, Request };

                  string subject = _emailHelper.GetSubjectFromTemplate(emailName, model, _email.Renderer);
                  _email.To(receipient)
                      .Subject(subject)
                      .UsingTemplateFromFile(_emailHelper.GetBodyTemplateFile(emailName), model)
                      .SendAsync();

                  ViewData["Receipient"] = receipient;
                  ViewData["Subject"] = subject;

                  return RedirectToAction("MyRequests", "Request");
              }

              public Requester updateInfo(Requester current, Requester req)
              {
                  current.FirstName = req.FirstName;
                  current.LastName = req.LastName;
                  current.Address = req.Address;
                  current.City = req.City;
                  current.State = req.State;
                  current.Zip = req.Zip;
                  current.Email = req.Email;
                  current.Phone = req.Phone;
                  current.CellPhone = req.CellPhone;

                  return current;
              }
              */
    }
}
