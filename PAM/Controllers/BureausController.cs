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
    public class BureausController : Controller
    {
        private readonly AppDbContext _context;

        public BureausController(AppDbContext context)
        {
            _context = context;
        }
        //-----------------------------------------------------------------------------------------------------------
        // GET: Bureaus
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Bureaus.Include(b => b.BureauType);
            return View(await appDbContext.ToListAsync());
        }
        //-----------------------------------------------------------------------------------------------------------
        // GET: Bureaus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bureau = await _context.Bureaus
                .Include(b => b.BureauType)
                .FirstOrDefaultAsync(m => m.BureauId == id);
            if (bureau == null)
            {
                return NotFound();
            }

            return View(bureau);
        }
        //-----------------------------------------------------------------------------------------------------------
        // GET: Bureaus/Create
        public IActionResult Create()
        {
            ViewData["BureauTypeId"] = new SelectList(_context.Set<BureauType>(), "BureauTypeId", "Code");
            return View();
        }
        //-----------------------------------------------------------------------------------------------------------
        // POST: Bureaus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BureauId,Code,Description,BureauTypeId,DisplayOrder")] Bureau bureau)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bureau);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BureauTypeId"] = new SelectList(_context.Set<BureauType>(), "BureauTypeId", "Code", bureau.BureauTypeId);
            return View(bureau);
        }
        //-----------------------------------------------------------------------------------------------------------
        // GET: Bureaus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bureau = await _context.Bureaus.FindAsync(id);
            if (bureau == null)
            {
                return NotFound();
            }
            ViewData["BureauTypeId"] = new SelectList(_context.Set<BureauType>(), "BureauTypeId", "Code", bureau.BureauTypeId);
            return View(bureau);
        }
        //-----------------------------------------------------------------------------------------------------------
        // POST: Bureaus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BureauId,Code,Description,BureauTypeId,DisplayOrder")] Bureau bureau)
        {
            if (id != bureau.BureauId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bureau);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BureauExists(bureau.BureauId))
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
            ViewData["BureauTypeId"] = new SelectList(_context.Set<BureauType>(), "BureauTypeId", "Code", bureau.BureauTypeId);
            return View(bureau);
        }
        //-----------------------------------------------------------------------------------------------------------
        // GET: Bureaus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bureau = await _context.Bureaus
                .Include(b => b.BureauType)
                .FirstOrDefaultAsync(m => m.BureauId == id);
            if (bureau == null)
            {
                return NotFound();
            }

            return View(bureau);
        }
        //-----------------------------------------------------------------------------------------------------------
        // POST: Bureaus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bureau = await _context.Bureaus.FindAsync(id);
            _context.Bureaus.Remove(bureau);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BureauExists(int id)
        {
            return _context.Bureaus.Any(e => e.BureauId == id);
        }
    }
}
