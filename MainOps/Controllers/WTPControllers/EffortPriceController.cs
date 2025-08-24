using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainOps.Data;
using MainOps.Models.WTPClasses.MainClasses;
using MainOps.Models.WTPClasses.MixedClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace App2.Controllers.MainControllers.MixedMainConntrollers
{
    [Authorize(Roles = ("Admin"))]
    public class EffortPriceController : Controller
    {
        private readonly DataContext _context;
        public EffortPriceController(DataContext context)
        {
            _context = context;
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            return RedirectToAction("Delete", "Prices", new { id });
        }
        
        public ActionResult Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            return RedirectToAction("Details", "Prices", new { id });
        }
      
        public ActionResult Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            return RedirectToAction("Edit", "Prices", new { id });
        }
        
        [HttpPost]
        public ActionResult Create(EffortPrice effortPrice)
        {
            var efp = new Price(effortPrice);
            _context.Prices.Add(efp);
            _context.SaveChanges();
            return RedirectToAction("Priceblock", "EffortPrice", new { effortPrice.pxx.id });
        }
        
        public async Task<IActionResult> Priceblock(int? id)
        {
            EffortPrice effortPrice;
            var priceList = new List<Price>();
            if (id == null)
            {
                return NotFound();
            }
            var effortblock = await _context.Efforts
                  .SingleOrDefaultAsync(e => e.Id == id);

            var pricesblah = await _context.Prices
                .Include(x=>x.division)
                .Include(x=>x.EkdT)
                .Include(x=>x.unit)
                .Include(x=>x.unit_p)
                .Include(x=>x.unit_r)
                .Where(p => p.name.Equals(effortblock.Name, StringComparison.OrdinalIgnoreCase)).ToListAsync();

            var units = await _context.WTPUnits
               .OrderBy(x => x.the_unit).GroupBy(x => x.the_unit).Select(group => group.First()).ToListAsync();
            var categories = await _context.Categories
                .OrderBy(x => x.category).ToListAsync();
            var divisions = await _context.Divisions
                .OrderBy(x => x.Name).ToListAsync();
            foreach (var u in units)
            {
                u.the_unit = u.the_unit.Replace("&sup2;", "2").Replace("&sup3;", "3");
            }
            if (pricesblah == null)
            {
                //return NotFound();
                effortPrice = new EffortPrice(effortblock, priceList);
            }
            else
            {
                foreach(var p in pricesblah)
                {
                    priceList.Add(p);
                }
                effortPrice = new EffortPrice(effortblock, priceList);
            }
            ViewData["units"] = new SelectList(units, "id", "the_unit");
            ViewData["categories"] = new SelectList(categories, "id", "category");
            ViewData["divisions"] = new SelectList(divisions, "Id", "Division_name");
            return View(effortPrice);
        }
    } 
}
