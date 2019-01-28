using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PAM.Data;
using PAM.Extensions;

namespace PAM.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly RequestService _requestService;

        public RequestController(RequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpGet]
        public IActionResult Self()
        {
            string username = ((ClaimsIdentity)User.Identity).GetClaim(ClaimTypes.NameIdentifier);
            ViewData["Requests"] = _requestService.GetRequests(username);
            return View();
        }
        /*
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var requests = await _dbContext.Requests
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (requests == null) return View("Registrations");
            else return View(await _dbContext.Requests.ToListAsync());
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var request = await _dbContext.Requests.FindAsync(id);
            _dbContext.Requests.Remove(request);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Registrations");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        } */
    }
}
