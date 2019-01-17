using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PAM.Models;
using PAM.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace PAM.Controllers
{
    public class RequestController : Controller
    {
        private readonly AppDbContext _context;

        public RequestController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestId", "RequestTypeId", "RequestById",
            "RequestForId",  "RequestStatus", "SubmittedOn", "IsContractor", "IsHighProfileAccess", "IsGlobalAccess",
            "CaseloadType", "CaseloadFunction", "CaseloadNumber", "OldCaseloadNumber", "TransferredFromUnitId",
            "DepartureReason", "IpAddress", "Notes")] Request req)
        {
            try
            {
                Console.WriteLine("Request ID is " + req.RequestTypeId.ToString());
                if (ModelState.IsValid)
                {
                    Console.WriteLine("ENTERED CREATE");
                    _context.Add(req);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Finished Adding object");
                    return RedirectToAction("MyRegistrations", "Home");
                }
            }
            catch (DataException reqExc)
            {
                ModelState.AddModelError("", "Unable to save change");
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeNumber", req.RequestId);
            return View(req);
        }
    }
}
