using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MainOps.Data;
using MainOps.Models.WTPClasses.MainClasses;
using MainOps.Models.WTPClasses.MixedClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static System.String;

namespace App2.Controllers.MainControllers.MixedMainConntrollers
{
    /// <summary>
    /// Controller for the Actions with DeviceEffort.
    /// </summary>
    /// [Authorize(Roles = ("Admin"))]
    public class DeviceEffortController : Controller
    {
        private readonly DataContext _context;
        public DeviceEffortController(DataContext context)
        {
            _context = context;
        }
        
        [HttpPost]
        public ActionResult Create(DeviceEffort deviceEffort)
        {
            var eff = new Effort();
            string[] s = { deviceEffort.wtp_block_name, deviceEffort.wtp_block_size.ToString() };
            eff.Name = deviceEffort.efx.Name;
            eff.CategoryId = deviceEffort.efx.CategoryId;
            eff.WTP_block_name = Join("_",s);
            eff.effort = deviceEffort.efx.effort;
            eff.Temp_sectionid = deviceEffort.efx.Temp_sectionid;
            eff.WTPUnitId = deviceEffort.efx.WTPUnitId;
            eff.DivisionId = deviceEffort.efx.DivisionId;
            eff.Wtp_luxurityId = deviceEffort.efx.Wtp_luxurityId;
            _context.Efforts.Add(eff);
            _context.SaveChanges();
            return RedirectToAction("Effortblock", "DeviceEffort",new { id = deviceEffort.wtp_block_id });
        }
        
