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
    /// Controller for the Actions with Contaminations.
    /// </summary>
    [Authorize(Roles = ("Admin"))]
    public class ContaminationsController : Controller
    {
        private readonly DataContext _context;
        private readonly LocService _localizer;
        public ContaminationsController(DataContext context, LocService localizer)
        {
            _context = context;
            _localizer = localizer;
        }
        /// <summary>
        /// Show the Contaminations/Index Page with all Contaminations in the datacontext
        /// </summary>
        /// <returns>Contaminations/Index Page with all Contaminations</returns>
        // GET: Contaminations
        public async Task<IActionResult> Index()
        {
            return View(await _context.Contaminations.OrderBy(c => c.contam_group).ThenBy(c => c.Name)
                .Include(c => c.Unit_limit)
                .ToListAsync());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ShowContamsWithoutCure()
        {
            var query =    await (from c in _context.Contaminations.Include(x=>x.Unit_limit)
                            where !(from mf in _context.MediaEfficiencies
                            select mf.contaminationId).Contains(c.Id)
                            select c).OrderBy(c => c.contam_group).ThenBy(c => c.Name).ToListAsync();
            return View(nameof(Index), query);
        }
        /// <summary>
        /// Show the Contaminations/Details Page with the Contamination which has the given Id
        /// </summary>
        /// <param name="id">Id of the Contamination to be shown</param>
        /// <returns>The Contaminations/Details Page if the Id is existing if not returns a NotFound</returns>
        // GET: Contaminations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var contamination = await _context.Contaminations
                            .Include(c => c.Unit_limit)
                            .SingleOrDefaultAsync(c => c.Id == id);
            if (contamination == null)
            {
                return NotFound();
            }
            return View(contamination);
        }
        /// <summary>
        /// Set Up for the Page of Creating a new Contamination.
        /// </summary>
        /// <returns>Contaminations/Create View</returns>
        // GET: Contaminations/Create
        public async Task<IActionResult> Create()
        {
            var units = await _context.Units
                .OrderBy(x => x.TheUnit).GroupBy(x => x.TheUnit).Select(group => group.First()).ToListAsync();
            foreach (var u in units)
            {
                u.TheUnit = u.TheUnit.Replace("&sup2;", "2").Replace("&sup3;", "3");
            }
            var selList = from s in units
                select new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = _localizer.GetLocalizedHtmlString(s.TheUnit)
                };
            ViewData["units"] = new SelectList(selList, "Value", "Text");
            return View();
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Manage(int? id)
        {
            if (ModelState.IsValid)
            {
                if (id != null)
                {
                    var conta = await _context.Contaminations.FindAsync(id);
                    if (conta != null)
                    {
                        return RedirectToAction("MediablockTwo", "VMMedia", new { id });
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return  RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// Checks if the Userinput is Valid and then Creates a new Contamination for the Userinput.
        /// </summary>
        /// <param name="contamination">Contamination with binded Attributes</param>
        /// <returns>VMMedia/MediablockTwo Page or the Contamination/Create Page again</returns>
        // POST: Contaminations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,contam_group,default_limit,unit_limitid")] Contamination contamination)
        {
           // var theUnit = await _context.Units.SingleOrDefaultAsync(x => x.id == _conta.unit_limitid);
           // _conta.unit_limit = theUnit;
            if (ModelState.IsValid)
            {
                _context.Add(contamination);
                await _context.SaveChangesAsync();
                return RedirectToAction("MediablockTwo", "VMMedia", new { contamination.Id });
            }
            return await Create();
        }
        /// <summary>
        /// Prepares the page for Editing the Contamination with the given Id
        /// </summary>
        /// <param name="id">Id of the Contamination to be Edited</param>
        /// <returns>The Contamination/Edit Page for the Contamination with the given Id or if it is not existing then a NotFound</returns>
        // GET: Contaminations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var contamination = await _context.Contaminations.FindAsync(id);
            if (contamination == null)
            {
                return NotFound();
            }
            var units = await _context.Units
                .OrderBy(x => x.TheUnit).GroupBy(x => x.TheUnit).Select(group => group.First()).ToListAsync();

            foreach (var u in units)
            {
                u.TheUnit = u.TheUnit.Replace("&sup2;", "2").Replace("&sup3;", "3");
            }
            var selList = from s in units
                select new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = _localizer.GetLocalizedHtmlString(s.TheUnit)
                };
            ViewData["units"] = new SelectList(selList, "Value", "Text");
            return View(contamination);
        }
        /// <summary>
        /// Checks if the Userinput is Valid and then Edits the Contamination with the given Id for the Userinput
        /// </summary>
        /// <param name="id">The Id of the Edited Contamination</param>
        /// <param name="contamination">Contamination with binded Attributes</param>
        /// <returns>If the Editing of the Contamination was Successful it goes Back to Contamination/Index if not back to Contamination/Edit</returns>
        // POST: Contaminations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,contam_group,default_limit,unit_limitid")] Contamination contamination)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contamination);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContaminationExists(contamination.Id))
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
            return View(contamination);
        }
        /// <summary>
        /// Prepares the page for Deleteing the Contamination with the given Id
        /// </summary>
        /// <param name="id">Id of the Contamination to be deleted</param>
        /// <returns>The Contamination/Delete Page with the Contamination that has the given Id</returns>
        // GET: Contaminations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var contamination = await _context.Contaminations
                .Include(c => c.Unit_limit)
                .SingleOrDefaultAsync(c => c.Id == id);
            if (contamination == null)
            {
                return NotFound();
            }
            return View(contamination);
        }
        /// <summary>
        /// It deletes the Contamination with the given id
        /// </summary>
        /// <param name="id">Id of the Contamination to be deleted</param>
        /// <returns>Contamination/Index Page after the Contamination is deleted</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            /** when deleting a contamination, we also have to delete the MediaEfficiency related to how this contamination
             * interacts with filtermaterials. We also have to update the filtermaterial contamination list. All of this is done below before
             * removing the contamination.
             **/
            var contamination = await _context.Contaminations.FindAsync(id);
            var mediaefficiencies = await _context.MediaEfficiencies.Include(x=>x.Contamination).Include(x=>x.filtermaterial)
                .Where(x => x.contaminationId.Equals(contamination.Id)).ToListAsync();
            foreach(var mf in mediaefficiencies)
            { 
                var filtMat = await _context.FilterMaterials.SingleOrDefaultAsync(f => f.id == mf.filtermaterialid);
                var sb = new StringBuilder();
                var contParts = filtMat.contaminations.Split(',');
                foreach (var s in contParts)
                {
                    if (sb.Length != 0)
                    {
                        sb.Append(", ");
                    }
                    if (s.Trim().ToLower() == mf.Contamination.Name.Trim().ToLower())
                    {
                        sb.Remove(sb.Length - 2, 2);
                    }
                    else
                    {
                        sb.Append(s.Trim());
                    }
                }
                if (sb[sb.Length - 2] == ',')
                {
                    sb.Remove(sb.Length - 2, 2);
                }
                filtMat.contaminations = sb.ToString();
                _context.Update(filtMat);
                _context.MediaEfficiencies.Remove(mf);
                await _context.SaveChangesAsync();
            }
            _context.Contaminations.Remove(contamination);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        /// <summary>
        /// Checks if the Contamination with the given Id is existing
        /// </summary>
        /// <param name="id">Id of the Contamination to be checked</param>
        /// <returns>True if the Contamination is existing and false if not</returns>
        private bool ContaminationExists(int id)
        {
            return _context.Contaminations.Any(c => c.Id == id);
        }
    }
}
