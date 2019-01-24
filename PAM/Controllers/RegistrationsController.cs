using System;
using System.Linq;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PAM.Extensions;
using PAM.Models;
using PAM.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PAM.Controllers
{
    public class RegistrationsController : Controller
    {
        private readonly AppDbContext _dbContext;
        //private readonly SessionHelper _sessionHelper;

        private IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public RegistrationsController (AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = context;
            _httpContextAccessor = httpContextAccessor;
            //_sessionHelper = _sessionHelper;
        }

        [HttpGet]
        public async Task<IActionResult> Registrations(Employee employee)
        {
            var empty = Enumerable.Empty<Request>();

            var requests = await _dbContext.Requests
                .FirstOrDefaultAsync(m => m.RequestId > 0 );
            if (requests == null) return View("Registrations", empty);
            else return View(await _dbContext.Requests.ToListAsync());
        }

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
        public async Task<IActionResult> DeleteConfirm (int id)
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
        }
    }
}
