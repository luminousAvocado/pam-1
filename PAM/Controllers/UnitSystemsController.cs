using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PAM.Data;
using PAM.Models;

namespace PAM.Controllers
{
    public class UnitSystemsController : Controller
    {
        private readonly AppDbContext _context;

        public UnitSystemsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: UnitSystems
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.UnitSystems.Include(u => u.System).Include(u => u.Unit);
            return View(await appDbContext.ToListAsync());
        }

        // GET: UnitSystems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitSystem = await _context.UnitSystems
                .Include(u => u.System)
                .Include(u => u.Unit)
                .FirstOrDefaultAsync(m => m.UnitId == id);
            if (unitSystem == null)
            {
                return NotFound();
            }

            return View(unitSystem);
        }

        // GET: UnitSystems/Create
        public IActionResult Create()
        {
            ViewData["SystemId"] = new SelectList(_context.Systems, "SystemId", "Name");
            ViewData["UnitId"] = new SelectList(_context.Units, "UnitId", "Name");
            return View();
        }

        // POST: UnitSystems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UnitId,SystemId")] UnitSystem unitSystem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(unitSystem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SystemId"] = new SelectList(_context.Systems, "SystemId", "Name", unitSystem.SystemId);
            ViewData["UnitId"] = new SelectList(_context.Units, "UnitId", "Name", unitSystem.UnitId);
            return View(unitSystem);
        }

        // GET: UnitSystems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitSystem = await _context.UnitSystems.FindAsync(id);
            if (unitSystem == null)
            {
                return NotFound();
            }
            ViewData["SystemId"] = new SelectList(_context.Systems, "SystemId", "Name", unitSystem.SystemId);
            ViewData["UnitId"] = new SelectList(_context.Units, "UnitId", "Name", unitSystem.UnitId);
            return View(unitSystem);
        }

        // POST: UnitSystems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UnitId,SystemId")] UnitSystem unitSystem)
        {
            if (id != unitSystem.UnitId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(unitSystem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UnitSystemExists(unitSystem.UnitId))
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
            ViewData["SystemId"] = new SelectList(_context.Systems, "SystemId", "Name", unitSystem.SystemId);
            ViewData["UnitId"] = new SelectList(_context.Units, "UnitId", "Name", unitSystem.UnitId);
            return View(unitSystem);
        }

        // GET: UnitSystems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitSystem = await _context.UnitSystems
                .Include(u => u.System)
                .Include(u => u.Unit)
                .FirstOrDefaultAsync(m => m.UnitId == id);
            if (unitSystem == null)
            {
                return NotFound();
            }

            return View(unitSystem);
        }

        // POST: UnitSystems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var unitSystem = await _context.UnitSystems.FindAsync(id);
            _context.UnitSystems.Remove(unitSystem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UnitSystemExists(int id)
        {
            return _context.UnitSystems.Any(e => e.UnitId == id);
        }
    }
}
