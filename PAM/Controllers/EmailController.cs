using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;
using PAM.Data;
using PAM.Services;

namespace PAM.Controllers
{
    public class EmailController : Controller
    {
        private readonly IFluentEmail _email;
        private readonly EmailHelper _emailHelper;
        private readonly UserService _userService;

        public EmailController(IFluentEmail email, EmailHelper emailHelper, UserService userService)
        {
            _email = email;
            _emailHelper = emailHelper;
            _userService = userService;
        }

        public IActionResult EmailApprover()
        {
            var supervisor = _userService.GetEmployeeByName((string)TempData["Supervisor"]);

            string receipient = supervisor.Email;
            //string emailName = "Test";
            string emailName = "ReviewRequest";

            // MAYBE send over the RequestId or Request object when going to this method/controller and
            // include that Request in the 'model' below
            // Brandon will implement to create Request entry in db early on
            var model = new { _emailHelper.AppUrl, _emailHelper.AppEmail };

            string subject = _emailHelper.GetSubjectFromTemplate(emailName, model, _email.Renderer);
            _email.To(receipient)
                .Subject(subject)
                .UsingTemplateFromFile(_emailHelper.GetBodyTemplateFile(emailName), model)
                .SendAsync();

            ViewData["Receipient"] = receipient;
            ViewData["Subject"] = subject;

            return RedirectToAction("Self", "Request");
        }

        public IActionResult Test()
        {
            string receipient = "cysun@localhost.localdomain";
            string emailName = "Test";
            var model = new { _emailHelper.AppUrl, _emailHelper.AppEmail };

            string subject = _emailHelper.GetSubjectFromTemplate(emailName, model, _email.Renderer);
            _email.To(receipient)
                .Subject(subject)
                .UsingTemplateFromFile(_emailHelper.GetBodyTemplateFile(emailName), model)
                .SendAsync();

            ViewData["Receipient"] = receipient;
            ViewData["Subject"] = subject;
            return View();
        }
    }
}
