using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        //-----------------------------------------------------------------------------------------------------------
        // GET: Bureaus
        public async Task<IActionResult> Index()
        {
            var bureaus = _context.Bureaus.Include(b => b.BureauType);
            return View(await bureaus.ToListAsync());
        }
        //-----------------------------------------------------------------------------------------------------------
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

    }
}


