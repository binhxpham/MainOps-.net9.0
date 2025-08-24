using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainOps.Data;
using MainOps.Models.WTPClasses.MainClasses;
using MainOps.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace App2.Controllers.MainControllers
{
    /// <summary>
    /// Controller for the Actions with Prices
    /// </summary>
    [Authorize(Roles = ("Admin"))]
    public class PricesController : Controller
    {
        private readonly DataContext _context;
        private readonly LocService _localizer;
        public PricesController(DataContext context, LocService localizer)
        {
            _context = context;
            _localizer = localizer;
        }
        public async Task<IActionResult> Index()
        {
            var selList = await CreateFilterlist();
            ViewData["Filterchoices"] = new SelectList(selList, "Value", "Text");
            return View(await _context.Prices.OrderBy(p=>p.name)
                .Include(p=>p.EkdT)
                .Include(p=>p.unit)
                .Include(p=>p.unit_p)
                .Include(p=>p.unit_r)
                .Include(p => p.division)
                .ToListAsync());
        }
        public async Task<IActionResult> Index2(string filter)
        {
            var selList = await CreateFilterlist();
            ViewData["Filterchoices"] = new SelectList(selList, "Value", "Text");
            return View(await _context.Prices.Where(p => p.name.Contains(filter)).OrderBy(x=>x.name).ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var price = await _context.Prices
                .Include(p=>p.division)
                .Include(p=>p.EkdT)
                .Include(p=>p.unit)
                .Include(p=>p.unit_p)
                .Include(p=>p.unit_r)
                .SingleOrDefaultAsync(m => m.id == id);
            if (price == null)
            {
                return NotFound();
            }
            return View(price);
        }
        /// <summary>
        /// Creates a List of all Categories
        /// </summary>
        /// <returns>Gives back the List of all Categories</returns>
        public async Task<IEnumerable<SelectListItem>> CreateFilterlist()
        {
            var categories = await _context.Categories
                .OrderBy(b => b.category).ToListAsync();
            var selList = from s in categories
                                                  select new SelectListItem
                                                  {
                                                      Value = s.id.ToString(),
                                                      Text = s.category
                                                  };
            return selList;
        }
        /// <summary>
        /// Searches for Prices where an Information is Missing (rental or  price)
        /// </summary>
        /// <returns>All Items where some Prices are missing</returns>
        public async Task<IActionResult> ShowMissingPrices()
        {
            var missingprices = await _context.Prices.Where(x => x.rent == null || x.price == null)
                .Include(p => p.EkdT)
                .Include(p => p.unit)
                .Include(p => p.unit_p)
                .Include(p => p.unit_r)
                .Include(p => p.division).ToListAsync();
            var selList = await CreateFilterlist();
            ViewData["Filterchoices"] = new SelectList(selList, "Value", "Text");
            return View("Index", missingprices);
        }
        /// <summary>
        /// Search the in the DataContext the Price with the Searchstring and the filterchoice
        /// </summary>
        /// <param name="searchstring">String that the user can type in</param>
        /// <param name="filterchoice">String the user choose from an Dropdown</param>
        /// <returns>The matching Prices</returns>
        [HttpPost]
        public async Task<IActionResult> Combsearch(string searchstring, string filterchoice)
        {
            var fCConverted = Convert.ToInt32(filterchoice);
            var selList = await CreateFilterlist();
            ViewData["Filterchoices"] = new SelectList(selList, "Value", "Text");
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
                {
                    return View(nameof(Index), await _context.Prices.OrderBy(p => p.name)
                        .Include(p => p.EkdT)
                        .Include(p => p.unit)
                        .Include(p => p.unit_p)
                        .Include(p => p.unit_r)
                        .Include(p => p.division)
                        .ToListAsync());
                }
                else if (string.IsNullOrEmpty(searchstring) && (!string.IsNullOrEmpty(filterchoice) || !filterchoice.Equals("All")))
                {
                    var selectionPrices = await _context.Prices.OrderBy(p => p.name)
                        .Include(p => p.EkdT)
                        .Include(p => p.unit)
                        .Include(p => p.unit_p)
                        .Include(p => p.unit_r)
                        .Include(p => p.division).Where(p => p.EkdT.Equals(fCConverted)).OrderBy(b => b.name).ThenBy(b => b.size).ToListAsync();
                    return View(nameof(Index), selectionPrices);
                }
                else if (!string.IsNullOrEmpty(searchstring) && (string.IsNullOrEmpty(filterchoice) || filterchoice.Equals("All")))
                {
                    var selectionPrices = await _context.Prices.Include(p => p.EkdT)
                        .Include(p => p.unit)
                        .Include(p => p.unit_p)
                        .Include(p => p.unit_r)
                        .Include(p => p.division).Where(p=>p.name.ToLower().Contains(searchstring.ToLower())).OrderBy(b => b.name).ThenBy(b => b.size).ToListAsync();
                    return View(nameof(Index), selectionPrices);
                }
                else
                {
                    var selectionPrices = await _context.Prices.Include(p => p.EkdT)
                        .Include(p => p.unit)
                        .Include(p => p.unit_p)
                        .Include(p => p.unit_r)
                        .Include(p => p.division).Where(p => (p.name.ToLower().Contains(searchstring.ToLower())) && p.EkdT.Equals(fCConverted)).OrderBy(b => b.name).ThenBy(b => b.size).ToListAsync();
                    return View(nameof(Index), selectionPrices);
                }
            }
            return View(nameof(Index));
        }
        /// <summary>
        /// Gives the Prices which matches the Search as an List
        /// </summary>
        /// <param name="search">The string which the searched Prices should contain.</param>
        /// <returns>The List of Prices which contains the search string</returns>
        [HttpGet]
        public JsonResult AutoComplete(string search)
        {
            var prices = _context.Prices
                .Where(r => r.name.ToLower().Contains(search.ToLower())).ToList();
            return Json(prices.Select(m => new
            {
                m.id,
                value = m.name,
                label = m.name + '_' + m.size.ToString()
            }).OrderBy(r => r.label));
        }
        /// <summary>
        /// Creates a List of all Units
        /// </summary>
        /// <returns>Gives back the List of all Units</returns>
        public async Task<IEnumerable<SelectListItem>> CreateUnitlist()
        {
            var filternames = await _context.Units
                .OrderBy(b => b.TheUnit).ToListAsync();
            foreach (var u in filternames)
            {
                u.TheUnit = u.TheUnit.Replace("&sup2;", "2").Replace("&sup3;", "3");
            }
            var selList = from s in filternames
                select new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = _localizer.GetLocalizedHtmlString(s.TheUnit)
                };
            return selList;
        }
        /// <summary>
        /// Set Up for the Page of Creating a new Price.
        /// </summary>
        /// <returns>Prices/Create View</returns>
        // GET: Prices/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories
                .OrderBy(x => x.category).ToListAsync();
            var divisions = await _context.Divisions
                .OrderBy(x => x.Name).ToListAsync();
            
            ViewData["units"] = new SelectList(await CreateUnitlist(), "Value", "Text");
            ViewData["categories"] = new SelectList(categories, "id", "category");
            ViewData["divisions"] = new SelectList(divisions, "Id", "Division_name");
            return View();
        }
        /// <summary>
        /// Checks the string for an 2 or a 3 to write is as an exponent
        /// </summary>
        /// <param name="s">The string that should be checked for 2 or 3</param>
        /// <returns></returns>
        public static string ToHTML(string s)
        {
            var sb = new StringBuilder();
            if (s.Contains("&su"))
            {
                return s;
            }
            foreach(var c in s)
            {
                if (c == '2')
                {
                    sb.Append("&sup2;");
                }
                else if(c == '3')
                {
                    sb.Append("&sup3;");
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();

        }
        /// <summary>
        /// Checks if the Userinput is Valid and then Creates a new Price for the Userinput.
        /// </summary>
        /// <param name="pricen">Price with binded Attributes</param>
        /// <returns>Prices/Index Page or the Prices/Create Page again</returns>
        // POST: Prices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,ger_name,EkdTid,size,unitid,price,unit_pid,rent,unit_rid,divisionid")] Price pricen)
        {
            if (ModelState.IsValid)
            {
                pricen.unit = await _context.WTPUnits.SingleOrDefaultAsync(u => u.id == pricen.unitid);
                pricen.unit_p = await _context.WTPUnits.SingleOrDefaultAsync(u => u.id == pricen.unit_pid);
                pricen.unit_r = await _context.WTPUnits.SingleOrDefaultAsync(u => u.id == pricen.unit_rid);
                pricen.unit.the_unit = ToHTML(pricen.unit.the_unit);
                pricen.unit_p.the_unit = ToHTML(pricen.unit_p.the_unit);
                pricen.unit_r.the_unit = ToHTML(pricen.unit_r.the_unit);
                _context.Add(pricen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pricen);
        }
        public async Task<IActionResult> EditFirst(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var price = await _context.Prices.SingleOrDefaultAsync(m => m.id == id);
            if (price == null)
            {
                return NotFound();
            }
            var categories = await _context.Categories
                .OrderBy(x => x.category).ToListAsync();
            var divisions = await _context.Divisions
                .OrderBy(x => x.Name).ToListAsync();
            ViewData["units"] = new SelectList(await CreateUnitlist(), "Value", "Text");
            ViewData["categories"] = new SelectList(categories, "id", "category");
            ViewData["divisions"] = new SelectList(divisions, "Id", "Division_name");
            return View(price);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var price = await _context.Prices.SingleOrDefaultAsync(m => m.id == id);
            if (price == null)
            {
                return NotFound();
            }
            var categories = await _context.Categories
                .OrderBy(x => x.category).ToListAsync();
            var divisions = await _context.Divisions
                .OrderBy(x => x.Name).ToListAsync();
            ViewData["units"] = new SelectList(await CreateUnitlist(), "Value", "Text");
            ViewData["categories"] = new SelectList(categories, "id", "category");
            ViewData["divisions"] = new SelectList(divisions, "Id", "Division_name");
            return View(price);
        }
        /// <summary>
        /// Checks if the Userinput is Valid and then Edits the Price with the given Id for the Userinput
        /// </summary>
        /// <param name="id">The Id of the Edited Price</param>
        /// <param name="pricen">Price with binded Attributes</param>
        /// <returns>If the Editing of the Price was Successful it goes Back to Prices/Index if not back to Prices/Edit</returns>
        // POST: Prices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,ger_name,EkdTid,size,unitid,price,unit_pid,rent,unit_rid,divisionid")] Price pricen)
        {
            if (id != pricen.id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var sizeunit = await _context.WTPUnits.SingleOrDefaultAsync(x => x.id.Equals(pricen.unitid));
                    var priceunit = await _context.WTPUnits.SingleOrDefaultAsync(x => x.id.Equals(pricen.unit_pid));
                    var rentunit = await _context.WTPUnits.SingleOrDefaultAsync(x => x.id.Equals(pricen.unit_rid));
                    pricen.unit = sizeunit;
                    pricen.unit_p = priceunit;
                    pricen.unit_r = rentunit;
                    pricen.unit.the_unit = ToHTML(pricen.unit.the_unit);
                    pricen.unit_p.the_unit = ToHTML(pricen.unit_p.the_unit);
                    pricen.unit_r.the_unit = ToHTML(pricen.unit_r.the_unit);
                    _context.Update(pricen);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriceExists(pricen.id))
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
            return View(pricen);
        }
        /// <summary>
        /// Checks if the Userinput is Valid and then Edits the Price with the given Id for the Userinput
        /// </summary>
        /// <param name="id">The Id of the Edited Price</param>
        /// <param name="pricen">Price with binded Attributes</param>
        /// <returns>If the Editing of the Price was Successful it goes Back to DeviceEffort/EffortBlock if not back to Prices/EditFirst</returns>
        // POST: Prices/EditFirst/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFirst(int id, [Bind("id,name,EkdTid,size,unitid,price,unit_pid,rent,unit_rid,divisionid")] Price pricen)
        {
            if (id != pricen.id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var sizeunit = await _context.WTPUnits.SingleOrDefaultAsync(x => x.id.Equals(pricen.unitid));
                    var priceunit = await _context.WTPUnits.SingleOrDefaultAsync(x => x.id.Equals(pricen.unit_pid));
                    var rentunit = await _context.WTPUnits.SingleOrDefaultAsync(x => x.id.Equals(pricen.unit_rid));
                    pricen.unit = sizeunit;
                    pricen.unit_p = priceunit;
                    pricen.unit_r = rentunit;
                    pricen.unit.the_unit = ToHTML(pricen.unit.the_unit);
                    pricen.unit_p.the_unit = ToHTML(pricen.unit_p.the_unit);
                    pricen.unit_r.the_unit = ToHTML(pricen.unit_r.the_unit);
                    _context.Update(pricen);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriceExists(pricen.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var relatedWtpblock = await _context.WTP_blocks.SingleOrDefaultAsync(b => b.name == pricen.name);
                if (relatedWtpblock != null)
                {
                    return RedirectToAction("Effortblock", "DeviceEFfort", new { relatedWtpblock.id });
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pricen);
        }
        /// <summary>
        /// Prepares the page for Deleteing the Price with the given Id
        /// </summary>
        /// <param name="id">Id of the Price to be deleted</param>
        /// <returns>The Prices/Delete Page with the Price that has the given Id</returns>
        // GET: Prices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var price = await _context.Prices
                .Include(x => x.EkdT)
                .Include(x => x.unit)
                .Include(x => x.unit_p)
                .Include(x => x.unit_r)
                .SingleOrDefaultAsync(m => m.id == id);
            if (price == null)
            {
                return NotFound();
            }
            return View(price);
        }
        /// <summary>
        /// It deletes the Price with the given id
        /// </summary>
        /// <param name="id">Id of the Price to be deleted</param>
        /// <returns>Prices/Index Page after the Price is deleted</returns>
        // POST: Prices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var price = await _context.Prices.SingleOrDefaultAsync(m => m.id == id);
            _context.Prices.Remove(price);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// Checks if the Price with the given Id is existing
        /// </summary>
        /// <param name="id">Id of the Price to be checked</param>
        /// <returns>True if the Price is existing and false if not</returns>
        private bool PriceExists(int id)
        {
            return _context.Prices.Any(e => e.id == id);
        }
    }
}
