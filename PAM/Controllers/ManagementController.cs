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
    public class ManagementController : Controller
    {
        private readonly AppDbContext _context;

        public ManagementController(AppDbContext context)
        {
            _context = context;
        }
        //this is the page we want the user to see first when pressing Management in the Navigation BAR
        public ActionResult WelcomePage()
        {
            return View();
        }
        //-----------------------------------------------------------------------------------------------------------
        //Burea CRUD operations
        // GET: Bureaus
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
        [HttpPost, ActionName("DeleteUnit")]
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



        //============================================================================
        //Systems CRUD Actions


        // GET: Systems1
        public async Task<IActionResult> Systems()
        {
            return View(await _context.Systems.ToListAsync());
        }

        // GET: Systems1/Details/5
        public async Task<IActionResult> DetailsSystem(int? id)
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
        public IActionResult CreateSystem()
        {
            return View();
        }

        // POST: Systems1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSystem([Bind("SystemId,Name,Description,Owner,Retired")] Models.System system)
        {
            if (ModelState.IsValid)
            {
                _context.Add(system);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Systems));
            }
            return View(system);
        }

        // GET: Systems1/Edit/5
        public async Task<IActionResult> EditSystem(int? id)
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
        public async Task<IActionResult> EditSystem(int id, [Bind("SystemId,Name,Description,Owner,Retired")] Models.System system)
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
                return RedirectToAction(nameof(Systems));
            }
            return View(system);
        }

        // GET: Systems1/Delete/5
        public async Task<IActionResult> DeleteSystem(int? id)
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
        [HttpPost, ActionName("DeleteSystem")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedSystem(int id)
        {
            var system = await _context.Systems.FindAsync(id);
            _context.Systems.Remove(system);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Systems));
        }

        private bool SystemExists(int id)
        {
            return _context.Systems.Any(e => e.SystemId == id);
        }

        //============================================================================
        //Locations CRUD Actions
        // GET: Locations
        public async Task<IActionResult> Locations()
        {
            return View(await _context.Locations.ToListAsync());
        }

        // GET: Locations/Details/5
        public async Task<IActionResult> DetailsLocation(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations
                .FirstOrDefaultAsync(m => m.LocationId == id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        // GET: Locations/Create
        public IActionResult CreateLocation()
        {
            return View();
        }

        // POST: Locations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLocation([Bind("LocationId,Name,Address,City,State,Zip")] Location location)
        {
            if (ModelState.IsValid)
            {
                _context.Add(location);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Locations));
            }
            return View(location);
        }

        // GET: Locations/Edit/5
        public async Task<IActionResult> EditLocation(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLocation(int id, [Bind("LocationId,Name,Address,City,State,Zip")] Location location)
        {
            if (id != location.LocationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(location);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationExists(location.LocationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Locations));
            }
            return View(location);
        }

        // GET: Locations/Delete/5
        public async Task<IActionResult> DeleteLocation(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations
                .FirstOrDefaultAsync(m => m.LocationId == id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        // POST: Locations/Delete/5
        [HttpPost, ActionName("DeleteLocation")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedLocation(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Locations));
        }

        private bool LocationExists(int id)
        {
            return _context.Locations.Any(e => e.LocationId == id);
        }
    }
}
