using Microsoft.AspNetCore.Mvc;
using PAM.Data;

namespace PAM.Controllers
{
    public class EditRequestController : Controller
    {
        private readonly RequestService _requestService;

        public EditRequestController(RequestService requestService)
        {
            _requestService = requestService;
        }

        public IActionResult Index(int id)
        {
            var request = _requestService.GetRequest(id);
            switch (request.RequestTypeId)
            {
                default:
                    return RedirectToAction("RequesterInfo", "EditPortfolioRequest", new { id });
            }
        }
    }
}
