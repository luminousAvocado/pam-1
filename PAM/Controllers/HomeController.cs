using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PAM.Models;
using PAM.Data;

namespace PAM.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        /*
        [HttpGet]
        public IActionResult MyRegistrations()
        {
            return View("MyRegistrations");
        }
        */
        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyRegistrations([Bind("RequesterId", "Username", "EmployeeNumber", "FirstName",
            "LastName", "Email", "BuearuId", "UnitId", "MiddleName", "PayrollTitle", "Department",
            "DepartmentCode", "WorkAddress", "WorkCity", "WorkState", "WorkZip", "WorkPhone", "CellPhone")] Requester requester)
        {
            if (ModelState.IsValid)
            {
                _context.Add(requester);
                await _context.SaveChangesAsync();
                return RedirectToAction("MyRegistrations", "Home");
            }
            return View("MyRegistrations");
        }

        /*
        public async Task<IActionResult> MyRegistrations()
        {
            return View();
        }
        */

        [HttpGet]
        public IActionResult NewRegistrations()
        {
            return View("NewRegistrations");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
