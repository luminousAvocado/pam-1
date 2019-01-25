using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAM.Data;
namespace PAM.Controllers
{
    public class SystemsController : Controller
    {
        private readonly AppDbContext _context;
        //-----------------------------------------------------------------------------------------------------------
        public SystemsController(AppDbContext context)
        {
            _context = context;
        }
        //-----------------------------------------------------------------------------------------------------------
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Systems;
            return View(await appDbContext.ToListAsync());
        }
        //-----------------------------------------------------------------------------------------------------------
        public IActionResult Details(int? id)
        {
            if (id == null)
            { return NotFound(); }
            //the obj matched to the ID
            var System = _context.Systems.FirstOrDefault(s => s.SystemId == id);
            if (System == null)
            {
                return NotFound();
            }
            return View(System);
        }
        //-----------------------------------------------------------------------------------------------------------
        public IActionResult Edit()
        {


            return View();
        }
        //-----------------------------------------------------------------------------------------------------------
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var system = _context.Systems.FirstOrDefaultAsync(s => s.SystemId == id);
            if (system == null)
            {
                //system does not exist 
                return NotFound();
            }

            return View(system);
        }
    }
}
