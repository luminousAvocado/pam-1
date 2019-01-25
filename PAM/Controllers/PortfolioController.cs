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
    public class PortfolioController : Controller
    {
        private readonly AppDbContext _context;

        public PortfolioController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET: Bureaus
        public async Task<IActionResult> Index()
        {
            var bureaus = _context.Bureaus.Include(b => b.BureauType);
            return View(await bureaus.ToListAsync());
        }
       
        public async Task<IActionResult> Units(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            //this list of only parent Units
            //https://docs.microsoft.com/en-us/ef/ef6/querying/related-data
            var units = _context.Units.Include(u => u.Systems).Include(u => u.Bureau).Include(u => u.Parent).Include(u => u.UnitType).Include(u => u.Children).Where(u => u.BureauId == id && u.Parent == null);
            if (units == null)
            {
                return NotFound();
            }
            return View(await units.ToListAsync());

           
            
        }
        public async Task<IActionResult> DefaultPortfolio(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            System.Diagnostics.Debug.WriteLine(id + "****************");
            Console.WriteLine(id + "****************");
           
            var units = _context.Units.Include(u => u.Systems).Include(u => u.Bureau).Include(u => u.Parent).Include(u => u.UnitType).Include(u => u.Children).Where(u => u.ParentId == id );
            if (units == null)
            {
                return NotFound();
            }
            return View(await units.ToListAsync());
        }



        //Burea CRUD operations 
        //----------------------------------------------------------------------------------------------
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
                return RedirectToAction(nameof(Index));
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
                return RedirectToAction(nameof(Index));
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
            return RedirectToAction(nameof(Index));
        }

        private bool BureauExists(int id)
        {
            return _context.Bureaus.Any(e => e.BureauId == id);
        }








        //Units CRUD operations
        //-------------------------------------------------------------------------------------------

        public async Task<IActionResult> DetailsUnit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unit = await _context.Units
                .Include(u => u.Bureau)
                .Include(u => u.Parent)
                .Include(u => u.UnitType)
                .FirstOrDefaultAsync(m => m.UnitId == id);
            if (unit == null)
            {
                return NotFound();
            }

            return View(unit);
        }

        // GET: Units/Create
        public IActionResult CreateUnit()
        {
            ViewData["BureauId"] = new SelectList(_context.Bureaus, "BureauId", "Code");
            ViewData["ParentId"] = new SelectList(_context.Units, "UnitId", "Name");
            ViewData["UnitTypeId"] = new SelectList(_context.Set<UnitType>(), "UnitTypeId", "Code");
            return View();
        }

        // POST: Units/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUnit([Bind("UnitId,Name,BureauId,UnitTypeId,ParentId,DisplayOrder")] Unit unit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(unit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BureauId"] = new SelectList(_context.Bureaus, "BureauId", "Code", unit.BureauId);
            ViewData["ParentId"] = new SelectList(_context.Units, "UnitId", "Name", unit.ParentId);
            ViewData["UnitTypeId"] = new SelectList(_context.Set<UnitType>(), "UnitTypeId", "Code", unit.UnitTypeId);
            return View(unit);
        }

        // GET: Units/Edit/5
        public async Task<IActionResult> EditUnit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unit = await _context.Units.FindAsync(id);
            if (unit == null)
            {
                return NotFound();
            }
            ViewData["BureauId"] = new SelectList(_context.Bureaus, "BureauId", "Code", unit.BureauId);
            ViewData["ParentId"] = new SelectList(_context.Units, "UnitId", "Name", unit.ParentId);
            ViewData["UnitTypeId"] = new SelectList(_context.Set<UnitType>(), "UnitTypeId", "Code", unit.UnitTypeId);
            return View(unit);
        }

        // POST: Units/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUnit(int id, [Bind("UnitId,Name,BureauId,UnitTypeId,ParentId,DisplayOrder")] Unit unit)
        {
            if (id != unit.UnitId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(unit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UnitExists(unit.UnitId))
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
            ViewData["BureauId"] = new SelectList(_context.Bureaus, "BureauId", "Code", unit.BureauId);
            ViewData["ParentId"] = new SelectList(_context.Units, "UnitId", "Name", unit.ParentId);
            ViewData["UnitTypeId"] = new SelectList(_context.Set<UnitType>(), "UnitTypeId", "Code", unit.UnitTypeId);
            return View(unit);
        }

        // GET: Units/Delete/5
        public async Task<IActionResult> DeleteUnit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unit = await _context.Units
                .Include(u => u.Bureau)
                .Include(u => u.Parent)
                .Include(u => u.UnitType)
                .FirstOrDefaultAsync(m => m.UnitId == id);
            if (unit == null)
            {
                return NotFound();
            }

            return View(unit);
        }

        // POST: Units/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedUnit(int id)
        {
            var unit = await _context.Units.FindAsync(id);
            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UnitExists(int id)
        {
            return _context.Units.Any(e => e.UnitId == id);
        }





    }
}


