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
using PAM.Services;
using System.Diagnostics;

namespace PAM.Controllers
{
    public class UnitController : Controller
    {
        private readonly AppDbContext _context;
        private readonly TreeViewService _treeService;
        public UnitController(AppDbContext context, TreeViewService treeService)
        {
            _treeService = treeService;
            _context = context;
        }

        
        public IActionResult SelectUnit()
        {
            var myTree = _treeService.GenerateTree();
            ViewData["MyTree"] = myTree;

            return View();
            //return View("../SelectUnit/SelectUnit");
        }

        
        
        public async Task<IActionResult> DetailsUnit(int? selectedUnit)
        {
            Debug.WriteLine(selectedUnit + "************************");
            if (selectedUnit == null)
            {
                return NotFound();
            }
            Debug.WriteLine(selectedUnit + "************************");

            var unit = await _context.Units
                .Include(u => u.Bureau)
                .Include(u => u.Parent)
                .Include(u => u.UnitType)
                .FirstOrDefaultAsync(m => m.UnitId == selectedUnit);
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
                return RedirectToAction(nameof(SelectUnit));
            }
            ViewData["BureauId"] = new SelectList(_context.Bureaus, "BureauId", "Code", unit.BureauId);
            ViewData["ParentId"] = new SelectList(_context.Units, "UnitId", "Name", unit.ParentId);
            ViewData["UnitTypeId"] = new SelectList(_context.Set<UnitType>(), "UnitTypeId", "Code", unit.UnitTypeId);
            return View(unit);
        }

        // GET: Units/Edit/5
        [HttpGet]
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
                return RedirectToAction(nameof(SelectUnit));
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
            return RedirectToAction(nameof(SelectUnit));
        }

        private bool UnitExists(int id)
        {
            return _context.Units.Any(e => e.UnitId == id);
        }

        public async Task<IActionResult> SystemPortfolio(int? id)
        {
            //get related data
            var systemPortfolio = _context.UnitSystems.Include(u => u.System).Where(u => u.UnitId == id).ToListAsync();

            return View(await systemPortfolio);
        }

        //-------------------------------------------------------------
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
            return RedirectToAction(nameof(SystemPortfolio));
        }

        private bool SystemExists(int id)
        {
            return _context.Systems.Any(e => e.SystemId == id);
        }

    }
}
