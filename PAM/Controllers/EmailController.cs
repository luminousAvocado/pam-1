using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;
using PAM.Services;

namespace PAM.Controllers
{
    public class EmailController : Controller
    {
        private readonly IFluentEmail _email;
        private readonly EmailHelper _emailHelper;

        public EmailController(IFluentEmail email, EmailHelper emailHelper)
        {
            _email = email;
            _emailHelper = emailHelper;
        }

        public IActionResult EmailApprover()
        {
            return View();
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
