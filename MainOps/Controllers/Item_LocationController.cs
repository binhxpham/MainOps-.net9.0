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
using Microsoft.AspNetCore.Authorization;
using Rotativa.AspNetCore;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,ProjectMember,Manager,Supervisor")]
    public class Item_LocationController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public Item_LocationController(DataContext context,UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            
            _context = context;
            _userManager = userManager;
        }

        // GET: Item_Location
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            DateTime startofyear = new DateTime(DateTime.Now.Year, 1, 1);
            ViewData["Startdate"] = startofyear;
            ViewData["Enddate"] = DateTime.Now;
            var dataContext = await _context.Item_Locations.Include(x => x.HJItem).ThenInclude(x=> x.Division).Include(x=>x.HJItem).ThenInclude(x=>x.HJItemClass).Where(x=>x.HJItem.DivisionId.Equals(user.DivisionId)).Include(i => i.HJItem).Include(i => i.Project).Include(i => i.SubProject).ToListAsync();
            return View(dataContext);
        }
        [HttpGet]
        public async Task<IActionResult> CurrentItemLocations(DateTime? Start = null,DateTime? End = null)
        {
            var user = await _userManager.GetUserAsync(User);
            DateTime StartUsed = DateTime.Now.AddYears(-10).Date;
            DateTime EndUsed = DateTime.Now.Date;
            if(Start != null) {
                StartUsed = Convert.ToDateTime(Start).Date;
            }
            if(End != null)
            {
                EndUsed = Convert.ToDateTime(End).Date;
            }
            ViewData["Startdate"] = Start;
            ViewData["Enddate"] = End;
            var dataContext = await _context.Item_Locations
                .Include(i => i.HJItem).ThenInclude(x => x.HJItemClass)
                .Include(i => i.Project).ThenInclude(x => x.Division)
                .Include(i => i.SubProject).ToListAsync();
            List<Item_Location> items = new List<Item_Location>();
            foreach (var item in dataContext)
            {
                if (item.EndTime == null)
                {
                    if (item.StartTime < EndUsed)
                    {
                        items.Add(item);
                    }
                }
                else
                {
                    if (item.StartTime > StartUsed && item.StartTime < EndUsed)
                    {
                        items.Add(item);
                    }
                    else if (item.StartTime <= StartUsed && item.EndTime <= EndUsed && item.EndTime >= StartUsed)
                    {
                        items.Add(item);
                    }
                }
            }
            return View("Index", items);

        }
        [HttpGet]
        public async Task<IActionResult> CurrentItemLocations_PDF(DateTime? Start = null, DateTime? End = null)
        {
            var user = await _userManager.GetUserAsync(User);
            DateTime StartUsed = DateTime.Now.AddYears(-10).Date;
            DateTime EndUsed = DateTime.Now.Date;
            if (Start != null)
            {
                StartUsed = Convert.ToDateTime(Start).Date;
            }
            if (End != null)
            {
                EndUsed = Convert.ToDateTime(End).Date;
            }
                Item_Locations_VM model = new Item_Locations_VM();
                model.StartDate = Start;
                model.EndDate = End;
                var dataContext = await _context.Item_Locations
                .Include(i => i.HJItem).ThenInclude(x => x.HJItemClass)
                .Include(i => i.Project).ThenInclude(x => x.Division)
                .Include(i => i.SubProject).ToListAsync();
                List<Item_Location> items = new List<Item_Location>();
                foreach (var item in dataContext)
                {
                    if (item.EndTime == null)
                    {
                        if (item.StartTime < EndUsed)
                        {
                            items.Add(item);
                        }
                    }
                    else
                    {
                        if (item.StartTime > StartUsed && item.StartTime < EndUsed)
                        {
                            items.Add(item);
                        }
                        else if (item.StartTime <= StartUsed && item.EndTime <= EndUsed && item.EndTime >= StartUsed)
                        {
                            items.Add(item);
                        }
                    }
                }
                model.Locations = items;
                return new ViewAsPdf("_Index", model);
        }
        // GET: Item_Location/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var item_Location = await _context.Item_Locations
                .Include(i => i.HJItem).ThenInclude(x => x.HJItemClass)
                .Include(i => i.Project)
                .Include(i => i.SubProject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if(!item_Location.HJItem.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to view this item" });
            }
            if (item_Location == null)
            {
                return NotFound();
            }

            return View(item_Location);
        }

        // GET: Item_Location/Create
        public async Task<IActionResult> Create()
        {

            var user = await _userManager.GetUserAsync(User);
            ViewData["HJItemId"] = await createHJItemlist();
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            return View();
        }

        // POST: Item_Location/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HJItemId,ProjectId,SubProjectId,StartTime,EndTime")] Item_Location item_Location)
        {
            if (ModelState.IsValid)
            {
                var oldplacement = await _context.Item_Locations.Where(x => x.HJItemId.Equals(item_Location.HJItemId)).OrderByDescending(x=>x.StartTime).FirstOrDefaultAsync();
                if(oldplacement != null)
                {
                    oldplacement.EndTime = item_Location.StartTime.AddDays(-1);
                    _context.Update(oldplacement);
                }
                _context.Add(item_Location);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }
            var user = await _userManager.GetUserAsync(User);
            ViewData["HJItemId"] = await createHJItemlist();
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            return View(item_Location);
        }

        // GET: Item_Location/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var item_Location = await _context.Item_Locations.Include(x=>x.HJItem).ThenInclude(x => x.HJItemClass).SingleOrDefaultAsync(x=>x.Id.Equals(id));
            if(!item_Location.HJItem.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }
            if (item_Location == null)
            {
                return NotFound();
            }
            ViewData["HJItemId"] = await createHJItemlist();
            ViewData["DivisionId"] = new SelectList(_context.Divisions, "Id", "Name");
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            return View(item_Location);
        }

        // POST: Item_Location/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HJItemId,ProjectId,SubProjectId,StartTime,EndTime")] Item_Location item_Location)
        {
            if (id != item_Location.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item_Location);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Item_LocationExists(item_Location.Id))
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
            var user = await _userManager.GetUserAsync(User);
            ViewData["HJItemId"] = await createHJItemlist();
            ViewData["ProjectId"] = await GetProjectList();
            ViewData["SubProjectId"] = new SelectList(_context.SubProjects.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            return View(item_Location);
        }

        // GET: Item_Location/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var item_Location = await _context.Item_Locations
                .Include(i => i.HJItem).ThenInclude(x => x.HJItemClass)
                .Include(i => i.Project)
                .Include(i => i.SubProject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if(!item_Location.HJItem.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to delete this item" });
            }
            if (item_Location == null)
            {
                return NotFound();
            }

            return View(item_Location);
        }

        // POST: Item_Location/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item_Location = await _context.Item_Locations.FindAsync(id);
            _context.Item_Locations.Remove(item_Location);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Item_LocationExists(int id)
        {
            return _context.Item_Locations.Any(e => e.Id == id);
        }
        public async Task<IEnumerable<SelectListItem>> createHJItemlist()
        {
            var user = await _userManager.GetUserAsync(User);
            var filternames = await _context.HJItems.Where(x=>x.DivisionId.Equals(user.DivisionId)).Include(x => x.HJItemClass).ThenInclude(x => x.HJItemMasterClass).OrderBy(b => b.HJItemClass.HJItemMasterClass.ClassNumber).ThenBy(x => x.HJItemClass.ClassNumber).ThenBy(x => x.HJId).ToListAsync();


            IEnumerable<SelectListItem> selList = from s in filternames
                                                  select new SelectListItem
                                                  {
                                                      Value = s.Id.ToString(),
                                                      Text = s.HJId + " : " + s.Name
                                                  };
            return selList;
        }

    }
}
