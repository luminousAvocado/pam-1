using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PAM.Models;
using PAM.Data;

namespace PAM.Controllers
{
    public class RegistrationsController : Controller
    {
        private readonly AppDbContext _context;

        public RegistrationsController(AppDbContext context)
        {
            _context = context;
        }

        /*
        // GET: Registrations
        public async Task<IActionResult> Index()
        {
            var pAMContext = _context.Registrations.Include(r => r.Employees);
            return View(await pAMContext.ToListAsync());
        }

        // GET: Registrations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registrations = await _context.Registrations
                .Include(r => r.Employees)
                .FirstOrDefaultAsync(m => m.RegistrationId == id);
            if (registrations == null)
            {
                return NotFound();
            }

            return View(registrations);
        }
        */

        // GET: Registrations/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeNumber");
            return View();
        }

        // POST: Registrations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestId", "RequestTypeId", "RequestType", "RequestById", "RequestedBy",
           "RequestForId", "RequestedFor", "RequestStatus", "SubmittedOn", "IsContractor", "IsHighProfileAccess", "IsGlobalAccess",
            "Systems", "Reviews", "CaseloadType", "CaseloadFunction", "CaseloadNumber", "TransferredFromUnitId", "TransferredFromUnit",
            "DepartureReason", "IpAddress", "Notes")] Request req)
        {
            if (ModelState.IsValid)
            {
                _context.Add(req);
                await _context.SaveChangesAsync();
                return RedirectToAction("MyRegistrations", "Home");
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeNumber", req.RequestId);
            return View(req);
        }

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegistrationId,EmployeeId,RequestTypeLkupId,StatusLkupId,CaseloadNumber,OldCaseloadNumber,Contractor,ClusterNumber,DepartureReasonLkupId,HighProfile,Global,SupervisorName,SupervisorPhone,SupervisorDate,DirectorName,DirectorPhone,DirectorDate,BureauChiefName,BureauChiefDate,RequestEffectiveDate,CreateDate,Pdf,WetPdf,Version,IpAddress,CaseloadTypeLkupId,CaseloadFunctionLkupId,TransferFrom,TransferTo,UploadDate,BureauUnitId,CreatedUser,Notes,JobFunction,RejectComment")] Registrations registrations)
        {
            if (ModelState.IsValid)
            {
                _context.Add(registrations);
                await _context.SaveChangesAsync();
                return RedirectToAction("MyRegistrations", "Home");
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeNumber", registrations.EmployeeId);
            return View(registrations);
        }

        // GET: Registrations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registrations = await _context.Registrations.FindAsync(id);
            if (registrations == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeNumber", registrations.EmployeeId);
            return View(registrations);
        }

        // POST: Registrations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RegistrationId,EmployeeId,RequestTypeLkupId,StatusLkupId,CaseloadNumber,OldCaseloadNumber,Contractor,ClusterNumber,DepartureReasonLkupId,HighProfile,Global,SupervisorName,SupervisorPhone,SupervisorDate,DirectorName,DirectorPhone,DirectorDate,BureauChiefName,BureauChiefDate,RequestEffectiveDate,CreateDate,Pdf,WetPdf,Version,IpAddress,CaseloadTypeLkupId,CaseloadFunctionLkupId,TransferFrom,TransferTo,UploadDate,BureauUnitId,CreatedUser,Notes,JobFunction,RejectComment")] Registrations registrations)
        {
            if (id != registrations.RegistrationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(registrations);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistrationsExists(registrations.RegistrationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeNumber", registrations.EmployeeId);
            return View(registrations);
        }

        // GET: Registrations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registrations = await _context.Registrations
                .Include(r => r.Employees)
                .FirstOrDefaultAsync(m => m.RegistrationId == id);
            if (registrations == null)
            {
                return NotFound();
            }

            return View(registrations);
        }

        // POST: Registrations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registrations = await _context.Registrations.FindAsync(id);
            _context.Registrations.Remove(registrations);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegistrationsExists(int id)
        {
            return _context.Registrations.Any(e => e.RegistrationId == id);
        }
        */
    }
}