        public async Task<DeviceEffort> GetDeviceEffort(WTP_block b)
        {
            DeviceEffort deviceEffort;
            var effortList = new List<Effort>();
            var effortsblah = await _context.Efforts
                .Where(e => e.WTP_block_name.Equals(b.uniquename(), StringComparison.OrdinalIgnoreCase)).ToListAsync();
            //.Where(e => e.WTP_block_name.Equals(wtpblock.uniquename(wtpblock.name,wtpblock.size), StringComparison.OrdinalIgnoreCase)).ToListAsync();
            if (effortsblah == null)
            {
                deviceEffort = new DeviceEffort(b, effortList, false);
            }
            else
            {
                foreach (var e in effortsblah)
                {
                    effortList.Add(e);
                }
                deviceEffort = new DeviceEffort(b, effortList, false);
            }
            return deviceEffort;
        }
        /// <summary>
        /// Jump back to the Summarypage of the Wtp Block with Efforts
        /// </summary>
        /// <param name="id">The Id of the WtpBlock to be shown with the associated Efforts</param>
        /// <returns>The DeviceEffort/Effortblock Page with the wtpBlock of the given id</returns>
        public async Task<IActionResult> EffortblockRedirect(int? id)
        {
            var effort = await _context.Efforts.FindAsync(id);
            var wtpBlock = await (from wb in _context.WTP_blocks
                                   join e in _context.Efforts
                                    on wb.uniquename() equals e.WTP_block_name
                                   where e.Id.Equals(effort.Id)
                                   select wb).FirstAsync();
            return RedirectToAction("Effortblock", "DeviceEffort", new {wtpBlock.id });
        }
        public async Task<IActionResult> Effortblock(int? id)
        {
            DeviceEffort deviceEffort;
            var effortList = new List<Effort>();
            if (id == null)
            {
                return NotFound();
            }
            var wtpblock = await _context.WTP_blocks
                .FindAsync(id);
            var effortsblah = await _context.Efforts
                .Where(e => e.WTP_block_name.Equals(wtpblock.uniquename(), StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
            
            if (effortsblah != null)
            {
                foreach (var e in effortsblah)
                {
                    effortList.Add(e);
                }
                deviceEffort = new DeviceEffort(wtpblock, effortList, false);
            }
            else
            {
                deviceEffort = new DeviceEffort(wtpblock, effortList, false);
            }
            var dbCountries = await _context.Divisions
                .OrderBy(c => c.Name).ToListAsync();
            var dbUnits = await _context.WTPUnits
                .OrderBy(u => u.the_unit).ToListAsync();
            var dbTempSections = await _context.Temporal_sections
                .OrderBy(n => n.section).ToListAsync();
            var dbCategories = await _context.Categories
                .OrderBy(n => n.category).ToListAsync();
            var dbLuxurities = await _context.Luxurities
                .OrderBy(l => l.wtp_luxurity).ToListAsync();
            var selList = await CreateWtPblocklist();
            foreach (var u in dbUnits)
            {
                u.the_unit = u.the_unit.Replace("&sup2;", "2").Replace("&sup3;", "3");
            }
            ViewData["WTPblocknames"] = new SelectList(selList, "Value", "Text");
            ViewData["Luxurity_choices"] = new SelectList(dbLuxurities, "id", "wtp_luxurity");
            ViewData["Country_choices"] = new SelectList(dbCountries, "id", "country");
            ViewData["Unit_choices"] = new SelectList(dbUnits, "id", "the_unit");
            ViewData["Temporal_sections"] = new SelectList(dbTempSections, "id", "section");
            ViewData["Category_choices"] = new SelectList(dbCategories, "id", "category");
            return View(deviceEffort);
        }
       
        [HttpPost]
        public async Task<IActionResult> CopyTo(string posWtpBlock, int? wtpBlockId)
        {
            if (posWtpBlock != "Please Select")
            {
                var chosenWtpBlock = await _context.WTP_blocks.SingleOrDefaultAsync(b => b.id == wtpBlockId);
                var effortsOnThatBlock = await _context.Efforts
                .Where(e => e.WTP_block_name.Equals(chosenWtpBlock.uniquename(), StringComparison.OrdinalIgnoreCase)).ToListAsync();
                var theWtpBlock = await _context.WTP_blocks
                    .SingleOrDefaultAsync(b => b.id == Convert.ToInt32(posWtpBlock));
                var effortsOnReceiver = await _context.Efforts
                    .Where(e => e.WTP_block_name.Equals(theWtpBlock.uniquename(), StringComparison.OrdinalIgnoreCase)).ToListAsync();
                foreach (var e in effortsOnThatBlock)
                {
                    var exiEf = effortsOnReceiver.Where(i => i.Name == e.Name).ToList();
                    if (exiEf.Count() < 1)
                    {
                        //string[] newname_parts = e.nameparts(e.WTP_block_name);
                        //for (int i = 0; i < (newname_parts.Count() - 1); i++)
                        //{
                        //     finalNewName = String.Concat(final_new_name, newname_parts[i]);
                        // }
                        var finalNewName = Concat(theWtpBlock.name, "_", Convert.ToString(theWtpBlock.size));
                        var eNew = new Effort(e, finalNewName);
                        _context.Add(eNew);
                    }
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Effortblock", "DeviceEffort", new { id = Convert.ToInt32(posWtpBlock) });
        }
        /// <summary>
        /// Copies all Efforts from the posWtpBlock to the wtpBlock with the wtpBlockId
        /// </summary>
        /// <param name="posWtpBlock">The Id of the wtpBlock from which the Effort should be Copied</param>
        /// <param name="wtpBlockId">The Id of the wtpBlock to which the Effort should be Copied</param>
        /// <returns>The DeviceEffort/Effortblock with the wtpBlockId</returns>
        [HttpPost]
        public async Task<IActionResult> CopyFrom(string posWtpBlock,int? wtpBlockId)
        {
            if (posWtpBlock != "Please Select")
            {
                var chosenWtpBlock = await _context.WTP_blocks.SingleOrDefaultAsync(b => b.id == Convert.ToInt32(posWtpBlock));
                var effortsOnThatBlock = await _context.Efforts
                .Where(e => e.WTP_block_name.Equals(chosenWtpBlock.uniquename(), StringComparison.OrdinalIgnoreCase)).ToListAsync();
                var theWtpBlock = await _context.WTP_blocks
                    .SingleOrDefaultAsync(b => b.id == wtpBlockId);
                var effortsOnReceiver = await _context.Efforts
                    .Where(e => e.WTP_block_name.Equals(theWtpBlock.uniquename(), StringComparison.OrdinalIgnoreCase)).ToListAsync();
                foreach (var e in effortsOnThatBlock)
                {
                    var exiEf = effortsOnReceiver.Where(i => i.Name == e.Name).ToList();
                    if (exiEf.Count() < 1)
                    {
                        var finalNewName = "";
                        //string[] newname_parts = e.nameparts(e.WTP_block_name);
                        //for (int i = 0; i < (newname_parts.Count() - 1); i++)
                        //{
                        //     finalNewName = String.Concat(final_new_name, newname_parts[i]);
                        //}
                        finalNewName = Concat(theWtpBlock.name, "_", Convert.ToString(theWtpBlock.size));
                        var eNew = new Effort(e, finalNewName);
                        _context.Add(eNew);
                    }
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Effortblock", "DeviceEffort", new { id = wtpBlockId });
        }
        /// <summary>
        /// Creates a List of all WtpBlocks
        /// </summary>
        /// <returns>Gives back the List of all WtpBlocks</returns>
        public async Task<IEnumerable<SelectListItem>> CreateWtPblocklist()
        {
            var wtpblocknames = await _context.WTP_blocks
                .OrderBy(b => b.name).ToListAsync();
            var selList = from s in wtpblocknames
                                                  select new SelectListItem
                                                  {
                                                      Value = s.id.ToString(),
                                                      Text = s.name + "_" + s.size
                                                  };
            return selList;
        }
        /// <summary>
        /// Show the DeviceEffort/Details Page with the WtpBlock which has the given Id
        /// </summary>
        /// <param name="id">Id of the DeviceEffort to be shown</param>
        /// <returns>The DeviceEffort/Details Page if the Id is existing if not returns a NotFound</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var effort = await _context.Efforts
                .Include(e => e.Category)
                .Include(e => e.WTPUnit)
                .Include(e => e.Temp_section)
                .Include(e => e.Division)
                .Include(e => e.Wtp_luxurity)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (effort == null)
            {
                return NotFound();
            }
            return View(effort);
        }
       
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
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
            var dbUnits = await _context.WTPUnits
                .OrderBy(u => u.the_unit).ToListAsync();
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
                u.the_unit = u.the_unit.Replace("&sup2;", "2").Replace("&sup3;", "3");
            }
            ViewData["Luxurity_choices"] = new SelectList(dbLuxurities, "id", "wtp_luxurity");
            ViewData["Country_choices"] = new SelectList(dbCountries, "id", "country");
            ViewData["Unit_choices"] = new SelectList(dbUnits, "id", "the_unit");
            ViewData["WTPblocknames"] = new SelectList(selList, "Value", "Text");
            ViewData["Temporal_sections"] = new SelectList(dbTempSections, "id", "section");
            ViewData["Category_choices"] = new SelectList(dbCategories, "id", "category");
            return View(effort);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Name,WTP_block_name,categoryid,effort,WTPUnitId,temp_sectionid,countryid,wtp_luxurityid")] Effort efforten)
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
                    throw;
                }
                var thename = efforten.nameparts(efforten.WTP_block_name)[0];
                var thesize = Convert.ToDouble(efforten.nameparts(efforten.WTP_block_name)[1]);
                var wtpBlock = await _context.WTP_blocks.SingleOrDefaultAsync(m => m.name == thename && m.size == thesize);
                return RedirectToAction("Effortblock", "DeviceEffort", new {wtpBlock.id });
            }
            return View(efforten);
        }
        
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
            var thename = effort.nameparts(effort.WTP_block_name)[0];
            var thesize = Convert.ToDouble(effort.nameparts(effort.WTP_block_name)[1]);
            var wtpBlock = await _context.WTP_blocks.SingleOrDefaultAsync(m => m.name == thename && m.size == thesize);
            _context.Efforts.Remove(effort);
            await _context.SaveChangesAsync();
            return RedirectToAction("Effortblock", "DeviceEffort", new { wtpBlock.id });
        }
        
