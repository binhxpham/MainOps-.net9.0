using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MainOps.Data;
using MainOps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MainOps.Controllers
{
    [Authorize(Roles = "Admin,DivisionAdmin,Manager,ProjectMember,Member")]
    public class PumpActivitiesController : BaseController
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PumpActivitiesController(DataContext context,UserManager<ApplicationUser> userManager):base(context,userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: PumpActivities
        public async Task<IActionResult> Index() 
        {
            if (User.IsInRole("Admin"))
            {
                var dataContext = _context.PumpActivities.Include(p => p.MeasPoint);
                return View(await dataContext.ToListAsync());
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                var dataContext = _context.PumpActivities.Include(p => p.MeasPoint).ThenInclude(x => x.Project).Where(x=>x.MeasPoint.Project.DivisionId.Equals(user.DivisionId));
                return View(await dataContext.ToListAsync());
            }
            
        }

        // GET: PumpActivities/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var pumpActivity = await _context.PumpActivities
                .Include(p => p.MeasPoint).ThenInclude(x=>x.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pumpActivity == null)
            {
                return NotFound();
            }
            else if (!pumpActivity.MeasPoint.Project.DivisionId.Equals(user.DivisionId))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item." });
            }

            return View(pumpActivity);
        }

        // GET: PumpActivities/Create
        public async Task<IActionResult> Create()
        {            
            if (User.IsInRole("Admin")) 
            {
                ViewData["MeasPointId"] = new SelectList(_context.MeasPoints, "Id", "Name");
                return View();
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
                return View();
            }
            
        }

        // POST: PumpActivities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MeasPointId,Start_activity,End_activity")] PumpActivity pumpActivity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pumpActivity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (User.IsInRole("Admin"))
            {
                ViewData["MeasPointId"] = new SelectList(_context.MeasPoints, "Id", "Name");
                return View(pumpActivity);
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
                return View(pumpActivity);
            }
        }

        // GET: PumpActivities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var pumpActivity = await _context.PumpActivities.FindAsync(id);
            if (pumpActivity == null)
            {
                return NotFound();
            }
            else if (!pumpActivity.MeasPoint.Project.DivisionId.Equals(user.DivisionId))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item." });
            }
            if (User.IsInRole("Admin"))
            {
                ViewData["MeasPointId"] = new SelectList(_context.MeasPoints, "Id", "Name",pumpActivity.MeasPointId);
                return View(pumpActivity);
            }
            else
            {
                ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name",pumpActivity.MeasPointId);
                return View(pumpActivity);
            }
        }

        // POST: PumpActivities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MeasPointId,Start_activity,End_activity")] PumpActivity pumpActivity)
        {
            if (id != pumpActivity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pumpActivity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PumpActivityExists(pumpActivity.Id))
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
            if (User.IsInRole("Admin"))
            {
                ViewData["MeasPointId"] = new SelectList(_context.MeasPoints, "Id", "Name");
                return View(pumpActivity);
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                ViewData["MeasPointId"] = new SelectList(_context.MeasPoints.Include(x => x.Project).Where(x => x.Project.DivisionId.Equals(user.DivisionId)), "Id", "Name");
                return View(pumpActivity);
            }
        }

        // GET: PumpActivities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var pumpActivity = await _context.PumpActivities
                .Include(p => p.MeasPoint)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pumpActivity == null)
            {
                return NotFound();
            }
            else if (!pumpActivity.MeasPoint.Project.DivisionId.Equals(user.DivisionId))
            {
                return RedirectToAction("ErrorMessage", "Home", new { text = "You do not have access to this item" });
            }

            return View(pumpActivity);
        }

        // POST: PumpActivities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pumpActivity = await _context.PumpActivities.FindAsync(id);
            _context.PumpActivities.Remove(pumpActivity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PumpActivityExists(int id)
        {
            return _context.PumpActivities.Any(e => e.Id == id);
        }
    }
}
