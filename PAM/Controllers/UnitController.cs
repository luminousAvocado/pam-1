using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PAM.Data;
using PAM.Models;
using PAM.Services;

namespace PAM.Controllers
{
    public class UnitController : Controller
    {
        private readonly OrganizationService _organizationService;
        private readonly TreeViewService _treeViewService;
        private readonly IMapper _mapper;
        private readonly ILogger<UnitController> _logger;

        public UnitController(OrganizationService organizationService, TreeViewService treeViewService,
            IMapper mapper, ILogger<UnitController> logger )
        {
            _organizationService = organizationService;
            _treeViewService = treeViewService;
            _mapper = mapper;
            _logger = logger;
        }

        public IActionResult Units()
        {
            ViewData["tree"] = _treeViewService.GenerateTreeInJson();
            return View();
        }

        /*
        public IActionResult SelectUnit()
        {
            var myTree = _treeService.GenerateTree();
            ViewData["MyTree"] = myTree;

            return View();
            //return View("../UnitSelection/UnitSelection");
        }



        public async Task<IActionResult> DetailsUnit(int? selectedUnit)
        {

            if (selectedUnit == null)
            {
                return NotFound();
            }
           ;

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

        [HttpGet]
        public IActionResult SystemPortfolio(int? id)
        {
            //fix naming convention

            TempData["UnitId"] = id;
            ViewData["SystemPortfolio"] = _context.UnitSystems.Include(u => u.System).Where(u => u.UnitId == id).ToList();
            var employees = _context.Systems.ToList();
            List<String> employeeName = new List<string>();
            foreach (var employee in employees)
            {
                employeeName.Add(employee.Name);
            }
            ViewData["adEmployees"] = employeeName;


            return View();
        }


        [HttpPost]
        public IActionResult SystemPortfolio(string adEmployees)
        {
            //this allows us to keep systems in display fixthis to keep in session
            var employees = _context.Systems.ToList();
            List<String> employeeName = new List<string>();
            foreach (var employee in employees)
            {
                employeeName.Add(employee.Name);
            }
            ViewData["adEmployees"] = employeeName;
            //--------------------------------------

            int unitId = (int)TempData["UnitId"];

            ViewData["SystemPortfolio"] = _context.UnitSystems.Include(u => u.System).Where(u => u.UnitId == unitId).ToList();

            //this is th esytem we want to update
            var unitSystem = _context.UnitSystems.FirstOrDefault(b => b.System.Name.Equals(adEmployees));
            Debug.WriteLine(unitSystem.UnitId);
            if (unitSystem != null)
            {
                unitSystem.UnitId = unitId;
                Debug.WriteLine(unitSystem.UnitId);
                //_context.Update(unitSystem);
                //_context.SaveChangesAsync();

                _context.Update(unitSystem);
                _context.SaveChanges();



            }

            return View();
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

    */
    }


}
