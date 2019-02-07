using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAM.Data;
using PAM.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PAM.Controllers
{
    public class BureauController : Controller
    {
        private readonly AppDbContext _context;

        public BureauController(AppDbContext context)
        {
            _context = context;
        }
        //this is the page we want the user to see first when pressing Management in the Navigation BAR
        public ActionResult WelcomePage()
        {
            return View();
        }
        public async Task<IActionResult> Bureaus()
        {
            var appDbContext = _context.Bureaus.Include(b => b.BureauType);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Bureaus/Details/5
        public async Task<IActionResult> DetailsBureau(int? id)
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

        // GET: Bureaus/Create
        public IActionResult CreateBureau()
        {
            ViewData["BureauTypeId"] = new SelectList(_context.Set<BureauType>(), "BureauTypeId", "Code");
            return View();
        }

        // POST: Bureaus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBureau([Bind("BureauId,Code,Description,BureauTypeId,DisplayOrder")] Bureau bureau)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bureau);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Bureaus));
            }
            ViewData["BureauTypeId"] = new SelectList(_context.Set<BureauType>(), "BureauTypeId", "Code", bureau.BureauTypeId);
            return View(bureau);
        }

        // GET: Bureaus/Edit/5
        public async Task<IActionResult> EditBureau(int? id)
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

        // POST: Bureaus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBureau(int id, [Bind("BureauId,Code,Description,BureauTypeId,DisplayOrder")] Bureau bureau)
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
                return RedirectToAction(nameof(Bureaus));
            }
            ViewData["BureauTypeId"] = new SelectList(_context.Set<BureauType>(), "BureauTypeId", "Code", bureau.BureauTypeId);
            return View(bureau);
        }

        // GET: Bureaus/Delete/5
        public async Task<IActionResult> DeleteBureau(int? id)
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

        // POST: Bureaus/Delete/5
        [HttpPost, ActionName("DeleteBureau")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedBureau(int id)
        {
            var bureau = await _context.Bureaus.FindAsync(id);
            _context.Bureaus.Remove(bureau);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Bureaus));
        }

        private bool BureauExists(int id)
        {
            return _context.Bureaus.Any(e => e.BureauId == id);
        }
    }
}
