using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainOps.Data;
using MainOps.Models.WTPClasses.MainClasses;
using MainOps.Models.WTPClasses.MixedClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace App2.Controllers.MainControllers
{
    [Authorize(Roles = ("Admin"))]
    public class FilterMaterialsController : Controller
    {
        private readonly DataContext _context;
        public FilterMaterialsController(DataContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.FilterMaterials.OrderBy(f=>f.Name)
                .Include(f=>f.water_type)
                .ToListAsync());
        }
       
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var filterMaterial = await _context.FilterMaterials
                .Include(f=>f.water_type)
                .SingleOrDefaultAsync(m => m.id == id);
            if (filterMaterial == null)
            {
                return NotFound();
            }
            return View(filterMaterial);
        }
       
        public async Task<IActionResult> Create()
        {
            var dbwatertypes = await _context.Water_types
                .OrderBy(w => w.water_type).ToListAsync();
            var wtpblocknames = await _context.WTP_blocks
                .OrderBy(x => x.name).GroupBy(x => x.name).Select(group => group.First()).ToListAsync();
            var dbContams = await _context.Contaminations.OrderBy(x => x.Name).ToListAsync();
            ViewData["contaminations"] = new SelectList(dbContams, "Id", "Name");
            ViewData["water_types"] = new SelectList(dbwatertypes, "id", "water_type");
            ViewData["WTPblocknames"] = new SelectList(wtpblocknames, "name", "name");
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Name,contaminations,device,water_typeid")] FilterMaterial filterMaterial)
        {
            var dbwatertypes = await _context.Water_types
                .OrderBy(w => w.water_type).ToListAsync();
            var wtpblocknames = await _context.WTP_blocks
                .OrderBy(x => x.name).GroupBy(x => x.name). Select(group => group.First()).ToListAsync();
            ViewData["water_types"] = new SelectList(dbwatertypes, "id", "water_type");
            ViewData["WTPblocknames"] = new SelectList(wtpblocknames, "name", "name");
            if (ModelState.IsValid)
            {
                await _context.FilterMaterials.AddAsync(filterMaterial);
                await _context.SaveChangesAsync();
                var diffConts = filterMaterial.contaminations.Split(",");
                foreach(var s in diffConts)
                {
                    var contam = await _context.Contaminations.SingleOrDefaultAsync(c => c.Name.ToLower().Equals(s.Trim().ToLower()));
                    if (contam != null)
                    {
                        var mf = new MediaEfficiency(filterMaterial,contam);
                        _context.MediaEfficiencies.Add(mf);
                        await _context.SaveChangesAsync();
                    }
                }
                return RedirectToAction("Mediablock", "VMMedia", new { filterMaterial.id });
            }
            return View(filterMaterial);
        }
        /// <summary>
        /// Prepares the page for Editing the FilterMaterial with the given Id
        /// </summary>
        /// <param name="id">Id of the FilterMaterial to be Edited</param>
        /// <returns>The FilterMaterials/Edit Page for the FilterMaterial with the given Id or if it is not existing then a NotFound</returns>
        // GET: FilterMaterials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var filterMaterial = await _context.FilterMaterials.SingleOrDefaultAsync(m => m.id == id);
            if (filterMaterial == null)
            {
                return NotFound();
            }
            var dbwatertypes = await _context.Water_types
                .OrderBy(w => w.water_type).ToListAsync();
            var wtpblocknames = await _context.WTP_blocks
                .OrderBy(x => x.name).GroupBy(x => x.name).Select(group => group.First()).ToListAsync();
            ViewData["water_types"] = new SelectList(dbwatertypes, "id", "water_type");
            ViewData["WTPblocknames"] = new SelectList(wtpblocknames, "name", "name");
            return View(filterMaterial);
        }
        /// <summary>
        /// Checks if the Userinput is Valid and then Edits the FilterMaterial with the given Id for the Userinput
        /// </summary>
        /// <param name="id">The Id of the Edited FilterMaterial</param>
        /// <param name="filterMaterial">FilterMaterial with binded Attributes</param>
        /// <returns>If the Editing of the FilterMaterial was Successful it goes Back to FilterMaterials/Index if not back to FilterMaterials/Edit</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Name,contaminations,device,water_typeid")] FilterMaterial filterMaterial)
        {
            if (id != filterMaterial.id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var diffConts = filterMaterial.contaminations.Split(",");
                    var mefs = await _context.MediaEfficiencies.Include(x=>x.Contamination).Where(x => x.filtermaterialid.Equals(id)).ToListAsync();
                    foreach(var mo in mefs)
                    {
                        if (diffConts.ToList().IndexOf(mo.Contamination.Name) >= 0 )
                        {
                        }
                        else
                        {
                            _context.Remove(mo);
                            await _context.SaveChangesAsync();
                        }

                    }
                    var sb = new StringBuilder();
                    var counter = 0;
                    foreach (var s in diffConts)
                    {
                        var contam = await _context.Contaminations.SingleOrDefaultAsync(c => c.Name.ToLower().Equals(s.Trim().ToLower()));
            
                        if (contam != null)
                        {
                            if (counter == 0)
                            {
                                sb.Append(s);
                            }
                            else
                            {
                                sb.Append(String.Concat(",", s));
                            }
                            var themef = await _context.MediaEfficiencies.SingleOrDefaultAsync(x => x.Contamination.Equals(contam) && x.filtermaterial.Equals(filterMaterial));
                            if(themef == null)
                            {
                                var mf = new MediaEfficiency(filterMaterial, contam);
                                _context.MediaEfficiencies.Add(mf);
                                await _context.SaveChangesAsync();
                            }
                            counter += 1;
                        }
                    }
                    filterMaterial.contaminations = sb.ToString();
                    _context.Update(filterMaterial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilterMaterialExists(filterMaterial.id))
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
            return View(filterMaterial);
        }
        /// <summary>
        /// Prepares the page for Deleteing the FilterMaterial with the given Id
        /// </summary>
        /// <param name="id">Id of the FilterMaterial to be deleted</param>
        /// <returns>The FilterMaterials/Delete Page with the FilterMaterial that has the given Id</returns>
        // GET: FilterMaterials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var filterMaterial = await _context.FilterMaterials
                .SingleOrDefaultAsync(m => m.id == id);
            if (filterMaterial == null)
            {
                return NotFound();
            }
            return View(filterMaterial);
        }
        /// <summary>
        /// It deletes the FilterMaterial with the given id
        /// </summary>
        /// <param name="id">Id of the FilterMaterial to be deleted</param>
        /// <returns>FilterMaterials/Index Page after the FilterMaterial is deleted</returns>
        // POST: FilterMaterials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var filterMaterial = await _context.FilterMaterials.SingleOrDefaultAsync(m => m.id == id);
            var mediaeffs = await _context.MediaEfficiencies.Where(m => m.filtermaterial == filterMaterial).ToListAsync();
            foreach (var me in mediaeffs)
            {
                _context.Remove(me);
            }            
            _context.Remove(filterMaterial);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ManageContaminations(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            return RedirectToAction("Mediablock", "VMMedia", new { id });
        }
        /// <summary>
        /// Checks if the FilterMaterial with the given Id is existing
        /// </summary>
        /// <param name="id">Id of the FilterMaterial to be checked</param>
        /// <returns>True if the FilterMaterial is existing and false if not</returns>
        private bool FilterMaterialExists(int id)
        {
            return _context.FilterMaterials.Any(e => e.id == id);
        }
    }
}
