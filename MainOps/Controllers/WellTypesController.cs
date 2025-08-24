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

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,Manager,Supervisor")]
    public class WellTypesController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WellTypesController(DataContext context,UserManager<ApplicationUser> userManager) : base(context, userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: WellTypes
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var dataContext = _context.WellTypes.Include(w => w.Project).Include(x => x.ItemType).Where(x => x.Project.DivisionId.Equals(user.DivisionId));
            return View(await dataContext.ToListAsync());
        }

        // GET: WellTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var wellType = await _context.WellTypes
                .Include(w => w.Project).Include(x => x.ItemType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if(!wellType.Project.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin")) {
                return RedirectToAction("ErrorMessage", "Home", new { text = "you do not have access to this resource" });
            }
            if (wellType == null)
            {
                return NotFound();
            }

            return View(wellType);
        }

        // GET: WellTypes/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ProjectId"] = await GetProjectList();
            return View();
        }

        // POST: WellTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,ProjectId,ItemTypeId")] WellType wellType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wellType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes.Where(x => x.ProjectId.Equals(wellType.ProjectId) && x.ReportTypeId.Equals(13)), "Id", "Item_Type");
            ViewData["ProjectId"] = await GetProjectList();
            return View(wellType);
        }
        
        // GET: WellTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var wellType = await _context.WellTypes.Include(x => x.Project).SingleOrDefaultAsync(x => x.Id.Equals(id));
            if (wellType == null)
            {
                return NotFound();
            }
            if (!wellType.Project.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "you do not have access to this resource" });
            }

            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes.Where(x => x.ProjectId.Equals(wellType.ProjectId) && x.ReportTypeId.Equals(13)),"Id","Item_Type");
            ViewData["ProjectId"] = await GetProjectList();
            return View(wellType);
        }

        // POST: WellTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,ProjectId,ItemTypeId")] WellType wellType)
        {
            if (id != wellType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wellType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WellTypeExists(wellType.Id))
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

            ViewData["ItemTypeId"] = new SelectList(_context.ItemTypes.Where(x => x.ProjectId.Equals(wellType.ProjectId) && x.ReportTypeId.Equals(13)), "Id", "Item_Type");
            ViewData["ProjectId"] = await GetProjectList();
            return View(wellType);
        }

        // GET: WellTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var wellType = await _context.WellTypes
                .Include(w => w.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (!wellType.Project.DivisionId.Equals(user.DivisionId) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "you do not have access to this resource" });
            }
            if (wellType == null)
            {
                return NotFound();
            }

            return View(wellType);
        }

        // POST: WellTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wellType = await _context.WellTypes.FindAsync(id);
            _context.WellTypes.Remove(wellType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WellTypeExists(int id)
        {
            return _context.WellTypes.Any(e => e.Id == id);
        }
    }
}
