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
    public class TitlesController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TitlesController(DataContext context,UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Titles
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Titles.Include(x=>x.Project).Include(t => t.ItemType);
            return View(await dataContext.ToListAsync());
        }

        // GET: Titles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var title = await _context.Titles
                .Include(t => t.ItemType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (title == null)
            {
                return NotFound();
            }

            return View(title);
        }

        // GET: Titles/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes, "Id", "Item_Type");
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId)), "Id", "Name");
            return View();
        }

        // POST: Titles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TheTitle,ItemTypeId,ProjectId,Worker")] Title title)
        {
            if (ModelState.IsValid)
            {
                _context.Add(title);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var user = await _userManager.GetUserAsync(User);
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes, "Item_Type", "Id", title.ItemTypeId);
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(x => x.DivisionId.Equals(user.DivisionId)), "Name", "Id", title.ItemTypeId);
            return View(title);
        }

        // GET: Titles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var title = await _context.Titles.FindAsync(id);
            if (title == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes.Where(x => x.Rental_UnitId.Equals(5) && x.ProjectId.Equals(title.ProjectId)), "Item_Type", "Id", title.ItemTypeId);
            ViewData["ProjectId"] = await GetProjectList();
            return View(title);
        }

        // POST: Titles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TheTitle,ItemTypeId,ProjectId,Worker")] Title title)
        {
            if (id != title.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(title);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TitleExists(title.Id))
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
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes.Where(x => x.Rental_UnitId.Equals(5) && x.ProjectId.Equals(title.ProjectId)), "Item_Type", "Id", title.ItemTypeId);
            ViewData["ProjectId"] = await GetProjectList();
            return View(title);
        }

        // GET: Titles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var title = await _context.Titles
                .Include(t => t.ItemType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (title == null)
            {
                return NotFound();
            }

            return View(title);
        }

        // POST: Titles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var title = await _context.Titles.FindAsync(id);
            _context.Titles.Remove(title);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TitleExists(int id)
        {
            return _context.Titles.Any(e => e.Id == id);
        }
        public JsonResult GetTitlesProject(string theId)
        {
            int Id = Convert.ToInt32(theId);
            var thedata = _context.Titles.Where(x => x.ProjectId.Equals(Id)).OrderBy(x => x.TheTitle).ToList();
            return Json(thedata);
        }
    }
}
