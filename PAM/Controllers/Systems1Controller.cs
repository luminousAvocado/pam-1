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
    public class Systems1Controller : Controller
    {
        private readonly AppDbContext _context;

        public Systems1Controller(AppDbContext context)
        {
            _context = context;
        }

        // GET: Systems1
        public async Task<IActionResult> Index()
        {
            return View(await _context.Systems.ToListAsync());
        }

        // GET: Systems1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var system = await _context.Systems
                .FirstOrDefaultAsync(m => m.SystemId == id);
            if (system == null)
            {
                return NotFound();
            }

            return View(system);
        }

        // GET: Systems1/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Systems1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SystemId,Name,Description,Owner,Retired")] Models.System system)
        {
            if (ModelState.IsValid)
            {
                _context.Add(system);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(system);
        }

        // GET: Systems1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var system = await _context.Systems.FindAsync(id);
            if (system == null)
            {
                return NotFound();
            }
            return View(system);
        }

        // POST: Systems1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SystemId,Name,Description,Owner,Retired")] Models.System system)
        {
            if (id != system.SystemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(system);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemExists(system.SystemId))
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
            return View(system);
        }

        // GET: Systems1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var system = await _context.Systems
                .FirstOrDefaultAsync(m => m.SystemId == id);
            if (system == null)
            {
                return NotFound();
            }

            return View(system);
        }

        // POST: Systems1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var system = await _context.Systems.FindAsync(id);
            _context.Systems.Remove(system);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SystemExists(int id)
        {
            return _context.Systems.Any(e => e.SystemId == id);
        }
    }
}