        public async Task<IActionResult> GetPrice(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var theprices = new List<Price>();
            var efforten = await _context.Efforts.SingleOrDefaultAsync(e => e.Id == id);
            try
            {
                var pricen = await _context.Prices.SingleOrDefaultAsync(p => p.name.Equals(efforten.Name, StringComparison.OrdinalIgnoreCase));
                theprices.Add(pricen);
            }
            catch
            {
                var pricen = await _context.Prices.Where(p => p.name.Equals(efforten.Name, StringComparison.OrdinalIgnoreCase)).ToListAsync();
                foreach(var p in pricen)
                {
                    theprices.Add(p);
                }
                RedirectToAction("Index2", "Prices", new { filter = efforten.Name });
            } 
            if (theprices.Count == 0)
            {
                return RedirectToAction("Create","Prices");
            }
            return RedirectToAction("Priceblock", "EffortPrice", new { efforten.Id });
        }
        /// <summary>
        /// An Autocompletion for the wtpblockname.
        /// </summary>
        /// <param name="term">The given term which should be completed</param>
        /// <returns>a List of possible Completations</returns>
        public async Task<JsonResult> NameAutoComplete(string term)
        {
            var matching =  IsNullOrWhiteSpace(term) ?
                           await _context.Prices.ToArrayAsync() :
                        await _context.Prices.Where(p => p.name.ToLower().Contains(term.ToLower())).ToArrayAsync();
            return Json(matching.Select(m => new
            {
                m.id,
                value = m.name,
                label = m.name + '_' + m.size.ToString()
            }));
        }
        /// <summary>
        /// Checks if the Effort with the given Id is existing
        /// </summary>
        /// <param name="id">Id of the Effort to be checked</param>
        /// <returns>True if the Effort is existing and false if not</returns>
        private bool EffortExists(int id)
        {
            return _context.Efforts.Any(e => e.Id == id);
        }
    }


}
