using Microsoft.AspNetCore.Mvc;
using PAM.Models;

namespace PAM.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Default()
        {
            return View();
        }

        public IActionResult Unimplemented(ErrorViewModel error)
        {
            return View(error);
        }
    }
}
