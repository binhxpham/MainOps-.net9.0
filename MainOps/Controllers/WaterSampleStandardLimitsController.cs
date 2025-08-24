using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models;
using Microsoft.AspNetCore.Identity;

namespace MainOps.Controllers
{
    public class WaterSampleStandardLimitsController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WaterSampleStandardLimitsController(DataContext context,UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: WaterSampleStandardLimits
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.StandardLimits.Include(w => w.WaterSamplePlaceType);
            return View(await dataContext.ToListAsync());
        }

        // GET: WaterSampleStandardLimits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterSampleStandardLimit = await _context.StandardLimits
                .Include(w => w.WaterSamplePlaceType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (waterSampleStandardLimit == null)
            {
                return NotFound();
            }

            return View(waterSampleStandardLimit);
        }
        public async Task<IEnumerable<SelectListItem>> createFilterlist()
        {
            var filternames = await _context.WaterSampleTypes.GroupBy(x=>x.Komponent)
                                         .ToListAsync();
            IEnumerable<SelectListItem> selList = from s in filternames
                                                  select new SelectListItem
                                                  {
                                                      Value = s.First().Id.ToString(),
                                                      Text = s.First().Komponent
                                                  };
            return selList;
        }
        // GET: WaterSampleStandardLimits/Create
        public async Task<IActionResult> Create()
        {
            var komponents = await createFilterlist();
            ViewData["WaterSamplePlaceTypeId"] = new SelectList(_context.WaterSamplePlaceTypes, "Id", "Type");
            ViewData["Komponents"] = new SelectList(komponents, "Value", "Text");
            return View();
        }

        // POST: WaterSampleStandardLimits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,WaterSamplePlaceTypeId,MaxLimit,MeanLimit,Komponent")] WaterSampleStandardLimit waterSampleStandardLimit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(waterSampleStandardLimit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WaterSamplePlaceTypeId"] = new SelectList(_context.WaterSamplePlaceTypes, "Id", "Type", waterSampleStandardLimit.WaterSamplePlaceTypeId);
            return View(waterSampleStandardLimit);
        }

        // GET: WaterSampleStandardLimits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterSampleStandardLimit = await _context.StandardLimits.FindAsync(id);
            if (waterSampleStandardLimit == null)
            {
                return NotFound();
            }
            ViewData["WaterSamplePlaceTypeId"] = new SelectList(_context.WaterSamplePlaceTypes, "Id", "Type", waterSampleStandardLimit.WaterSamplePlaceTypeId);
            return View(waterSampleStandardLimit);
        }

        // POST: WaterSampleStandardLimits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,WaterSamplePlaceTypeId,MaxLimit,MeanLimit,Komponent")] WaterSampleStandardLimit waterSampleStandardLimit)
        {
            if (id != waterSampleStandardLimit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(waterSampleStandardLimit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WaterSampleStandardLimitExists(waterSampleStandardLimit.Id))
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
            ViewData["WaterSamplePlaceTypeId"] = new SelectList(_context.WaterSamplePlaceTypes, "Id", "Type", waterSampleStandardLimit.WaterSamplePlaceTypeId);
            return View(waterSampleStandardLimit);
        }

        // GET: WaterSampleStandardLimits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterSampleStandardLimit = await _context.StandardLimits
                .Include(w => w.WaterSamplePlaceType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (waterSampleStandardLimit == null)
            {
                return NotFound();
            }

            return View(waterSampleStandardLimit);
        }

        // POST: WaterSampleStandardLimits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var waterSampleStandardLimit = await _context.StandardLimits.FindAsync(id);
            _context.StandardLimits.Remove(waterSampleStandardLimit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WaterSampleStandardLimitExists(int id)
        {
            return _context.StandardLimits.Any(e => e.Id == id);
        }
    }
}
