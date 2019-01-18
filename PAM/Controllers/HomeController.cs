using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PAM.Extensions;
using PAM.Models;
using PAM.Data;
using System.Threading.Tasks;

namespace PAM.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;
        //private readonly SessionHelper _sessionHelper;

        private IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public HomeController (AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = context;
            _httpContextAccessor = httpContextAccessor;
            //_sessionHelper = _sessionHelper;
        }

        [HttpGet]
        public async Task<IActionResult> Registrations()
        {
            Employee employee = _session.GetObject<Employee>("Employee");
            if(employee != null)
            {
                Console.WriteLine("Proper EMPLOYEE w/ ID: " + employee.EmployeeId);
            }

            var requests = await _dbContext.Requests
                .FirstOrDefaultAsync(m => m.RequestId > 0 );//m => m.RequestId == employee.EmployeeId);
            if (requests == null) return View("Registrations");
            else return View(await _dbContext.Requests.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
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
