using System.Linq;
using System.Threading.Tasks;
using MainOps.Data;
using MainOps.Models.WTPClasses.MainClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace App2.Controllers.MainControllers
{
    [Authorize(Roles = ("Admin"))]
    public class EffortsController : Controller
    {
        private readonly DataContext _context;
        /// <summary>
        /// COnstructor for the EffortsController
        /// </summary>
        /// <param name="context"></param>
        public EffortsController(DataContext context)
        {
            _context = context;
        }
       
        public async Task<IActionResult> Index()
        {
            return View(await _context.Efforts.OrderBy(e=>e.WTP_block_name).ThenBy(e=>e.Name)
                .Include(e=>e.Category)
                .Include(e=>e.WTPUnit)
                .Include(e=>e.Temp_section)
                .Include(e=>e.Division)
                .Include(e=>e.Wtp_luxurity)
                .ToListAsync());
        }
        /// <summary>
        /// Show the Efforts/Details Page with the Effort which has the given Id
        /// </summary>
        /// <param name="id">Id of the Effort to be shown</param>
        /// <returns>The Efforts/Details Page if the Id is existing if not returns a NotFound</returns>
        // GET: Efforts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var effort = await _context.Efforts
                .Include(e=>e.Category)
                .Include(e=>e.WTPUnit)
                .Include(e=>e.Temp_section)
                .Include(e=>e.Division)
                .Include(e=>e.Wtp_luxurity)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (effort == null)
            {
                return NotFound();
            }
            return View(effort);
        }
        /// <summary>
        /// Set Up for the Page of Creating a new Effort.
        /// </summary>
        /// <returns>Efforts/Create View</returns>
        // GET: Efforts/Create
        public async Task<IActionResult> Create()
        {
            var dbUnits = await _context.Units
                .OrderBy(u => u.TheUnit).ToListAsync();     
            var dbTempSections = await _context.Temporal_sections
                .OrderBy(n => n.section).ToListAsync();
            var dbCategories = await _context.Categories
                .OrderBy(n => n.category).ToListAsync();
            var wtpblocknames = await _context.WTP_blocks
                .OrderBy(b=>b.name).ToListAsync();
            var dbLuxurities = await _context.Luxurities
                .OrderBy(l => l.wtp_luxurity).ToListAsync();
            var selList = from s in wtpblocknames
                                                     select new SelectListItem
                                                     {
                                                         Value = s.name + "_" + s.size.ToString(),
                                                         Text = s.name + "_" + s.size.ToString()
                                                     };
            var dbCountries = await _context.Divisions
                .OrderBy(c => c.Name).ToListAsync();
            foreach (var u in dbUnits)
            {
                u.TheUnit = u.TheUnit.Replace("&sup2;", "2").Replace("&sup3;", "3");
            }
            ViewData["Country_choices"] = new SelectList(dbCountries, "id", "country");
            ViewData["Luxurity_choices"] = new SelectList(dbLuxurities, "id", "wtp_luxurity");
            ViewData["Unit_choices"] = new SelectList(dbUnits, "id", "the_unit");
            ViewData["WTPblocknames"] = new SelectList(selList, "Value", "Text");
            ViewData["Temporal_sections"] = new SelectList(dbTempSections, "id", "section");
            ViewData["Category_choices"] = new SelectList(dbCategories, "id", "category");
            return View();
        }
        /// <summary>
        /// Checks if the Userinput is Valid and then Creates a new Effort for the Userinput.
        /// </summary>
        /// <param name="efforten">Effort with binded Attributes</param>
        /// <returns>Efforts/Index Page or the Efforts/Create Page again</returns>
        // POST: Efforts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Name,ger_name,WTP_block_name,categoryid,effort,unitid,temp_sectionid,countryid,wtp_luxurityid")] Effort efforten)
        {
            if (ModelState.IsValid)
            {
                var theunit = _context.WTPUnits.Find(efforten.WTPUnitId);
                efforten.WTPUnit = theunit;
                efforten.WTPUnit.the_unit = MainOps.ExtensionMethods.UrlHelperExtensions.ToHTML(efforten.WTPUnit.the_unit);
                _context.Add(efforten);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(efforten);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var effort = await _context.Efforts.Where(m => m.Id.Equals(id))
                .Include(x => x.WTPUnit)
                .Include(x => x.Wtp_luxurity)
                .Include(x => x.Temp_section)
                .Include(x => x.Division)
                .Include(x => x.Category).SingleAsync();
            if (effort == null)
            {
                return NotFound();
            }
            var dbCountries = await _context.Divisions
                .OrderBy(c => c.Name).ToListAsync();
            var dbUnits = await _context.Units
                .OrderBy(u => u.TheUnit).ToListAsync();
            var dbTempSections = await _context.Temporal_sections
                .OrderBy(n => n.section).ToListAsync();
            var dbCategories = await _context.Categories
                .OrderBy(n => n.category).ToListAsync();
            var wtpblocknames = await _context.WTP_blocks
                .OrderBy(b => b.name).ToListAsync();
            var dbLuxurities = await _context.Luxurities
                .OrderBy(l => l.wtp_luxurity).ToListAsync();
            var selList = from s in wtpblocknames
                                                  select new SelectListItem
                                                  {
                                                      Value = s.name + "_" + s.size.ToString(),
                                                      Text = s.name + "_" + s.size.ToString()
                                                  };
            foreach (var u in dbUnits)
            {
                u.TheUnit = u.TheUnit.Replace("&sup2;", "2").Replace("&sup3;", "3");
            }
            ViewData["Luxurity_choices"] = new SelectList(dbLuxurities, "id", "wtp_luxurity");
            ViewData["Country_choices"] = new SelectList(dbCountries, "id", "country");
            ViewData["Unit_choices"] = new SelectList(dbUnits, "id", "the_unit");
            ViewData["WTPblocknames"] = new SelectList(selList, "Value", "Text");
            ViewData["Temporal_choices"] = new SelectList(dbTempSections, "id", "section");
            ViewData["Category_choices"] = new SelectList(dbCategories, "id", "category");
            return View(effort);
        }
        /// <summary>
        /// Checks if the Userinput is Valid and then Edits the Effort with the given Id for the Userinput
        /// </summary>
        /// <param name="id">The Id of the Edited Effort</param>
        /// <param name="efforten">Effort with binded Attributes</param>
        /// <returns>If the Editing of the Effort was Successful it goes Back to Efforts/Index if not back to Efforts/Edit</returns>
        // POST: Efforts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Name,ger_name,WTP_block_name,categoryid,effort,unitid,temp_sectionid,countryid,wtp_luxurityid")] Effort efforten)
        {
            if (id != efforten.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var theunit = _context.WTPUnits.Find(efforten.WTPUnitId);
                    efforten.WTPUnit = theunit;
                    efforten.WTPUnit.the_unit = MainOps.ExtensionMethods.UrlHelperExtensions.ToHTML(efforten.WTPUnit.the_unit);
                    _context.Update(efforten);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EffortExists(efforten.Id))
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
            return View(efforten);
        }
        // GET: Efforts/Delete/5
        /// <summary>
        /// Prepares the page for Deleteing the Effort with the given Id
        /// </summary>
        /// <param name="id">Id of the Effort to be deleted</param>
        /// <returns>The Efforts/Delete Page with the Effort that has the given Id</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var effort = await _context.Efforts
                .Include(x => x.Category)
                .Include(x => x.WTPUnit)
                .Include(x => x.Temp_section)
                .Include(x => x.Division)
                .Include(x => x.Wtp_luxurity)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (effort == null)
            {
                return NotFound();
            }
            return View(effort);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var effort = await _context.Efforts.SingleOrDefaultAsync(m => m.Id == id);
            _context.Efforts.Remove(effort);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
       
        private bool EffortExists(int id)
        {
            return _context.Efforts.Any(e => e.Id == id);
        }
        
        public async Task<JsonResult> NameAutoComplete(string term)
        {
            var matching = string.IsNullOrWhiteSpace(term) ?
                           await _context.Prices.ToArrayAsync() :
                        await _context.Prices.Where(p => p.name.ToUpper().Contains(term.ToUpper())).ToArrayAsync();
            return Json(matching.Select(m => new
            {
                m.id,
                value = m.name,
                label = m.name + '_' + m.size.ToString()
            }));
        }
    }
}
